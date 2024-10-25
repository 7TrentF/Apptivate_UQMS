// password-toggle.js
document.addEventListener('DOMContentLoaded', function () {
    // Password toggle functionality with improved UI
    const togglePassword = document.querySelectorAll('.toggle-password');

    togglePassword.forEach(toggle => {
        toggle.addEventListener('click', function () {
            const input = this.previousElementSibling;
            const type = input.getAttribute('type') === 'password' ? 'text' : 'password';
            input.setAttribute('type', type);
            this.textContent = type === 'password' ? '👁️' : '👁️‍🗨️';

            // Add ripple effect
            const ripple = document.createElement('span');
            ripple.className = 'ripple';
            this.appendChild(ripple);
            setTimeout(() => ripple.remove(), 1000);
        });
    });

    // Create and add password strength meter
    const passwordInput = document.getElementById('password');
    const strengthMeter = document.createElement('div');
    strengthMeter.className = 'password-strength-meter';
    passwordInput.parentElement.appendChild(strengthMeter);

    // Form sections for progress steps
    const sections = {
        personal: ['name', 'surname'],
        account: ['email', 'password', 'confirmPassword'],
        academic: ['department', 'course', 'year']
    };

    // Update progress steps based on field completion
    function updateProgress() {
        const steps = document.querySelectorAll('.progress-step');

        // Reset all steps
        steps.forEach(step => step.classList.remove('active'));

        // Check personal info section
        if (sections.personal.every(id => document.getElementById(id)?.value)) {
            steps[0].classList.add('active');
        }

        // Check account section
        if (sections.account.every(id => document.getElementById(id)?.value)) {
            steps[1].classList.add('active');
        }

        // Check academic section
        if (sections.academic.every(id => document.getElementById(id)?.value)) {
            steps[2].classList.add('active');
        }
    }

    // Advanced form validation with real-time feedback
    const form = document.querySelector('form');
    const inputs = form.querySelectorAll('input, select');

    function validateInput(input) {
        const value = input.value.trim();
        let isValid = true;
        let message = '';

        switch (input.id) {
            case 'email':
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                isValid = emailRegex.test(value);
                message = isValid ? '' : 'Please enter a valid email address';
                break;
            case 'password':
                isValid = value.length >= 8;
                message = isValid ? '' : 'Password must be at least 8 characters';
                updatePasswordStrength(value);
                break;
            case 'confirmPassword':
                const password = document.getElementById('password').value;
                isValid = value === password;
                message = isValid ? '' : 'Passwords do not match';
                break;
        }

        // Update input styling
        input.style.borderColor = isValid ? '' : '#e74c3c';

        // Update error message
        const errorSpan = input.nextElementSibling;
        if (errorSpan && errorSpan.classList.contains('text-danger')) {
            errorSpan.textContent = message;
            errorSpan.style.opacity = message ? '1' : '0';
        }

        return isValid;
    }

    // Password strength checker
    function updatePasswordStrength(password) {
        let strength = 0;
        const meter = document.querySelector('.password-strength-meter');

        // Length check
        if (password.length >= 8) strength++;
        // Uppercase check
        if (/[A-Z]/.test(password)) strength++;
        // Lowercase check
        if (/[a-z]/.test(password)) strength++;
        // Number check
        if (/[0-9]/.test(password)) strength++;
        // Special character check
        if (/[^A-Za-z0-9]/.test(password)) strength++;

        const percentage = (strength / 5) * 100;
        meter.style.setProperty('--strength', `${percentage}%`);

        // Update meter color based on strength
        const colors = ['#ff4d4d', '#ffa64d', '#ffff4d', '#4dff4d', '#4d4dff'];
        meter.style.backgroundColor = colors[strength - 1] || '#eee';
    }

    // Add input event listeners
    inputs.forEach(input => {
        input.addEventListener('input', function () {
            validateInput(this);
            updateProgress();
        });

        input.addEventListener('focus', function () {
            this.parentElement.classList.add('focused');
        });

        input.addEventListener('blur', function () {
            if (!this.value) {
                this.parentElement.classList.remove('focused');
            }
            validateInput(this);
        });
    });

    // Enhanced course loading with loading state and animation
    window.loadCourses = async function (departmentId) {
        const courseSelect = document.getElementById("course");
        const loadingOption = document.createElement('option');
        loadingOption.text = "Loading courses...";
        courseSelect.innerHTML = '';
        courseSelect.appendChild(loadingOption);
        courseSelect.disabled = true;

        if (departmentId) {
            try {
                const response = await fetch(`/Account/GetCoursesByDepartment?departmentId=${departmentId}`);

                if (!response.ok) throw new Error("Failed to fetch courses");

                const courses = await response.json();

                courseSelect.innerHTML = '<option value="">Select Course</option>';
                courses.forEach(course => {
                    const option = document.createElement("option");
                    option.value = course.courseID;
                    option.text = course.courseCode;
                    courseSelect.appendChild(option);
                });

                // Add fade-in animation
                courseSelect.style.opacity = '0';
                courseSelect.disabled = false;
                setTimeout(() => {
                    courseSelect.style.opacity = '1';
                }, 0);

            } catch (error) {
                console.error(error);
                courseSelect.innerHTML = '<option value="">Error loading courses</option>';
            }
        }
    };

    // Form submission handling with validation
   
});