document.addEventListener("DOMContentLoaded", () => {
    let selectedRecommendationId: number | null = null;

    const confirmationModal = document.getElementById("confirmationModal") as HTMLElement;
    const stopVotingButton = confirmationModal?.querySelector(".btn-danger") as HTMLButtonElement;
    const waitButton = confirmationModal?.querySelector(".btn-success") as HTMLButtonElement;

    const endVoteButtons = document.querySelectorAll('.btn-stop-voting')

    endVoteButtons.forEach(button => {
        button.addEventListener("click", () => {
            const recommendationId = (button as HTMLElement).getAttribute("data-recommendation-id");
            if (recommendationId) {
                selectedRecommendationId = parseInt(recommendationId);
            }
        });
    });

    stopVotingButton?.addEventListener("click", async () => {
        if (selectedRecommendationId === null) return;

        fetch(`/Recommendation/StopVoting/${selectedRecommendationId}`, {
            method: 'POST'
        })
            .then(res => {
                if (!res.ok) {
                    throw Error(`Received status code ${res.status}.`)
                }
            })
            .catch(err => alert('Something went wrong: ' + err));

        endVoteButtons.forEach(button => {
            const recommendationId = (button as HTMLElement).getAttribute("data-recommendation-id");
            if (recommendationId && parseInt(recommendationId) === selectedRecommendationId) {
                (button as HTMLButtonElement).hidden = true;
            }
        })

        const toHideText  = document.getElementById('toHideText' + selectedRecommendationId);
        if (toHideText) {
            toHideText.hidden = true;
        }

        const hiddenText = document.getElementById('hiddenText' + selectedRecommendationId);
        hiddenText?.removeAttribute('hidden');

        const modalInstance = bootstrap.Modal.getInstance(confirmationModal);
        modalInstance?.hide();
        selectedRecommendationId = null;
    });

    waitButton?.addEventListener("click", () => {
        const modalInstance = bootstrap.Modal.getInstance(confirmationModal);
        modalInstance?.hide();
        selectedRecommendationId = null;
    });
});
