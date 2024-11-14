// Add smooth scrolling behavior
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        document.querySelector(this.getAttribute('href')).scrollIntoView({
            behavior: 'smooth'
        });
    });
});

// Animate stats on scroll
const animateStats = () => {
    const statCards = document.querySelectorAll('.stat-card');
    const observerOptions = {
        threshold: 0.1,
        root: null
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    statCards.forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'opacity 0.5s ease-out, transform 0.5s ease-out';
        observer.observe(card);
    });
};

// Table row hover effects
const initializeTableEffects = () => {
    const rows = document.querySelectorAll('.query-row');
    rows.forEach(row => {
        row.addEventListener('mouseenter', () => {
            row.style.transform = 'translateY(-2px)';
            row.style.boxShadow = '0 4px 6px -1px rgba(0, 0, 0, 0.1)';
        });

        row.addEventListener('mouseleave', () => {
            row.style.transform = 'translateY(0)';
            row.style.boxShadow = 'none';
        });
    });
};

// Notification system
const initializeNotifications = () => {
    const notificationIcon = document.querySelector('.notification-icon img');
    if (notificationIcon) {
        notificationIcon.addEventListener('click', () => {
            notificationIcon.style.transform = 'scale(1.1)';
            setTimeout(() => {
                notificationIcon.style.transform = 'scale(1)';
            }, 200);
        });
    }
};

// Show more functionality
const initializeShowMore = () => {
    const showMoreBtn = document.querySelector('.show-more-btn');
    if (showMoreBtn) {
        showMoreBtn.addEventListener('click', () => {
            showMoreBtn.textContent = 'Loading...';
            showMoreBtn.disabled = true;

            // Simulate loading
            setTimeout(() => {
                // Add new items to activity list
                const activityList = document.querySelector('.activity-list');
                if (activityList) {
                    const newItem = document.createElement('li');
                    newItem.innerHTML = `
                        <img src="/Images/profile-pic.png" alt="User">
                        <div>
                            <p>New activity item added</p>
                            <span>${new Date().toLocaleString()}</span>
                        </div>
                    `;
                    activityList.appendChild(newItem);
                }

                showMoreBtn.textContent = 'Show More';
                showMoreBtn.disabled = false;
            }, 1000);
        });
    }
};

// Handle responsive table
const initializeResponsiveTable = () => {
    const table = document.querySelector('.table-responsive');
    if (table) {
        let isDown = false;
        let startX;
        let scrollLeft;

        table.addEventListener('mousedown', (e) => {
            isDown = true;
            table.style.cursor = 'grabbing';
            startX = e.pageX - table.offsetLeft;
            scrollLeft = table.scrollLeft;
        });

        table.addEventListener('mouseleave', () => {
            isDown = false;
            table.style.cursor = 'default';
        });

        table.addEventListener('mouseup', () => {
            isDown = false;
            table.style.cursor = 'default';
        });

        table.addEventListener('mousemove', (e) => {
            if (!isDown) return;
            e.preventDefault();
            const x = e.pageX - table.offsetLeft;
            const walk = (x - startX) * 2;
            table.scrollLeft = scrollLeft - walk;
        });
    }
};

// Initialize everything when the DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    animateStats();
    initializeTableEffects();
    initializeNotifications();
    initializeShowMore();
    initializeResponsiveTable();
});

// Handle window resize
let resizeTimeout;
window.addEventListener('resize', () => {
    clearTimeout(resizeTimeout);
    resizeTimeout = setTimeout(() => {
        // Reinitialize any necessary responsive features
        initializeResponsiveTable();
    }, 250);
});