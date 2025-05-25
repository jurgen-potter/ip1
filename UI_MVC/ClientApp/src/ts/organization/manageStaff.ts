interface OrganizationDto {
    id: string;
    email: string;
    isSuper: boolean;
}

const orgTenantId = window.location.pathname.split('/')[1];

document.addEventListener('DOMContentLoaded', () => {
    if (!orgTenantId) {
        console.error('Missing tenant ID');
        return;
    }

    loadOrganizations();
});

function loadOrganizations() {
    fetch(`/${orgTenantId}/api/Organizations`, {
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
        .then((organizations: OrganizationDto[]) => {
            showOrganizations(organizations);
        })
        .catch(error => alert(`Failed to load organizations: ${error.message}`));
}

function showOrganizations(organizations: OrganizationDto[]): void {
    const tableBody = document.getElementById("organizationTableBody") as HTMLTableElement;
    tableBody.innerHTML = "";

    organizations.forEach(organization => {
        const row = document.createElement("tr");
        
        row.innerHTML = `
            <td>
                ${organization.email}
                ${organization.isSuper ? `<i title="Superbeheerder" class="fa-solid fa-star"></i>` : ""}
            </td>
            <td></td>
        `;

        const deleteBtn = document.createElement("button");
        deleteBtn.className = "btn btn-danger btn-sm";
        deleteBtn.textContent = "Verwijderen";
        deleteBtn.addEventListener("click", () => deleteOrganization(organization.id));

        const deleteCell = row.querySelector("td:last-child");
        if (deleteCell) deleteCell.appendChild(deleteBtn);
        
        tableBody.appendChild(row);
    });
}

function deleteOrganization(organizationId: string): void {
    if (!confirm("Weet je zeker dat je dit organizatie account wilt verwijderen?")) return;

    fetch(`/${orgTenantId}/api/Organizations/${organizationId}`, {
        method: "DELETE",
        headers: {
            "Accept": "application/json"
        }
    })
        .then(response => {
            if (response.ok) {
                loadOrganizations();
            } else {
                alert("Verwijderen mislukt.");
            }
        })
        .catch(error => alert(`Error bij verwijderen: ${error.message}`));
}
