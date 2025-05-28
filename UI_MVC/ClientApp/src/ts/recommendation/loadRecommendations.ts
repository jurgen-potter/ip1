interface MeetingDto {
    meetingId: number;
    meetingTitle: string;
    participants: number;
    amountVotable: number;
    recs: RecDto[];
}

interface RecDto {
    id: number;
    title: string;
    description: string;
    anonymous: boolean;
    votable: boolean;
    votes: number;
    votesFor: number;
    votesAgainst: number;
    neededPercentages: number;
}

let selectedRecommendationId: number | null = null;
let selectedRecommendationTitle: string | null = null;
let lastFocusedTrigger: HTMLElement | null = null;
const votersModalElement = document.getElementById('votersModal') as HTMLElement;

const tenant = window.location.pathname.split('/')[1];
const panelId = Number(document.getElementById('panel-id')?.dataset.panelId);
const currRole = (document.getElementById('current-user-role') as HTMLInputElement).value;

window.addEventListener('DOMContentLoaded', () => {
    loadMeetings();
});
function initializeModalButtons(closeButton :HTMLButtonElement, buttons : NodeListOf<HTMLButtonElement>) {
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

                setModalText();
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

function setModalText() {
    fetch(`/Recommendation/GetVoters/${selectedRecommendationId}`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
    })
        .then(res => {
            if (!res.ok) {
                throw Error(`Received status code ${res.status}.`);
            }
            else{
                setModalBodyText(res);
            }
        })
        .catch(err => alert(`Something went wrong: ${err}`));
}

async function setModalBodyText(response: Response) {
    const modalTitleElement = votersModalElement.querySelector<HTMLElement>('.member-reg-modal-title') as HTMLElement;
    const modalBodyElement = votersModalElement.querySelector<HTMLElement>('.member-reg-modal-body') as HTMLElement;

    const recommendationTitle = selectedRecommendationTitle ?? 'Stemmers Details';
    modalTitleElement.textContent = `Stemming voor aanbeveling: ${recommendationTitle}`;

    const htmlContent = await response.text();

    if (htmlContent.trim().length > 0) {
        modalBodyElement.innerHTML = htmlContent;
    }
    else{
        let userMessage = 'Kon de lijst met stemmers niet laden.';
        modalBodyElement.innerHTML = `<div class="alert alert-danger m-3">${userMessage}</div>`;
    }
}

function loadMeetings() {
    fetch(`/${tenant}/api/Meetings/getMeetings/${panelId}`, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        }
    })
        .then(res => {
            if (res.ok) {
                return res.json();
            } else {
                throw Error(`Received status code ${res.status}.`);
            }
        })
        .then(data => {
            addMeetings(data);
            loadButtons();
        })
        .catch(err => alert('Something went wrong: ' + err));
}

function addMeetings(meetings: MeetingDto[]) {
    const activeDiv = document.getElementById('active-body') as HTMLDivElement;
    const notActiveDiv = document.getElementById('not-active-body') as HTMLDivElement;
    const activeHeader = document.getElementById('active-header') as HTMLElement;
    const notActiveHeader = document.getElementById('not-active-header') as HTMLElement;

    activeDiv.innerHTML = '';
    notActiveDiv.innerHTML = '';
    activeHeader.classList.add('hidden');
    notActiveHeader.classList.add('hidden');
    
    meetings.forEach((meeting: MeetingDto) => createMeeting(meeting));
}


function createMeeting(meeting: MeetingDto): void {
    if (meeting.recs.length > 0) {
        const activeRecs = meeting.recs.filter(r => r.votable);
        const inactiveRecs = meeting.recs.filter(r => !r.votable);

        if (activeRecs.length > 0) {
            const activeHeader = document.getElementById('active-header') as HTMLHeadElement;
            activeHeader.classList.remove('hidden');
            const activeDiv = document.getElementById('active-body') as HTMLDivElement;

            const meetingContainer = document.createElement('div');
            meetingContainer.className = 'recommendation-meeting-group-container';

            const meetingTitle = document.createElement('h3');
            meetingTitle.className = 'recommendation-meeting-title';
            meetingTitle.textContent = meeting.meetingTitle;
            meetingContainer.appendChild(meetingTitle);

            const recsGrid = document.createElement('div');
            recsGrid.className = 'recommendations-list-grid';

            activeRecs.forEach(rec => {
                recsGrid.appendChild(generateRecommendationHtml(rec, meeting.participants));
            });

            meetingContainer.appendChild(recsGrid);
            activeDiv.appendChild(meetingContainer);
        }

        if (inactiveRecs.length > 0) {
            const notActiveHeader = document.getElementById('not-active-header') as HTMLHeadElement;
            notActiveHeader.classList.remove('hidden');
            const notActiveDiv = document.getElementById('not-active-body') as HTMLDivElement;

            const meetingContainer = document.createElement('div');
            meetingContainer.className = 'recommendation-meeting-group-container';

            const meetingTitle = document.createElement('h3');
            meetingTitle.className = 'recommendation-meeting-title';
            meetingTitle.textContent = meeting.meetingTitle;
            meetingContainer.appendChild(meetingTitle);

            const recsGrid = document.createElement('div');
            recsGrid.className = 'recommendations-list-grid';

            inactiveRecs.forEach(rec => {
                recsGrid.appendChild(generateRecommendationHtml(rec, meeting.participants));
            });

            meetingContainer.appendChild(recsGrid);
            notActiveDiv.appendChild(meetingContainer);
        }
    }
}

function generateRecommendationHtml(recommendation: RecDto, participants: number): HTMLElement {
    const card = document.createElement('div');
    card.className = 'recommendation-card';

    const body = document.createElement('div');
    body.className = 'recommendation-card-body';

    // Title with anonymous label if needed
    const title = document.createElement('h4');
    title.className = 'recommendation-card-title';
    title.textContent = recommendation.title;
    if (recommendation.anonymous) {
        const anonymousLabel = document.createElement('span');
        anonymousLabel.className = 'anonymous-label';
        anonymousLabel.textContent = ' (Anonieme stemming)';
        title.appendChild(anonymousLabel);
    }

    // Description
    const description = document.createElement('p');
    description.className = 'recommendation-card-text';
    description.textContent = recommendation.description;

    // Status
    const status = document.createElement('div');
    status.className = `recommendation-card-status ${recommendation.votable ? 'status-votable' : 'status-ended'}`;
    if (!recommendation.votable) {
        const percFor = recommendation.votesFor / recommendation.votes;
        status.textContent = `Het stemmen is afgelopen. ${(percFor >= (recommendation.neededPercentages / 100)) ? 'Aangenomen!' : 'Niet aangenomen.'}`;
    } else {
        status.textContent = 'Stemming open';
    }

    // Vote info (only for Organization)
    if (currRole === 'Organization') {
        const voteInfo = document.createElement('div');
        voteInfo.className = 'recommendation-card-vote-info';

        const votesList = document.createElement('ul');
        votesList.innerHTML = `
            <li><p>Stemmen: <span id="vote-count-${recommendation.id}">${recommendation.votes}/${participants}</span></p></li>
            <li><p>Voor: <span class="vote-count-for">${recommendation.votesFor}</span></p></li>
            <li><p>Tegen: <span class="vote-count-against">${recommendation.votesAgainst}</span></p></li>
            <li><p>Percentage nodig: ${recommendation.neededPercentages}%</p></li>
        `;
        voteInfo.appendChild(votesList);
        body.appendChild(voteInfo);
    }

    // Actions
    const actions = document.createElement('div');
    if (currRole === 'Organization') {
        actions.className = 'recommendation-card-actions';
        if (!recommendation.anonymous) {
            const viewBtn = document.createElement('button');
            viewBtn.className = 'btn btn-secondary btn-show-voters';
            viewBtn.setAttribute('data-recommendation-id', recommendation.id.toString());
            viewBtn.setAttribute('data-recommendation-title', recommendation.title);
            viewBtn.innerHTML = '<i class="fas fa-users"></i> Bekijk stemmers';
            actions.appendChild(viewBtn);
        }

        if (recommendation.votable) {
            const stopBtn = document.createElement('button');
            stopBtn.className = 'btn btn-primary btn-stop-voting';
            stopBtn.setAttribute('data-recommendation-id', recommendation.id.toString());
            stopBtn.innerHTML = '<i class="fas fa-stop-circle"></i> Stop stemming';
            actions.appendChild(stopBtn);
        }
    } else if (currRole === 'Member' && recommendation.votable) {
        actions.className = 'recommendation-vote-forms';

        // Create separate forms for vote for and vote against (matching original structure)
        const voteForForm = document.createElement('form');
        voteForForm.className = 'vote-form';
        voteForForm.innerHTML = `
            <input type="hidden" name="id" value="${recommendation.id}">
            <button type="submit" class="btn btn-success">Stem voor</button>
        `;

        const voteAgainstForm = document.createElement('form');
        voteAgainstForm.className = 'vote-form';
        voteAgainstForm.innerHTML = `
            <input type="hidden" name="id" value="${recommendation.id}">
            <button type="submit" class="btn btn-danger">Stem tegen</button>
        `;

        actions.appendChild(voteForForm);
        actions.appendChild(voteAgainstForm);
    }

    // Append all elements
    body.appendChild(title);
    body.appendChild(description);
    body.appendChild(status);
    body.appendChild(actions);
    card.appendChild(body);

    return card;
}

// Modified loadButtons function to include vote form setup
function loadButtons(): void {
    const confirmationModal = document.getElementById("confirmationModal") as HTMLElement;
    const stopVotingButton = confirmationModal?.querySelector("#stopVotingBtn") as HTMLButtonElement;
    const waitButton = confirmationModal?.querySelector("#cancel-stop-voting") as HTMLButtonElement;

    const endVoteButtons = document.querySelectorAll('.btn-stop-voting');

    endVoteButtons.forEach(button => {
        button.addEventListener("click", () => {
            const recommendationId = (button as HTMLElement).getAttribute("data-recommendation-id");
            if (recommendationId) {
                selectedRecommendationId = parseInt(recommendationId);
            }
            confirmationModal.classList.remove('hidden');
        });
    });

    stopVotingButton?.addEventListener("click", () => {
        if (selectedRecommendationId === null) return;

        fetch(`/Recommendation/StopVoting/${selectedRecommendationId}`, {
            method: 'GET'
        })
            .then(res => {
                if (!res.ok) {
                    throw Error(`Received status code ${res.status}.`)
                }
                loadMeetings();
            })
            .catch(err => alert('Something went wrong: ' + err));

        endVoteButtons.forEach(button => {
            const recommendationId = (button as HTMLElement).getAttribute("data-recommendation-id");
            if (recommendationId && parseInt(recommendationId) === selectedRecommendationId) {
                (button as HTMLButtonElement).hidden = true;
            }
        })

        confirmationModal.classList.add('hidden');
        selectedRecommendationId = null;

    });

    waitButton?.addEventListener("click", () => {
        confirmationModal.classList.add('hidden');
        selectedRecommendationId = null;
    });

    const showVotersButtons = document.querySelectorAll('.btn-show-voters') as NodeListOf<HTMLButtonElement>;
    const closeButton = votersModalElement?.querySelector("#closeModal") as HTMLButtonElement;

    initializeModalButtons(closeButton, showVotersButtons);

    // Setup voting forms
    setupVoteForms();
}

function setupVoteForms(): void {
    const voteForms = document.querySelectorAll<HTMLFormElement>('.vote-form');
    const votes: Record<string, boolean> = {};

    const updateButton = (form: HTMLFormElement, voted: boolean) => {
        const btns = form.getElementsByTagName('button');
        if (!btns || btns.length === 0) return;

        // Find the parent container to access both forms
        const parentContainer = form.parentElement;
        if (!parentContainer) return;

        const allForms = parentContainer.querySelectorAll<HTMLFormElement>('.vote-form');
        if (allForms.length < 2) return;

        const forForm = allForms[0];
        const againstForm = allForms[1];
        const forBtn = forForm.querySelector('button');
        const againstBtn = againstForm.querySelector('button');

        if (!forBtn || !againstBtn) return;

        if (!voted) {
            forBtn.classList.add('btn-success');
            forBtn.classList.remove('btn-primary');
            forBtn.textContent = 'Stem voor';
            againstBtn.hidden = false;
            againstBtn.textContent = 'Stem tegen';
        } else {
            forBtn.classList.remove('btn-success');
            forBtn.classList.add('btn-primary');
            forBtn.textContent = 'Stem terugtrekken';
            againstBtn.hidden = true;
        }
    };

    const updateAllButtons = () => {
        const processedIds = new Set<string>();
        voteForms.forEach(form => {
            const id = form.querySelector<HTMLInputElement>('input[name="id"]')?.value;
            if (id && !processedIds.has(id)) {
                updateButton(form, votes[id]);
                processedIds.add(id);
            }
        });
    };

    // Fetch existing votes
    fetch(`/${tenant}/api/Recommendations/userVotes`)
        .then(res => res.ok ? res.json() : Promise.reject())
        .then((ids: string[]) => {
            ids.forEach(id => votes[id] = true);
            updateAllButtons();
        })
        .catch(err => console.error('Error fetching user votes:', err));

    // Add event listeners to all vote forms
    voteForms.forEach(form => {
        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            const id = form.querySelector<HTMLInputElement>('input[name="id"]')?.value;
            if (!id) return;

            const voted = votes[id];
            const url = voted
                ? `/${tenant}/api/Recommendations/remove-vote`
                : `/${tenant}/api/Recommendations/vote`;

            const btn = form.querySelector<HTMLButtonElement>('button');
            if (btn) btn.disabled = true;

            try {
                const isVoteFor = btn?.textContent?.includes('voor');
                const userVote = {
                    id: parseInt(id),
                    recommended: voted ? true : (isVoteFor ? true : false)
                };

                const res = await fetch(url, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(userVote)
                });

                if (!res.ok) {
                    throw new Error(`Server returned ${res.status}`);
                }

                const { id: returnedId, votes: voteCount } = await res.json();

                // Update vote count display
                const countEl = document.getElementById(`vote-count-${returnedId}`);
                if (countEl) {
                    const currentText = countEl.textContent || '';
                    const parts = currentText.split('/');
                    if (parts.length === 2) {
                        countEl.textContent = `${voteCount}/${parts[1]}`;
                    }
                }

                // Update button states
                votes[id] = !voted;
                updateAllButtons();

            } catch (error) {
                console.error('Error during voting:', error);
                alert('Er is een fout opgetreden bij het stemmen. Probeer het opnieuw.');
            } finally {
                if (btn) btn.disabled = false;
            }
        });
    });
}