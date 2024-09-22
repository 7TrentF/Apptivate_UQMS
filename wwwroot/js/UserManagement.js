$(document).ready(function () {
    // Handle Search Functionality
    $('#searchInput').on('keyup', function () {
        var value = $(this).val().toLowerCase();
        $('#usersTable tbody tr').filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    // Handle Sort Functionality
    $('#sortSelect').change(function () {
        var sortOrder = $(this).val();
        var table = $('#usersTable tbody');
        var rows = table.find('tr').get();

        rows.sort(function (a, b) {
            var keyA, keyB;
            switch (sortOrder) {
                case 'asc':
                case 'desc':
                    keyA = $(a).children('td').eq(0).text().toUpperCase();
                    keyB = $(b).children('td').eq(0).text().toUpperCase();
                    break;
                case 'email_asc':
                case 'email_desc':
                    keyA = $(a).children('td').eq(1).text().toUpperCase();
                    keyB = $(b).children('td').eq(1).text().toUpperCase();
                    break;
                case 'role_asc':
                case 'role_desc':
                    keyA = $(a).children('td').eq(3).text().toUpperCase();
                    keyB = $(b).children('td').eq(3).text().toUpperCase();
                    break;
                default:
                    keyA = $(a).children('td').eq(0).text().toUpperCase();
                    keyB = $(b).children('td').eq(0).text().toUpperCase();
            }

            if (sortOrder.includes('asc')) {
                return (keyA < keyB) ? -1 : (keyA > keyB) ? 1 : 0;
            } else if (sortOrder.includes('desc')) {
                return (keyA > keyB) ? -1 : (keyA < keyB) ? 1 : 0;
            }
        });

        $.each(rows, function (index, row) {
            table.append(row);
        });
    });
});
