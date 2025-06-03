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

            // Render video OR image from videoUrl
            if (section.videoUrl) {
                const extension = section.videoUrl.split('.').pop()?.toLowerCase() || "";

                const imageExtensions = ["jpg", "jpeg", "png", "gif", "webp"];
                const videoExtensions = ["mp4", "webm", "ogg"];

                if (videoExtensions.includes(extension)) {
                    const videoEl = document.createElement("video");
                    videoEl.controls = true;
                    videoEl.className = "w-full max-w-lg my-4";
                    videoEl.src = section.videoUrl;
                    videoEl.setAttribute("preload", "metadata");
                    sectionDiv.appendChild(videoEl);
                } else if (imageExtensions.includes(extension)) {
                    const imgEl = document.createElement("img");
                    imgEl.className = "w-full max-w-lg my-4 rounded";
                    imgEl.src = section.videoUrl;
                    imgEl.alt = "Afbeelding";
                    sectionDiv.appendChild(imgEl);
                }
            }

            // If there's a file URL, add a download link
            if (section.fileUrl) {
                const fileLink = document.createElement("a");
                fileLink.href = section.fileUrl;
                fileLink.textContent = "Download bestand";
                fileLink.className = "block mt-2 text-blue-600 underline";
                fileLink.target = "_blank";
                sectionDiv.appendChild(fileLink);
            }

            sectionsContainer.appendChild(sectionDiv);
        });
    }
}

