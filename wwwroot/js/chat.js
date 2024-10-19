const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

let currentReceiverId = null;
let lastMessageTimestamp = null; // Variable to store the last message timestamp

connection.start().then(function () {
    console.log("SignalR Connected");
}).catch(function (err) {
    return console.error(err.toString());
});

// Poll messages every 3 seconds
setInterval(pollMessages, 3000);  // 3000 milliseconds = 3 seconds

// Poll user status every 5 seconds
setInterval(pollUserStatus, 5000); // 5000 milliseconds = 5 seconds
// Refresh the message thread every 10 seconds (or your desired interval)

setInterval(refreshMessageThread, 1500); // 10000 milliseconds = 10 seconds

function pollMessages() {
    if (currentReceiverId) {
        $.get(`/Chat/GetMessages?userId=${currentReceiverId}`, function (data) {
            // Track whether new messages are appended
            let newMessages = [];

            // Append only new messages
            data.forEach(function (message) {
                // Check if the message is new based on its timestamp
                if (!lastMessageTimestamp || new Date(message.timestamp) > new Date(lastMessageTimestamp)) {
                    newMessages.push(message);
                    lastMessageTimestamp = message.timestamp; // Update the last message timestamp
                }
            });

            // Append new messages to the message thread if there are any
            if (newMessages.length > 0) {
                newMessages.forEach(function (message) {
                    appendMessage(message.content, message.isFromCurrentUser, message.timestamp);
                });
            }
        });
    }
}

function refreshMessageThread() {
    if (currentReceiverId) {
        // Fetch messages from the server
        $.get(`/Chat/GetMessages?userId=${currentReceiverId}`, function (data) {
            $("#messageThread").empty();  // Clear the message thread before fetching new messages
            lastMessageTimestamp = null;  // Reset to fetch all messages

            data.forEach(function (message) {
                appendMessage(message.content, message.isFromCurrentUser, message.timestamp);
                lastMessageTimestamp = message.timestamp; // Update last message timestamp
            });
        });
    }
}

function pollUserStatus() {
    $.get('/Chat/GetUsersStatus', function (usersStatus) {
        usersStatus.forEach(function (user) {
            const userItem = $(`.user-item[data-user-id="${user.id}"]`);
            if (userItem.length) {
                // Update the status
                userItem.find('.status-indicator').removeClass('online offline')
                    .addClass(user.isOnline ? 'online' : 'offline');
            }
        });
    });
}

connection.on("ReceiveMessage", function (senderId, message) {
    if (senderId === currentReceiverId) {
        // Add the new message directly to the chat without reloading all messages
        appendMessage(message.content, false, message.timestamp); // 'false' since it's from the other user
    } else {
        updateUnreadCount(senderId);
    }
});

$("#userList").on("click", ".user-item", function () {
    const userId = $(this).data("user-id");
    console.log(`Clicked user with ID: ${userId}`);
    loadChat(userId);  // Load chat for the selected user
});

$("#sendButton").click(function () {
    const message = $("#messageInput").val();
    if (currentReceiverId && message) {
        connection.invoke("SendMessage", currentReceiverId, message).catch(function (err) {
            return console.error(err.toString());
        });
        appendMessage(message, true, new Date());  // Add the message to the thread
        $("#messageInput").val(''); // Clear input field
        lastMessageTimestamp = new Date(); // Update the last message timestamp to prevent duplication
    }
});

function loadChat(userId) {
    currentReceiverId = userId;
    $("#messageThread").empty();  // Clear message thread before fetching new messages
    $(".chat-area").show();
    lastMessageTimestamp = null; // Reset to fetch all messages

    // Fetch messages from the server
    $.get(`/Chat/GetMessages?userId=${userId}`, function (data) {
        data.forEach(function (message) {
            appendMessage(message.content, message.isFromCurrentUser, message.timestamp);
            lastMessageTimestamp = message.timestamp; // Set the last message timestamp
        });
    });

    // Mark messages as read in the conversation
    connection.invoke("MarkAsRead", userId);
}

function appendMessage(content, isFromCurrentUser, timestamp = new Date()) {
    const messageElement = $("<div>")
        .addClass("message")
        .addClass(isFromCurrentUser ? "sent" : "received")
        .text(content);

    const timeElement = $("<small>")
        .addClass("timestamp")
        .text(new Date(timestamp).toLocaleTimeString());

    messageElement.append(timeElement);
    $("#messageThread").append(messageElement);
    $("#messageThread").scrollTop($("#messageThread")[0].scrollHeight);  // Scroll to the bottom
}

function updateUnreadCount(senderId) {
    const userItem = $(`.user-item[data-user-id="${senderId}"]`);
    const unreadCountElement = userItem.find(".unread-count");
    let count = parseInt(unreadCountElement.text()) || 0;
    unreadCountElement.text(++count).show();
}