interface AdminDto {
    id: string;
    email: string;
    isSuper: boolean;
}

let currentAdminId: string | null = null;

window.addEventListener("DOMContentLoaded", () => {
    loadAdmins();
    setupDeleteAdminModal();
});

function setupDeleteAdminModal(): void {
    const modal = document.getElementById("deleteConfirmationModal")!;
    const confirmBtn = document.getElementById("confirmDelete")!;
    const cancelBtn = document.getElementById("cancelDelete")!;

    confirmBtn.addEventListener("click", () => {
        if (currentAdminId) {
            performDeleteAdmin(currentAdminId);
            modal.classList.add("hidden");
        }
    });

    cancelBtn.addEventListener("click", () => {
        modal.classList.add("hidden");
        currentAdminId = null;
    });
}

function loadAdmins(): void {
    fetch("/api/Admins", {
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
        .then((admins: AdminDto[]) => showAdmins(admins))
        .catch(error => alert(`Failed to load admins: ${error.message}`));
}

function showAdmins(admins: AdminDto[]): void {
    const tableBody = document.getElementById("adminTableBody")!;
    tableBody.innerHTML = "";

    admins.forEach(admin => {
        const row = document.createElement("tr");

        const adminCell = document.createElement("td");
        adminCell.innerHTML = `
            ${admin.email}
            ${admin.isSuper ? `<i title="Superadmin" class="fa-solid fa-star"></i>` : ""}
        `;

        const deleteCell = document.createElement("td");
        const deleteBtn = document.createElement("button");
        deleteBtn.textContent = "Verwijderen";
        deleteBtn.className = "btn btn-danger btn-sm";
        deleteBtn.onclick = () => deleteAdmin(admin.id);
        deleteCell.appendChild(deleteBtn);

        row.appendChild(adminCell);
        row.appendChild(deleteCell);
        tableBody.appendChild(row);
    });
}

function deleteAdmin(adminId: string): void {
    currentAdminId = adminId;
    const modal = document.getElementById("deleteConfirmationModal")!;
    modal.classList.remove("hidden");
}

function performDeleteAdmin(adminId: string): void {
    fetch(`/api/Admins/${adminId}`, {
        method: "DELETE",
        headers: {
            "Accept": "application/json"
        }
    })
        .then(response => {
            if (response.ok) {
                loadAdmins();
            } else {
                alert("Verwijderen mislukt.");
            }
        })
        .catch(error => alert(`Error bij verwijderen: ${error.message}`));
    
    currentAdminId = null;
}
