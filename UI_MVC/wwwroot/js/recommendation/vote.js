document.addEventListener('DOMContentLoaded', function () {
    var voteForms = document.querySelectorAll('.vote-form');
    var currentUserIdElement = document.getElementById('current-user-id');
    var currentUserId = (currentUserIdElement === null || currentUserIdElement === void 0 ? void 0 : currentUserIdElement.getAttribute('data-user-id')) || 'anonymous';
    var voteStorageKey = "userVotes_".concat(currentUserId);
    var storedVotes = localStorage.getItem(voteStorageKey);
    var userVotes = storedVotes ? JSON.parse(storedVotes) : {};
    fetch('/Recommendation/GetUserVotes')
        .then(function (response) { return response.json(); })
        .then(function (data) {
        var serverVotes = {};
        data.forEach(function (recommendationId) {
            serverVotes[recommendationId] = true;
        });
        localStorage.setItem(voteStorageKey, JSON.stringify(serverVotes));
        updateButtonStates(serverVotes);
    })
        .catch(function (error) {
        console.error('Fout bij ophalen van gebruikersstemmen:', error);
        updateButtonStates(userVotes);
    });
    function updateButtonStates(votes) {
        voteForms.forEach(function (form) {
            var idInput = form.querySelector('input[name="id"]');
            if (!idInput)
                return;
            var recommendationId = idInput.value;
            var submitButton = form.querySelector('button');
            if (submitButton) {
                if (votes[recommendationId]) {
                    submitButton.textContent = 'Stem terugtrekken';
                    submitButton.classList.add('voted');
                }
                else {
                    submitButton.textContent = 'Stem';
                    submitButton.classList.remove('voted');
                }
            }
        });
    }
    voteForms.forEach(function (form) {
        form.addEventListener('submit', function (event) {
            event.preventDefault();
            var idInput = form.querySelector('input[name="id"]');
            if (!idInput)
                return;
            var recommendationId = idInput.value;
            var storedVotes = localStorage.getItem(voteStorageKey);
            var currentVotes = storedVotes ? JSON.parse(storedVotes) : {};
            var submitButton = form.querySelector('button');
            if (submitButton)
                submitButton.disabled = true;
            var isVoteRemoval = currentVotes[recommendationId] === true;
            var endpoint = isVoteRemoval ? '/Recommendation/RemoveVote' : '/Recommendation/Vote';
            var tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
            var token = (tokenInput === null || tokenInput === void 0 ? void 0 : tokenInput.value) || '';
            fetch(endpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify(parseInt(recommendationId, 10))
            })
                .then(function (response) {
                if (!response.ok) {
                    return response.json().then(function (errorData) {
                        throw new Error(errorData.message || (isVoteRemoval ? 'Stem terugtrekken mislukt' : 'Stemmen mislukt'));
                    });
                }
                return response.json();
            })
                .then(function (data) {
                var voteCountSpan = document.getElementById('vote-count-' + data.id);
                if (voteCountSpan) {
                    voteCountSpan.textContent = data.votes.toString();
                }
                if (isVoteRemoval) {
                    delete currentVotes[recommendationId];
                }
                else {
                    currentVotes[recommendationId] = true;
                }
                localStorage.setItem(voteStorageKey, JSON.stringify(currentVotes));
                updateButtonStates(currentVotes);
            })
                .catch(function (error) {
                console.error('Error bij stemactie:', error);
                alert(error.message || 'Er is een fout opgetreden bij het verwerken van uw stem.');
            })
                .finally(function () {
                if (submitButton)
                    submitButton.disabled = false;
            });
        });
    });
});
