// Wait for DOM to load
document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const content = document.querySelector('.content');

    // Check for saved state
    const sidebarState = localStorage.getItem('sidebarState');
    if (sidebarState === 'collapsed') {
        sidebar.classList.add('collapsed');
        content?.classList.add('sidebar-collapsed');
    }

    // Toggle sidebar
    sidebarToggle.addEventListener('click', function () {
        sidebar.classList.toggle('collapsed');
        content?.classList.toggle('sidebar-collapsed');

        // Save state
        const isCollapsed = sidebar.classList.contains('collapsed');
        localStorage.setItem('sidebarState', isCollapsed ? 'collapsed' : 'expanded');
    });

    // Handle mobile responsive behavior
    const mobileToggle = document.createElement('button');
    mobileToggle.className = 'mobile-toggle d-md-none';
    mobileToggle.innerHTML = '<span></span>';
    document.body.appendChild(mobileToggle);

    mobileToggle.addEventListener('click', function () {
        sidebar.classList.toggle('show');
    });

    // Close sidebar when clicking outside on mobile
    document.addEventListener('click', function (event) {
        const isMobile = window.innerWidth < 768;
        if (isMobile && !sidebar.contains(event.target) && !mobileToggle.contains(event.target)) {
            sidebar.classList.remove('show');
        }
    });

    // Handle window resize
    window.addEventListener('resize', function () {
        if (window.innerWidth >= 768) {
            sidebar.classList.remove('show');
        }
    });
});

