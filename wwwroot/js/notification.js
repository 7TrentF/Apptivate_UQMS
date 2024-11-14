// wwwroot/js/notification.js
"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Debug)  // Set to Debug for more detailed logs
    .build();

// Test function to verify toast display
function testToast() {
    showToast("Test notification - If you see this, the toast display is working!");
}

function showToast(message) {
    console.log("Showing toast for message:", message);

    const toast = document.createElement('div');
    toast.classList.add('toast');
    toast.textContent = message;
    document.body.appendChild(toast);

    // Force a reflow to ensure animation plays
    toast.offsetHeight;

    // Add visible class to trigger animation
    toast.classList.add('toast-visible');

    setTimeout(() => {
        toast.classList.add('toast-hiding');
        setTimeout(() => {
            toast.remove();
        }, 500); // Match this with CSS animation duration
    }, 5000);
}

// Verify message receiving
connection.on("ReceiveNotification", (message) => {
    console.log("ReceiveNotification event triggered with message:", message);
    showToast(message);
});

async function startConnection() {
    try {
        await connection.start();
        console.log("SignalR Connected successsssssssssfully");

        // Test that the connection can receive messages
        connection.invoke("TestConnection").catch(err =>
            console.error("Test connection failed:", err)
        );

    } catch (err) {
        console.error("SignalR Connection Error:", err);
        setTimeout(startConnection, 5000);
    }
}

// Start the connection when the document is ready
document.addEventListener('DOMContentLoaded', () => {
    startConnection();

    // Add a test button to the page (for debugging)
    const testButton = document.createElement('button');
    testButton.textContent = "Test Toast";
    testButton.style.position = 'fixed';
    testButton.style.bottom = '20px';
    testButton.style.right = '20px';
    testButton.onclick = testToast;
    document.body.appendChild(testButton);
});

// Add event listener for page visibility changes
document.addEventListener('visibilitychange', () => {
    if (document.visibilityState === 'visible') {
        // Reconnect if the page becomes visible and connection is lost
        if (connection.state === signalR.HubConnectionState.Disconnected) {
            startConnection();
        }
    }
});