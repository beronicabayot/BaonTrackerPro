// --- Filter by period ---
function filterBudget(value, label) {
    document.getElementById('filterLabel').textContent = label;
    document.querySelectorAll('.budget-card').forEach(card => {
        card.style.display = (value === 'All' || card.getAttribute('data-period') === value)
            ? 'block' : 'none';
    });
}

// --- Others: Set Budget modal ---
function handleCategoryChange(select) {
    const othersDiv = document.getElementById('othersInput');
    const othersInput = document.getElementById('othersSpecify');
    if (select.value === 'Others') {
        othersDiv.style.display = 'block';
        othersInput.setAttribute('required', 'required');
    } else {
        othersDiv.style.display = 'none';
        othersInput.removeAttribute('required');
        othersInput.value = '';
    }
}

// --- Others: Edit Budget modal ---
function handleEditCategoryChange(select) {
    const othersDiv = document.getElementById('editOthersInput');
    const othersInput = document.getElementById('editOthersSpecify');
    if (select.value === 'Others') {
        othersDiv.style.display = 'block';
        othersInput.setAttribute('required', 'required');
    } else {
        othersDiv.style.display = 'none';
        othersInput.removeAttribute('required');
        othersInput.value = '';
    }
}

// --- Resolve "Others" to custom value before submit ---
function resolveOthers() {
    const select = document.getElementById('categorySelect');
    const specify = document.getElementById('othersSpecify');
    if (select.value === 'Others' && specify.value.trim() !== '') {
        const customValue = specify.value.trim();
        const newOption = document.createElement('option');
        newOption.value = customValue;
        newOption.text = customValue;
        newOption.selected = true;
        select.appendChild(newOption);
        select.value = customValue;
    }
}

// --- Show Budget Delete Modal ---
function showBudgetDeleteModal(id) {
    document.getElementById('deleteBudgetId').value = id;
    const modal = new bootstrap.Modal(document.getElementById('deleteBudgetModal'));
    modal.show();
}

document.addEventListener('DOMContentLoaded', function () {

    // --- Populate Edit modal ---
    const editModal = document.getElementById('editBudgetModal');
    if (editModal) {
        editModal.addEventListener('show.bs.modal', function (e) {
            const btn = e.relatedTarget;
            document.getElementById('editId').value = btn.getAttribute('data-id');
            document.getElementById('editAmount').value = btn.getAttribute('data-amount');

            const category = btn.getAttribute('data-category');
            const period = btn.getAttribute('data-period');

            const catSelect = document.getElementById('editCategorySelect');
            const perSelect = document.getElementById('editPeriod');

            for (let opt of catSelect.options) {
                opt.selected = opt.value === category;
            }
            for (let opt of perSelect.options) {
                opt.selected = opt.value === period;
            }

            // Show "Please specify" if category is not in the list
            const knownCategories = [
                'Food & Drinks', 'Transportation', 'Personal/Lifestyle',
                'Bills & Utilities', 'Entertainment', 'Education', 'Health',
                'Allowance', 'Salary', 'Bonus', 'Gift', 'Others'
            ];
            if (!knownCategories.includes(category)) {
                document.getElementById('editOthersInput').style.display = 'block';
                document.getElementById('editOthersSpecify').value = category;
            }
        });
    }

});