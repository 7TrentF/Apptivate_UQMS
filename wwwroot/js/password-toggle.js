document.addEventListener('DOMContentLoaded', function () {
    const togglePassword = document.querySelector('.toggle-password');
    const passwordField = document.querySelector('#password');

    togglePassword.addEventListener('click', function () {
        // Toggle the type attribute using a ternary operator
        const type = passwordField.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordField.setAttribute('type', type);

        // Optionally, toggle the icon
        this.textContent = type === 'password' ? '👁️' : '🙈';
    });
});
