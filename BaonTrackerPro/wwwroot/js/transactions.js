document.addEventListener('DOMContentLoaded', function () {

    // --- Segmented Toggle Logic ---
    const options = document.querySelectorAll('.segmented-option');
    const transactionTypeInput = document.getElementById('TransactionType');
    const amountInput = document.getElementById('Amount');

    // --- Category options per type ---
    const categoryOptions = {
        expense: [
            { value: 'Food & Drinks', label: '🍔 Food & Drinks' },
            { value: 'Transportation', label: '🚕 Transportation' },
            { value: 'Personal/Lifestyle', label: '👗 Personal/Lifestyle' },
            { value: 'Bills & Utilities', label: '📄 Bills & Utilities' },
            { value: 'Entertainment', label: '🎮 Entertainment' },
            { value: 'Education', label: '📚 Education' },
            { value: 'Health', label: '💊 Health' },
            { value: 'Others', label: '📦 Others' },
        ],
        income: [
            { value: 'Salary', label: '💼 Salary' },
            { value: 'Freelance', label: '💻 Freelance' },
            { value: 'Allowance', label: '🎒 Allowance' },
            { value: 'Gift', label: '🎁 Gift' },
            { value: 'Others', label: '📦 Others' },
        ]
    };

    function updateCategories(type) {
        const categorySelect = document.getElementById('Category');
        if (!categorySelect) return;
        categorySelect.innerHTML = '<option value="" disabled selected>Select category</option>';
        categoryOptions[type].forEach(opt => {
            const option = document.createElement('option');
            option.value = opt.value;
            option.textContent = opt.label;
            categorySelect.appendChild(option);
        });
    }

    function updateActive(activeType) {
        options.forEach(opt => {
            opt.classList.remove('active');
            if (opt.getAttribute('data-type') === activeType) {
                opt.classList.add('active');
            }
        });
        if (transactionTypeInput) transactionTypeInput.value = activeType;
        updateCategories(activeType); // swap categories on toggle
    }

    options.forEach(opt => {
        opt.addEventListener('click', () => {
            const type = opt.getAttribute('data-type');
            updateActive(type);
        });
    });

    // Set default active to Expense
    updateActive('expense');

    // --- Date/Time combination ---
    const datePicker = document.getElementById('DatePicker');
    const timePicker = document.getElementById('TimePicker');
    const dateHidden = document.getElementById('Date');
    const today = new Date();

    if (datePicker) {
        datePicker.value = today.toISOString().split('T')[0];
    }
    if (timePicker) {
        const hours = today.getHours().toString().padStart(2, '0');
        const minutes = today.getMinutes().toString().padStart(2, '0');
        timePicker.value = `${hours}:${minutes}`;
    }

    // --- Form submission ---
    const form = document.getElementById('addTransactionForm');
    if (form) {
        form.addEventListener('submit', function (e) {
            // Combine date + time into hidden Date field
            if (datePicker && timePicker && dateHidden) {
                const dateValue = datePicker.value;
                const timeValue = timePicker.value;
                if (dateValue && timeValue) {
                    dateHidden.value = `${dateValue}T${timeValue}:00`;
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

    // --- Modal reset when closed ---
    const modal = document.getElementById('addTransactionModal');
    if (modal) {
        modal.addEventListener('hidden.bs.modal', function () {
            updateActive('expense'); // resets to expense + reloads expense categories

            const descInput = document.getElementById('Description');
            if (descInput) descInput.value = '';
            if (amountInput) amountInput.value = '';

            const categorySelect = document.getElementById('Category');
            if (categorySelect) categorySelect.value = '';

            const notesInput = document.getElementById('Notes');
            if (notesInput) notesInput.value = '';

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