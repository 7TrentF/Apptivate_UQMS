document.addEventListener('DOMContentLoaded', function () {
    // Add hover effect to document links
    const documentLinks = document.querySelectorAll('.document-link');
    documentLinks.forEach(link => {
        link.addEventListener('mouseover', function () {
            this.style.transform = 'translateX(5px)';
        });
        link.addEventListener('mouseout', function () {
            this.style.transform = 'translateX(0)';
        });
    });

    // Add smooth scroll behavior
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            document.querySelector(this.getAttribute('href')).scrollIntoView({
                behavior: 'smooth'
            });
        });
    });

    // Animate cards on page load
    const cards = document.querySelectorAll('.details-card');
    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        setTimeout(() => {
            card.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, 100 * index);
    });
});