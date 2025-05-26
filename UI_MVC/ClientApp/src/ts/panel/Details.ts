const checkbox = document.getElementById('rejectBox') as HTMLInputElement;
const rejectValue = document.getElementById('rejectValue') as HTMLInputElement;

window.addEventListener('DOMContentLoaded', () => {
    rejected();
    if (checkbox !== null){
        checkbox.addEventListener('change', () => {
            rejected();
            if(checkbox.checked){
                const tenant = window.location.pathname.split('/')[1];
                const panelId = Number((document.querySelector('.add-post-button') as HTMLAnchorElement).getAttribute('data-panel-id'));

                const panelDto = {id: panelId, showRejected: checkbox.checked}
                fetch(`/${tenant}/api/Panels/edit`, {
                    method: 'POST',
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(panelDto),
                })
                    .then(res => {
                        if (!res.ok) {
                            throw Error(`Received status code ${res.status}.`)
                        }
                        else{
                            return res.json();
                        }
                    })
                    .then(data => showRejected(data))
                    .catch(err => alert('Something went wrong: ' + err));
            }
        });
    }
});

function rejected() {
    const rejected = document.getElementById('rejected') as HTMLTableSectionElement;
    let value = false;
    if (checkbox !== null){
        value = checkbox.checked;
    }
    else {
        value = rejectValue.value as unknown as boolean;
    }
    if (value) {
        rejected.classList.remove('hidden');
    }
    else {
        rejected.classList.add('hidden');
    }
}

function showRejected(recs: any): void  {
    const list = document.getElementById('rejectedList') as HTMLUListElement;
    list.innerHTML = '';
    if (recs.length > 0) {
        for (let i = 0; i < recs.length; i++) {
            const newRec = document.createElement('li');
            newRec.classList.add('recommendation-item');
            newRec.innerHTML = `
                <strong>${recs[i].title}</strong>
                <p>${recs[i].description}</p>`;
            
            list.appendChild(newRec);
        }
    }
    else {
        list.innerHTML = `<p>Er zijn nog geen afgewezen aanbevelingen`;
    }
}