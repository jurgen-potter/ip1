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

    // Selecteer alle knoppen die de modal kunnen sluiten (zowel 'X' als 'Annuleren')
    const cancelButtons = confirmationModal?.querySelectorAll<HTMLButtonElement>('[data-modal-close="confirmationModalOverview"]');

    const hasSufficientRegistrationsElement = document.getElementById('hasSufficientRegistrations');
    const hasSufficient = hasSufficientRegistrationsElement?.dataset.value === 'true';

    function openModal(): void {
        if (confirmationModal) {
            if (!hasSufficient && insufficientWarning) {
                insufficientWarning.style.display = 'block';
            } else if (insufficientWarning) {
                insufficientWarning.style.display = 'none';
            }
            confirmationModal.classList.add('is-open');
            document.body.classList.add('overflow-hidden');
        }
    }

    function closeModal(): void {
        if (confirmationModal) {
            confirmationModal.classList.remove('is-open');
            document.body.classList.remove('overflow-hidden');
        }
    }

    if (startSecondPhaseBtn) {
        startSecondPhaseBtn.addEventListener('click', (e) => {
            e.preventDefault(); // Voorkom direct submitten van form
            openModal();
        });
    }

    if (confirmStartFinalPhaseBtn) {
        confirmStartFinalPhaseBtn.addEventListener('click', () => {
            const form = document.getElementById('StartFinalPhaseForm') as HTMLFormElement | null;
            if (form) {
                form.submit();
            }
            closeModal(); // Sluit modal na poging tot submit
        });
    }

    if (cancelButtons) {
        cancelButtons.forEach(btn => {
            btn.addEventListener('click', () => {
                closeModal();
            });
        });
    }

    // Sluit modal bij klikken op overlay
    if (confirmationModal) {
        confirmationModal.addEventListener('click', (event) => {
            // Controleer of de klik direct op de overlay was, en niet op een kind-element (zoals de modal-dialog)
            if (event.target === confirmationModal) {
                closeModal();
            }
        });
    }
    
    const generateButton = document.getElementById('generateInvites') as HTMLButtonElement;
    generateButton.addEventListener('click', () => {
        const panelId = generateButton.dataset.panelId;
        const bucketJson = generateButton.dataset.buckets as string;
        const parsedBuckets = JSON.parse(bucketJson);
        const body = {panelId: panelId, buckets: parsedBuckets};
        fetch(`api/Invitations/makeInvitations/${panelId}`, {
          method: 'POST', 
          headers: {
              'Content-Type': 'application/json',
              'Accept': 'application/json'
          },
          body: JSON.stringify(body)
      })
          .then(response => {
              if (response.ok) {
                  alert('Nieuwe uitnodigingen zijn aangemaakt.');
              }
          })
          .catch(err => alert('Er ging iets fout: ' + err));
    })
});