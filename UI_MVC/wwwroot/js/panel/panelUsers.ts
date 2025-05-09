const panelIdGet : HTMLInputElement | null = document.querySelector('#panelId');
const panelId = ((panelIdGet) as HTMLInputElement).value;

document.addEventListener('DOMContentLoaded', () => {
    loadUsers();
});

function loadUsers() {
    fetch('/api/Members/' + panelId, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        }
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            }
            else {
                throw new Error('Kon geen leden laden!');
            }
        })
        .then(data => {
            console.log("First user:", data[0]);
            showUsers(data);
        })
        .catch(error => alert('Er ging iets fout! ' + error.status));
}

function showUsers(users : any) {
    const userTableBody = (document.querySelector('#members tbody') as HTMLTableElement).innerHTML = '';
    users.forEach((user: any) => addUserToList(user));
}


function addUserToList(user : any) {
    const tableGet : HTMLTableElement | null = document.querySelector('#members tbody');
    let tableBody : HTMLTableElement = tableGet as HTMLTableElement;
    
    tableBody.insertAdjacentHTML('beforeend',
        `<tr>
<td>${user.email}</td>
<td>${user.age}</td>
<td>${user.gender}</td>
</tr>`)
}