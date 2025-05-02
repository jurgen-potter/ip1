interface Meeting {
    id: number;
    title: string;
    date: string;
}

interface FormElements extends HTMLFormControlsCollection {
    Date: HTMLInputElement;
    Title: HTMLInputElement;
}

interface MeetingFormElement extends HTMLFormElement {
    elements: FormElements;
}

//Initialize timeline functionality when DOM is loaded
document.addEventListener('DOMContentLoaded', init);


//Main initialization function for timeline features
async function init(): Promise<void> {
    // Get panel ID from DOM
    const panelEl = document.body.querySelector<HTMLElement>('[data-panel-id]');
    const panelId = panelEl?.dataset.panelId ?? '';

    // Set up timeline item click handling
    setupTimelineItemClicks(panelId);

    // Initialize form and modal listeners
    setupForm(panelId);
    setupModalListeners();
}

//Set up click handling for timeline items
function setupTimelineItemClicks(panelId: string): void {
    document.body.addEventListener('click', (e) => {
        // Delegate clicks on .timeline-item
        const target = (e.target as HTMLElement).closest('.timeline-item') as HTMLElement | null;
        if (!target) return;

        e.preventDefault();

        const meetingId = target.dataset.meetingId;
        if (!meetingId) return;

        // Navigate to meeting details with appropriate parameters
        const params = new URLSearchParams({
            id: meetingId,
            panelId,
        });

        window.location.href = `/Meeting/Details?${params.toString()}`;
    });
}

//Set up modal event listeners
function setupModalListeners(): void {
    const modal = document.getElementById('createMeetingModal');
    if (!modal) return;
    modal.addEventListener('hidden.bs.modal', () => {
        resetForm();
    });
}

//Reset form to initial state voor validatie te resetten
function resetForm(): void {
    const form = document.getElementById('createMeetingForm') as MeetingFormElement | null;
    if (!form) return;

    // Reset validation state
    form.classList.remove('was-validated');

    // Clear error messages
    const errorElements = {
        title: document.getElementById('errorTitle'),
        date: document.getElementById('errorDate')
    };

    errorElements.title?.classList.add('d-none');
    errorElements.date?.classList.add('d-none');

    // Reset all form fields
    form.reset();

    // Set default date to tomorrow
    const dateInput = form.elements.Date;
    dateInput.value = getTomorrowDateString();

    // Clear validation visual states
    form.querySelectorAll('input').forEach(input => {
        input.classList.remove('is-invalid', 'is-valid');
    });
}

//Get tomorrow's date as ISO string (YYYY-MM-DD) voor deftig te displayen.
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
function setupForm(panelId: string): void {
    const form = document.getElementById('createMeetingForm') as MeetingFormElement | null;
    if (!form) return;
    const elements = {
        saveBtn: document.getElementById('saveMeetingBtn') as HTMLButtonElement,
        dateInput: form.elements.Date,
        titleInput: form.elements.Title,
        errorTitle: document.getElementById('errorTitle'),
        errorDate: document.getElementById('errorDate')
    };
    // Initialize date input
    const today = getTodayAtMidnight();
    initializeDateInput(elements.dateInput, elements.errorDate, today);
    // Clear validation errors when inputs change
    setupInputListeners(elements);
    // Handle form submission
    form.addEventListener('submit', (e) => handleFormSubmit(e, form, elements, panelId));
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
    // Validate on initial load
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
    titleInput.addEventListener('input', () => {
        errorTitle?.classList.add('d-none');
        titleInput.classList.remove('is-invalid');
    });
    dateInput.addEventListener('input', () => {
        errorDate?.classList.add('d-none');
        dateInput.classList.remove('is-invalid');
    });
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
    panelId: string
): Promise<void> {
    e.preventDefault();
    const { saveBtn, dateInput, titleInput, errorTitle, errorDate } = elements;

    // Reset error messages and validation states
    errorTitle?.classList.add('d-none');
    errorDate?.classList.add('d-none');
    titleInput.classList.remove('is-invalid', 'is-valid');
    dateInput.classList.remove('is-invalid', 'is-valid');

    // Perform client-side validation
    if (!validateForm(titleInput, dateInput, errorTitle, errorDate)) {
        form.classList.add('was-validated');
        return;
    }

    // Show loading state
    setButtonLoadingState(saveBtn, true);

    try {
        // Submit form data
        const result = await submitFormData(form, panelId);

        if (result.success) {
            // Handle successful submission
            addToTimeline(result.meeting);
            resetForm();
            closeModal();
        } else {
            // Handle validation errors from server
            handleServerValidationErrors(result.errors, elements);
        }
    } catch (err) {
        console.error('Error submitting form:', err);
    } finally {
        // Reset button state
        setButtonLoadingState(saveBtn, false);
    }
}

//Validate form inputs
function validateForm(
    titleInput: HTMLInputElement,
    dateInput: HTMLInputElement,
    errorTitle: HTMLElement | null,
    errorDate: HTMLElement | null
): boolean {
    let isValid = true;
    const today = getTodayAtMidnight();

    // Validate title
    if (!titleInput.value.trim()) {
        showError(errorTitle, 'Vul een titel in');
        titleInput.classList.add('is-invalid');
        isValid = false;
    } else {
        titleInput.classList.add('is-valid');
    }

    // Validate date - only add validation classes if there's an actual issue
    const selectedDate = new Date(dateInput.value);
    selectedDate.setHours(0, 0, 0, 0);

    if (!dateInput.value) {
        showError(errorDate, 'Selecteer een datum');
        dateInput.classList.add('is-invalid');
        isValid = false;
    } else if (selectedDate < today) {
        showError(errorDate, 'De datum mag niet in het verleden liggen');
        dateInput.classList.add('is-invalid');
        isValid = false;
    } else if (isValid) { // Only mark as valid if the entire form is valid so far
        dateInput.classList.add('is-valid');
    }

    return isValid;
}

//Show an error message in the specified element
function showError(errorElement: HTMLElement | null, message: string): void {
    if (!errorElement) return;

    errorElement.classList.remove('d-none');
    errorElement.textContent = message;
}

//Set button loading state
function setButtonLoadingState(button: HTMLButtonElement, isLoading: boolean): void {
    button.disabled = isLoading;

    if (isLoading) {
        button.innerHTML = `
      <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
      Opslaan...
    `;
    } else {
        button.textContent = 'Opslaan';
    }
}

//Submit form data to server
async function submitFormData(form: HTMLFormElement, panelId: string): Promise<any> {
    const data = new FormData(form);
    data.set('panelId', panelId);

    const response = await fetch('/Meeting/Create', {
        method: 'POST',
        body: data
    });

    return await response.json();
}

//Close the modal programmatically
function closeModal(): void {
    const modal = document.getElementById('createMeetingModal');
    if (!modal) return;

    const modalInstance = bootstrap.Modal.getInstance(modal);
    modalInstance?.hide();
}

//Handle validation errors returned from server
function handleServerValidationErrors(
    errors: any,
    elements: {
        titleInput: HTMLInputElement;
        dateInput: HTMLInputElement;
        errorTitle: HTMLElement | null;
        errorDate: HTMLElement | null;
    }
): void {
    const { titleInput, dateInput, errorTitle, errorDate } = elements;

    if (!errors) return;

    // First clear any previous validation states to avoid cross-field effects
    titleInput.classList.remove('is-invalid', 'is-valid');
    dateInput.classList.remove('is-invalid', 'is-valid');
    errorTitle?.classList.add('d-none');
    errorDate?.classList.add('d-none');

    // Handle ModelState errors (object)
    if (typeof errors === 'object') {
        if (errors.Title) {
            showError(errorTitle, Array.isArray(errors.Title) ? errors.Title[0] : errors.Title);
            titleInput.classList.add('is-invalid');
        }

        if (errors.Date) {
            showError(errorDate, Array.isArray(errors.Date) ? errors.Date[0] : errors.Date);
            dateInput.classList.add('is-invalid');
        }
    }
    // Handle string errors
    else if (typeof errors === 'string') {
        if (errors.includes('date') || errors.includes('datum')) {
            showError(errorDate, errors);
            dateInput.classList.add('is-invalid');
        } else {
            // Show generic error on title field as default
            showError(errorTitle, errors);
            titleInput.classList.add('is-invalid');
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

        // Apply invalid state
        dateInput.classList.add('is-invalid');
        dateInput.classList.remove('is-valid');
        dateInput.setCustomValidity(errorMessage);

        // Show error message
        showError(errorDateElement, errorMessage);
        return false;
    } else {
        // Clear validation states
        dateInput.classList.remove('is-invalid');
        dateInput.classList.add('is-valid');
        dateInput.setCustomValidity('');
        errorDateElement?.classList.add('d-none');
        return true;
    }
}

//Add a new meeting to the timeline UI
function addToTimeline(meeting: Meeting): void {
    const container = document.querySelector<HTMLElement>('.timeline-line');
    if (!container) return;

    // Parse the date string from the server
    const meetingDate = new Date(meeting.date);

    // Format the date for display using Dutch locale
    const formattedDate = meetingDate.toLocaleDateString('nl-NL', {
        day: '2-digit',
        month: 'short',
    });

    // Create the new timeline item element
    const newTimelineItem = createTimelineItem(meeting, formattedDate);

    // Insert at the correct chronological position
    insertTimelineItemInOrder(container, newTimelineItem, meetingDate);

    // Remove animation class after it plays
    setTimeout(() => newTimelineItem.classList.remove('timeline-item-new'), 600);
}

//Create a timeline item element
function createTimelineItem(meeting: Meeting, formattedDate: string): HTMLElement {
    const el = document.createElement('div');
    el.className = 'timeline-item timeline-item-new';
    el.dataset.meetingId = String(meeting.id);
    el.dataset.date = meeting.date;  // Store ISO date for sorting

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

    // Find index of first item that's later than new item
    const insertIndex = existingItems.findIndex(item => {
        const itemDateStr = item.dataset.date;
        if (!itemDateStr) {
            return false;
        }

        const itemDate = new Date(itemDateStr);
        return newItemDate < itemDate;
    });

    // Insert at found index, or append if no later items found
    if (insertIndex >= 0) {
        container.insertBefore(newItem, existingItems[insertIndex]);
    } else {
        container.append(newItem);
    }
}