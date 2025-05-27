interface InfoSectionDto {
    title: string;
    text: string;
}

interface InfoPageDto {
    mainTitle: string;
    sections: InfoSectionDto[];
}

document.addEventListener("DOMContentLoaded", () => {
    loadInfoPageContent();
});

function loadInfoPageContent(): void {
    fetch('/api/InfoPageContents', {
        method: "GET",
        headers: { "Accept": "application/json" }
    })
        .then(res => {
            if (!res.ok) throw new Error("Failed to load info page content");
            return res.json();
        })
        .then((data: InfoPageDto) => {
            renderInfoPage(data);
        })
        .catch(err => {
            const container = document.getElementById("infoSection");
            if (container) container.innerHTML = `<p class="text-red-600">Fout bij laden: ${err.message}</p>`;
        });
}

function renderInfoPage(infoPage: InfoPageDto): void {
    const mainTitleEl = document.getElementById("mainTitle");
    const sectionsContainer = document.getElementById("sectionsContainer");

    if (mainTitleEl) {
        mainTitleEl.textContent = infoPage.mainTitle || "Geen titel beschikbaar";
    }

    if (sectionsContainer) {
        sectionsContainer.innerHTML = "";

        infoPage.sections.forEach(section => {
            const sectionDiv = document.createElement("div");
            sectionDiv.className = "mb-8";

            const titleEl = document.createElement("h2");
            titleEl.className = "text-2xl font-semibold mt-8 mb-4";
            titleEl.textContent = section.title;

            const textEl = document.createElement("p");
            textEl.className = "whitespace-pre-line";
            textEl.textContent = section.text;

            sectionDiv.appendChild(titleEl);
            sectionDiv.appendChild(textEl);

            sectionsContainer.appendChild(sectionDiv);
        });
    }
}
