function showGoalDeleteModal(id) {
    document.getElementById('deleteGoalId').value = id;
    const modal = new bootstrap.Modal(document.getElementById('deleteGoalModal'));
    modal.show();
}

function showEditGoalModal(id, name, target, deadline, notes) {
    document.getElementById('editGoalId').value = id;
    document.getElementById('editGoalName').value = name;
    document.getElementById('editGoalTarget').value = target;
    document.getElementById('editGoalDeadline').value = deadline;
    document.getElementById('editGoalNotes').value = notes !== 'null' ? notes : '';
    const modal = new bootstrap.Modal(document.getElementById('editGoalModal'));
    modal.show();
}

function showAddFundsModal(id, name, current, target) {
    document.getElementById('addFundsGoalId').value = id;
    document.getElementById('addFundsGoalName').textContent = name;
    document.getElementById('addFundsProgress').textContent =
        '₱' + parseFloat(current).toFixed(2) + ' / ₱' + parseFloat(target).toFixed(2);
    document.getElementById('addFundsTarget').value = target;

    var remaining = parseFloat(target) - parseFloat(current);
    document.getElementById('addFundsRemaining').textContent = '₱' + remaining.toFixed(2);

    document.getElementById('fundsAmount').value = '';
    document.getElementById('capMessage').style.display = 'none';
    setFundsAction('add');

    const modal = new bootstrap.Modal(document.getElementById('addFundsModal'));
    modal.show();
}

function setFundsAction(action) {
    document.getElementById('fundsAction').value = action;
    document.getElementById('addOption').classList.toggle('active', action === 'add');
    document.getElementById('deductOption').classList.toggle('active', action === 'deduct');

    if (action === 'add') {
        capAmount();
    } else {
        document.getElementById('capMessage').style.display = 'none';
    }
}

function capAmount() {
    const action = document.getElementById('fundsAction').value;
    if (action !== 'add') return;

    const target = parseFloat(document.getElementById('addFundsTarget').value);
    const progressText = document.getElementById('addFundsProgress').textContent;
    const current = parseFloat(progressText.replace('₱', '').split('/')[0].trim());
    const remaining = target - current;
    const input = document.getElementById('fundsAmount');
    const capMsg = document.getElementById('capMessage');

    if (parseFloat(input.value) > remaining) {
        input.value = remaining.toFixed(2);
        input.readOnly = true;
        input.style.backgroundColor = '#f8d7da';
        capMsg.style.display = 'block';
    } else {
        input.readOnly = false;
        input.style.backgroundColor = '';
        capMsg.style.display = 'none';
    }
}

// ✅ NOW OUTSIDE — globally accessible
function showMarkAsDoneModal(id, name, amount) {
    document.getElementById('markAsDoneGoalId').value = id;
    document.getElementById('markAsDoneGoalName').textContent = name;
    document.getElementById('markAsDoneAmount').textContent = '₱' + parseFloat(amount).toFixed(2);
    const modal = new bootstrap.Modal(document.getElementById('markAsDoneModal'));
    modal.show();
}