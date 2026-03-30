document.addEventListener('DOMContentLoaded', function() {
    // --- Custom Dropdown Logic ---
    const dropdownTriggers = document.querySelectorAll('.dropdown-trigger');
    const dropdownPanels = document.querySelectorAll('.dropdown-panel');
    const dropdownItems = document.querySelectorAll('.dropdown-item');

    function closeAllDropdowns() {
        dropdownPanels.forEach(panel => panel.classList.remove('open'));
    }

    dropdownTriggers.forEach(trigger => {
        trigger.addEventListener('click', (e) => {
            e.stopPropagation();
            const panel = trigger.nextElementSibling;
            if (panel.classList.contains('open')) {
                panel.classList.remove('open');
            } else {
                closeAllDropdowns();
                panel.classList.add('open');
            }
        });
    });

    dropdownItems.forEach(item => {
        item.addEventListener('click', (e) => {
            const value = item.getAttribute('data-value');
            const display = item.querySelector('span:not(.item-icon)')?.innerText || value;
            const trigger = item.closest('.custom-dropdown').querySelector('.dropdown-trigger');
            const selectedDisplay = trigger.querySelector('.selected-category-display');
            const hiddenInput = item.closest('.custom-dropdown').querySelector('input[name="Category"]');

            selectedDisplay.innerText = display;
            hiddenInput.value = value;

            const panel = item.closest('.dropdown-panel');
            panel.querySelectorAll('.dropdown-item').forEach(i => i.classList.remove('selected'));
            item.classList.add('selected');

            closeAllDropdowns();
            e.stopPropagation();
        });
    });

    document.addEventListener('click', (e) => {
        if (!e.target.closest('.custom-dropdown')) {
            closeAllDropdowns();
        }
    });

    // --- Segmented Toggle Logic ---
    const options = document.querySelectorAll('.segmented-option');
    const transactionTypeInput = document.getElementById('TransactionType');
    const amountInput = document.getElementById('Amount');

    function updateActive(activeType) {
        options.forEach(opt => {
            opt.classList.remove('active');
            if (opt.getAttribute('data-type') === activeType) {
                opt.classList.add('active');
            }
        });
        if (transactionTypeInput) transactionTypeInput.value = activeType;
    }

    options.forEach(opt => {
        opt.addEventListener('click', () => {
            const type = opt.getAttribute('data-type');
            updateActive(type);
        });
    });

    // --- Date/Time combination ---
    const datePicker = document.getElementById('DatePicker');      // visible date picker
    const timePicker = document.getElementById('TimePicker');      // visible time picker
    const dateHidden = document.getElementById('Date');            // hidden field that holds combined value
    const today = new Date();

    // Set default date & time to current values
    if (datePicker) {
        datePicker.value = today.toISOString().split('T')[0];
    }
    if (timePicker) {
        const hours = today.getHours().toString().padStart(2, '0');
        const minutes = today.getMinutes().toString().padStart(2, '0');
        timePicker.value = `${hours}:${minutes}`;
    }

    // --- Form submission: combine date/time and adjust amount sign ---
    const form = document.getElementById('addTransactionForm');
    if (form) {
        form.addEventListener('submit', function(e) {
            // Combine date and time into the hidden field
            if (datePicker && timePicker && dateHidden) {
                const dateValue = datePicker.value;
                const timeValue = timePicker.value;
                if (dateValue && timeValue) {
                    const combined = `${dateValue}T${timeValue}:00`;
                    dateHidden.value = combined;
                }
            }

            // Adjust amount sign based on Expense/Income
            let amount = parseFloat(amountInput.value);
            if (isNaN(amount)) amount = 0;
            const type = transactionTypeInput ? transactionTypeInput.value : 'expense';
            if (type === 'expense' && amount > 0) {
                amountInput.value = -Math.abs(amount);
            } else if (type === 'income' && amount < 0) {
                amountInput.value = Math.abs(amount);
            }
        });
    }

    // Set default active to Expense
    updateActive('expense');

    // --- Modal reset when closed ---
    const modal = document.getElementById('addTransactionModal');
    if (modal) {
        modal.addEventListener('hidden.bs.modal', function () {
            // Reset category dropdown
            const triggers = document.querySelectorAll('.dropdown-trigger');
            triggers.forEach(trigger => {
                const selectedDisplay = trigger.querySelector('.selected-category-display');
                if (selectedDisplay) selectedDisplay.innerText = 'Select category';
            });
            const hiddenInputs = document.querySelectorAll('input[name="Category"]');
            hiddenInputs.forEach(input => input.value = '');
            document.querySelectorAll('.dropdown-item').forEach(item => item.classList.remove('selected'));

            // Reset segmented control
            updateActive('expense');

            // Reset other fields
            const descInput = document.getElementById('Description');
            if (descInput) descInput.value = '';
            if (amountInput) amountInput.value = '';
            const notesInput = document.getElementById('Notes');
            if (notesInput) notesInput.value = '';

            // Reset date & time pickers to current values
            if (datePicker) datePicker.value = today.toISOString().split('T')[0];
            if (timePicker) {
                const hours = today.getHours().toString().padStart(2, '0');
                const minutes = today.getMinutes().toString().padStart(2, '0');
                timePicker.value = `${hours}:${minutes}`;
            }
            if (dateHidden) dateHidden.value = '';
        });
    }
});