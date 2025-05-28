document.addEventListener('DOMContentLoaded', () => {
    const excelButton = document.getElementById('excel-download-button') as HTMLButtonElement;
    excelButton.addEventListener("click", () => {
        const panelId = excelButton.dataset.panelId;
        const tenant = window.location.pathname.split('/')[1];
        let selectedBatch = (document.querySelector("input[name='inline-radio-group']:checked") as HTMLInputElement)?.value;

        if (!selectedBatch) {
            selectedBatch = "1";
        }
        fetch(`/${tenant}/api/Invitations/download/${panelId}/${selectedBatch}`, {
            method: 'GET',
            headers: {
                'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Download failed.");
                }
                return response.blob();
            })
            .then(blob => {
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement("a");
                a.href = url;
                a.download = `Invitations_${new Date().toISOString().replace(/[:.]/g, '-')}.xlsx`;
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
                window.URL.revokeObjectURL(url);
            })
            .catch(error => {
                console.error("Download error:", error);
                alert("Er ging iets fout bij het downloaden: " + error.message);
            });
    });
});

