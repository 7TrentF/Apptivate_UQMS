$(document).ready(function () {
    // Initialize tooltips
    initializeTooltips();

    // Initialize stats animation
    animateStatistics();

    // Initialize progress bars
    initializeProgressBars();

    // Initialize interactive features
    initializeInteractions();

    // Initialize table features
    initializeTableFeatures();
});

// Tooltip initialization
function initializeTooltips() {
    $('[data-tooltip]').each(function () {
        const $element = $(this);

        $element.hover(
            function (e) {
                const tooltipText = $(this).data('tooltip');
                const tooltip = $('<div class="custom-tooltip"></div>')
                    .text(tooltipText)
                    .appendTo('body');

                const elementOffset = $(this).offset();
                const tooltipWidth = tooltip.outerWidth();
                const tooltipHeight = tooltip.outerHeight();

                tooltip.css({
                    top: elementOffset.top - tooltipHeight - 10,
                    left: elementOffset.left - (tooltipWidth / 2) + ($(this).outerWidth() / 2)
                });

                tooltip.fadeIn(200);
            },
            function () {
                $('.custom-tooltip').remove();
            }
        );
    });
}

// Statistics animation
function animateStatistics() {
    $('.stat-card p').each(function () {
        const $this = $(this);
        const finalValue = parseInt($this.text().replace(/,/g, ''));

        $({ Counter: 0 }).animate({
            Counter: finalValue
        }, {
            duration: 2000,
            easing: 'swing',
            step: function () {
                $this.text(Math.ceil(this.Counter).toLocaleString());
            },
            complete: function () {
                $this.text(finalValue.toLocaleString());
            }
        });
    });
}

// Progress bars initialization
function initializeProgressBars() {
    $('.progress-bar').each(function () {
        const $bar = $(this);
        const percentage = $bar.data('percentage') || $bar.width();

        $bar.css('width', '0%').animate({
            width: percentage + '%'
        }, {
            duration: 1500,
            easing: 'easeInOutQuart'
        });
    });
}

// Table features initialization
function initializeTableFeatures() {
    // Enhanced Search with debouncing
    let searchTimeout;
    $('#searchInput').on('input', function () {
        clearTimeout(searchTimeout);
        const $this = $(this);

        searchTimeout = setTimeout(function () {
            const searchTerm = $this.val().toLowerCase();

            $('#usersTable tbody tr').each(function () {
                const $row = $(this);
                const text = $row.text().toLowerCase();

                if (text.includes(searchTerm)) {
                    $row.slideDown(300);
                } else {
                    $row.slideUp(300);
                }
            });
        }, 300);
    });

    // Enhanced Sorting
    $('#sortSelect').on('change', function () {
        const sortValue = $(this).val();
        const $tbody = $('#usersTable tbody');
        const rows = $tbody.find('tr').toArray();

        $tbody.fadeOut(200, function () {
            rows.sort((a, b) => {
                let aVal, bVal;

                switch (sortValue) {
                    case 'asc':
                    case 'desc':
                        aVal = $(a).find('td:first').text();
                        bVal = $(b).find('td:first').text();
                        break;
                    case 'email_asc':
                    case 'email_desc':
                        aVal = $(a).find('td:eq(1)').text();
                        bVal = $(b).find('td:eq(1)').text();
                        break;
                    case 'role_asc':
                    case 'role_desc':
                        aVal = $(a).find('td:eq(3)').text();
                        bVal = $(b).find('td:eq(3)').text();
                        break;
                }

                if (sortValue.includes('desc')) {
                    return bVal.localeCompare(aVal);
                }
                return aVal.localeCompare(bVal);
            });

            $tbody.empty().append(rows).fadeIn(200);
        });
    });

    // Row hover effects
    $('#usersTable tbody tr').hover(
        function () {
            $(this).addClass('row-hover');
        },
        function () {
            $(this).removeClass('row-hover');
        }
    );
}

// Initialize interactive features
function initializeInteractions() {
    // Enhanced Delete Confirmation
    $('.action-icon[title="Delete"]').click(function (e) {
        e.preventDefault();
        const $row = $(this).closest('tr');
        const userName = $row.find('td:first').text();

        showConfirmDialog({
            title: 'Confirm Deletion',
            message: `Are you sure you want to delete user "${userName}"?`,
            confirmText: 'Delete',
            cancelText: 'Cancel',
            onConfirm: () => {
                $row.fadeOut(400, function () {
                    showToast('User successfully deleted', 'success');
                    // Here you would typically make the actual delete request
                    // window.location.href = $(this).attr('href');
                });
            }
        });
    });

    // Add User Button Effect
    $('.add-user-btn').hover(
        function () {
            $(this).addClass('btn-hover');
        },
        function () {
            $(this).removeClass('btn-hover');
        }
    );
}

// Toast notification system
function showToast(message, type = 'info') {
    const toast = $(`
        <div class="toast toast-${type}">
            <div class="toast-content">
                <i class="fas fa-${type === 'success' ? 'check-circle' : 'info-circle'}"></i>
                <span>${message}</span>
            </div>
        </div>
    `);

    $('body').append(toast);

    setTimeout(() => {
        toast.addClass('show');

        setTimeout(() => {
            toast.removeClass('show');
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }, 100);
}

// Confirmation dialog
function showConfirmDialog({ title, message, confirmText, cancelText, onConfirm }) {
    const dialog = $(`
        <div class="confirm-dialog-overlay">
            <div class="confirm-dialog">
                <h3>${title}</h3>
                <p>${message}</p>
                <div class="confirm-dialog-buttons">
                    <button class="btn-cancel">${cancelText}</button>
                    <button class="btn-confirm">${confirmText}</button>
                </div>
            </div>
        </div>
    `).appendTo('body');

    dialog.find('.btn-confirm').click(() => {
        onConfirm();
        dialog.fadeOut(200, function () {
            $(this).remove();
        });
    });

    dialog.find('.btn-cancel').click(() => {
        dialog.fadeOut(200, function () {
            $(this).remove();
        });
    });

    dialog.fadeIn(200);
}

// Add custom easing function
$.easing.easeInOutQuart = function (x, t, b, c, d) {
    if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
    return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
};