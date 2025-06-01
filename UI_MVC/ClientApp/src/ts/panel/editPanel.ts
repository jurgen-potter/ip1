interface PanelDto {
    id: number;
    name: string;
    description: string;
    endDate: string;
    coverImagePath: string;
    bannerImagePath: string;
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
    document.getElementById("coverUpload")?.addEventListener("change", uploadCover);
});

function loadPanelData() {
    fetch(`/${editPanelTenantId}/api/Panels/${editPanelId}`, {
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

    const bannerImg = document.getElementById("bannerPreview") as HTMLImageElement;
    const coverImg = document.getElementById("coverPreview") as HTMLImageElement;

    if (panelData.coverImagePath) {
        coverImg.src = panelData.coverImagePath;
        coverImg.style.display = "block";
    } else {
        coverImg.src = "";
        coverImg.style.display = "none";
    }
    if (panelData.bannerImagePath) {
        bannerImg.src = panelData.bannerImagePath;
        bannerImg.style.display = "block";
    } else {
        bannerImg.src = "";
        bannerImg.style.display = "none";
    }
}

function savePanel() {
    if (!validateForm()) return;
    
    panelData.name = (document.getElementById("titleInput") as HTMLInputElement).value;
    panelData.description = (document.getElementById("descriptionInput") as HTMLTextAreaElement).value;
    panelData.endDate = (document.getElementById("endDateInput") as HTMLInputElement).value;
    
    fetch(`/${editPanelTenantId}/api/Panels/${editPanelId}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(panelData),
    })
        .then(res => {
            if (!res.ok) throw new Error("Opslaan mislukt");
            window.location.href = `/Panel/Index/${editPanelId}`;
        })
        .catch(err => alert(err.message));
}

function uploadBanner(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const formData = new FormData();
    formData.append("file", input.files[0]);

    fetch(`/${editPanelTenantId}/api/Panels/${editPanelId}/UploadBanner`, {
        method: "POST",
        body: formData
    })
        .then(res => res.json())
        .then(data => {
            (document.getElementById("bannerPreview") as HTMLImageElement).src = data.path;
            panelData.coverImagePath = data.path;
        })
        .catch(err => alert("Fout bij uploaden: " + err.message));
}

function uploadCover(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const formData = new FormData();
    formData.append("file", input.files[0]);

    fetch(`/${editPanelTenantId}/api/Panels/${editPanelId}/uploadCover`, {
        method: "POST",
        body: formData
    })
        .then(res => res.json())
        .then(data => {
            (document.getElementById("coverPreview") as HTMLImageElement).src = data.path;
            panelData.coverImagePath = data.path;
        })
        .catch(err => alert("Fout bij uploaden: " + err.message));
}
function validateForm(): boolean {
    let valid = true;

    const title = (document.getElementById("titleInput") as HTMLInputElement).value.trim();
    const endDate = (document.getElementById("endDateInput") as HTMLInputElement).value;

    // Reset validation messages
    document.getElementById("titleValidation")!.textContent = '';
    document.getElementById("endDateValidation")!.textContent = '';

    if (!title) {
        document.getElementById("titleValidation")!.textContent = "Titel is verplicht.";
        valid = false;
    }

    if (!endDate) {
        document.getElementById("endDateValidation")!.textContent = "Einddatum is verplicht.";
        valid = false;
    } else {
        const today = new Date();
        const selectedDate = new Date(endDate);
        
        today.setHours(0, 0, 0, 0);

        if (selectedDate < today) {
            document.getElementById("endDateValidation")!.textContent = "Einddatum mag niet in het verleden liggen.";
            valid = false;
        }
    }

    return valid;
}
