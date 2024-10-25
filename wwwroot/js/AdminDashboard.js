// AdminDashboard.js
document.addEventListener('DOMContentLoaded', () => {
    initializeAnimations();
    initializeCharts();
    initializeInteractions();
});

function initializeAnimations() {
    // Animate stats counters
    const stats = document.querySelectorAll('.stat-item p');
    stats.forEach(stat => {
        const finalValue = parseInt(stat.textContent.replace(/,/g, ''));
        animateCounter(stat, finalValue);
    });

    // Animate progress bars
    const progressBars = document.querySelectorAll('.progress-bar');
    progressBars.forEach(bar => {
        const percentage = bar.style.width;
        bar.style.width = '0%';
        setTimeout(() => {
            bar.style.width = percentage;
        }, 100);
    });
}

function animateCounter(element, finalValue) {
    const duration = 2000;
    const steps = 60;
    const stepValue = finalValue / steps;
    let currentValue = 0;
    let currentStep = 0;

    const animate = () => {
        currentStep++;
        currentValue += stepValue;

        if (currentStep === steps) {
            element.textContent = finalValue.toLocaleString();
        } else {
            element.textContent = Math.round(currentValue).toLocaleString();
            requestAnimationFrame(animate);
        }
    };

    requestAnimationFrame(animate);
}

function initializeCharts() {
    // Add any chart initializations here
    // For example, using Chart.js or other visualization libraries
}

function initializeInteractions() {
    // Stats hover effects
    const statItems = document.querySelectorAll('.stat-item');
    statItems.forEach(item => {
        item.addEventListener('mouseover', () => {
            item.style.transform = 'translateY(-4px)';
        });

        item.addEventListener('mouseout', () => {
            item.style.transform = 'translateY(0)';
        });
    });

    // Activity list interactions
    const activityItems = document.querySelectorAll('.activity-list li');
    activityItems.forEach(item => {
        item.addEventListener('mouseenter', () => {
            item.style.backgroundColor = '#f8fafc';
        });

        item.addEventListener('mouseleave', () => {
            item.style.backgroundColor = 'transparent';
        });
    });

    // Show more button functionality
    const showMoreBtn = document.querySelector('.show-more-btn');
    if (showMoreBtn) {
        const activityList = document.querySelector('.activity-list');
        let isExpanded = false;

        showMoreBtn.addEventListener('click', () => {
            isExpanded = !isExpanded;

            if (isExpanded) {
                activityList.style.maxHeight = `${activityList.scrollHeight}px`;
                showMoreBtn.textContent = 'Show Less';
            } else {
                activityList.style.maxHeight = '400px';
                showMoreBtn.textContent = 'Show More';
            }
        });
    }

    // Add smooth scroll behavior
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            document.querySelector(this.getAttribute('href')).scrollIntoView({
                behavior: 'smooth'
            });
        });
    });
}

// Handle responsive layout changes
const resizeObserver = new ResizeObserver(entries => {
    entries.forEach(entry => {
        const width = entry.contentRect.width;
        const container = entry.target;

        if (width < 768) {
            container.classList.add('mobile-layout');
        } else {
            container.classList.remove('mobile-layout');
        }
    });
});

const container = document.querySelector('.container');
if (container) {
    resizeObserver.observe(container);
}

// Add smooth loading transitions
window.addEventListener('load', () => {
    document.body.classList.add('loaded');

    // Stagger the appearance of stat items
    const stats = document.querySelectorAll('.stat-item');
    stats.forEach((stat, index) => {
        setTimeout(() => {
            stat.style.opacity = '1';
            stat.style.transform = 'translateY(0)';
        }, 100 * index);
    });
});