const tenant = window.location.pathname.split('/')[1];
const panelId = Number(document.getElementById('panel-id')?.dataset.panelId);
const currRole = (document.getElementById('current-user-role') as HTMLInputElement).nodeValue;

window.addEventListener('DOMContentLoaded', () => {
    loadMeetings();
});

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
                throw Error('Received status code ${res.status}.');
            }
        })
        .then(data => addMeetings(data))
        .catch(err => alert('Something went wrong: ' + err));
}

function addMeetings(meetings: any) {
    const meetingsDiv = document.getElementById('active-body') as HTMLDivElement;
    meetingsDiv.innerHTML = '';
    
    meetings.forEach((meeting: any) => createMeeting(meeting));
/*
        for (let i = 0; i < meeting.recIds.length; i++) {
            meetingData.recommendations.push({
                id: meeting.recIds[i],
                title: meeting.recTitles[i],
                description: meeting.recDescriptions[i],
                isAnonymous: meeting.recAnon[i],
                isVotable: meeting.recVotable[i],
                votes: meeting.recVotes[i],
                votesFor: meeting.recVotesFor[i],
                votesAgainst: meeting.recVotesAgainst[i],
                showEnoughVotesText: meeting.recVotesFor[i] + meeting.recVotesAgainst[i] >= 10
            });*/
}


function createMeeting(meeting: any): void {
    if (meeting.recIds.length > 0){
        if (meeting.amountVotable !== 0){
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
        for (let i = 0; i < meeting.recIds.length; i++) {
            createRecommendation(meeting,i);
        }
    }
}

function createRecommendation(meeting: any, currentRec: number): HTMLElement {
    if(currRole === 'Organization'){
        
    }
}


function generateMeetingHtml(meeting: any, active: string) : HTMLElement {
    const newMeeting = document.createElement('div');
    newMeeting.classList.add(active + '-meeting-body-' + meeting.meetingId);
    newMeeting.innerHTML = `
        <h3>${meeting.meetingTitle}</h3>
        <div id="active-recommendation-body-${meeting.meetingId}">
        
        </div>`;

    return newMeeting;
}

function generateRecommendationHtml(meeting: any, organization: string) : HTMLElement {
    
}

