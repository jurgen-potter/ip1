interface TenantDto {
    id: string;
    name: string;
}

window.addEventListener("DOMContentLoaded", () => {
    loadTenants();

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

        const deleteCell = document.createElement("td");
        const deleteBtn = document.createElement("button");
        deleteBtn.textContent = "Verwijderen";
        deleteBtn.className = "btn btn-danger btn-sm";
        deleteBtn.onclick = () => deleteTenant(tenant.id);
        deleteCell.appendChild(deleteBtn);

        row.appendChild(tenantCell);
        row.appendChild(deleteCell);
        tableBody.appendChild(row);
    });
}

function deleteTenant(tenantId: string): void {
    if (!confirm("Weet je zeker dat je deze organisatie wilt verwijderen?")) return;

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
}

function filterTenants(tenants: TenantDto[], query: string): TenantDto[] {
    if (!query) return tenants;
    return tenants.filter(t => t.name.toLowerCase().includes(query.toLowerCase()));
}
