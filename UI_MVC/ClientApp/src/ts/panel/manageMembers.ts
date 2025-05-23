interface MemberDto {
    id: string;
    name: string;
    email: string;
    isStaff: boolean;
}

const panelInput = document.querySelector<HTMLInputElement>('#panelId');
const panelId = panelInput?.value;
const tenantId = window.location.pathname.split('/')[1];

document.addEventListener('DOMContentLoaded', () => {
    if (!panelId || !tenantId) {
        console.error('Missing panel ID or tenant ID');
        return;
    }

    loadMembers();
});

function loadMembers() {
    fetch(`/${tenantId}/api/Members/${panelId}`, {
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
        .then((members: MemberDto[]) => {
            showMembers(members);
        })
        .catch(error => alert(`Failed to load members: ${error.message}`));
}

function showMembers(members: MemberDto[]): void {
    const tableBody = document.getElementById("memberTableBody") as HTMLTableElement;
    tableBody.innerHTML = "";

    members.forEach(member => {
        const row = document.createElement("tr");
        
        row.innerHTML = `
            <td>
                ${member.name}
                ${member.isStaff ? `<i title="Staff" class="fa-solid fa-star"></i>` : ""}
            </td>
            <td>${member.email}</td>
            <td></td>
        `;

        const deleteBtn = document.createElement("button");
        deleteBtn.className = "btn btn-danger btn-sm";
        deleteBtn.textContent = "Verwijderen";
        deleteBtn.addEventListener("click", () => deleteMember(member.id));

        const deleteCell = row.querySelector("td:last-child");
        if (deleteCell) deleteCell.appendChild(deleteBtn);
        
        tableBody.appendChild(row);
    });
}

function deleteMember(memberId: string): void {
    if (!confirm("Weet je zeker dat je dit panellid wilt verwijderen?")) return;

    fetch(`/${tenantId}/api/Members/${memberId}`, {
        method: "DELETE",
        headers: {
            "Accept": "application/json"
        }
    })
        .then(response => {
            if (response.ok) {
                loadMembers();
            } else {
                alert("Verwijderen mislukt.");
            }
        })
        .catch(error => alert(`Error bij verwijderen: ${error.message}`));
}
