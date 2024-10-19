const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

let currentReceiverId = null;
let lastMessageTimestamp = null;

connection.start().then(function () {
    console.log("SignalR Connected");
    loadInitialUserStatuses(); // Load initial user statuses after connection is established
}).catch(function (err) {
    return console.error(err.toString());
});

// Remove the pollUserStatus interval as we'll now use real-time updates
// setInterval(pollUserStatus, 5000);

setInterval(pollMessages, 3000);
setInterval(refreshMessageThread, 1500);

function pollMessages() {
    if (currentReceiverId) {
        $.get(`/Chat/GetMessages?userId=${currentReceiverId}`, function (data) {
            let newMessages = [];
            data.forEach(function (message) {
                if (!lastMessageTimestamp || new Date(message.timestamp) > new Date(lastMessageTimestamp)) {
                    newMessages.push(message);
                    lastMessageTimestamp = message.timestamp;
                }
            });
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
        $.get(`/Chat/GetMessages?userId=${currentReceiverId}`, function (data) {
            $("#messageThread").empty();
            lastMessageTimestamp = null;
            data.forEach(function (message) {
                appendMessage(message.content, message.isFromCurrentUser, message.timestamp);
                lastMessageTimestamp = message.timestamp;
            });
        });
    }
}

// Add this new function to handle real-time user status updates
connection.on("UserStatusChanged", function (userId, isOnline) {
    updateUserStatus(userId, isOnline);
});

function updateUserStatus(userId, isOnline) {
    const userItem = $(`.user-item[data-user-id="${userId}"]`);
    if (userItem.length) {
        userItem.find('.user-status').removeClass('online offline')
            .addClass(isOnline ? 'online' : 'offline');
    }
}

// Add this function to load initial user statuses
function loadInitialUserStatuses() {
    $.get('/api/users/statuses', function (statuses) {
        statuses.forEach(function (status) {
            updateUserStatus(status.userId, status.isOnline);
        });
    });
}

connection.on("ReceiveMessage", function (senderId, message) {
    if (senderId === currentReceiverId) {
        appendMessage(message.content, false, message.timestamp);
    } else {
        updateUnreadCount(senderId, message.unreadCount);
    }
});

connection.on("UnreadCountUpdated", function (senderId, unreadCount) {
    updateUnreadCount(senderId, unreadCount);
});

$("#userList").on("click", ".user-item", function () {
    const userId = $(this).data("user-id");
    console.log(`Clicked user with ID: ${userId}`);
    loadChat(userId);
    updateUnreadCount(userId, 0); // Reset unread count when opening chat
});

$("#sendButton").click(function () {
    const message = $("#messageInput").val();
    if (currentReceiverId && message) {
        connection.invoke("SendMessage", currentReceiverId, message).catch(function (err) {
            return console.error(err.toString());
        });
        appendMessage(message, true, new Date());
        $("#messageInput").val('');
        lastMessageTimestamp = new Date();
    }
});

function loadChat(userId) {
    currentReceiverId = userId;
    $("#messageThread").empty();
    $(".chat-area").show();
    lastMessageTimestamp = null;

    $.get(`/Chat/GetMessages?userId=${userId}`, function (data) {
        data.forEach(function (message) {
            appendMessage(message.content, message.isFromCurrentUser, message.timestamp);
            lastMessageTimestamp = message.timestamp;
        });
    });

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
    $("#messageThread").scrollTop($("#messageThread")[0].scrollHeight);
}

function updateUnreadCount(senderId, count) {
    const userItem = $(`.user-item[data-user-id="${senderId}"]`);
    const unreadCountElement = userItem.find(".unread-count");

    if (count > 0) {
        unreadCountElement.text(count).removeClass('d-none').addClass('d-inline-block');
    } else {
        unreadCountElement.removeClass('d-inline-block').addClass('d-none');
    }
}