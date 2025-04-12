document.addEventListener('DOMContentLoaded', function () {
    const voteForms = document.querySelectorAll<HTMLFormElement>('.vote-form');

    const currentUserIdElement = document.getElementById('current-user-id');
    const currentUserId = currentUserIdElement?.getAttribute('data-user-id') || 'anonymous';

    const voteStorageKey = `userVotes_${currentUserId}`;

    type VotesRecord = Record<string, boolean>;

    const storedVotes = localStorage.getItem(voteStorageKey);
    const userVotes: VotesRecord = storedVotes ? JSON.parse(storedVotes) : {};

    interface UserVotesResponse {
        recommendationId: string;
    }

    fetch('/Recommendation/GetUserVotes')
        .then(response => response.json() as Promise<string[]>)
        .then(data => {
            const serverVotes: VotesRecord = {};

            data.forEach(recommendationId => {
                serverVotes[recommendationId] = true;
            });

            localStorage.setItem(voteStorageKey, JSON.stringify(serverVotes));
            updateButtonStates(serverVotes);
        })
        .catch(error => {
            console.error('Fout bij ophalen van gebruikersstemmen:', error);
            updateButtonStates(userVotes);
        });

    function updateButtonStates(votes: VotesRecord) {
        voteForms.forEach(form => {
            const idInput = form.querySelector<HTMLInputElement>('input[name="id"]');
            if (!idInput) return;

            const recommendationId = idInput.value;
            const submitButton = form.querySelector<HTMLButtonElement>('button');

            if (submitButton) {
                if (votes[recommendationId]) {
                    submitButton.textContent = 'Stem terugtrekken';
                    submitButton.classList.add('voted');
                } else {
                    submitButton.textContent = 'Stem';
                    submitButton.classList.remove('voted');
                }
            }
        });
    }

    voteForms.forEach(form => {
        form.addEventListener('submit', function (event: Event) {
            event.preventDefault();

            const idInput = form.querySelector<HTMLInputElement>('input[name="id"]');
            if (!idInput) return;

            const recommendationId = idInput.value;

            const storedVotes = localStorage.getItem(voteStorageKey);
            const currentVotes: VotesRecord = storedVotes ? JSON.parse(storedVotes) : {};

            const submitButton = form.querySelector<HTMLButtonElement>('button');
            if (submitButton) submitButton.disabled = true;

            const isVoteRemoval = currentVotes[recommendationId] === true;
            const endpoint = isVoteRemoval ? '/Recommendation/RemoveVote' : '/Recommendation/Vote';

            const tokenInput = document.querySelector<HTMLInputElement>('input[name="__RequestVerificationToken"]');
            const token = tokenInput?.value || '';

            fetch(endpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify(parseInt(recommendationId, 10))
            })
                .then(response => {
                    if (!response.ok) {
                        return response.json().then((errorData: { message?: string }) => {
                            throw new Error(errorData.message || (isVoteRemoval ? 'Stem terugtrekken mislukt' : 'Stemmen mislukt'));
                        });
                    }
                    return response.json();
                })
                .then((data: { id: string; votes: number }) => {
                    const voteCountSpan = document.getElementById('vote-count-' + data.id);
                    if (voteCountSpan) {
                        voteCountSpan.textContent = data.votes.toString();
                    }

                    if (isVoteRemoval) {
                        delete currentVotes[recommendationId];
                    } else {
                        currentVotes[recommendationId] = true;
                    }
                    localStorage.setItem(voteStorageKey, JSON.stringify(currentVotes));

                    updateButtonStates(currentVotes);
                })
                .catch(error => {
                    console.error('Error bij stemactie:', error);
                    alert(error.message || 'Er is een fout opgetreden bij het verwerken van uw stem.');
                })
                .finally(() => {
                    if (submitButton) submitButton.disabled = false;
                });
        });
    });
});
