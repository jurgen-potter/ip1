const rejectCheckbox = document.getElementById('reject-box') as HTMLInputElement;
const rejectValue = document.getElementById('reject-value') as HTMLInputElement;

const finishRecCheckboxes = document.querySelectorAll('.finish-rec-checkbox') as NodeListOf<HTMLInputElement>;

window.addEventListener('DOMContentLoaded', () => {
    rejected();
    updateItems();
    
    if (rejectCheckbox !== null){
        rejectCheckbox.addEventListener('change', () => rejected() );
    }
    
    finishRecCheckboxes.forEach(checkbox => finishCheckboxHandler(checkbox));
});

function rejected() {
    const rejected = document.getElementById('rejected') as HTMLTableSectionElement;
    let value;
    if (rejectCheckbox !== null){
        value = rejectCheckbox.checked;
    }
    else {
        value = (rejectValue.value == "true");
    }
    if (value) {
        rejected.classList.remove('hidden');
    }
    else {
        rejected.classList.add('hidden');
    }

    const tenant = window.location.pathname.split('/')[1];
    const panelId = Number(rejected.getAttribute('data-panel-id'));

    const panelDto = {id: panelId, showRejected: value}
    fetch(`/${tenant}/api/Panels/edit`, {
        method: 'PUT',
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

function showRejected(recs: RecDto[]): void  {
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
        list.innerHTML = `<p>Er zijn nog geen afgewezen aanbevelingen.`;
    }
}

function finishCheckboxHandler(checkbox: HTMLInputElement): void {
    checkbox.addEventListener('change', () => {
        const tenant = window.location.pathname.split('/')[1];
        const recId = checkbox.getAttribute('data-recommendation-id') as string;
        
        const panelDto = {id: recId, isDone: checkbox.checked}
        
        fetch(`/${tenant}/api/Recommendations/edit`, {
            method: 'PUT',
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
                else {
                    updateRecItem(Number(recId), checkbox.checked)
                }
            })
            .catch(err => alert('Something went wrong: ' + err));
    });
}

function updateItems() {
    const update = document.querySelectorAll('.finished-message') as NodeListOf<HTMLInputElement>;
    
    update.forEach(item => {
        let isDone = false;
        if (item.value === "true"){
            isDone = true;
        }
        updateRecItem(Number(item.getAttribute('data-recommendation-id')), isDone);
    });
}

function updateRecItem(recId: number, isDone: boolean): void {
    const recItem = document.getElementById('rec-item-' + recId) as HTMLLIElement;
    if (isDone) {
        recItem.insertAdjacentHTML('afterbegin', `
        <p id="rec-item-finished-${recId}"><strong style="color: green">Deze aanbeveling is voltooid</strong></p>
    `);
    }
    else {
        const recItemFinished = document.getElementById('rec-item-finished-' + recId);
        if (recItemFinished) {
            recItemFinished.remove();
        }
    }
}