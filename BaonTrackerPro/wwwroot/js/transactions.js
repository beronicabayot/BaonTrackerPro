let transactionToDelete = null;

document.addEventListener('DOMContentLoaded', function () {

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
            { value: 'Bonus', label: '🎉 Bonus' },
            { value: 'Gift', label: '🎁 Gift' },
            { value: 'Others', label: '📦 Others' },
        ]
    };

    function updateCategories(type, selectedValue = '') {
        const categorySelect = document.getElementById('Category');
        if (!categorySelect) return;
        categorySelect.innerHTML = '<option value="" disabled selected>Select category</option>';
        categoryOptions[type].forEach(opt => {
            const option = document.createElement('option');
            option.value = opt.value;
            option.textContent = opt.label;
            if (opt.value === selectedValue) option.selected = true;
            categorySelect.appendChild(option);
        });

        // Hide others input when categories change
        const othersDiv = document.getElementById('transactionOthersInput');
        const othersInput = document.getElementById('transactionOthersSpecify');
        if (othersDiv) othersDiv.style.display = 'none';
        if (othersInput) othersInput.value = '';

        // If selected value is not in list (custom Others), show specify field
        const knownValues = categoryOptions[type].map(o => o.value);
        if (selectedValue && !knownValues.includes(selectedValue)) {
            if (othersDiv) othersDiv.style.display = 'block';
            if (othersInput) othersInput.value = selectedValue;
            // Add custom option
            const customOption = document.createElement('option');
            customOption.value = selectedValue;
            customOption.textContent = selectedValue;
            customOption.selected = true;
            categorySelect.appendChild(customOption);
        }
    }

    // --- Segmented Toggle Logic ---
    const options = document.querySelectorAll('.segmented-option');
    const transactionTypeInput = document.getElementById('TransactionType');
    const amountInput = document.getElementById('Amount');

    function updateActive(activeType, selectedCategory = '') {
        options.forEach(opt => {
            opt.classList.remove('active');
            if (opt.getAttribute('data-type') === activeType) {
                opt.classList.add('active');
            }
        });
        if (transactionTypeInput) transactionTypeInput.value = activeType;
        updateCategories(activeType, selectedCategory);
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

    if (datePicker) datePicker.value = today.toISOString().split('T')[0];
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
            updateActive('expense');

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

            // Reset others input
            const othersDiv = document.getElementById('transactionOthersInput');
            const othersInput = document.getElementById('transactionOthersSpecify');
            if (othersDiv) othersDiv.style.display = 'none';
            if (othersInput) othersInput.value = '';

            // Reset modal title and button
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
        confirmDeleteBtn.addEventListener('click', function () {
            if (transactionToDelete !== null) {
                deleteTransaction(transactionToDelete);
                const deleteModal = bootstrap.Modal.getInstance(document.getElementById('deleteConfirmationModal'));
                deleteModal.hide();
                transactionToDelete = null;
            }
        });
    }

    // --- Type filter dropdown logic ---
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

            filterItems.forEach(item => {
                item.addEventListener('click', (e) => {
                    e.stopPropagation();
                    const value = item.getAttribute('data-value');
                    const displayText = item.innerText.trim();
                    setSelectedItem(value);
                    updateFilter(value, displayText);
                    const panel = item.closest('.dropdown-panel');
                    if (panel) panel.classList.remove('open');
                });
            });

            const currentTriggerText = filterTrigger ? filterTrigger.innerText : '';
            let currentValue = '';
            if (currentTriggerText === 'Income') currentValue = 'Income';
            else if (currentTriggerText === 'Expense') currentValue = 'Expense';
            setSelectedItem(currentValue);
            if (selectedTypeInput) selectedTypeInput.value = currentValue;
        }
    }

}); // end DOMContentLoaded

// --- Others: Transaction modal ---
function handleTransactionCategoryChange(select) {
    const othersDiv = document.getElementById('transactionOthersInput');
    const othersInput = document.getElementById('transactionOthersSpecify');

    if (!othersDiv || !othersInput) return;

    if (select.value === 'Others') {
        othersDiv.style.display = 'block';
        othersInput.setAttribute('required', 'required');
        othersInput.focus();
    } else {
        othersDiv.style.display = 'none';
        othersInput.removeAttribute('required');
        othersInput.value = '';
    }
}

// --- Resolve "Others" to custom value before submit ---
function resolveTransactionOthers() {
    const select = document.getElementById('Category');
    const specify = document.getElementById('transactionOthersSpecify');
    if (select && specify && select.value === 'Others' && specify.value.trim() !== '') {
        const customValue = specify.value.trim();
        const newOption = document.createElement('option');
        newOption.value = customValue;
        newOption.text = customValue;
        newOption.selected = true;
        select.appendChild(newOption);
        select.value = customValue;
    }
}

// --- Show delete modal ---
function showDeleteModal(id) {
    transactionToDelete = id;
    const deleteModal = new bootstrap.Modal(document.getElementById('deleteConfirmationModal'));
    deleteModal.show();
}

// --- Edit transaction ---
function editTransaction(id) {
    fetch(`/Transactions/GetTransaction?id=${id}`)
        .then(response => response.json())
        .then(data => {
            document.getElementById('Description').value = data.description;
            document.getElementById('Amount').value = Math.abs(data.amount);
            document.getElementById('DatePicker').value = data.date;
            document.getElementById('TimePicker').value = data.time;
            document.getElementById('Notes').value = data.notes || '';

            // Set Expense/Income toggle first so correct categories load
            const isExpense = data.isExpense;
            const toggleType = isExpense ? 'expense' : 'income';
            const toggleOption = document.querySelector(`.segmented-option[data-type="${toggleType}"]`);
            if (toggleOption) toggleOption.click();

            // Wait for categories to update then set selected category
            setTimeout(() => {
                const categorySelect = document.getElementById('Category');
                if (categorySelect) {
                    categorySelect.value = data.category;
                    // If not found (custom Others value), trigger Others flow
                    if (!categorySelect.value) {
                        categorySelect.value = 'Others';
                        const othersDiv = document.getElementById('transactionOthersInput');
                        const othersInput = document.getElementById('transactionOthersSpecify');
                        if (othersDiv) othersDiv.style.display = 'block';
                        if (othersInput) othersInput.value = data.category;
                    }
                }
            }, 50);

            // Set hidden Id
            const transactionId = document.getElementById('TransactionId');
            if (transactionId) transactionId.value = data.id;

            // Update modal title and button
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

// --- Delete transaction ---
function deleteTransaction(id) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    fetch(`/Transactions/Delete/${id}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: new URLSearchParams({ '__RequestVerificationToken': token })
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