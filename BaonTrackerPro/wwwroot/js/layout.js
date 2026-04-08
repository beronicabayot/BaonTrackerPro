document.addEventListener('DOMContentLoaded', function () {
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('sidebarOverlay');
    const topbar = document.querySelector('.mobile-topbar');

    function setTopOffsetVar() {
        if (!topbar) return;
        const isMobile = window.matchMedia('(max-width: 767px)').matches;
        const h = isMobile ? Math.ceil(topbar.getBoundingClientRect().height) : 0;
        document.documentElement.style.setProperty('--app-top-offset', `${h}px`);
    }

    function closeSidebar() {
        sidebar?.classList.remove('open');
        overlay?.classList.remove('active');
    }

    sidebarToggle?.addEventListener('click', () => {
        sidebar?.classList.toggle('open');
        overlay?.classList.toggle('active');
    });

    overlay?.addEventListener('click', () => {
        closeSidebar();
    });

    document.querySelectorAll('#sidebar .nav-link').forEach(link => {
        link.addEventListener('click', () => {
            closeSidebar();
        });
    });

    // Keep content from hiding behind the fixed topbar.
    setTopOffsetVar();
    window.addEventListener('resize', () => {
        setTopOffsetVar();

        // If we leave mobile breakpoint, ensure drawer/overlay state is reset.
        const isMobile = window.matchMedia('(max-width: 767px)').matches;
        if (!isMobile) closeSidebar();
    });
});
