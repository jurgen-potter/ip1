// dist/js/dashboard/timeline.ts
document.addEventListener('DOMContentLoaded', init);

async function init() {
    // 1) Make sure we treat it as an HTMLElement (so `.dataset` is known)
    const panelEl = document.body.querySelector<HTMLElement>('[data-panel-id]');
    const panelId = panelEl?.dataset.panelId ?? '';      // never undefined

    // 2) Delegate clicks on .timeline-item
    document.body.addEventListener('click', (e) => {
        // cast to HTMLElement so .closest returns an HTMLElement
        const target = (e.target as HTMLElement).closest('.timeline-item') as HTMLElement | null;
        if (!target) return;
        e.preventDefault();

        const meetingId = target.dataset.meetingId;
        if (!meetingId) return;

        // 3) URLSearchParams expects a Record<string, string>
        const params = new URLSearchParams({
            id: meetingId,
            panelId,          
        });

        window.location.href = `/Meeting/Details?${params.toString()}`;
    });

    setupForm(panelId);
}


function setupForm(panelId: string) {
    const form = document.getElementById('createMeetingForm') as HTMLFormElement | null;
    if (!form) return;

    const saveBtn  = document.getElementById('saveMeetingBtn')  as HTMLButtonElement;
    const dateInput = form.elements.namedItem('Date') as HTMLInputElement;

    // set min & default date
    const today    = new Date();
    today.setHours(0,0,0,0);
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);

    dateInput.min   = today.toISOString().slice(0,10);
    dateInput.value = tomorrow.toISOString().slice(0,10);

    form.addEventListener('submit', async (e) => {
        e.preventDefault();

        if (!form.checkValidity()) {
            form.classList.add('was-validated');
            return;
        }

        saveBtn.disabled = true;
        saveBtn.innerHTML = `
      <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
      Opslaan...
    `;

        const data = new FormData(form);
        // 4) FormData.set expects string | Blob, so ensure panelId is string
        data.set('panelId', panelId);

        try {
            const res = await fetch('/Meeting/Create', { method: 'POST', body: data });
            const json = await res.json();
            if (json.success) {
                addToTimeline(json.meeting);
                form.reset();
                dateInput.value = tomorrow.toISOString().slice(0,10);
                bootstrap.Modal.getInstance(
                    document.getElementById('createMeetingModal')!
                )?.hide();
            } else {
                console.error(json.errors);
            }
        } catch (err) {
            console.error(err);
        } finally {
            saveBtn.disabled = false;
            saveBtn.textContent = 'Opslaan';
        }
    });
}


function addToTimeline(meeting: { id: number; title: string; date: string }) {
    const container = document.querySelector<HTMLElement>('.timeline-line')!;
    const dt = new Date(meeting.date);
    const formatted = dt.toLocaleDateString('nl-NL', {
        day:   '2-digit',
        month: 'short',
    });

    const el = document.createElement('div');
    el.className = 'timeline-item timeline-item-new';
    el.dataset.meetingId = String(meeting.id);
    el.innerHTML = `
    <div class="timeline-box">
      <div class="date">${formatted}</div>
      <h4 class="title">${meeting.title || 'Meeting'}</h4>
    </div>
  `;

    // insert chronologically
    const items = Array.from(container.children) as HTMLElement[];
    const idx   = items.findIndex(item => {
        const text = item.querySelector('.date')?.textContent ?? '';
        const [day, mon] = text.split(' ');
        const months = ['jan','feb','mrt','apr','mei','jun','jul','aug','sep','okt','nov','dec'];
        const year   = new Date().getFullYear();
        const itemDate = new Date(year, months.indexOf(mon.toLowerCase()), parseInt(day,10));
        return dt < itemDate;
    });

    if (idx >= 0) container.insertBefore(el, items[idx]);
    else           container.append(el);

    // remove animation class after it plays
    setTimeout(() => el.classList.remove('timeline-item-new'), 600);
}
