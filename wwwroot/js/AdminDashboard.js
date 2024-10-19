// Wait for the document to be ready
$(document).ready(function () {
    // Initialize search functionality
    const searchInput = $('#searchInput');
    const usersTable = $('#usersTable');

    searchInput.on('input', function () {
        const searchTerm = $(this).val().toLowerCase();

        $('#usersTable tbody tr').each(function () {
            const row = $(this);
            const text = row.text().toLowerCase();
            row.toggle(text.indexOf(searchTerm) > -1);
        });
    });

    // Initialize sorting
    $('#sortSelect').on('change', function () {
        const sortValue = $(this).val();
        const tbody = $('#usersTable tbody');
        const rows = tbody.find('tr').toArray();

        rows.sort(function (a, b) {
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

        tbody.empty();
        tbody.append(rows);
    });

    // Animate statistics on page load
    $('.stat-card').each(function (index) {
        $(this).delay(100 * index).animate({
            opacity: 1,
            top: 0
        }, 500);
    });

    // Progress bar animation
    $('.progress-bar').each(function () {
        const bar = $(this);
        const width = bar.css('width');
        bar.css('width', '0');
        setTimeout(() => {
            bar.css('width', width);
        }, 100);
    });

    // Add smooth hover effects to table rows
    $('#usersTable tbody tr').hover(
        function () {
            $(this).css('transition', 'background-color 0.3s ease');
        },
        function () {
            $(this).css('transition', 'background-color 0.3s ease');
        }
    );

    // Add confirmation dialog for delete action
    $('.action-icon[title="Delete"]').click(function (e) {
        if (!confirm('Are you sure you want to delete this user? This action cannot be undone.')) {
            e.preventDefault();
        }
    });

    // Add tooltip functionality
    $('[title]').tooltip({
        placement: 'top',
        trigger: 'hover'
    });

    // Add responsive table handling
    $(window).on('resize', function () {
        if ($(window).width() < 768) {
            $('.table-responsive').css('max-height', '500px');
        } else {
            $('.table-responsive').css('max-height', 'none');
        }
    }).trigger('resize');
});