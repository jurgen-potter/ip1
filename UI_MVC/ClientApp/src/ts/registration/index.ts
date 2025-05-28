interface Bucket {
    criteriaNames: string[];
    subCriteriaNames: string[];
    count: number;
    registeredCount: number;
    isSufficient: boolean;
}

document.addEventListener('DOMContentLoaded', () => {
    const startSecondPhaseBtn = document.getElementById('startSecondPhaseBtn') as HTMLButtonElement | null;
    const confirmationModal = document.getElementById('confirmationModalOverview') as HTMLElement | null;
    const insufficientWarning = document.getElementById('insufficientWarningOverview') as HTMLElement | null;
    const confirmStartFinalPhaseBtn = document.getElementById('confirmStartFinalPhaseBtn') as HTMLButtonElement | null;
    const generateInvitesModal = document.getElementById('generateInvitesModal') as HTMLElement | null;
    const loadingOverlay = document.getElementById('loadingOverlay') as HTMLElement | null;

    // Selecteer alle knoppen die de modals kunnen sluiten
    const cancelButtons = confirmationModal?.querySelectorAll<HTMLButtonElement>('[data-modal-close="confirmationModalOverview"]');
    const generateInvitesCancelButtons = generateInvitesModal?.querySelectorAll<HTMLButtonElement>('[data-modal-close="generateInvitesModal"]');

    const hasSufficientRegistrationsElement = document.getElementById('hasSufficientRegistrations');
    const hasSufficient = hasSufficientRegistrationsElement?.dataset.value === 'true';

    function openModal(modal: HTMLElement): void {
        if (modal) {
            if (modal.id === 'confirmationModalOverview' && !hasSufficient && insufficientWarning) {
                insufficientWarning.style.display = 'block';
            } else if (insufficientWarning) {
                insufficientWarning.style.display = 'none';
            }
            modal.classList.add('is-open');
            document.body.classList.add('overflow-hidden');
        }
    }

    function closeModal(modal: HTMLElement): void {
        if (modal) {
            modal.classList.remove('is-open');
            document.body.classList.remove('overflow-hidden');
        }
    }

    function showLoadingOverlay(): void {
        if (loadingOverlay) {
            loadingOverlay.style.display = 'flex';
        }
    }

    function hideLoadingOverlay(): void {
        if (loadingOverlay) {
            loadingOverlay.style.display = 'none';
        }
    }

    if (startSecondPhaseBtn && confirmationModal) {
        startSecondPhaseBtn.addEventListener('click', (e) => {
            e.preventDefault(); // Voorkom direct submitten van form
            openModal(confirmationModal);
        });
    }

    if (confirmStartFinalPhaseBtn && confirmationModal) {
        confirmStartFinalPhaseBtn.addEventListener('click', () => {
            const form = document.getElementById('StartFinalPhaseForm') as HTMLFormElement | null;
            if (form) {
                form.submit();
            }
            closeModal(confirmationModal);
        });
    }

    if (cancelButtons) {
        cancelButtons.forEach(btn => {
            btn.addEventListener('click', () => {
                if (confirmationModal) closeModal(confirmationModal);
            });
        });
    }

    if (generateInvitesCancelButtons) {
        generateInvitesCancelButtons.forEach(btn => {
            btn.addEventListener('click', () => {
                if (generateInvitesModal) closeModal(generateInvitesModal);
            });
        });
    }

    // Sluit modals bij klikken op overlay
    [confirmationModal, generateInvitesModal].forEach(modal => {
        if (modal) {
            modal.addEventListener('click', (event) => {
                if (event.target === modal) {
                    closeModal(modal);
                }
            });
        }
    });
    
    const generateButton = document.getElementById('generateInvites') as HTMLButtonElement;
    const confirmGenerateInvitesBtn = document.getElementById('confirmGenerateInvitesBtn') as HTMLButtonElement;

    if (generateButton && generateInvitesModal) {
        generateButton.addEventListener('click', () => {
            openModal(generateInvitesModal);
        });
    }

    if (confirmGenerateInvitesBtn && generateInvitesModal) {
        confirmGenerateInvitesBtn.addEventListener('click', async () => {
            closeModal(generateInvitesModal);
            showLoadingOverlay();

            const panelId = generateButton.dataset.panelId;
            const bucketJson = generateButton.dataset.buckets as string;
            const parsedBuckets = JSON.parse(bucketJson);
            const body = {panelId: panelId, buckets: parsedBuckets};

            try {
                const response = await fetch(`/antwerpen/api/Invitations/makeInvitations/${panelId}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json'
                    },
                    body: JSON.stringify(body)
                });

                const responseText = await response.text();
                console.log('Server response:', responseText);

                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}, response: ${responseText}`);
                }

                // Only try to parse JSON if we actually got a response
                if (responseText) {
                    try {
                        const result = JSON.parse(responseText);
                        console.log('Parsed result:', result);
                    } catch (jsonError) {
                        console.log('Response was not JSON, but request succeeded');
                    }
                }

                // Reload regardless of JSON parsing
                location.reload();
            } catch (error) {
                console.error('Error details:', error);
                alert('Er is een fout opgetreden bij het genereren van uitnodigingen. Probeer het later opnieuw.');
            } finally {
                hideLoadingOverlay();
            }
        });
    }
});