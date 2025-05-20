document.addEventListener('DOMContentLoaded', () => {
    const panelInput = document.querySelector<HTMLInputElement>('#panelId');
    const panelId = panelInput?.value;
    const tenantId = window.location.pathname.split('/')[1];

    if (!panelId || !tenantId) {
        console.error('Missing panel ID or tenant ID');
        return;
    }

    loadUsers(tenantId, panelId);
});

async function loadUsers(tenantId: string, panelId: string): Promise<void> {
    try {
        const response = await fetch(`/${tenantId}/api/Members/${panelId}`, {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            }
        });

        if (!response.ok) throw new Error(`HTTP ${response.status}`);

        const users = await response.json();
        showUsers(users);
    } catch (error) {
        console.error('Error loading users:', error);
        alert('Er ging iets fout bij het laden van de leden!');
    }
}

function showUsers(users: any[]): void {
    const tbody = document.querySelector<HTMLTableSectionElement>('#members tbody');
    if (!tbody) return;

    tbody.innerHTML = ''; // Clear table first

    users.forEach(user => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td class="py-2 px-4 border-b">${user.email}</td>
            <td class="py-2 px-4 border-b text-center">${user.age}</td>
            <td class="py-2 px-4 border-b text-center">${user.gender}</td>
        `;
        tbody.appendChild(tr);
    });
}
