// Initialize charts
let resolutionChart = null;
let volumeChart = null;

document.addEventListener('DOMContentLoaded', function () {
    // Set default dates
    const today = new Date();
    const thirtyDaysAgo = new Date(today.getTime() - (30 * 24 * 60 * 60 * 1000));

    document.getElementById('startDate').valueAsDate = thirtyDaysAgo;
    document.getElementById('endDate').valueAsDate = today;

    // Initialize tabs
    $('#reportTabs a').on('click', function (e) {
        e.preventDefault();
        $(this).tab('show');
        refreshActiveTab();
    });

    // Refresh button click handler
    document.getElementById('refreshData').addEventListener('click', refreshActiveTab);

    // Load initial data
    refreshActiveTab();
});

function refreshActiveTab() {
    const activeTab = document.querySelector('#reportTabs .nav-link.active').getAttribute('href');
    const startDate = document.getElementById('startDate').value;
    const endDate = document.getElementById('endDate').value;

    switch (activeTab) {
        case '#resolution':
            loadResolutionTimeReport(startDate, endDate);
            break;
        case '#volume':
            loadHighVolumeReport(startDate, endDate);
            break;
        case '#unresolved':
            loadUnresolvedReport();
            break;
        case '#performance':
            loadStaffPerformanceReport(startDate, endDate);
            break;
    }
}

async function loadResolutionTimeReport(startDate, endDate) {
    try {
        const response = await fetch(`/Reports/QueryResolutionTime?startDate=${startDate}&endDate=${endDate}`);
        const data = await response.json();

        // Update table
        const tbody = document.querySelector('#resolutionTable tbody');
        tbody.innerHTML = '';
        data.forEach(item => {
            tbody.innerHTML += `
                <tr>
                    <td>${item.category}</td>
                    <td>${item.department}</td>
                    <td>${item.queryCount}</td>
                    <td>${item.averageResponseTime.toFixed(1)}</td>
                    <td>${item.medianResponseTime.toFixed(1)}</td>
                </tr>
            `;
        });

        // Update chart
        if (resolutionChart) {
            resolutionChart.destroy();
        }

        const ctx = document.getElementById('resolutionChart').getContext('2d');
        resolutionChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.map(item => item.category),
                datasets: [{
                    label: 'Average Response Time (hrs)',
                    data: data.map(item => item.averageResponseTime),
                    backgroundColor: 'rgba(54, 162, 235, 0.5)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                }, {
                    label: 'Median Response Time (hrs)',
                    data: data.map(item => item.medianResponseTime),
                    backgroundColor: 'rgba(255, 99, 132, 0.5)',
                    borderColor: 'rgba(255, 99, 132, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Hours'
                        }
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error loading resolution time report:', error);
    }
}

async function loadHighVolumeReport(startDate, endDate) {
    try {
        const response = await fetch(`/Reports/HighVolumeQueries?startDate=${startDate}&endDate=${endDate}`);
        const data = await response.json();

        // Update table
        const tbody = document.querySelector('#volumeTable tbody');
        tbody.innerHTML = '';
        data.forEach(item => {
            tbody.innerHTML += `
                <tr>
                    <td>${item.category}</td>
                    <td>${item.department}</td>
                    <td>${item.queryCount}</td>
                    <td>${item.percentage.toFixed(1)}%</td>
                </tr>
            `;
        });

        // Update chart
        if (volumeChart) {
            volumeChart.destroy();
        }

        const ctx = document.getElementById('volumeChart').getContext('2d');
        volumeChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: data[0].monthlyTrends.map(trend =>
                    moment(trend.month).format('MMM YYYY')),
                datasets: data.map(category => ({
                    label: category.category,
                    data: category.monthlyTrends.map(trend => trend.count),
                    borderColor: getRandomColor(),
                    fill: false
                }))
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Query Count'
                        }
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error loading high volume report:', error);
    }
}

async function loadUnresolvedReport() {
    try {
        const response = await fetch('/Reports/UnresolvedQueries');
        const data = await response.json();

        // Update summary cards
        document.getElementById('totalUnresolved').textContent = data.totalUnresolved;
        document.getElementById('totalOverdue').textContent = data.totalOverdue;
        document.getElementById('overduePercentage').textContent = `${data.overduePercentage.toFixed(1)}%`;

        // Update table
        const tbody = document.querySelector('#unresolvedTable tbody');
        tbody.innerHTML = '';
        data.overdueQueries.forEach(query => {
            tbody.innerHTML += `
                <tr>
                    <td>${query.ticketNumber}</td>
                    <td>${query.name}</td>
                    <td>${moment(query.submissionDate).format('MM/DD/YYYY')}</td>
                    <td>${query.pendingDays}</td>
                </tr>
            `;
        });
    } catch (error) {
        console.error('Error loading unresolved report:', error);
    }
}

async function loadStaffPerformanceReport(startDate, endDate) {
    try {
        const response = await fetch(`/Reports/StaffPerformance?startDate=${startDate}&endDate=${endDate}`);
        const data = await response.json();

        // Update table
        const tbody = document.querySelector('#performanceTable tbody');
        tbody.innerHTML = '';
        data.forEach(staff => {
            tbody.innerHTML += `
                <tr>
                    <td>${staff.staffName}</td>
                    <td>${staff.queriesHandled}</td>
                    <td>${staff.queriesResolved}</td>
                    <td>${staff.averageResponseTime.toFixed(1)}</td>
                    <td>${staff.resolutionRate.toFixed(1)}%</td>
                </tr>
            `;
        });
    } catch (error) {
        console.error('Error loading staff performance report:', error);
    }
}

function getRandomColor() {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}