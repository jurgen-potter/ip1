document.addEventListener('DOMContentLoaded', () => {
    const isConfirmedElem = document.getElementById('isConfirmed') as HTMLInputElement;
    const isConfirmed = isConfirmedElem.value.toString().toLowerCase() === 'true';
    if (isConfirmed) {
        return;
    }
    
    const modalElement = document.getElementById('confirmationModal');
    if (!modalElement) return;

    // Show the Tailwind modal
    modalElement.classList.remove('hidden');

    const confirmButton = document.getElementById('confirmParticipation');
    if (confirmButton) {
        confirmButton.addEventListener('click', () => {
            // Hide the Tailwind modal
            modalElement.classList.add('hidden');
        });
    }
});