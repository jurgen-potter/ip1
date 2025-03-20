document.addEventListener('DOMContentLoaded', function () {
    const startSecondPhaseBtn = document.getElementById('startSecondPhaseBtn');
    const confirmStartFinalPhase = document.getElementById('confirmStartFinalPhase');
    const StartFinalPhaseForm = document.getElementById('StartFinalPhaseForm');
    const insufficientWarning = document.getElementById('insufficientWarning');

    // Modal handling
    startSecondPhaseBtn.addEventListener('click', function() {

        let hasSufficientRegistrations;
        
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
