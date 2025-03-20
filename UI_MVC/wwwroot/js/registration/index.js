document.addEventListener('DOMContentLoaded', function () {
    const startSecondPhaseBtn = document.getElementById('startSecondPhaseBtn');
    const confirmStartSecondPhase = document.getElementById('confirmStartSecondPhase');
    const startSecondPhaseForm = document.getElementById('startSecondPhaseForm');
    const insufficientWarning = document.getElementById('insufficientWarning');

    // Modal handling
    startSecondPhaseBtn.addEventListener('click', function() {
        //const hasSufficientRegistrations = @Json.Serialize(hasSufficientRegistrations);

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
    confirmStartSecondPhase.addEventListener('click', function() {
        startSecondPhaseForm.submit();
    });
});
