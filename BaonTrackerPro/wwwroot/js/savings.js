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
    const modal = new bootstrap.Modal(document.getElementById('addFundsModal'));
    modal.show();
}
