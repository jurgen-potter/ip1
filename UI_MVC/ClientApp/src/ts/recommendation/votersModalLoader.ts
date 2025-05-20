let selectedRecommendationId: number | null = null;
let selectedRecommendationTitle: string | null = null;
let lastFocusedTrigger: HTMLElement | null = null;
const votersModalElement = document.getElementById('votersModal') as HTMLElement;

document.addEventListener('DOMContentLoaded', () => {
    
    const showVotersButtons = document.querySelectorAll('.btn-show-voters') as NodeListOf<HTMLButtonElement>;
    const closeButton = votersModalElement?.querySelector("#closeModal") as HTMLButtonElement;
    
    initializeButtons(closeButton, showVotersButtons);
});

function initializeButtons(closeButton :HTMLButtonElement,buttons : NodeListOf<HTMLButtonElement>) {
    buttons.forEach(button => {
        button.addEventListener("click", () => {
            lastFocusedTrigger = button;
            const recommendationId = button.getAttribute("data-recommendation-id");
            const recommendationTitle = button.getAttribute("data-recommendation-title");
            if (recommendationId !== null){
                if (recommendationId) {
                    selectedRecommendationId = parseInt(recommendationId);
                }

                selectedRecommendationTitle = recommendationTitle ?? 'Stemmers Details';
                votersModalElement.classList.remove("hidden");
                votersModalElement.setAttribute('aria-hidden', 'false');
                votersModalElement.classList.add("flex");

                setText();
            }
        })
    });

    closeButton.addEventListener("click", () => {
        votersModalElement.classList.remove("flex");
        votersModalElement.classList.add("hidden");
        votersModalElement.setAttribute('aria-hidden', 'true');

        if (lastFocusedTrigger) {
            lastFocusedTrigger.focus();
            lastFocusedTrigger = null;
        }
    });
}

function setText() {
    fetch(`/Recommendation/GetVoters/${selectedRecommendationId}`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
    })
        .then(res => {
            if (!res.ok) {
                throw Error(`Received status code ${res.status}.`);
            }
            else{
                setBodyText(res);
            }
        })
        .catch(err => alert(`Something went wrong: ${err}`));
}

async function setBodyText(response: Response) {
    const modalTitleElement = votersModalElement.querySelector<HTMLElement>('.member-reg-modal-title') as HTMLElement;
    const modalBodyElement = votersModalElement.querySelector<HTMLElement>('.member-reg-modal-body') as HTMLElement;

    const recommendationTitle = selectedRecommendationTitle ?? 'Stemmers Details';
    modalTitleElement.textContent = `Stemming voor aanbeveling: ${recommendationTitle}`;
    
    const htmlContent = await response.text();
    
    if (htmlContent.trim().length > 0) {
        modalBodyElement.innerHTML = htmlContent;
    }
    else{
        console.error('Fout bij ophalen/weergeven stemmers');
        let userMessage = 'Kon de lijst met stemmers niet laden.';
        modalBodyElement.innerHTML = `<div class="alert alert-danger m-3">${userMessage}</div>`;
    }
}