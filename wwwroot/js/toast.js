// wwwroot/js/toast.js
"use strict";

// Global toast container
function createToastContainer() {
    const container = document.createElement('div');
    container.id = 'toast-container';
    container.style.position = 'fixed';
    container.style.top = '20px';
    container.style.right = '20px';
    container.style.zIndex = '1000';
    document.body.appendChild(container);
    return container;
}

function showToast(message) {
    console.log('Attempting to show toast:', message);

    // Get or create toast container
    let container = document.getElementById('toast-container');
    if (!container) {
        container = createToastContainer();
    }

    // Create toast element
    const toast = document.createElement('div');
    toast.className = 'toast-notification';
    toast.textContent = message;

    // Add to container
    container.appendChild(toast);

    // Trigger animation
    setTimeout(() => {
        toast.classList.add('show');
    }, 10);

    // Remove after delay
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => {
            toast.remove();
        }, 300);
    }, 3000);
}

// Initialize test button
document.addEventListener('DOMContentLoaded', () => {
    console.log('Toast system initializing...');

    const testButton = document.createElement('button');
    testButton.textContent = 'Test Toast';
    testButton.style.position = 'fixed';
    testButton.style.bottom = '20px';
    testButton.style.right = '20px';
    testButton.style.padding = '10px 20px';
    testButton.style.backgroundColor = '#007bff';
    testButton.style.color = 'white';
    testButton.style.border = 'none';
    testButton.style.borderRadius = '5px';
    testButton.style.cursor = 'pointer';

    testButton.addEventListener('click', () => {
        console.log('Test button clicked');
        showToast('This is a test notification!');
    });

    document.body.appendChild(testButton);
    console.log('Toast system initialized');
});

// Initialize SignalR after toast system is working
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Debug)
    .build();

connection.on("ReceiveNotification", (message) => {
    console.log("SignalR message received:", message);
    showToast(message);
});

async function startSignalR() {
    try {
        await connection.start();
        console.log("SignalR Connected");
        showToast("SignalR Connected Successfully!");
    } catch (err) {
        console.error("SignalR Connection Error:", err);
        setTimeout(startSignalR, 5000);
    }
}

// Start SignalR after page loads
document.addEventListener('DOMContentLoaded', () => {
    startSignalR();
});