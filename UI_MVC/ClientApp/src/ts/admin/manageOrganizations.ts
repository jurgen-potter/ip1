interface TenantDto {
    id: string;
    name: string;
}

let currentTenantId: string | null = null;

window.addEventListener("DOMContentLoaded", () => {
    loadTenants();
    setupDeleteTenantModal();

    document.getElementById("searchForm")!.addEventListener("submit", e => {
        e.preventDefault();
        const query = (document.getElementById("searchInput") as HTMLInputElement).value;
        loadTenants(query);
    });

    document.getElementById("clearSearch")!.addEventListener("click", () => {
        const input = document.getElementById("searchInput") as HTMLInputElement;
        input.value = "";
        loadTenants();
    });
});

function setupDeleteTenantModal(): void {
    const modal = document.getElementById("deleteConfirmationModal")!;
    const confirmBtn = document.getElementById("confirmDelete")!;
    const cancelBtn = document.getElementById("cancelDelete")!;

    confirmBtn.addEventListener("click", () => {
        if (currentTenantId) {
            performDeleteTenant(currentTenantId);
            modal.classList.add("hidden");
        }
    });

    cancelBtn.addEventListener("click", () => {
        modal.classList.add("hidden");
        currentTenantId = null;
    });
}

function loadTenants(query: string = ""): void {
    fetch("/api/Tenants", {
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
        .then((tenants: TenantDto[]) => {
            const filtered = filterTenants(tenants, query);
            showTenants(filtered);
        })
        .catch(error => alert(`Failed to load tenants: ${error.message}`));
}

function showTenants(tenants: TenantDto[]): void {
    const tableBody = document.getElementById("tenantTableBody") as HTMLTableElement;
    tableBody.innerHTML = "";

    tenants.forEach(tenant => {
        const row = document.createElement("tr");

        const tenantCell = document.createElement("td");
        tenantCell.innerHTML = `
            ${tenant.name}
        `;

        const tenantIdCell = document.createElement("td");
        tenantIdCell.innerHTML = `
            ${tenant.id}
        `;

        const deleteCell = document.createElement("td");
        const deleteBtn = document.createElement("button");
        deleteBtn.textContent = "Verwijderen";
        deleteBtn.className = "btn btn-danger btn-sm";
        deleteBtn.onclick = () => deleteTenant(tenant.id);
        deleteCell.appendChild(deleteBtn);

        row.appendChild(tenantCell);
        row.appendChild(tenantIdCell);
        row.appendChild(deleteCell);
        tableBody.appendChild(row);
    });
}

function deleteTenant(tenantId: string): void {
    currentTenantId = tenantId;
    const modal = document.getElementById("deleteConfirmationModal")!;
    modal.classList.remove("hidden");
}

function performDeleteTenant(tenantId: string): void {
    fetch(`/api/Tenants/${tenantId}`, {
        method: "DELETE",
        headers: {
            "Accept": "application/json"
        }
    })
        .then(response => {
            if (response.ok) {
                loadTenants();
            } else {
                alert("Verwijderen mislukt.");
            }
        })
        .catch(error => alert(`Error bij verwijderen: ${error.message}`));
    
    currentTenantId = null;
}

function filterTenants(tenants: TenantDto[], query: string): TenantDto[] {
    if (!query) return tenants;
    return tenants.filter(t => t.name.toLowerCase().includes(query.toLowerCase()));
}
