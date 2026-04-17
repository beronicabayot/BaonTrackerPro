document.addEventListener('DOMContentLoaded', () => {
    const MONTH_NAMES = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    let activeTrigger = null;
    let selectedMonth = null;
    let selectedYear = null;

    const picker = document.createElement('div');
    picker.className = 'custom-month-picker';
    picker.setAttribute('aria-hidden', 'true');
    picker.innerHTML = `
        <div class="cmp-header">
            <button type="button" class="cmp-year-nav" data-dir="-1" aria-label="Previous year">&#x2039;</button>
            <div class="cmp-year" id="cmpYearLabel"></div>
            <button type="button" class="cmp-year-nav" data-dir="1" aria-label="Next year">&#x203A;</button>
        </div>
        <div class="cmp-grid" id="cmpGrid"></div>
        <div class="cmp-footer">
            <button type="button" class="cmp-link-btn" data-action="clear">Clear</button>
            <button type="button" class="cmp-link-btn" data-action="today">This month</button>
        </div>
    `;
    document.body.appendChild(picker);

    const yearLabel = picker.querySelector('#cmpYearLabel');
    const grid = picker.querySelector('#cmpGrid');

    function parseMonthValue(value) {
        const [y, m] = (value || '').split('-').map(Number);
        if (!y || !m) {
            const now = new Date();
            return { year: now.getFullYear(), month: now.getMonth() + 1 };
        }
        return { year: y, month: m };
    }

    function formatMonthValue(year, month) {
        return `${year}-${String(month).padStart(2, '0')}`;
    }

    function getInput() {
        if (!activeTrigger) return null;
        const inputId = activeTrigger.getAttribute('data-month-input');
        return inputId ? document.getElementById(inputId) : null;
    }

    function navigateToMonth(value) {
        if (!activeTrigger) return;
        const template = activeTrigger.getAttribute('data-month-url');
        if (!template) return;
        window.location.href = template.replace('{value}', encodeURIComponent(value));
    }

    function renderMonths() {
        yearLabel.textContent = selectedYear;
        grid.innerHTML = MONTH_NAMES.map((name, index) => {
            const month = index + 1;
            const activeClass = selectedMonth === month ? 'active' : '';
            return `<button type="button" class="cmp-month ${activeClass}" data-month="${month}">${name}</button>`;
        }).join('');
    }

    function openPicker(trigger) {
        activeTrigger = trigger;
        const input = getInput();
        const parsed = parseMonthValue(input?.value);
        selectedYear = parsed.year;
        selectedMonth = parsed.month;
        renderMonths();

        const rect = trigger.getBoundingClientRect();
        picker.style.top = `${rect.bottom + window.scrollY + 8}px`;
        picker.style.left = `${Math.max(12, rect.left + window.scrollX)}px`;

        picker.classList.add('open');
        picker.setAttribute('aria-hidden', 'false');
    }

    function closePicker() {
        picker.classList.remove('open');
        picker.setAttribute('aria-hidden', 'true');
        activeTrigger = null;
    }

    document.querySelectorAll('.month-picker-trigger').forEach((trigger) => {
        trigger.addEventListener('click', (event) => {
            event.stopPropagation();
            openPicker(trigger);
        });

        trigger.addEventListener('keydown', (event) => {
            if (event.key === 'Enter' || event.key === ' ') {
                event.preventDefault();
                openPicker(trigger);
            }
        });
    });

    picker.addEventListener('click', (event) => {
        const monthButton = event.target.closest('.cmp-month');
        const yearNav = event.target.closest('.cmp-year-nav');
        const footerAction = event.target.closest('.cmp-link-btn');

        if (monthButton) {
            const month = Number(monthButton.getAttribute('data-month'));
            const value = formatMonthValue(selectedYear, month);
            const input = getInput();
            if (input) input.value = value;
            closePicker();
            navigateToMonth(value);
            return;
        }

        if (yearNav) {
            selectedYear += Number(yearNav.getAttribute('data-dir'));
            renderMonths();
            return;
        }

        if (footerAction) {
            const action = footerAction.getAttribute('data-action');
            if (action === 'today') {
                const now = new Date();
                const value = formatMonthValue(now.getFullYear(), now.getMonth() + 1);
                const input = getInput();
                if (input) input.value = value;
                closePicker();
                navigateToMonth(value);
                return;
            }

            if (action === 'clear') {
                closePicker();
            }
        }
    });

    document.addEventListener('click', (event) => {
        if (!picker.classList.contains('open')) return;
        if (picker.contains(event.target)) return;
        if (event.target.closest('.month-picker-trigger')) return;
        closePicker();
    });

    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape') {
            closePicker();
        }
    });
});
