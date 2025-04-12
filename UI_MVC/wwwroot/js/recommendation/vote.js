document.addEventListener('DOMContentLoaded', function () {
    // Selecteer alle formulieren met de klasse 'vote-form'
    var voteForms = document.querySelectorAll('.vote-form');
    voteForms.forEach(function (form) {
        form.addEventListener('submit', function (event) {
            event.preventDefault();
            // Haal het id van de aanbeveling op
            var idInput = form.querySelector('input[name="id"]');
            if (!idInput) {
                return;
            }
            var recommendationId = parseInt(idInput.value);
            // knop uitschakelen om dubbel stemmen te voorkomen
            var submitButton = form.querySelector('button');
            if (submitButton) {
                submitButton.disabled = true;
            }
            // Verstuur een POST-verzoek met de id in de body als JSON
            fetch('/Recommendation/Vote', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                    // hier mss een anti forgery token
                },
                body: JSON.stringify(recommendationId)
            })
                .then(function (response) {
                if (!response.ok) {
                    throw new Error('Stemmen mislukt');
                }
                return response.json();
            })
                .then(function (data) {
                // Update de DOM met het nieuwe aantal stemmen
                var voteCountSpan = document.getElementById('vote-count-' + data.id);
                if (voteCountSpan) {
                    voteCountSpan.textContent = data.votes.toString();
                }
            })
                .catch(function (error) {
                console.error('Error bij stemmen:', error);
            })
                .finally(function () {
                if (submitButton) {
                    submitButton.disabled = false;
                }
            });
        });
    });
});
