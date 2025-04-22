document.addEventListener('DOMContentLoaded', () => {
    const startSecondPhaseBtn = document.getElementById('startSecondPhaseBtn') as HTMLButtonElement;
    const confirmStartFinalPhase = document.getElementById('confirmStartFinalPhase') as HTMLButtonElement;
    const StartFinalPhaseForm = document.getElementById('StartFinalPhaseForm') as HTMLFormElement;
    const insufficientWarning = document.getElementById('insufficientWarning') as HTMLDivElement;
    const hasSufficientElement = document.getElementById('hasSufficientRegistrations') as HTMLDivElement;
    const hasSufficientRegistrations = hasSufficientElement?.dataset.value ?? 'false';

    if (!startSecondPhaseBtn || !confirmStartFinalPhase || !StartFinalPhaseForm || !insufficientWarning || !hasSufficientElement) {
        console.warn('Some DOM elements are missing. Script will not run.');
        return;
    }

    // Modal handling
    startSecondPhaseBtn.addEventListener('click', () => {
        if (hasSufficientRegistrations.toLowerCase() === 'true') {
            insufficientWarning.style.display = 'none';
        } else {
            insufficientWarning.style.display = 'block';
        }

        // Show the modal
        const modalElement = document.getElementById('confirmationModal');
        if (modalElement) {
            const confirmationModal = new bootstrap.Modal(modalElement);
            confirmationModal.show();
        }
    });

    // Form submission on confirmation
    confirmStartFinalPhase.addEventListener('click', () => {
        StartFinalPhaseForm.submit();
    });
});
