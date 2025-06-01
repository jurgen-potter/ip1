interface ReserveDto {
    id: number;
    email: string;
}

const resTenantId = window.location.pathname.split('/')[1];

document.addEventListener('DOMContentLoaded', () => {
    if (!resTenantId) {
        console.error('Missing tenant ID');
        return;
    }

    loadReserves();
    initForm();
});

function loadReserves() {
    const panelIdElem = document.getElementById("panel-id") as HTMLInputElement;
    const resPanelId = panelIdElem.value;
    fetch(`/${resTenantId}/api/Invitations/${resPanelId}`, {
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
        .then((reserves: ReserveDto[]) => {
            showReserves(reserves);
        })
        .catch(error => alert(`Failed to load reserves: ${error.message}`));
}

function showReserves(reserves: ReserveDto[]): void {
    const tableBody = document.getElementById("reservesTableBody") as HTMLTableElement;
    tableBody.innerHTML = "";

    const inviteForm = document.getElementById("inviteForm") as HTMLElement;
    if (reserves.length === 0) {
        if (inviteForm) {
            inviteForm.style.display = "none";
        }
        return;
    }

    reserves.forEach(reserve => {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${reserve.email}</td>
            <td>
                <input type="checkbox" class="userCheckbox" value="${reserve.id}" />
            </td>
        `;
        tableBody.appendChild(row);
    });
}

function initForm() {
    document.getElementById("inviteForm")?.addEventListener("submit", (e) => {
        const selectedIds = Array.from(document.querySelectorAll<HTMLInputElement>(".userCheckbox:checked"))
            .map(checkbox => checkbox.value);

        if (selectedIds.length === 0) {
            e.preventDefault();
            console.log("Selecteer ten minste één gebruiker.");
            return;
        }

        const hiddenInput = document.getElementById("selectedInvitationIds") as HTMLInputElement;
        hiddenInput.value = JSON.stringify(selectedIds.map(id => parseInt(id)));
    });
}