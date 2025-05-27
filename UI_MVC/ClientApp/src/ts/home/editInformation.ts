interface InfoSectionDto {
    title: string;
    text: string;
}

interface InfoPageDto {
    mainTitle: string;
    sections: InfoSectionDto[];
}

let infoPage: InfoPageDto = { mainTitle: "", sections: [] };

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
        headers: { "Accept": "application/json" }
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
            <button class="delete-btn text-red-600 mt-2 underline">Verwijderen</button>
        `;

        div.querySelector(".title-input")?.addEventListener("input", (e: Event) => {
            infoPage.sections[index].title = (e.target as HTMLInputElement).value;
        });

        div.querySelector(".text-input")?.addEventListener("input", (e: Event) => {
            infoPage.sections[index].text = (e.target as HTMLTextAreaElement).value;
        });

        div.querySelector(".delete-btn")?.addEventListener("click", () => {
            if (confirm("Deze sectie verwijderen?")) {
                infoPage.sections.splice(index, 1);
                renderEditor();
            }
        });

        container.appendChild(div);
    });
}

function addSection(): void {
    infoPage.sections.push({ title: "", text: "" });
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
