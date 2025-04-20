document.addEventListener('DOMContentLoaded', () => {

    const votersModalElement = document.getElementById('votersModal');

    if (votersModalElement) {

        const modalTitleElement = votersModalElement.querySelector<HTMLElement>('.modal-title');
        const modalBodyElement = votersModalElement.querySelector<HTMLElement>('.modal-body');

        if (!modalTitleElement || !modalBodyElement) { //null check vr typescript
            console.error('modellTitleElement of modalBodyElement niet gevonden');
            return;
        }

        votersModalElement.addEventListener('show.bs.modal', async (event) => {
 
            const potentialTarget = (event as Event & { relatedTarget?: Element | null }).relatedTarget;

           
            if (potentialTarget instanceof HTMLButtonElement && potentialTarget.dataset.recommendationId) {
                const triggerButton = potentialTarget;

                const recommendationId = triggerButton.dataset.recommendationId;
                const recommendationTitle = triggerButton.dataset.recommendationTitle || 'Stemmers Details';

                modalTitleElement.textContent = `Stemmers voor: ${recommendationTitle}`;
                
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
    } 
});