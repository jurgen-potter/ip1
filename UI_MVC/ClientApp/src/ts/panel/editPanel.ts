interface PanelDto {
    id: number;
    name: string;
    description: string;
    endDate: string;
    coverImagePath: string;
}

let panelData: PanelDto;

const editPanelInput = document.querySelector<HTMLInputElement>('#panelId');
const editPanelId = editPanelInput?.value;
const editPanelTenantId = window.location.pathname.split('/')[1];

document.addEventListener("DOMContentLoaded", () => {
    if (!editPanelId || !editPanelTenantId) {
        console.error('Missing panel ID or tenant ID');
        return;
    }
    
    loadPanelData();

    document.getElementById("saveBtn")?.addEventListener("click", savePanel);
    document.getElementById("bannerUpload")?.addEventListener("change", uploadBanner);
});

function loadPanelData() {
    fetch(`${editPanelTenantId}/api/Panels/${editPanelId}`, {
        method: "GET",
        headers: {"Accept": "application/json"}
    })
        .then(res => res.json())
        .then(data => {
            panelData = data;
            showPanelData();
        })
        .catch(err => console.error("Error loading panel data:", err));
}

function showPanelData() {
    (document.getElementById("titleInput") as HTMLInputElement).value = panelData.name;
    (document.getElementById("descriptionInput") as HTMLTextAreaElement).value = panelData.description;
    (document.getElementById("endDateInput") as HTMLInputElement).value = panelData.endDate;
    (document.getElementById("bannerPreview") as HTMLImageElement).src = panelData.coverImagePath || "";
}

function savePanel() {
    panelData.name = (document.getElementById("titleInput") as HTMLInputElement).value;
    panelData.description = (document.getElementById("descriptionInput") as HTMLTextAreaElement).value;
    panelData.endDate = (document.getElementById("endDateInput") as HTMLInputElement).value;
    
    fetch(`${editPanelTenantId}/api/Panels/${editPanelId}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(panelData),
    })
        .then(res => {
            if (!res.ok) throw new Error("Opslaan mislukt");
            window.location.href = "/Panel/Index";
        })
        .catch(err => alert(err.message));
}

function uploadBanner(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const formData = new FormData();
    formData.append("file", input.files[0]);

    fetch(`${editPanelTenantId}/api/Panels/${editPanelId}/UploadBanner`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: formData
    })
        .then(res => res.json())
        .then(data => {
            (document.getElementById("bannerPreview") as HTMLImageElement).src = data.path;
            panelData.coverImagePath = data.path;
        })
        .catch(err => alert("Fout bij uploaden: " + err.message));
}
