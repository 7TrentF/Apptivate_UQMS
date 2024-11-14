// Create this as wwwroot/js/responseTimeChart.js
function initializeResponseTimeChart() {
    const ctx = document.getElementById('responseTimeChart').getContext('2d');
    let responseTimeChart;

    function updateChart(days) {
        fetch(`/Dashboard/GetResponseTimeTrends?days=${days}`)
            .then(response => response.json())
            .then(data => {
                const labels = data.map(d => new Date(d.date).toLocaleDateString());
                const values = data.map(d => (d.averageResponseTime / 60).toFixed(2)); // Convert to hours

                if (responseTimeChart) {
                    responseTimeChart.destroy();
                }

                responseTimeChart = new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: 'Average Response Time (Hours)',
                            data: values,
                            borderColor: 'rgb(59, 130, 246)',
                            backgroundColor: 'rgba(59, 130, 246, 0.1)',
                            tension: 0.4,
                            fill: true
                        }]
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            legend: {
                                position: 'top',
                            },
                            tooltip: {
                                callbacks: {
                                    label: function (context) {
                                        return `Response Time: ${context.raw} hours`;
                                    }
                                }
                            }
                        },
                        scales: {
                            y: {
                                beginAtZero: true,
                                title: {
                                    display: true,
                                    text: 'Hours'
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
    document.querySelector('[data-period="7"]').addEventListener('click', () => {
        updateChart(7);
        toggleActiveButton(7);
    });

    document.querySelector('[data-period="30"]').addEventListener('click', () => {
        updateChart(30);
        toggleActiveButton(30);
    });

    function toggleActiveButton(days) {
        document.querySelectorAll('[data-period]').forEach(button => {
            button.classList.remove('bg-blue-500', 'text-white');
            button.classList.add('bg-gray-100');

            if (button.getAttribute('data-period') == days) {
                button.classList.remove('bg-gray-100');
                button.classList.add('bg-blue-500', 'text-white');
            }
        });
    }

    // Initial load
    updateChart(30);
}

// Initialize when document is ready
document.addEventListener('DOMContentLoaded', initializeResponseTimeChart);