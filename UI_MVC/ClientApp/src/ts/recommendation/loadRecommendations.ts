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

function loadButtons() : void {
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
                console.log("Fetch status:", res.status); // ✅ Add this

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

        // 🪵 Add logs here
        console.log("✅ Just called loadMeetings(). Current DOM state:");
        console.log("Active body:", document.getElementById('active-body')?.innerHTML);
        console.log("Not-active body:", document.getElementById('not-active-body')?.innerHTML);

    });

    waitButton?.addEventListener("click", () => {
        confirmationModal.classList.add('hidden');
        selectedRecommendationId = null;
    });

    const showVotersButtons = document.querySelectorAll('.btn-show-voters') as NodeListOf<HTMLButtonElement>;
    const closeButton = votersModalElement?.querySelector("#closeModal") as HTMLButtonElement;

    initializeModalButtons(closeButton, showVotersButtons);

    const voteForms = document.querySelectorAll<HTMLFormElement>('.vote-form');
    // Houd bij op welke items al gestemd is 
    const votes: Record<string, boolean> = {};

    const updateButton = (form: HTMLFormElement, voted: boolean) => {
        // Vind de knop in het formulier
        const btns = form.getElementsByTagName('button');
        if (!btns) return; // typescript validatie
        // Past de tekst van de btn aan
        if (!voted) {
            btns[0].classList.add('btn-success');
            btns[0].classList.remove('btn-primary');
            btns[0].textContent = 'Stem voor'
            btns[1].hidden = false;
            btns[1].textContent = 'Stem tegen'
            // Als je deze tekst aanpast moet je bij let userVote de volgende lijn ook aanpassen(lijn 68: op het moment van schrijven)
        }
        else {
            btns[0].classList.remove('btn-success');
            btns[0].classList.add('btn-primary');
            btns[0].textContent = 'Stem terugtrekken'
            btns[1].hidden = true
        }
        //btnFor[0].textContent = voted ? 'Stem terugtrekken' : 'Stem voor';
        //btn.classList.toggle('voted', voted);
    };
    const updateAllButtons = () => {
        voteForms.forEach(form => {
            // Haal het id op van recommendation
            const id = form.querySelector<HTMLInputElement>('input[name="id"]')?.value;
            if (id) updateButton(form, votes[id]);
        });
    };

    fetch(`/${tenant}/api/Recommendations/userVotes`)
        .then(res => res.ok ? res.json() : Promise.reject()) //bij fout gaat naar de catch
        .then((ids: string[]) => {
            // Zet voor elk ontvangen id de stemstatus op true
            ids.forEach(id => votes[id] = true);
            // Werk de knoppen bij volgens de opgehaalde data
            updateAllButtons();
        });

    // Voeg submit-handler toe aan elk stem-formulier
    voteForms.forEach(form => {
        form.querySelectorAll('button').forEach(form2 => {
            form2.addEventListener('click', async e => {
                e.preventDefault();
                const id = form.querySelector<HTMLInputElement>('input[name="id"]')?.value;
                if (!id) return;

                // Bepaal of we een stem trekken of uitbrengen
                const voted = votes[id];
                const url = voted
                    ? `/${tenant}/api/Recommendations/remove-vote`
                    : `/${tenant}/api/Recommendations/vote`;
                // Vind de knop en zet deze tijdelijk uit
                const btn = form.querySelector<HTMLButtonElement>('button');
                btn?.setAttribute('disabled', '');

                try {
                    // Stuur het stemverzoek naar de server
                    let userVote = {id: +id, recommended: true}
                    if (form2.textContent === 'Stem tegen') {
                        userVote.recommended = false;
                    }
                    const res = await fetch(url, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        // Stuur het id als getal
                        body: JSON.stringify(userVote)
                    });

                    // Ontvang het nieuwe stemtotaal
                    const { id: returnedId, votes: voteCount } = await res.json();
                    // Werk de teller op de pagina bij
                    const countEl = document.getElementById(`vote-count-${returnedId}`);
                    if (countEl) countEl.textContent = String(voteCount); //if statement staat er voor typescript

                    // Update in-memory stemstatus en pas knoppen aan
                    votes[id] = !voted;
                    updateAllButtons();
                } catch {
                    // Toon foutmelding bij mislukking
                    console.log('Fout bij verwerken van uw stem.');
                } finally {
                    // Zet de knop weer aan
                    btn?.removeAttribute('disabled');
                }
            })
        })
    });

}

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
        console.error('Fout bij ophalen/weergeven stemmers');
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
            console.log("Loaded meetings:", data); // ✅ Add this

            addMeetings(data);
            loadButtons();
        })
        .catch(err => alert('Something went wrong: ' + err));
}

function addMeetings(meetings: any) {
    const activeDiv = document.getElementById('active-body') as HTMLDivElement;
    const notActiveDiv = document.getElementById('not-active-body') as HTMLDivElement;
    const activeHeader = document.getElementById('active-header') as HTMLElement;
    const notActiveHeader = document.getElementById('not-active-header') as HTMLElement;

    activeDiv.innerHTML = '';
    notActiveDiv.innerHTML = '';
    activeHeader.classList.add('hidden');
    notActiveHeader.classList.add('hidden');
    
    meetings.forEach((meeting: any) => createMeeting(meeting));
}


function createMeeting(meeting: any): void {
    if (meeting.recIds.length > 0){
        if (meeting.amountVotable !== 0){
            const activeHeader = document.getElementById('active-header') as HTMLHeadElement;
            activeHeader.classList.remove('hidden');
            const meetingsDiv = document.getElementById('active-body') as HTMLDivElement;
            const newMeeting = generateMeetingHtml(meeting, 'active');
            meetingsDiv.appendChild(newMeeting);
        }
        if (meeting.amountVotable !== meeting.recIds.length){
            const notActiveHeader = document.getElementById('not-active-header') as HTMLHeadElement;
            notActiveHeader.classList.remove('hidden');
            const meetingsDiv = document.getElementById('not-active-body') as HTMLDivElement;
            const newMeeting = generateMeetingHtml(meeting, 'not-active');
            meetingsDiv.appendChild(newMeeting);
        }
        createRecommendations(meeting);
    }
}

function createRecommendations(meeting: any): void {
    for (let i = 0; i < meeting.recIds.length; i++) {
        console.log(`Rec ${meeting.recIds[i]} votable?`, meeting.recVotable[i]); // ✅ Add this

        if (meeting.recVotable[i]) {
            const recsDiv = document.getElementById('active-recommendation-body-' + meeting.meetingId) as HTMLDivElement;
            const newRec = generateRecommendationHtml(meeting,i);
            recsDiv.appendChild(newRec);
        }
        else {
            const recsDiv = document.getElementById('not-active-recommendation-body-' + meeting.meetingId) as HTMLDivElement;
            const newRec = generateRecommendationHtml(meeting,i);
            recsDiv.appendChild(newRec);
        }
    }
}


function generateMeetingHtml(meeting: any, active: string) : HTMLElement {
    const newMeeting = document.createElement('div');
    newMeeting.classList.add(active + '-meeting-body-' + meeting.meetingId);
    newMeeting.innerHTML = `
        <h3>${meeting.meetingTitle}</h3>
        <div id="${active}-recommendation-body-${meeting.meetingId}">
        
        </div>`;

    return newMeeting;
}

function generateRecommendationHtml(meeting: any, currRec: number) : HTMLElement {
    const newRec = document.createElement('div');
    newRec.classList.add('recommendation-body-' + meeting.recIds[currRec]);
    const anonymousHeader = (meeting.recAnon[currRec] as boolean) 
        ? "(Anonieme stemming) " 
        : "";
    
    const finished = (!meeting.recVotable[currRec] as boolean)
        ? `<p class="mb-1"><strong>Het stemmen is afgelopen</strong></p>`
        : (meeting.recNeededVotes[currRec] <= meeting.recVotes[currRec])
            ? `<p class="mb-1"><strong>U heeft genoeg stemmen voor een gebalanceerd resultaat</strong></p>`
            : ""
    
    let text = "";
    
    if (currRole === 'Organization'){
        let buttons = "";
        if (!meeting.recAnon[currRec]){
            buttons += `
                <button type="button" class="btn btn-sm btn-outline-info btn-show-voters"
                        data-recommendation-id=${meeting.recIds[currRec]}
                        data-recommendation-title=${meeting.recTitles[currRec]}>
                    Bekijk Stemmers
                </button>`
        }
        if (meeting.recVotable[currRec]){
            buttons += `
                <button type="button" class="btn btn-sm btn-outline-danger btn-stop-voting"
                        data-recommendation-id=${meeting.recIds[currRec]}>
                    Stop stemronde
                </button>
            `
        }
        text += `
            <div class="mt-auto">
                <p class="mb-1">Alle stemmen: <span id="vote-count-${meeting.recIds[currRec]}">${meeting.recVotes[currRec]}</span></p>
                <ul>
                    <li>
                        <p class="mb-1" style="color: limegreen">Voor deze aanbeveling: ${meeting.recVotesFor[currRec]}</p>
                    </li>
                    <li>
                        <p class="mb-1" style="color: red">Tegen deze aanbeveling: ${meeting.recVotesAgainst[currRec]}</p>
                    </li>
                </ul>
                <div class="d-flex justify-content-start mt-4 gap-2">
                    ${buttons}
                </div>
            </div>`
    }
    else if (currRole === 'Member' && meeting.recVotable[currRec]){
        text += `
            <div class="vote-form">
                <form class="mt-auto">
                    <input type="hidden" name="id" value=${meeting.recIds[currRec]}>
                    <button type="submit" id="vote-for" class="btn btn-success">Stem voor</button>
                </form>

                <form class="mt-auto">
                    <input type="hidden" name="id" value=${meeting.recIds[currRec]}>
                    <button type="submit" id="vote-against" class="btn btn-danger">Stem tegen</button>
                </form>
            </div>`
    }
    
    newRec.innerHTML += `
        <div class="card h-100 w-100">
            <div class="card-body d-flex flex-column">
                <h5 class="card-title">
                    ${anonymousHeader}${meeting.recTitles[currRec]}
                </h5>
                <p class="card-text flex-grow-1">${meeting.recDescriptions[currRec]}</p>
                <p class="mb-1" id=${meeting.recIds[currRec]} hidden>
                    <strong>Het stemmen is afgelopen</strong>
                </p>

                ${finished}
                ${text}
            </div>
        </div>`
    return newRec;
}

