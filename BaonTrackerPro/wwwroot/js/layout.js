document.addEventListener('DOMContentLoaded', function () {
    const sidebarToggleDesktop = document.getElementById('sidebarToggleDesktop');
    const sidebarToggleDesktopIcon = document.getElementById('sidebarToggleDesktopIcon');
    const sidebarToggleMobile = document.getElementById('sidebarToggleMobile');
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('sidebarOverlay');
    const topbar = document.querySelector('.mobile-topbar');
    const body = document.body;
    const desktopStateKey = 'sidebarCollapsedDesktop';

    function setTopOffsetVar() {
        if (!topbar) return;
        const isMobile = window.matchMedia('(max-width: 767px)').matches;
        const h = isMobile ? Math.ceil(topbar.getBoundingClientRect().height) : 0;
        document.documentElement.style.setProperty('--app-top-offset', `${h}px`);
    }

    function closeSidebar() {
        sidebar?.classList.remove('open');
        overlay?.classList.remove('active');
        sidebarToggleMobile?.setAttribute('aria-expanded', 'false');
    }

    function setDesktopCollapsed(collapsed) {
        body.classList.toggle('sidebar-collapsed', collapsed);
        body.classList.toggle('sidebar-expanded', !collapsed);
        sidebar?.classList.toggle('w-16', collapsed);
        sidebar?.classList.toggle('w-64', !collapsed);
        sidebarToggleDesktopIcon?.classList.toggle('iconoir-sidebar-expand', collapsed);
        sidebarToggleDesktopIcon?.classList.toggle('iconoir-sidebar-collapse', !collapsed);
        sidebarToggleDesktop?.setAttribute('aria-expanded', (!collapsed).toString());
    }

    function isMobileView() {
        return window.matchMedia('(max-width: 767px)').matches;
    }

    sidebarToggleDesktop?.addEventListener('click', () => {
        if (isMobileView()) {
            sidebar?.classList.toggle('open');
            overlay?.classList.toggle('active');
            const isOpen = sidebar?.classList.contains('open');
            sidebarToggleDesktop.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
            return;
        }

        const collapsed = !body.classList.contains('sidebar-collapsed');
        setDesktopCollapsed(collapsed);
        localStorage.setItem(desktopStateKey, collapsed ? '1' : '0');
    });

    sidebarToggleMobile?.addEventListener('click', () => {
        sidebar?.classList.toggle('open');
        overlay?.classList.toggle('active');
        const isOpen = sidebar?.classList.contains('open');
        sidebarToggleMobile.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
    });

    overlay?.addEventListener('click', () => {
        closeSidebar();
    });

    document.querySelectorAll('#sidebar .nav-link').forEach(link => {
        link.addEventListener('click', () => {
            closeSidebar();
        });
    });

    document.querySelectorAll('.bottom-nav .bottom-nav-item').forEach(link => {
        link.addEventListener('click', () => {
            closeSidebar();
        });
    });

    // Keep content from hiding behind the fixed topbar.
    setTopOffsetVar();
    const savedCollapsed = localStorage.getItem(desktopStateKey) === '1';
    setDesktopCollapsed(savedCollapsed);

    window.addEventListener('resize', () => {
        setTopOffsetVar();

        // If we leave mobile breakpoint, ensure drawer/overlay state is reset.
        if (!isMobileView()) closeSidebar();
    });
});
