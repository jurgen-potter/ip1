document.addEventListener('DOMContentLoaded', () => {
    const excelButton = document.getElementById('excel-download-button') as HTMLButtonElement | null;
    const loadingOverlay = document.getElementById('loadingOverlay') as HTMLDivElement | null;

    if (excelButton && loadingOverlay) {
        const originalButtonText = excelButton.innerHTML;

        excelButton.addEventListener("click", () => {
            loadingOverlay.classList.add('is-active');
            excelButton.disabled = true;
            excelButton.innerHTML = `
                <span class="inline-block animate-spin rounded-full h-4 w-4 border-t-2 border-b-2 border-white mr-2 align-middle"></span>
                Aanmaken...
            `;

            const panelId = excelButton.dataset.panelId;
            const tenant = window.location.pathname.split('/')[1];
            let selectedBatch = (document.querySelector("input[name='inline-radio-group']:checked") as HTMLInputElement)?.value;

            if (!selectedBatch) {
                selectedBatch = "1"; // Default to batch 1 if none selected
            }

            fetch(`/${tenant}/api/Invitations/download/${panelId}/${selectedBatch}`, {
                method: 'GET',
                headers: {
                    'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
                }
            })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(text => {
                            let errorMsg: string;
                            try {
                                const errorJson = JSON.parse(text);
                                errorMsg = errorJson.message || errorJson.title || text;
                            } catch (e) {
                                errorMsg = text || `Download failed with status: ${response.status}`;
                            }
                            throw new Error(errorMsg);
                        });
                    }
                    return response.blob();
                })
                .then(blob => {
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement("a");
                    a.href = url;
                    a.download = `Uitnodigingen_Batch${selectedBatch}_${new Date().toISOString().replace(/[:.]/g, '-')}.xlsx`;
                    document.body.appendChild(a);
                    a.click();
                    document.body.removeChild(a);
                    window.URL.revokeObjectURL(url);
                })
                .catch(error => {
                    console.error("Download error:", error);
                    alert("Er ging iets fout bij het downloaden: " + error.message);
                })
                .finally(() => {
                    if (loadingOverlay) {
                        loadingOverlay.classList.remove('is-active');
                    }
                    if (excelButton) {
                        excelButton.disabled = false;
                        excelButton.innerHTML = originalButtonText; 
                    }
                });
        });
    } else {
        if (!excelButton) console.error('Element "excel-download-button" niet gevonden.');
        if (!loadingOverlay) console.error('Element "loadingOverlay" niet gevonden.');
    }
});