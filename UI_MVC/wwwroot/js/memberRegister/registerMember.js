document.addEventListener('DOMContentLoaded', function () {
    var confirmationModal = new bootstrap.Modal(document.getElementById('confirmationModal'));
    confirmationModal.show();

    document.getElementById('confirmParticipation').addEventListener('click', function () {
        confirmationModal.hide();
    });
});
