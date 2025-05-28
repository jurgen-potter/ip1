interface OrganizationMemberDto {
    id: string;
    name: string;
}

const orgMemTenantId = window.location.pathname.split('/')[1];

document.addEventListener('DOMContentLoaded', () => {
    if (!orgMemTenantId) {
        console.error('Missing tenant ID');
        return;
    }

    loadOrganizationMembers();
});

function loadOrganizationMembers() {
    const panelIdElem = document.getElementById("panel-id") as HTMLInputElement;
    const panelId = panelIdElem.value;
    fetch(`/${orgMemTenantId}/api/Organizations/${panelId}`, {
        method: "GET",
        headers: {
            "Accept": "application/json"
        }
    })
        .then(response => {
            if (response.status === 204) {
                return [];
            }

            if (!response.ok) throw new Error("Network response was not ok");
            return response.json();
        })
        .then((organizationMembers: OrganizationMemberDto[]) => {
            showOrganizationMembers(organizationMembers);
        })
        .catch(error => alert(`Failed to load organization members: ${error.message}`));
}

function showOrganizationMembers(organizationMembers: OrganizationMemberDto[]): void {
    const tableBody = document.getElementById("memberTableBody") as HTMLTableElement;
    tableBody.innerHTML = "";

    if (organizationMembers.length === 0) {
        const row = document.createElement("tr");
        row.innerHTML = `<td colspan="2"><em>Geen organisatieleden gevonden.</em></td>`;
        tableBody.appendChild(row);
        return;
    }

    organizationMembers.forEach(organizationMember => {
        const row = document.createElement("tr");
        
        row.innerHTML = `
            <td>
                ${organizationMember.name}
            </td>
            <td></td>
        `;

        const addBtn = document.createElement("button");
        addBtn.className = "btn btn-success btn-sm";
        addBtn.textContent = "+";
        addBtn.addEventListener("click", () => addOrganizationMember(organizationMember.id));

        const addCell = row.querySelector("td:last-child");
        if (addCell) addCell.appendChild(addBtn);
        
        tableBody.appendChild(row);
    });
}

function addOrganizationMember(organizationMemberId: string): void {
    if (!confirm("Weet je zeker dat je dit organisatie account wilt toevoegen aan het panel?")) return;
    
    const panelIdElem = document.getElementById("panel-id") as HTMLInputElement;
    const panelId = panelIdElem.value;

    fetch(`/${orgMemTenantId}/api/OrganizationMembers/${panelId}/add-user/${organizationMemberId}`, {
        method: "POST",
        headers: {
            "Accept": "application/json"
        }
    })
        .then(response => {
            if (response.ok) {
                loadOrganizationMembers();
            } else {
                alert("Toevoegen mislukt.");
            }
        })
        .catch(error => alert(`Error bij toevoegen: ${error.message}`));
}
