// Create this as a new file: wwwroot/js/dashboard.js

document.addEventListener('DOMContentLoaded', function () {
    initializeTabs();
    initializeCollapsibles();
    initializeLoadingStates();
    updateChartContainers();
});

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