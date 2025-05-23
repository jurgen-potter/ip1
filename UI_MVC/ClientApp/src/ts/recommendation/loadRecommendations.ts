const tenant = window.location.pathname.split('/')[1];
const panelId = Number(document.getElementById('panel-id')?.dataset.panelId);

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
                throw Error(`Received status code ${res.status}.`);
            }
        })
        .then(data => addMeetings(data))
        .catch(err => alert('Something went wrong: ' + err));
}

function addMeetings(meetings: any) {
    
    meetings.forEach((meeting: any) => createMeeting(meeting));

    meetings.forEach(meeting => {
        const meetingData: any = {
            title: meeting.meetingTitle,
            recommendations: []
        };

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
            });
        }

        const newMeetingElement = createMeeting(meetingData);
        newMeetingElement;
    });
}


function createMeeting(meeting: any): void {
    if (meeting.amountVotable == 0){
        
    }
    else if (meeting.recIds.length > 0){
        
    }
    for (let i = 0; i < meeting.recIds.length; i++) {
        createRecommendation(meeting,i);
    }
}

function createRecommendation(recommendation: any, currentRec: number): HTMLElement {
    
}

