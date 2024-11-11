// AdminDashboard.js
document.addEventListener('DOMContentLoaded', () => {
    initializeAnimations();
    initializeCharts();
    initializeInteractions();


    initializeTabs();
    initializeCollapsibles();
    initializeLoadingStates();
    updateChartContainers();
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



function initializeTabs() {
    const tabButtons = document.querySelectorAll('.tab-button');
    const tabContents = document.querySelectorAll('.tab-content');

    // Show first tab by default
    if (tabContents.length > 0) {
        tabContents[0].classList.add('active');
    }

    tabButtons.forEach(button => {
        button.addEventListener('click', () => {
            const tabId = button.dataset.tab;

            // Update buttons
            tabButtons.forEach(btn => {
                btn.classList.remove('border-blue-500', 'text-blue-600');
                btn.classList.add('border-transparent', 'text-gray-500');
            });
            button.classList.add('border-blue-500', 'text-blue-600');
            button.classList.remove('border-transparent', 'text-gray-500');

            // Update tab contents
            tabContents.forEach(content => {
                content.classList.remove('active');
                if (content.id === tabId) {
                    content.classList.add('active');
                    // Reinitialize charts in the newly active tab
                    reinitializeCharts(content);
                }
            });
        });
    });
}

function initializeCollapsibles() {
    const collapseButtons = document.querySelectorAll('.collapse-button');
    collapseButtons.forEach(button => {
        button.addEventListener('click', () => {
            const content = button.closest('.chart-container').querySelector('.collapsible-content');
            button.classList.toggle('collapsed');

            if (content.style.maxHeight) {
                content.style.maxHeight = null;
            } else {
                content.style.maxHeight = content.scrollHeight + "px";
            }
        });
    });
}

function initializeLoadingStates() {
    const chartContainers = document.querySelectorAll('.chart-container');
    chartContainers.forEach(container => {
        // Add loading overlay to each chart container
        const loading = document.createElement('div');
        loading.className = 'loading';
        loading.innerHTML = '<i class="fas fa-circle-notch fa-spin fa-2x text-blue-500"></i>';
        container.appendChild(loading);
    });
}

function showLoading(chartContainer) {
    const loading = chartContainer.querySelector('.loading');
    if (loading) {
        loading.style.display = 'flex';
    }
}

function hideLoading(chartContainer) {
    const loading = chartContainer.querySelector('.loading');
    if (loading) {
        loading.style.display = 'none';
    }
}

function updateChartContainers() {
    const chartContainers = document.querySelectorAll('.chart-container');
    chartContainers.forEach(container => {
        // Show loading state
        showLoading(container);

        // Simulate data loading (replace with your actual data fetching)
        setTimeout(() => {
            hideLoading(container);
        }, 1000);
    });
}

function reinitializeCharts(tabContent) {
    const charts = tabContent.querySelectorAll('canvas');
    charts.forEach(chart => {
        // Get the chart instance if it exists
        const chartInstance = Chart.getChart(chart);
        if (chartInstance) {
            // Update or reinitialize the chart
            chartInstance.update();
        }
    });
}

// Add this if you want to periodically refresh data
function setupAutoRefresh(refreshInterval = 300000) { // 5 minutes default
    setInterval(() => {
        const activeTab = document.querySelector('.tab-content.active');
        if (activeTab) {
            const charts = activeTab.querySelectorAll('.chart-container');
            charts.forEach(container => {
                showLoading(container);
                // Implement your data refresh logic here
                setTimeout(() => hideLoading(container), 1000);
            });
        }
    }, refreshInterval);
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