document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('createPanelForm') as HTMLFormElement | null;
    const loadingOverlay = document.getElementById('loadingOverlay');
    const submitButton = document.getElementById('createPanelSubmitBtn') as HTMLButtonElement | null;

    if (form && loadingOverlay && submitButton) {
        form.addEventListener('submit', (event) => {
            event.preventDefault(); // Voorkom de standaard formulier submit

            if (loadingOverlay) {
                loadingOverlay.classList.add('is-active');
            }

            if (submitButton) {
                submitButton.disabled = true;
                submitButton.innerHTML = `
                    <span class="inline-block animate-spin rounded-full h-4 w-4 border-t-2 border-b-2 border-white mr-2 align-middle"></span>
                    Aanmaken...
                `;

                // Maak een verborgen input aan om de geklikte knop te simuleren
                const hiddenInput = document.createElement('input');
                hiddenInput.type = 'hidden';
                hiddenInput.name = submitButton.name;
                hiddenInput.value = submitButton.value;
                form.appendChild(hiddenInput);
            }

            // Wacht 5 seconden voordat het formulier daadwerkelijk wordt verstuurd
            setTimeout(() => {
                form.submit(); 
            }, 5000); 
        });
    } else {
        if (!form) console.error('Formulier "createPanelForm" niet gevonden.');
        if (!loadingOverlay) console.error('Element "loadingOverlay" niet gevonden.');
        if (!submitButton) console.error('Knop "createPanelSubmitBtn" niet gevonden.');
    }
});