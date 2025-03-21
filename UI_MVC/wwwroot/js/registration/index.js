document.addEventListener('DOMContentLoaded', function () {
    const startSecondPhaseBtn = document.getElementById('startSecondPhaseBtn');
    const confirmStartFinalPhase = document.getElementById('confirmStartFinalPhase');
    const StartFinalPhaseForm = document.getElementById('StartFinalPhaseForm');
    const insufficientWarning = document.getElementById('insufficientWarning');
    const hasSufficientRegistrations = document.getElementById('hasSufficientRegistrations').getAttribute('data-value') === 'True';

    // Modal handling
    startSecondPhaseBtn.addEventListener('click', function() {
        if (!hasSufficientRegistrations) {
            insufficientWarning.style.display = 'block';
        } else {
            insufficientWarning.style.display = 'none';
        }

        // Show the modal
        const confirmationModal = new bootstrap.Modal(document.getElementById('confirmationModal'));
        confirmationModal.show();
    });

    // Form submission on confirmation
    confirmStartFinalPhase.addEventListener('click', function() {
        StartFinalPhaseForm.submit();
    });
});
