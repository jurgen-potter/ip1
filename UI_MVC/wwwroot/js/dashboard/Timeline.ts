document.addEventListener('DOMContentLoaded', () => {
    const panelId = document
            .querySelector<HTMLElement>('[data-panel-id]')
            ?.dataset
            .panelId
        ?? '';
    
    document.body.addEventListener('click', (e) => {
        const target = (e.target as HTMLElement).closest('.timeline-item') as HTMLElement | null;
        if (!target) return;

        e.preventDefault();
        const meetingId = target.dataset.meetingId;
        if (!meetingId) return;

        let url = `/Meeting/Details?id=${meetingId}`;
        if (panelId) {
            url += `&panelId=${panelId}`;
        }
        window.location.href = url;
    });
});
