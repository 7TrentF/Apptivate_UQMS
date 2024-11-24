// Create this as wwwroot/js/resolutionRateChart.js
function initializeResolutionRateChart() {
    const ctx = document.getElementById('resolutionRateChart').getContext('2d');
    let resolutionRateChart;

    function updateChart(days) {
        fetch(`/Dashboard/GetQueryResolutionRates?days=${days}`)
            .then(response => response.json())
            .then(data => {
                const labels = data.map(d => new Date(d.date).toLocaleDateString());
                const resolutionRates = data.map(d => parseFloat(d.resolutionRate.toFixed(1)));
                const totalQueries = data.map(d => d.totalQueries);
                const resolvedQueries = data.map(d => d.resolvedQueries);

                if (resolutionRateChart) {
                    resolutionRateChart.destroy();
                }

                resolutionRateChart = new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: labels,
                        datasets: [
                            {
                                label: 'Resolution Rate (%)',
                                data: resolutionRates,
                                borderColor: 'rgb(16, 185, 129)',
                                backgroundColor: 'rgba(16, 185, 129, 0.1)',
                                yAxisID: 'y',
                                tension: 0.4,
                                fill: true
                            },
                            {
                                label: 'Total Queries',
                                data: totalQueries,
                                borderColor: 'rgb(59, 130, 246)',
                                borderDash: [5, 5],
                                tension: 0.4,
                                yAxisID: 'y1'
                            },
                            {
                                label: 'Resolved Queries',
                                data: resolvedQueries,
                                borderColor: 'rgb(255, 165, 0)',
                                borderDash: [5, 5],
                                tension: 0.4,
                                yAxisID: 'y1'
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        interaction: {
                            intersect: false,
                            mode: 'index'
                        },
                        plugins: {
                            legend: {
                                position: 'top',
                            },
                            tooltip: {
                                callbacks: {
                                    label: function (context) {
                                        if (context.dataset.label === 'Resolution Rate (%)') {
                                            return `Resolution Rate: ${context.raw}%`;
                                        }
                                        return `${context.dataset.label}: ${context.raw}`;
                                    }
                                }
                            }
                        },
                        scales: {
                            y: {
                                type: 'linear',
                                display: true,
                                position: 'left',
                                title: {
                                    display: true,
                                    text: 'Resolution Rate (%)'
                                },
                                min: 0,
                                max: 100
                            },
                            y1: {
                                type: 'linear',
                                display: true,
                                position: 'right',
                                title: {
                                    display: true,
                                    text: 'Number of Queries'
                                },
                                min: 0,
                                grid: {
                                    drawOnChartArea: false
                                }
                            },
                            x: {
                                title: {
                                    display: true,
                                    text: 'Date'
                                }
                            }
                        }
                    }
                });
            });
    }

    // Event listeners for time period buttons
    document.querySelector('[data-resolution-period="7"]').addEventListener('click', () => {
        updateChart(7);
        toggleActiveButton(7);
    });

    document.querySelector('[data-resolution-period="30"]').addEventListener('click', () => {
        updateChart(30);
        toggleActiveButton(30);
    });

    function toggleActiveButton(days) {
        document.querySelectorAll('[data-resolution-period]').forEach(button => {
            button.classList.remove('bg-blue-500', 'text-white');
            button.classList.add('bg-gray-100');

            if (button.getAttribute('data-resolution-period') == days) {
                button.classList.remove('bg-gray-100');
                button.classList.add('bg-blue-500', 'text-white');
            }
        });
    }

    // Initial load
    updateChart(30);
}

document.addEventListener('DOMContentLoaded', initializeResolutionRateChart);