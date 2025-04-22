document.addEventListener('DOMContentLoaded', () => {
    const modalElement = document.getElementById('confirmationModal');
    if (!modalElement) return;

    const confirmationModal = new bootstrap.Modal(modalElement);

    confirmationModal.show();

    const confirmButton = document.getElementById('confirmParticipation');
    if (confirmButton) {
        confirmButton.addEventListener('click', () => {
            confirmationModal.hide();
        });
    }
});