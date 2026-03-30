let transactionToDelete = null;

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

            // Reset modal title and button text
            const modalTitle = document.getElementById('addTransactionModalLabel');
            if (modalTitle) modalTitle.innerText = 'Add Transaction';
            const submitBtn = document.querySelector('.add-transaction-btn');
            if (submitBtn) submitBtn.innerText = 'Add Transaction';
            const transactionId = document.getElementById('TransactionId');
            if (transactionId) transactionId.value = '0';
        });
    }

    // --- Delete confirmation modal logic ---
    const confirmDeleteBtn = document.getElementById('confirmDeleteBtn');
    if (confirmDeleteBtn) {
        confirmDeleteBtn.addEventListener('click', function() {
            if (transactionToDelete !== null) {
                deleteTransaction(transactionToDelete);
                const deleteModal = bootstrap.Modal.getInstance(document.getElementById('deleteConfirmationModal'));
                deleteModal.hide();
                transactionToDelete = null;
            }
        });
    }

    // --- NEW: Type filter dropdown logic ---
    const filterDropdown = document.querySelector('.filter-dropdown');
    if (filterDropdown) {
        const filterItems = filterDropdown.querySelectorAll('.dropdown-item');
        const filterTrigger = filterDropdown.querySelector('.dropdown-trigger .selected-category-display');
        const selectedTypeInput = document.getElementById('selectedType');
        const filterForm = document.getElementById('typeFilterForm');

        if (filterForm && selectedTypeInput) {
            function updateFilter(selectedValue, selectedText) {
                if (filterTrigger) filterTrigger.innerText = selectedText;
                selectedTypeInput.value = selectedValue;
                filterForm.submit();
            }

            function setSelectedItem(selectedValue) {
                filterItems.forEach(item => {
                    item.classList.remove('selected');
                    if (item.getAttribute('data-value') === selectedValue) {
                        item.classList.add('selected');
                    }
                });
            }

            // Attach click handlers
            filterItems.forEach(item => {
                item.addEventListener('click', (e) => {
                    e.stopPropagation();
                    const value = item.getAttribute('data-value');
                    const displayText = item.innerText.trim(); // "All Types", "Income", "Expense"
                    setSelectedItem(value);
                    updateFilter(value, displayText);
                    // Close the dropdown panel
                    const panel = item.closest('.dropdown-panel');
                    if (panel) panel.classList.remove('open');
                });
            });

            // Highlight the current selection on page load (based on the trigger's current text)
            const currentTriggerText = filterTrigger ? filterTrigger.innerText : '';
            let currentValue = '';
            if (currentTriggerText === 'Income') currentValue = 'Income';
            else if (currentTriggerText === 'Expense') currentValue = 'Expense';
            else currentValue = ''; // All Types
            setSelectedItem(currentValue);
            if (selectedTypeInput) selectedTypeInput.value = currentValue;
        }
    }

}); // end of DOMContentLoaded

// --- Function to show the custom delete modal ---
function showDeleteModal(id) {
    transactionToDelete = id;
    const deleteModal = new bootstrap.Modal(document.getElementById('deleteConfirmationModal'));
    deleteModal.show();
}

// --- Function to load transaction for editing (must be globally accessible) ---
function editTransaction(id) {
    fetch(`/Transactions/GetTransaction?id=${id}`)
        .then(response => response.json())
        .then(data => {
            // Populate fields
            document.getElementById('Description').value = data.description;
            document.getElementById('Amount').value = data.amount;
            document.getElementById('DatePicker').value = data.date;
            document.getElementById('TimePicker').value = data.time;
            document.getElementById('Notes').value = data.notes || '';

            // Set category dropdown
            const categorySelect = document.getElementById('Category');
            if (categorySelect) {
                categorySelect.value = data.category;
                // Force change event to ensure category options are updated (if needed)
                categorySelect.dispatchEvent(new Event('change'));
            }

            // Set Expense/Income toggle
            const isExpense = data.isExpense;
            const expenseOption = document.querySelector('.segmented-option[data-type="expense"]');
            const incomeOption = document.querySelector('.segmented-option[data-type="income"]');
            if (isExpense) {
                if (expenseOption) expenseOption.click();
            } else {
                if (incomeOption) incomeOption.click();
            }

            // Set hidden Id field
            const transactionId = document.getElementById('TransactionId');
            if (transactionId) transactionId.value = data.id;

            // Change modal title and button text
            const modalTitle = document.getElementById('addTransactionModalLabel');
            if (modalTitle) modalTitle.innerText = 'Edit Transaction';
            const submitBtn = document.querySelector('.add-transaction-btn');
            if (submitBtn) submitBtn.innerText = 'Update Transaction';

            // Open modal
            const modal = new bootstrap.Modal(document.getElementById('addTransactionModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error loading transaction:', error);
            alert('Failed to load transaction data.');
        });
}

// --- Function to delete transaction (fetch only, no confirm) ---
function deleteTransaction(id) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    fetch(`/Transactions/Delete/${id}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: new URLSearchParams({
            '__RequestVerificationToken': token
        })
    })
    .then(response => {
        if (response.ok) {
            window.location.reload();
        } else {
            response.text().then(text => alert(`Failed to delete: ${text}`));
        }
    })
    .catch(error => {
        console.error('Error:', error);
        alert('An error occurred.');
    });
}