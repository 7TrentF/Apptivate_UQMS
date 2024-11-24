document.addEventListener('DOMContentLoaded', function () {
    // Add click handler for close buttons
    document.querySelectorAll('.close-alert').forEach(button => {
        button.addEventListener('click', function () {
            const alert = this.closest('.alert');
            alert.style.opacity = '0';
            setTimeout(() => {
                alert.style.display = 'none';
            }, 150);
        });
    });
});