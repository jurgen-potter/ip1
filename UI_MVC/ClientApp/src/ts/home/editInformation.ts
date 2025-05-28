interface InfoSectionDto {
    title: string;
    text: string;
    videoUrl: string;
    fileUrl: string;
}

interface InfoPageDto {
    mainTitle: string;
    sections: InfoSectionDto[];
}

let infoPage: InfoPageDto = {mainTitle: "", sections: []};

document.addEventListener("DOMContentLoaded", () => {
    loadPageContent();

    const addBtn = document.getElementById("addSectionBtn");
    addBtn?.addEventListener("click", addSection);

    const saveBtn = document.getElementById("saveSectionsBtn");
    saveBtn?.addEventListener("click", saveSections);
});

function loadPageContent(): void {
    fetch(`/api/InfoPageContents`, {
        method: "GET",
        headers: {"Accept": "application/json"}
    })
        .then(res => res.ok ? res.json() : Promise.reject("Laden mislukt"))
        .then((data: InfoPageDto) => {
            infoPage = data;
            renderEditor();
        })
        .catch(err => alert("Laden mislukt: " + err));
}

function renderEditor(): void {
    const container = document.getElementById("sectionsContainer");
    const titleInput = document.getElementById("mainTitleInput") as HTMLInputElement;

    if (titleInput) {
        titleInput.value = infoPage.mainTitle;
        titleInput.addEventListener("input", () => {
            infoPage.mainTitle = titleInput.value;
        });
    }

    if (!container) return;
    container.innerHTML = "";

    infoPage.sections.forEach((section, index) => {
        const div = document.createElement("div");
        div.className = "section-edit p-4 mb-4 bg-gray-100 rounded";

        div.innerHTML = `
            <input type="text" class="title-input w-full mb-2 p-2" value="${section.title}" placeholder="Titel..." />
            <textarea class="text-input w-full p-2" rows="4" placeholder="Tekst...">${section.text}</textarea>

            <label class="block mt-2">Video upload:</label>
            <input type="file" accept="video/*" class="video-upload-input" />

            <label class="block mt-2">Bestand upload:</label>
            <input type="file" class="file-upload-input" />

            <div class="uploaded-video-url">
                ${section.videoUrl ? `URL: <a href="${section.videoUrl}" target="_blank">${section.videoUrl}</a> 
                <button class="remove-video-btn text-red-600 ml-2">Verwijder video</button>` : ""}
            </div>
            <div class="uploaded-file-url">
                ${section.fileUrl ? `URL: <a href="${section.fileUrl}" target="_blank">${section.fileUrl}</a> 
                <button class="remove-file-btn text-red-600 ml-2">Verwijder bestand</button>` : ""}
            </div>

            <button class="delete-btn text-red-600 mt-2 underline">Verwijderen sectie</button>
        `;

        // Title input handler
        div.querySelector(".title-input")?.addEventListener("input", (e: Event) => {
            infoPage.sections[index].title = (e.target as HTMLInputElement).value;
        });

        // Text input handler
        div.querySelector(".text-input")?.addEventListener("input", (e: Event) => {
            infoPage.sections[index].text = (e.target as HTMLTextAreaElement).value;
        });

        // Delete section handler
        div.querySelector(".delete-btn")?.addEventListener("click", () => {
            if (confirm("Deze sectie verwijderen?")) {
                infoPage.sections.splice(index, 1);
                renderEditor();
            }
        });

        // Upload video handler
        div.querySelector(".video-upload-input")?.addEventListener("change", async (e) => {
            const input = e.target as HTMLInputElement;
            if (!input.files || input.files.length === 0) return;

            const file = input.files[0];
            const formData = new FormData();
            formData.append("file", file);

            try {
                const res = await fetch("/api/Uploads", { method: "POST", body: formData });
                if (!res.ok) throw new Error("Upload failed");

                const data = await res.json();
                infoPage.sections[index].videoUrl = data.url;

                // Update UI
                renderEditor();
            } catch (err) {
                alert("Fout bij uploaden video: " + err);
            }
        });

        // Upload file handler
        div.querySelector(".file-upload-input")?.addEventListener("change", async (e) => {
            const input = e.target as HTMLInputElement;
            if (!input.files || input.files.length === 0) return;

            const file = input.files[0];
            const formData = new FormData();
            formData.append("file", file);

            try {
                const res = await fetch("/api/Uploads", { method: "POST", body: formData });
                if (!res.ok) throw new Error("Upload failed");

                const data = await res.json();
                infoPage.sections[index].fileUrl = data.url;

                // Update UI
                renderEditor();
            } catch (err) {
                alert("Fout bij uploaden bestand: " + err);
            }
        });

        // Remove video button handler
        div.querySelector(".remove-video-btn")?.addEventListener("click", () => {
            if (confirm("Video verwijderen?")) {
                infoPage.sections[index].videoUrl = "";
                renderEditor();
            }
        });

        // Remove file button handler
        div.querySelector(".remove-file-btn")?.addEventListener("click", () => {
            if (confirm("Bestand verwijderen?")) {
                infoPage.sections[index].fileUrl = "";
                renderEditor();
            }
        });

        container.appendChild(div);
    });
}


function addSection(): void {
    infoPage.sections.push({title: "", text: "", videoUrl: "", fileUrl: ""});
    renderEditor();
}

function saveSections(): void {
    fetch(`/api/InfoPageContents`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify(infoPage)
    })
        .then(res => {
            if (res.ok) {
                alert("Opgeslagen!");
                window.location.href = "/Home/Information";
            } else {
                throw new Error("Opslaan mislukt");
            }
        })
        .catch(err => alert("Fout bij opslaan: " + err.message));
}
