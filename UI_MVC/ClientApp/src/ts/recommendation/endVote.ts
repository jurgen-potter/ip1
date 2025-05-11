// endVote.ts

let endVoteConfirmationModal: HTMLElement | null;
let selectedRecIdForEndVote: number | null = null;

document.addEventListener('DOMContentLoaded', () => {
    endVoteConfirmationModal = document.getElementById('confirmationModal'); // ID van de stop-stemmen modal
    setupEndVoteModalTriggers();
    setupEndVoteConfirmAction();
});

function openEndVoteModal(modalElement: HTMLElement | null): void {
    if (!modalElement) return;
    modalElement.classList.add('is-open');
    document.body.classList.add('overflow-hidden');
}

function closeEndVoteModal(modalElement: HTMLElement | null): void {
    if (!modalElement) return;
    modalElement.classList.remove('is-open');
    document.body.classList.remove('overflow-hidden');
}

function setupEndVoteModalTriggers(): void {
    // Knoppen die de "Stop stemronde" modal openen
    document.querySelectorAll<HTMLButtonElement>('.btn-stop-voting').forEach(button => {
        button.addEventListener('click', () => {
            const modalId = button.dataset.modalTrigger; // Moet 'confirmationModal' zijn
            const recId = button.dataset.recommendationId;

            if (modalId && recId) {
                const modalToOpen = document.getElementById(modalId);
                if (modalToOpen === endVoteConfirmationModal) { // Zorg dat we de juiste modal openen
                    selectedRecIdForEndVote = parseInt(recId, 10);
                    openEndVoteModal(endVoteConfirmationModal);
                }
            }
        });
    });

    // Sluitknoppen specifiek voor deze modal (X knop en "Even wachten")
    if (endVoteConfirmationModal) {
        endVoteConfirmationModal.querySelectorAll<HTMLButtonElement>('[data-modal-close="confirmationModal"]').forEach(button => {
            button.addEventListener('click', () => {
                closeEndVoteModal(endVoteConfirmationModal);
                selectedRecIdForEndVote = null; // Reset ID wanneer geannuleerd
            });
        });
        // Optioneel: sluit bij klikken op overlay
        endVoteConfirmationModal.addEventListener('click', (event) => {
            if (event.target === endVoteConfirmationModal) {
                closeEndVoteModal(endVoteConfirmationModal);
                selectedRecIdForEndVote = null;
            }
        });
    }
}

function setupEndVoteConfirmAction(): void {
    const confirmButton = document.getElementById('confirmStopVotingBtn') as HTMLButtonElement | null;

    if (confirmButton && endVoteConfirmationModal) {
        confirmButton.addEventListener('click', async () => {
            if (selectedRecIdForEndVote === null) return;

            confirmButton.disabled = true;
            confirmButton.innerHTML = `<span class="inline-block animate-spin rounded-full h-4 w-4 border-t-2 border-b-2 border-white"></span> Stoppen...`;

            try {
                const response = await fetch(`/Recommendation/StopVoting/${selectedRecIdForEndVote}`, {
                    method: 'POST'
                });
                if (!response.ok) {
                    const errorData = await response.json().catch(() => ({ message: `Fout bij stoppen: ${response.statusText}` }));
                    throw new Error(errorData.message || `Received status code ${response.status}.`);
                }

                // Update UI na succesvol stoppen
                document.querySelectorAll<HTMLButtonElement>('.btn-stop-voting').forEach(button => {
                    if (button.dataset.recommendationId === String(selectedRecIdForEndVote)) {
                        button.style.display = 'none'; // Verberg de "Stop stemronde" knop
                    }
                });

                // Update de status tekst op de kaart
                const statusTextElement = document.getElementById(`statusText${selectedRecIdForEndVote}`);
                if (statusTextElement) {
                    statusTextElement.innerHTML = '<strong>Het stemmen is afgelopen</strong>';
                    statusTextElement.className = 'recommendation-card-status ended';
                }
                // Verberg de stemknoppen voor gebruikers
                const voteFormsContainer = document.querySelector(`.recommendation-card [data-recommendation-id="${selectedRecIdForEndVote}"] ~ .recommendation-vote-forms`);
                if(voteFormsContainer) (voteFormsContainer as HTMLElement).style.display = 'none';


            } catch (err: any) {
                console.error('Fout bij stoppen van stemronde:', err);
                alert(`Er is iets misgegaan: ${err.message || 'Onbekende fout'}`);
            } finally {
                confirmButton.disabled = false;
                confirmButton.textContent = 'Stop de stemming';
                closeEndVoteModal(endVoteConfirmationModal);
                selectedRecIdForEndVote = null;
            }
        });
    }
}
