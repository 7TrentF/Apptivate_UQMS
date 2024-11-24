document.querySelector('form').addEventListener('submit', async (e) => {
    e.preventDefault();

    try {
        const formData = new FormData(e.target);
        const response = await fetch('/Account/Register', {
            method: 'POST',
            body: formData
        });

        const responseData = await response.json();

        if (responseData.success) {
            showMessage(responseData.message, true);
            setTimeout(() => {
                window.location.href = '/Account/Login';
            }, 80000);
        } else {
            showMessage(responseData.message || 'Registration failed. Please try again.', false);
        }
    } catch (error) {
        showMessage('An error occurred during registration. Please try again.', false);
    }

    function showMessage(message, isSuccess) {
        const overlay = document.getElementById('messageOverlay');
        const content = document.getElementById('messageContent');

        content.style.color = isSuccess ? '#155724' : '#721c24';
        content.style.backgroundColor = isSuccess ? '#d4edda' : '#f8d7da';
        content.style.padding = '20px';
        content.style.borderRadius = '5px';
        content.style.marginBottom = '10px';

        content.textContent = message;
        overlay.style.display = 'flex';
    }

    document.querySelector('.close-button').addEventListener('click', hideMessage);

    function hideMessage() {
        const overlay = document.getElementById('messageOverlay');
        overlay.style.display = 'none';
    }
});
