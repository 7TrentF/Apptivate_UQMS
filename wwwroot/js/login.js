// password-toggle.js
document.addEventListener('DOMContentLoaded', function () {
    // Add particles background
    const particlesCount = 30;
    const colors = ['#6B73FF', '#000DFF', '#5D69BE', '#C89FEB'];
    const body = document.querySelector('.body');

    for (let i = 0; i < particlesCount; i++) {
        createParticle();
    }

    function createParticle() {
        const particle = document.createElement('div');
        particle.className = 'particle';

        const size = Math.random() * 15 + 5;
        const color = colors[Math.floor(Math.random() * colors.length)];

        particle.style.cssText = `
            position: absolute;
            width: ${size}px;
            height: ${size}px;
            background: ${color};
            border-radius: 50%;
            opacity: ${Math.random() * 0.2};
            pointer-events: none;
            top: ${Math.random() * 100}vh;
            left: ${Math.random() * 100}vw;
            transform: translate(-50%, -50%);
            animation: float ${Math.random() * 10 + 10}s linear infinite;
        `;

        body.appendChild(particle);
    }

    // Enhanced password toggle
    const togglePassword = document.querySelector('.toggle-password');
    const passwordInput = document.querySelector('#password');

    togglePassword.addEventListener('click', function () {
        const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordInput.setAttribute('type', type);

        // Update toggle icon with animation
        this.style.transform = 'translateY(-50%) scale(1.2)';
        this.textContent = type === 'password' ? '👁️' : '👁️‍🗨️';

        setTimeout(() => {
            this.style.transform = 'translateY(-50%) scale(1)';
        }, 200);
    });

    // Form validation with enhanced feedback
    const form = document.querySelector('form');
    const inputs = document.querySelectorAll('.form-control');

    inputs.forEach(input => {
        // Add focus effect
        input.addEventListener('focus', function () {
            this.parentElement.style.transform = 'scale(1.02)';
        });

        input.addEventListener('blur', function () {
            this.parentElement.style.transform = 'scale(1)';
        });

        // Add typing effect
        input.addEventListener('input', function () {
            if (this.value.length > 0) {
                this.style.boxShadow = '0 8px 16px rgba(107, 115, 255, 0.2)';
            } else {
                this.style.boxShadow = '';
            }
        });
    });

    form.addEventListener('submit', function (e) {
        const email = document.querySelector('#email');
        const password = document.querySelector('#password');
        let isValid = true;

        if (!isValidEmail(email.value)) {
            e.preventDefault();
            showError(email, 'Please enter a valid email address');
            isValid = false;
        }

        if (password.value.length < 6) {
            e.preventDefault();
            showError(password, 'Password must be at least 6 characters');
            isValid = false;
        }

        if (isValid) {
            // Add loading state to button
            const button = this.querySelector('.btn');
            button.innerHTML = '<span class="loading">Logging in...</span>';
            button.style.background = 'linear-gradient(135deg, #5D69BE 0%, #6B73FF 100%)';
        }
    });

    function isValidEmail(email) {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    }

    function showError(input, message) {
        const formGroup = input.parentElement;

        // Remove existing error
        const existingError = formGroup.querySelector('.error-message');
        if (existingError) {
            existingError.remove();
        }

        // Add new error with animation
        const errorDiv = document.createElement('div');
        errorDiv.className = 'error-message';
        errorDiv.textContent = message;

        formGroup.appendChild(errorDiv);

        // Add error styling
        input.style.borderColor = '#FF4B4B';
        input.style.boxShadow = '0 4px 12px rgba(255, 75, 75, 0.1)';

        // Remove error after 3 seconds
        setTimeout(() => {
            errorDiv.remove();
            input.style.borderColor = '';
            input.style.boxShadow = '';
        }, 3000);
    }

    // Add keyframe animation for loading
    const style = document.createElement('style');
    style.textContent = `
        @keyframes float {
            0% { transform: translate(-50%, -50%) translateY(0); }
            50% { transform: translate(-50%, -50%) translateY(-20px); }
            100% { transform: translate(-50%, -50%) translateY(0); }
        }
        
        .loading {
            display: inline-block;
            position: relative;
        }
        
        .loading::after {
            content: '...';
            position: absolute;
            animation: loading 1.5s infinite;
            margin-left: 5px;
        }
        
        @keyframes loading {
            0% { content: '.'; }
            33% { content: '..'; }
            66% { content: '...'; }
        }
    `;
    document.head.appendChild(style);
});