let selectedRecommendationId: number | null = null;
let selectedRecommendationTitle: string | null = null;
const votersModalElement = document.getElementById('votersModal') as HTMLElement;

document.addEventListener('DOMContentLoaded', () => {
    
    const showVotersButtons = document.querySelectorAll('.btn-show-voters') as NodeListOf<HTMLButtonElement>;
    const closeButton = votersModalElement?.querySelector("#closeModal") as HTMLButtonElement;
    
    initializeButtons(closeButton, showVotersButtons);
    
    /*
    if (votersModalElement) {

        const modalTitleElement = votersModalElement.querySelector<HTMLElement>('.member-reg-modal-title');
        const modalBodyElement = votersModalElement.querySelector<HTMLElement>('.member-reg-modal-body');

        if (!modalTitleElement || !modalBodyElement) { //null check vr typescript
            console.error('modalTitleElement of modalBodyElement niet gevonden');
            return;
        }
        
        votersModalElement.addEventListener('show.bs.modal', async (event) => {

            const potentialTarget = (event as Event & { relatedTarget?: Element | null }).relatedTarget;


            if (potentialTarget instanceof HTMLButtonElement && potentialTarget.dataset.recommendationId) {
                const triggerButton = potentialTarget;

                const recommendationId = triggerButton.dataset.recommendationId;
                const recommendationTitle = triggerButton.dataset.recommendationTitle || 'Stemmers Details';

                modalTitleElement.textContent = `Stemming voor aanbeveling: ${recommendationTitle}`;

                const url = `/Recommendation/GetVoters?recommendationId=${recommendationId}`;
                try {
                    // Probeer de stemmers op te halen via een fetch-aanroep
                    const response = await fetch(url);
                    if (!response.ok) {
                        throw new Error(`Serverfout: ${response.status} ${response.statusText}`);
                    }
                    // Zet de HTML-inhoud van het modal-body element op basis van de response
                    modalBodyElement.innerHTML = await response.text();
                } catch (error) {
                    console.error('Fout bij ophalen/weergeven stemmers:', error);
                    let userMessage = 'Kon de lijst met stemmers niet laden.';
                    modalBodyElement.innerHTML = `<div class="alert alert-danger m-3">${userMessage}</div>`; //laat error msg zien
                }
            }
        });
    }*/
});

function initializeButtons(closeButton :HTMLButtonElement,buttons : NodeListOf<HTMLButtonElement>) {
    buttons.forEach(button => {
        button.addEventListener("click", () => {
            const recommendationId = button.getAttribute("data-recommendation-id");
            const recommendationTitle = button.getAttribute("data-recommendation-title");
            if (recommendationId !== null){
                if (recommendationId) {
                    selectedRecommendationId = parseInt(recommendationId);
                }

                selectedRecommendationTitle = recommendationTitle ?? 'Stemmers Details';
                votersModalElement.classList.remove("hidden");

                setText();
            }
        })
    });

    closeButton.addEventListener("click", () => {
        votersModalElement.classList.add("hidden");
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