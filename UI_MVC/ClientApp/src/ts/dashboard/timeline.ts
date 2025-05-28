interface Meeting {
    id: number;
    title: string;
    date: string; 
}

type MeetingValidationErrors = Record<string, string | string[]> | string;
interface CreateMeetingResponse {
    success: boolean;
    meeting?: Meeting;
    errors?: Record<string, string | string[]> | string;
}

interface FormElements extends HTMLFormControlsCollection {
    Date: HTMLInputElement;
    Title: HTMLInputElement;
}

interface MeetingFormElement extends HTMLFormElement {
    elements: FormElements;
}

// DOM Elements voor de custom modal
let createMeetingModal: HTMLElement | null;
let openCreateMeetingModalBtn: HTMLButtonElement | null;
let closeCreateMeetingModalBtn: HTMLButtonElement | null;
let cancelCreateMeetingBtn: HTMLButtonElement | null;
let createMeetingForm: MeetingFormElement | null;

//Initialize timeline functionality when DOM is loaded
document.addEventListener('DOMContentLoaded', init);

//Main initialization function for timeline features
async function init(): Promise<void> {
    const panelEl = document.body.querySelector<HTMLElement>('[data-panel-id]');
    const panelId = panelEl?.dataset.panelId ?? '';
    const tenantId = window.location.pathname.split('/')[1];

    // Initialize modal elements
    createMeetingModal = document.getElementById('createMeetingModal');
    openCreateMeetingModalBtn = document.getElementById('openCreateMeetingModalBtn') as HTMLButtonElement;
    closeCreateMeetingModalBtn = document.getElementById('closeCreateMeetingModalBtn') as HTMLButtonElement;
    cancelCreateMeetingBtn = document.getElementById('cancelCreateMeetingBtn') as HTMLButtonElement;
    createMeetingForm = document.getElementById('createMeetingForm') as MeetingFormElement | null;

    setupTimelineItemClicks(panelId);
    setupCustomModalListeners();

    if (createMeetingForm) {
        setupForm(panelId, tenantId,createMeetingForm);
    }
}
//Set up click handling for timeline items
function setupTimelineItemClicks(panelId: string): void {
    document.body.addEventListener('click', (e) => {
        const target = (e.target as HTMLElement).closest('.timeline-item') as HTMLElement | null;
        if (!target) return;

        e.preventDefault(); // Voorkom standaardgedrag als het een link zou zijn

        const meetingId = target.dataset.meetingId;
        if (!meetingId) {
            console.error('Meeting ID not found on timeline item.', target);
            return;
        }

        const params = new URLSearchParams({ id: meetingId, panelId });
        window.location.href = `/Meeting/Details?${params.toString()}`;
    });
}

// --- Custom Modal Logic ---
function openModal(modal: HTMLElement | null): void {
    if (!modal) return;
    modal.classList.add('is-open');
    document.body.classList.add('overflow-hidden');
}

function closeModal(modal: HTMLElement | null): void {
    if (!modal) return;
    modal.classList.remove('is-open');
    document.body.classList.remove('overflow-hidden');
    resetForm();
}

function setupCustomModalListeners(): void {
    if (openCreateMeetingModalBtn && createMeetingModal) {
        openCreateMeetingModalBtn.addEventListener('click', () => {
            openModal(createMeetingModal);
        });
    }
    if (closeCreateMeetingModalBtn && createMeetingModal) {
        closeCreateMeetingModalBtn.addEventListener('click', () => {
            closeModal(createMeetingModal);
        });
    }
    if (cancelCreateMeetingBtn && createMeetingModal) {
        cancelCreateMeetingBtn.addEventListener('click', () => {
            closeModal(createMeetingModal);
        });
    }
    if (createMeetingModal) {
        createMeetingModal.addEventListener('click', (event) => {
            if (event.target === createMeetingModal) {
                closeModal(createMeetingModal);
            }
        });
    }
}

//Reset form to initial state
function resetForm(): void {
    if (!createMeetingForm) return;
    createMeetingForm.classList.remove('was-validated');
    const errorElements = {
        title: document.getElementById('errorTitle'),
        date: document.getElementById('errorDate')
    };
    errorElements.title?.classList.add('hidden');
    errorElements.date?.classList.add('hidden');
    createMeetingForm.reset();
    const dateInput = createMeetingForm.elements.Date;
    if (dateInput) {
        dateInput.value = getTomorrowDateString();
    }
    createMeetingForm.querySelectorAll('input').forEach(input => {
        input.classList.remove('is-invalid', 'is-valid');
    });
}

//Get tomorrow's date as ISO string (YYYY-MM-DD)
function getTomorrowDateString(): string {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().slice(0, 10);
}

//Get today's date with time set to midnight
function getTodayAtMidnight(): Date {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return today;
}

//Set up the meeting creation form
function setupForm(panelId: string,tenantId: string, form: MeetingFormElement): void {
    const elements = {
        saveBtn: document.getElementById('saveMeetingBtn') as HTMLButtonElement,
        dateInput: form.elements.Date,
        titleInput: form.elements.Title,
        errorTitle: document.getElementById('errorTitle'),
        errorDate: document.getElementById('errorDate')
    };
    const today = getTodayAtMidnight();
    if (elements.dateInput) {
        initializeDateInput(elements.dateInput, elements.errorDate, today);
    }
    setupInputListeners(elements);
    form.addEventListener('submit', (e) => handleFormSubmit(e, form, elements, panelId, tenantId));
}

//Initialize date input with validation and defaults
function initializeDateInput(
    dateInput: HTMLInputElement,
    errorDateElement: HTMLElement | null,
    today: Date
): void {
    dateInput.min = today.toISOString().slice(0, 10);
    dateInput.value = getTomorrowDateString();
    dateInput.addEventListener('change', () => {
        validateDateInput(dateInput, errorDateElement, today);
    });
    validateDateInput(dateInput, errorDateElement, today);
}

//Set up input listeners for validation reset
function setupInputListeners(elements: {
    titleInput: HTMLInputElement;
    dateInput: HTMLInputElement;
    errorTitle: HTMLElement | null;
    errorDate: HTMLElement | null;
}): void {
    const { titleInput, dateInput, errorTitle, errorDate } = elements;
    if (titleInput) {
        titleInput.addEventListener('input', () => {
            errorTitle?.classList.add('hidden');
            titleInput.classList.remove('is-invalid');
        });
    }
    if (dateInput) {
        dateInput.addEventListener('input', () => {
            errorDate?.classList.add('hidden');
            dateInput.classList.remove('is-invalid');
        });
    }
}

//Handle form submission for creating a meeting
async function handleFormSubmit(
    e: Event,
    form: MeetingFormElement,
    elements: {
        saveBtn: HTMLButtonElement;
        dateInput: HTMLInputElement;
        titleInput: HTMLInputElement;
        errorTitle: HTMLElement | null;
        errorDate: HTMLElement | null;
    },
    panelId: string,
    tenantId: string
): Promise<void> {
    e.preventDefault();
    const { saveBtn, dateInput, titleInput, errorTitle, errorDate } = elements;

    errorTitle?.classList.add('hidden');
    errorDate?.classList.add('hidden');
    if (titleInput) titleInput.classList.remove('is-invalid', 'is-valid');
    if (dateInput) dateInput.classList.remove('is-invalid', 'is-valid');

    if (!validateForm(titleInput, dateInput, errorTitle, errorDate)) {
        form.classList.add('was-validated');
        return;
    }

    setButtonLoadingState(saveBtn, true);

    try {
        const result = await submitFormData(form,tenantId, panelId);
        if (result.success && result.meeting) { // Controleer ook of result.meeting bestaat
            addToTimeline(result.meeting);
            closeModal(createMeetingModal);
        } else {
            handleServerValidationErrors(result.errors || "Er is een onbekende fout opgetreden.", elements);
        }
    } catch (err) {
        console.error('Error submitting form:', err);
        showError(errorTitle, "Fout bij het opslaan van de meeting."); // Algemene foutmelding
    } finally {
        setButtonLoadingState(saveBtn, false);
    }
}

//Validate form inputs
function validateForm(
    titleInput: HTMLInputElement | null,
    dateInput: HTMLInputElement | null,
    errorTitle: HTMLElement | null,
    errorDate: HTMLElement | null
): boolean {
    let isValid = true;
    const today = getTodayAtMidnight();

    if (!titleInput || !titleInput.value.trim()) {
        showError(errorTitle, 'Vul een titel in');
        titleInput?.classList.add('is-invalid');
        isValid = false;
    } else {
        titleInput.classList.add('is-valid');
    }

    if (!dateInput || !dateInput.value) {
        showError(errorDate, 'Selecteer een datum');
        dateInput?.classList.add('is-invalid');
        isValid = false;
    } else {
        const selectedDate = new Date(dateInput.value);
        selectedDate.setHours(0, 0, 0, 0);
        if (selectedDate < today) {
            showError(errorDate, 'De datum mag niet in het verleden liggen');
            dateInput.classList.add('is-invalid');
            isValid = false;
        } else if (isValid) {
            dateInput.classList.add('is-valid');
        }
    }
    return isValid;
}

//Show an error message in the specified element
function showError(errorElement: HTMLElement | null, message: string): void {
    if (!errorElement) return;
    errorElement.classList.remove('hidden');
    errorElement.textContent = message;
}

//Set button loading state
function setButtonLoadingState(button: HTMLButtonElement | null, isLoading: boolean): void {
    if (!button) return;
    button.disabled = isLoading;
    if (isLoading) {
        // Voor een Tailwind spinner zou je HTML moeten toevoegen of een CSS spinner gebruiken.
        // Dit is een simpele tekstuele indicatie.
        button.innerHTML = `
      <span class="inline-block animate-spin rounded-full h-4 w-4 border-t-2 border-b-2 border-white"></span>
      Opslaan...
    `;
    } else {
        button.textContent = 'Opslaan';
    }
}

//Submit form data to server
async function submitFormData(
    form: HTMLFormElement,
    tenantId: string,
    panelId: string
): Promise<CreateMeetingResponse> {
    const data = new FormData(form);
    data.set('panelId', panelId);

    try {
        const response = await fetch(`/${tenantId}/Meeting/Create`, {
            method: 'POST',
            body: data
        });

        const json = await response.json();

        if (!response.ok) {
            return {
                success: false,
                errors: json.errors || json.message || `Serverfout: ${response.statusText}`
            };
        }

        // Extra validatie op structuur van json
        if (typeof json.success === 'boolean' && (json.meeting || json.errors)) {
            return json as CreateMeetingResponse;
        }

        return {
            success: false,
            errors: 'Ongeldig antwoord van de server'
        };
    } catch (err) {
        return {
            success: false,
            errors: 'Netwerkfout of server niet bereikbaar'
        };
    }
}


//Handle validation errors returned from server
function handleServerValidationErrors(
    errors: MeetingValidationErrors,
    elements: {
        titleInput: HTMLInputElement;
        dateInput: HTMLInputElement;
        errorTitle: HTMLElement | null;
        errorDate: HTMLElement | null;
    }
): void {
    const { titleInput, dateInput, errorTitle, errorDate } = elements;
    if (!errors) return;

    if (titleInput) titleInput.classList.remove('is-invalid', 'is-valid');
    if (dateInput) dateInput.classList.remove('is-invalid', 'is-valid');
    errorTitle?.classList.add('hidden');
    errorDate?.classList.add('hidden');

    if (typeof errors === 'object') {
        if (errors.Title && titleInput) {
            showError(errorTitle, Array.isArray(errors.Title) ? errors.Title[0] : errors.Title);
            titleInput.classList.add('is-invalid');
        }
        if (errors.Date && dateInput) {
            showError(errorDate, Array.isArray(errors.Date) ? errors.Date[0] : errors.Date);
            dateInput.classList.add('is-invalid');
        }
    } else if (typeof errors === 'string') {
        // Probeer de foutmelding op het meest relevante veld te tonen
        if ((errors.toLowerCase().includes('date') || errors.toLowerCase().includes('datum')) && dateInput) {
            showError(errorDate, errors);
            dateInput.classList.add('is-invalid');
        } else if (titleInput) { // Default naar titelveld als het niet duidelijk is
            showError(errorTitle, errors);
            titleInput.classList.add('is-invalid');
        } else { // Of toon een algemene fout als geen van beide velden beschikbaar is
            alert("Serverfout: " + errors);
        }
    }
    console.error('Form validation failed:', errors);
}

//Validate date input and show appropriate visual feedback
function validateDateInput(
    dateInput: HTMLInputElement,
    errorDateElement: HTMLElement | null,
    today: Date
): boolean {
    const selectedDate = new Date(dateInput.value);
    selectedDate.setHours(0, 0, 0, 0);
    if (selectedDate < today) {
        const errorMessage = 'De datum mag niet in het verleden liggen';
        dateInput.classList.add('is-invalid');
        dateInput.classList.remove('is-valid');
        dateInput.setCustomValidity(errorMessage);
        showError(errorDateElement, errorMessage);
        return false;
    } else {
        dateInput.classList.remove('is-invalid');
        dateInput.classList.add('is-valid');
        dateInput.setCustomValidity('');
        errorDateElement?.classList.add('hidden');
        return true;
    }
}

//Add a new meeting to the timeline UI
function addToTimeline(meeting: Meeting): void {
    const container = document.querySelector<HTMLElement>('.timeline-line');
    if (!container) {
        console.error("Timeline container (.timeline-line) not found.");
        return;
    }
    if (!meeting || typeof meeting.date !== 'string' || typeof meeting.title !== 'string') {
        console.error("Invalid meeting data received:", meeting);
        return;
    }

    const meetingDate = new Date(meeting.date); 
    if (isNaN(meetingDate.getTime())) {
        console.error("Invalid date format for meeting:", meeting.date);
        return;
    }
    const noMeetingsMessage = document.getElementById('noMeetingsMessage');
    if (noMeetingsMessage) {
        noMeetingsMessage.style.display = 'none'; 
    }

    const formattedDate = meetingDate.toLocaleDateString('nl-NL', {
        day: '2-digit',
        month: 'short',
    });

    const newTimelineItem = createTimelineItem(meeting, formattedDate);
    insertTimelineItemInOrder(container, newTimelineItem, meetingDate);

    newTimelineItem.classList.add('timeline-item-new'); 
    setTimeout(() => {
        newTimelineItem.classList.remove('timeline-item-new');
    }, 600); 
}

//Create a timeline item element
function createTimelineItem(meeting: Meeting, formattedDate: string): HTMLElement {
    const el = document.createElement('div');
    el.className = 'timeline-item'; // Start zonder 'timeline-item-new'
    el.dataset.meetingId = String(meeting.id);
    el.dataset.date = meeting.date;
    el.innerHTML = `
    <div class="timeline-box">
      <div class="date">${formattedDate}</div>
      <h4 class="title">${meeting.title || 'Meeting'}</h4>
    </div>
  `;
    return el;
}

//Insert timeline item in chronological order
function insertTimelineItemInOrder(
    container: HTMLElement,
    newItem: HTMLElement,
    newItemDate: Date
): void {
    const existingItems = Array.from(container.children) as HTMLElement[];
    let inserted = false;
    for (let i = 0; i < existingItems.length; i++) {
        const itemDateStr = existingItems[i].dataset.date;
        if (itemDateStr) {
            const itemDate = new Date(itemDateStr);
            if (newItemDate < itemDate) {
                container.insertBefore(newItem, existingItems[i]);
                inserted = true;
                break;
            }
        }
    }
    if (!inserted) {
        container.append(newItem);
    }
}
