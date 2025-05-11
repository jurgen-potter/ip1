document.addEventListener('DOMContentLoaded', () => {
    // Get references to modal elements with null checks
    const votersModal = document.getElementById('votersModal');

    // Only proceed if the modal exists on the page
    if (!votersModal) {
        console.log('Modal not found, skipping voters modal setup');
        return;
    }

    const votersModalBody = document.getElementById('votersModalBody');
    const votersModalTitle = votersModal.querySelector('.custom-modal-title');

    // Make sure we have all required elements
    if (!votersModalBody || !votersModalTitle) {
        console.error('Required modal elements not found');
        return;
    }

    // Set up event listeners for the "Bekijk Stemmers" buttons
    const viewVotersButtons = document.querySelectorAll('.btn-show-voters');

    if (viewVotersButtons.length === 0) {
        console.log('No voter buttons found, skipping setup');
        return;
    }

    viewVotersButtons.forEach(button => {
        button.addEventListener('click', async () => {
            // Safely get data attributes
            const recommendationId = button.getAttribute('data-recommendation-id');
            const recommendationTitle = button.getAttribute('data-recommendation-title') || 'Stemmers Details';

            if (!recommendationId) {
                console.error('No recommendation ID found');
                return;
            }

            // Update modal title
            votersModalTitle.textContent = `Stemming voor aanbeveling: ${recommendationTitle}`;

            // Show loading message
            votersModalBody.innerHTML = '<p class="text-gray-500">Laden...</p>';

            // Open the modal
            votersModal.classList.add('is-open');
            document.body.classList.add('overflow-hidden');

            // Fetch voters data
            try {
                const url = `/Recommendation/GetVoters?recommendationId=${recommendationId}`;
                console.log('Fetching voters from:', url); // Debug logging

                const response = await fetch(url);

                if (!response.ok) {
                    throw new Error(`Serverfout: ${response.status} ${response.statusText}`);
                }

                // Update modal body with fetched HTML
                const responseText = await response.text();
                votersModalBody.innerHTML = responseText;

            } catch (error) {
                console.error('Fout bij ophalen/weergeven stemmers:', error);
                votersModalBody.innerHTML = '<div class="alert alert-danger m-3">Kon de lijst met stemmers niet laden.</div>';
            }
        });
    });

    // Set up close functionality for the modal
    const closeButtons = votersModal.querySelectorAll('[data-modal-close="votersModal"]');

    closeButtons.forEach(button => {
        button.addEventListener('click', () => {
            votersModal.classList.remove('is-open');
            document.body.classList.remove('overflow-hidden');
        });
    });

    // Close modal when clicking on the overlay
    votersModal.addEventListener('click', (event) => {
        if (event.target === votersModal) {
            votersModal.classList.remove('is-open');
            document.body.classList.remove('overflow-hidden');
        }
    });
});