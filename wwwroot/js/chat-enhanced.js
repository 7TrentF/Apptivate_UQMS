document.addEventListener('DOMContentLoaded', function () {
    // Create and insert chat header
    function createChatHeader() {
        const chatArea = document.querySelector('.chat-area');
        const chatHeader = document.createElement('div');
        chatHeader.className = 'chat-header';
        chatHeader.innerHTML = `
            <div class="chat-header-user">
                <div class="user-avatar">
                    <span id="activeUserInitials">...</span>
                </div>
                <div class="user-info">
                    <div class="user-name" id="activeUserName">Select a conversation</div>
                    <div class="user-status-text" id="activeUserStatus">-</div>
                </div>
            </div>
            <div class="chat-header-actions">
                <button class="header-action-button" title="Search conversation">
                    <svg width="20" height="20" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                    </svg>
                </button>
                <button class="header-action-button" title="More options">
                    <svg width="20" height="20" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 5v.01M12 12v.01M12 19v.01M12 6a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2z" />
                    </svg>
                </button>
            </div>
        `;
        chatArea.insertBefore(chatHeader, chatArea.firstChild);
    }

    // Enhance user list items with avatars and better structure
    function enhanceUserList() {
        const userItems = document.querySelectorAll('.user-item');
        userItems.forEach(item => {
            const userName = item.querySelector('.user-name').textContent;
            const initials = userName.split(' ').map(n => n[0]).join('');
            const statusClass = item.querySelector('.user-status').classList.contains('online') ? 'online' : 'offline';

            item.innerHTML = `
                <div class="user-avatar-wrapper">
                    <div class="user-avatar-small">${initials}</div>
                    <span class="user-status ${statusClass}"></span>
                </div>
                <div class="user-info">
                    <div class="user-name">${userName}</div>
                    <div class="user-preview">Click to start conversation</div>
                </div>
                <div class="user-meta">
                    <div class="unread-count d-none">0</div>
                </div>
            `;

            // Add ripple effect on click
            item.addEventListener('click', createRippleEffect);
        });
    }

    /*

    // Enhance input area with modern design and additional features
    function enhanceInputArea() {
        const inputGroup = document.querySelector('.input-group');
        inputGroup.className = 'input-area';
        inputGroup.innerHTML = `
            <div class="input-wrapper">
                <div class="input-actions">
                    <button class="input-action-button" id="emojiButton" title="Add emoji">
                        😊
                    </button>
                    <button class="input-action-button" title="Attach file">
                        <svg width="20" height="20" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.172 7l-6.586 6.586a2 2 0 102.828 2.828l6.414-6.586a4 4 0 00-5.656-5.656l-6.415 6.585a6 6 0 108.486 8.486L20.5 13" />
                        </svg>
                    </button>
                </div>
                <input type="text" id="messageInput" class="form-control" placeholder="Type a message...">
                <button id="sendButton" class="btn btn-primary" type="button">
                    <span>Send</span>
                    <svg width="20" height="20" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                    </svg>
                </button>
            </div>
        `;

        // Restore emoji picker functionality
        const emojiButton = document.getElementById('emojiButton');
        const emojiPicker = document.querySelector('emoji-picker');
        if (emojiButton && emojiPicker) {
            emojiButton.addEventListener('click', () => {
                emojiPicker.style.display = emojiPicker.style.display === 'none' ? 'block' : 'none';
            });
        }
    }
    */






    // Enhanced message display with animations and status indicators
    function enhanceMessageDisplay() {
        const originalAppendMessage = window.appendMessage;
        window.appendMessage = function (content, isFromCurrentUser, timestamp) {
            const messageThread = document.getElementById('messageThread');
            const messageElement = document.createElement('div');
            messageElement.className = `message ${isFromCurrentUser ? 'sent' : 'received'}`;

            messageElement.innerHTML = `
            <div class="message-content">${content}</div>
            <div class="message-meta">
                <span class="message-time">${new Date(timestamp).toLocaleTimeString()}</span>
                ${isFromCurrentUser ? `
                    <span class="message-status">
                        <svg width="16" height="16" fill="currentColor" viewBox="0 0 24 24">
                            <path d="M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41L9 16.17z"/>
                        </svg>
                    </span>
                ` : ''}
            </div>
        `;

            messageThread.appendChild(messageElement);

           
        };
    }
    // Utility function for ripple effect
    function createRippleEffect(event) {
        const button = event.currentTarget;
        const ripple = document.createElement('span');
        const rect = button.getBoundingClientRect();

        ripple.className = 'ripple';
        ripple.style.left = `${event.clientX - rect.left}px`;
        ripple.style.top = `${event.clientY - rect.top}px`;

        button.appendChild(ripple);
        setTimeout(() => ripple.remove(), 1000);
    }

    // Update active user information in header
    function updateActiveUser(userName, status) {
        const initials = userName.split(' ').map(n => n[0]).join('');
        document.getElementById('activeUserInitials').textContent = initials;
        document.getElementById('activeUserName').textContent = userName;
        document.getElementById('activeUserStatus').textContent = status ? 'Active now' : 'Offline';
        document.getElementById('activeUserStatus').className = `user-status-text ${status ? 'online' : 'offline'}`;
    }

    // Add typing indicator
    function showTypingIndicator() {
        const messageThread = document.getElementById('messageThread');
        const typingIndicator = document.createElement('div');
        typingIndicator.className = 'message received typing-indicator';
        typingIndicator.innerHTML = `
            <div class="typing-dots">
                <span></span>
                <span></span>
                <span></span>
            </div>
        `;
        messageThread.appendChild(typingIndicator);
        typingIndicator.scrollIntoView({ behavior: 'smooth', block: 'end' });
        return typingIndicator;
    }

    // Initialize enhanced features
    createChatHeader();
    enhanceUserList();
    enhanceInputArea();
    enhanceMessageDisplay();

    // Add event listeners for user interactions
    document.querySelectorAll('.user-item').forEach(item => {
        item.addEventListener('click', function () {
            const userName = this.querySelector('.user-name').textContent;
            const isOnline = this.querySelector('.user-status').classList.contains('online');
            updateActiveUser(userName, isOnline);
        });
    });

   
});