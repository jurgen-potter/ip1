document.addEventListener('DOMContentLoaded', () => {
    const excelButton = document.getElementById('excel-download-button') as HTMLButtonElement;
    excelButton.addEventListener("click", () => {
        const panelId = excelButton.dataset.panelId;
        const tenant = window.location.pathname.split('/')[1];

        fetch(`/${tenant}/api/Invitations/download/${panelId}`, {
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
                a.download = `Uitnodigingen_${new Date().toISOString().split("T")[0]}.xlsx`;
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

