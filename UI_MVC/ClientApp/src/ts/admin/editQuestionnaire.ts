let allQuestionsCollapsed: boolean = false;
let dragSrcEl: HTMLElement | null = null;
let sourceListId: string = "";
let dragType: "question" | "answer" | "" = "";
let isDiscover: boolean;

window.addEventListener('DOMContentLoaded', () => {
    editQuestionnaireInit();

    const toggleAllBtn = document.getElementById('toggle-all-btn') as HTMLButtonElement | null;
    if (!toggleAllBtn) return;

    const firstBody = document.querySelector<HTMLDivElement>('.criteria-card-body-wrapper');
    allQuestionsCollapsed = firstBody?.classList.contains('is-collapsed') ?? false;
    toggleAllBtn.textContent = allQuestionsCollapsed ? 'Alles uitvouwen' : 'Alles invouwen';
    
    toggleAllBtn.addEventListener('click', () => {
        allQuestionsCollapsed = !allQuestionsCollapsed;
        document.querySelectorAll<HTMLDivElement>('.criteria-card-body-wrapper').forEach(body => {
            const container = body.closest('.criteria-item-card');
            const button = container?.querySelector<HTMLButtonElement>('.criteria-expand-button');
            if (!button) return;

            if (allQuestionsCollapsed) {
                body.classList.add('is-collapsed');
                button.setAttribute('aria-expanded', 'false');
                updateExpandIcon(button, false);
            } else {
                body.classList.remove('is-collapsed');
                button.setAttribute('aria-expanded', 'true');
                updateExpandIcon(button, true);
            }
        });

        toggleAllBtn.textContent = allQuestionsCollapsed ? 'Alles uitvouwen' : 'Alles invouwen';
    });

});

// Set up all event handlers
function editQuestionnaireInit(): void {
    const questionnaireIdInput = document.getElementById("questionnaire-id") as HTMLInputElement;
    isDiscover = questionnaireIdInput.value === "1"

    const questionItems = document.querySelectorAll<HTMLLIElement>('.criteria-item-card');
    questionItems.forEach((question) => {
        if (!isDiscover) {
            const detailCheck = question.querySelector(".detail-check") as HTMLInputElement;
            const isDetail = question.querySelector('.detail-value') as HTMLInputElement;
            isDetail.value = detailCheck.checked.toString();
            toggleAdviceOnQuestion(question);
        }

        addQuestionHandlers(question);
    });

    const addQuestionButton = document.getElementById('add-question-btn') as HTMLButtonElement;
    addQuestionButton.addEventListener('click', () => {
        addQuestion();
    });
}

function updateExpandIcon(button: HTMLButtonElement, isOpen: boolean): void {
    const icon = button.querySelector('i');
    if (!icon) return;
    icon.classList.remove('fa-chevron-up', 'fa-chevron-down');
    icon.classList.add(isOpen ? 'fa-chevron-up' : 'fa-chevron-down');
}

function expandButtonHandler(button: HTMLButtonElement): void {
    const container = button.closest('.criteria-item-card');
    const body = container?.querySelector<HTMLDivElement>('.criteria-card-body-wrapper');
    if (!body) return;
    const isOpen = !body.classList.contains('is-collapsed');
    updateExpandIcon(button, isOpen);
    button.setAttribute('aria-expanded', String(isOpen));
    button.addEventListener('click', () => {
        body.classList.toggle('is-collapsed');
        const newOpen = !body.classList.contains('is-collapsed');
        updateExpandIcon(button, newOpen);
        button.setAttribute('aria-expanded', String(newOpen));
    });
}

// Add all event handlers for a question and its answers
function addQuestionHandlers(question: HTMLLIElement): void {
    const expandButton = question.querySelector('.criteria-expand-button') as HTMLButtonElement;
    expandButtonHandler(expandButton);

    const removeQuestionButton = question.querySelector(".btn-remove-criteria") as HTMLButtonElement;
    removeQuestionButton.addEventListener("click", () => {
        removeQuestion(question);
    });

    const addAnswerButton = question.querySelector(".btn-add-subcriteria") as HTMLButtonElement;
    addAnswerButton.addEventListener("click", () => {
        const questionIndex = addAnswerButton.getAttribute('question-index');
        if (questionIndex !== null) {
            addAnswer(questionIndex);
        }
    });

    const questionDescription = question.querySelector(".criteria-description") as HTMLInputElement;
    questionDescription.addEventListener('input', () => {
        questionDescription.setAttribute('value', questionDescription.value);
    });

    if (!isDiscover) {
        const detailCheck = question.querySelector(".detail-check") as HTMLInputElement;
        detailCheck.addEventListener('change', () => {
            const isDetail = question.querySelector('.detail-value') as HTMLInputElement;
            isDetail.value = detailCheck.checked.toString();
            toggleAdviceOnQuestion(question);
        });
    }

    addQuestionDnDHandlers(question);

    const answerItems = question.querySelectorAll<HTMLLIElement>('.subcriteria-item-row');
    answerItems.forEach(answer => {
        if (isDiscover) {
            const criticalCheck = answer.querySelector(".critical-check") as HTMLInputElement;
            const isCritical = answer.querySelector('.critical-value') as HTMLInputElement;
            isCritical.value = criticalCheck.checked.toString();
        }

        addAnswerHandlers(answer);
    });
}

// Add all event handlers for an answer
function addAnswerHandlers(answer: HTMLLIElement): void {
    const removeAnswerButton = answer.querySelector(".btn-remove-subcriteria") as HTMLButtonElement;
    removeAnswerButton.addEventListener("click", () => {
        removeAnswer(answer);
    });

    const answerDescription = answer.querySelector(".subcriteria-description") as HTMLInputElement;
    answerDescription.addEventListener('input', () => {
        answerDescription.setAttribute('value', answerDescription.value);
    });

    const answerAdvice = answer.querySelector(".answer-advice") as HTMLInputElement;
    answerAdvice.addEventListener('input', () => {
        answerAdvice.setAttribute('value', answerAdvice.value);
    });

    if (isDiscover) {
        const criticalCheck = answer.querySelector(".critical-check") as HTMLInputElement;
        criticalCheck.addEventListener('change', () => {
            const isCritical = answer.querySelector('.critical-value') as HTMLInputElement;
            isCritical.value = criticalCheck.checked.toString();
        });
    }

    addAnswerDnDHandlers(answer);
}

function toggleAdviceOnQuestion(question: HTMLLIElement): void {
    const detailCheck = question.querySelector(".detail-check") as HTMLInputElement;
    const answerItems = question.querySelectorAll<HTMLLIElement>('.subcriteria-item-row');
    answerItems.forEach((answer) => {
        toggleAdviceOnAnswer(answer, detailCheck);
    })
}

function toggleAdviceOnAnswer(answer: HTMLLIElement, detailCheck: HTMLInputElement): void {
    const advice = answer.querySelector('.answer-advice') as HTMLInputElement;
    const label = answer.querySelector('.advice-label') as HTMLLabelElement;

    advice.style.display = detailCheck.checked ? 'none' : '';
    label.style.display = detailCheck.checked ? 'none' : '';
}

// Create a new question
function addQuestion(): void {
    const questions = document.getElementById(`questions-list`) as HTMLUListElement;
    const index = questions.children.length;
    const newQuestion = generateQuestionHtml(index);
    questions.appendChild(newQuestion);
    addAnswer(index.toString());
    addQuestionHandlers(newQuestion);
}

// Create a new answer
function addAnswer(questionIndex: string): void {
    const answers = document.getElementById(`answers-list-${questionIndex}`) as HTMLUListElement;
    const index = answers.children.length;
    const newAnswer = generateAnswerHtml(questionIndex, index);
    addAnswerHandlers(newAnswer);
    answers.appendChild(newAnswer);
    updateQuestionIndices();
}

// Generate HTML for a new question given the question index
function generateQuestionHtml(questionIndex: number): HTMLLIElement {
    const isDetailHtml = isDiscover
        ? ""
        : `
        <div class="ml-4">
            <input type="hidden" value="false" class="detail-value"/>
            <input name="Questions[${questionIndex}].IsDetail" class="form-check-input detail-check" type="checkbox"/>
            <label class="panel-form-label pr-1">Detail?</label>
        </div>`;

    const newQuestion = document.createElement("li");
    newQuestion.classList.add("criteria-item-card");
    newQuestion.innerHTML = `
            <input name="Questions[${questionIndex}].Id" type="hidden" class="question-id" value="0"/>
        <input name="Questions[${questionIndex}].ToDelete" type="hidden" class="question-delete" value="false"/>

        <div class="criteria-card-header">
            <div class="fs-5 me-3">
                <span class="drag-handle">&#x2630;</span>
            </div>

            <div class="criteria-header-content">
                <label class="criteria-list-label question-number">Vraag ${questionIndex + 1}</label>
                <div class="criteria-header-input-group">
                    <div class="flex-grow-1">
                        <input name="Questions[${questionIndex}].Description" class="auth-input-field criteria-description"/>
                        <span data-valmsg-for="Questions[${questionIndex}].Description" data-valmsg-replace="true" class="text-danger small"></span>
                    </div>
                    <button type="button" class="btn btn-danger btn-sm-equivalent btn-remove-criteria">
                        <i class="fas fa-trash"></i>
                    </button>
                    ${isDetailHtml}
                </div>
            </div>
            <button type="button" class="criteria-expand-button"
                    aria-expanded="true"
                    aria-controls="question-body-${questionIndex}">
                <i class="fas fa-chevron-up"></i>
            </button>
        </div>

        <div class="criteria-card-body-wrapper" id="question-body-${questionIndex}">
            <div class="criteria-card-body card-body">
                <label class="subcriteria-list-label">Antwoorden</label>
                <ul id="answers-list-${questionIndex}" class="subcriteria-list">
                    <!-- Answer items will go here -->
                </ul>
                <div class="subcriteria-footer">
                    <button type="button" class="btn btn-secondary btn-sm-equivalent btn-add-subcriteria" question-index="${questionIndex}">
                        Voeg antwoordoptie toe
                    </button>
                </div>
            </div>
        </div>
        `;
    return newQuestion;
}

// Generate HTML for a new answer given the question and answer index
function generateAnswerHtml(questionIndex: string | number, answerIndex: number): HTMLLIElement {
    const isCriticalHtml = isDiscover
        ? `
        <div class="col-auto form-check">
            <input type="hidden" value="false" class="critical-value"/>
            <input name="Questions[${questionIndex}].Answers[${answerIndex}].IsCritical" class="form-check-input critical-check" type="checkbox"/>
            <label class="form-check-label">Breekpunt</label>
        </div>`
        : "";

    const newAnswer = document.createElement("li");
    newAnswer.classList.add("subcriteria-item-row");
    newAnswer.innerHTML = `
        <input name="Questions[${questionIndex}].Answers[${answerIndex}].Id" type="hidden" class="answer-id" value="0"/>
        <input name="Questions[${questionIndex}].Answers[${answerIndex}].ToDelete" type="hidden" class="answer-delete" value="false"/>

        <div class="col-auto fs-5 p-0">
            <span class="drag-handle">&#x2630;</span>
        </div>

        <div class="col">
            <div class="subcriteria-input-name-wrapper">
                <input name="Questions[${questionIndex}].Answers[${answerIndex}].Description"
                       id="Description-${questionIndex}-${answerIndex}"
                       class="auth-input-field subcriteria-description" placeholder="Beschrijving"/>
                <label for="Description-${questionIndex}-${answerIndex}">Beschrijving</label>
            </div>
            <span data-valmsg-for="Questions[${questionIndex}].Answers[${answerIndex}].Description" data-valmsg-replace="true"
                  class="auth-validation-message"></span>

            <div class="subcriteria-input-name-wrapper">
                <input name="Questions[${questionIndex}].Answers[${answerIndex}].Advice"
                       id="Advice-${questionIndex}-${answerIndex}"
                       class="auth-input-field answer-advice" placeholder="Aanbeveling"/>
                <label for="Advice-${questionIndex}-${answerIndex}" class="advice-label">Aanbeveling</label>
            </div>
            <span data-valmsg-for="Questions[${questionIndex}].Answers[${answerIndex}].Advice" data-valmsg-replace="true"
                  class="auth-validation-message"></span>
        </div>

        <div class="subcriteria-remove-button-wrapper">
            <button type="button" class="btn btn-danger btn-sm-equivalent btn-remove-subcriteria">
                Verwijder
            </button>
        </div>
        ${isCriticalHtml}
        `;
    return newAnswer;
}

// Sets a question to be deleted and ensures it is not visible anymore
function removeQuestion(question: HTMLLIElement): void {
    const toDeleteInput = question.querySelector('.question-delete') as HTMLInputElement;
    toDeleteInput.value = "true";
    question.classList.add('hidden');
    updateQuestionIndices();
}

// Sets an answer to be deleted and ensures it is not visible anymore
function removeAnswer(answer: HTMLLIElement): void {
    const toDeleteInput = answer.querySelector('.answer-delete') as HTMLInputElement;
    toDeleteInput.value = "true";
    answer.classList.add('hidden');
    const answersList = answer.closest("ul")!;
    updateAnswerIndices(answersList);
}

// Add drag and drop event handlers for a question
function addQuestionDnDHandlers(question: HTMLLIElement): void {
    const handle = question.querySelector('.criteria-card-header .drag-handle') as HTMLDivElement;
    question.draggable = false;

    handle.addEventListener('mousedown', (e) => {
        e.stopPropagation();
        question.draggable = true;
    });

    handle.addEventListener('mouseup', (e) => {
        e.stopPropagation();
        question.draggable = false;
    });

    question.addEventListener('dragstart', function (e) {
        dragType = "question";
        handleDragStart.call(this, e);
    }, false);
    question.addEventListener('dragover', handleDragOver, false);
    question.addEventListener('dragleave', handleDragLeave, false);
    question.addEventListener('drop', handleDrop, false);
    question.addEventListener('dragend', handleDragEnd, false);
}

// Add drag and drop event handlers for an answer
function addAnswerDnDHandlers(answer: HTMLLIElement): void {
    const handle = answer.querySelector('.drag-handle') as HTMLDivElement;
    answer.draggable = false;

    handle.addEventListener('mousedown', () => {
        answer.draggable = true;
    });

    handle.addEventListener('mouseup', () => {
        answer.draggable = false;
    });

    answer.addEventListener('dragstart', function (e) {
        dragType = "answer";
        e.stopPropagation();
        handleDragStart.call(this, e);
    }, false);
    answer.addEventListener('dragover', function (e) {
        e.stopPropagation();
        handleDragOver.call(this, e);
    }, false);
    answer.addEventListener('dragleave', function (e) {
        e.stopPropagation();
        handleDragLeave.call(this, e);
    }, false);
    answer.addEventListener('drop', function (e) {
        e.stopPropagation();
        handleDrop.call(this, e);
    }, false);
    answer.addEventListener('dragend', function (e) {
        e.stopPropagation();
        handleDragEnd.call(this, e);
    }, false);
}

// Set currently dragged element
function handleDragStart(this: HTMLElement, e: DragEvent): void {
    dragSrcEl = this;
    e.dataTransfer!.effectAllowed = 'move';
    e.dataTransfer!.setData('text/html', this.outerHTML);
    sourceListId = this.closest('ul')!.id;
    this.classList.add('dragged');
}

// Set behaviour when dragging another element over this element
function handleDragOver(this: HTMLElement, e: DragEvent): boolean {
    if (e.preventDefault) e.preventDefault();

    const isAnswer = dragType === "answer" && this.classList.contains('subcriteria-item-row');
    const isQuestion = dragType === "question" && this.classList.contains('criteria-item-card');

    if (isAnswer) {
        const currentListId = this.closest('ul')!.id;
        if (sourceListId === currentListId) {
            this.classList.add('over');
            e.dataTransfer!.dropEffect = 'move';
        } else {
            e.dataTransfer!.dropEffect = 'none';
        }
    } else if (isQuestion) {
        this.classList.add('over');
        e.dataTransfer!.dropEffect = 'move';
    } else {
        e.dataTransfer!.dropEffect = 'none';
    }

    return false;
}

// Reset behaviour when dragging another element away from this element
function handleDragLeave(this: HTMLElement, e: DragEvent): void {
    this.classList.remove('over');
    e.x;
}

// Drop the currently dragged element and set add all data to the correct position in the page
function handleDrop(this: HTMLElement, e: DragEvent): boolean {
    if (e.stopPropagation) e.stopPropagation();

    // Check if the drop target is valid
    if (dragSrcEl !== this) {
        const dropHTML = e.dataTransfer!.getData('text/html');
        dragSrcEl!.parentNode!.removeChild(dragSrcEl!);
        this.insertAdjacentHTML('beforebegin', dropHTML);
        const droppedElem = this.previousSibling as HTMLLIElement;

        if (dragType === "answer") {
            addAnswerHandlers(droppedElem);
            updateAnswerIndices(this.closest("ul")!);
        } else if (dragType === "question") {
            addQuestionHandlers(droppedElem);
            updateQuestionIndices();
        }
    }

    this.classList.remove('over');
    return false;
}

// Reset everything related to the drag
function handleDragEnd(this: HTMLElement, e: DragEvent): void {
    this.draggable = false;
    this.classList.remove('dragged');
    e.x;
    
    const overClass = dragType === "answer" ? '.subcriteria-item-row.over' : '.criteria-item-card.over';
    document.querySelectorAll<HTMLElement>(overClass).forEach(el => el.classList.remove('over'));
    dragType = "";
}

// Set all data of the corresponding question of the model behind the page to the correct values after changing the order of the answers
function updateAnswerIndices(answerList: HTMLUListElement): void {
    const answers = answerList.querySelectorAll<HTMLLIElement>('.subcriteria-item-row');
    const questionIndex = answerList.id.split('-')[2];

    answers.forEach((answer, index) => {
        const id = answer.querySelector('.answer-id') as HTMLInputElement;
        id.name = `Questions[${questionIndex}].Answers[${index}].Id`;

        const toDelete = answer.querySelector('.answer-delete') as HTMLInputElement;
        toDelete.name = `Questions[${questionIndex}].Answers[${index}].ToDelete`;

        const description = answer.querySelector('.subcriteria-description') as HTMLInputElement;
        description.name = `Questions[${questionIndex}].Answers[${index}].Description`;

        const advice = answer.querySelector('.answer-advice') as HTMLInputElement;
        advice.name = `Questions[${questionIndex}].Answers[${index}].Advice`;

        if (isDiscover) {
            const criticalCheck = answer.querySelector('.critical-check') as HTMLInputElement;
            const isCriticalValue = (answer.querySelector('.critical-value') as HTMLInputElement).value;
            criticalCheck.checked = isCriticalValue.toLowerCase() === "true";
        }
    });
}

// Set all data of the model behind the page to the correct values after changing the order of the questions
function updateQuestionIndices(): void {
    const questions = document.querySelectorAll<HTMLLIElement>('.criteria-item-card');
    let questionNumber = 1;

    questions.forEach((question, index) => {
        const collapseBody = question.querySelector('.criteria-card-body-wrapper') as HTMLDivElement;
        collapseBody.id = `question-body-${index}`;

        const expandButton = question.querySelector('.criteria-expand-button') as HTMLButtonElement;
        expandButton.setAttribute('aria-controls', `question-body-${index}`);

        const questionId = question.querySelector('.question-id') as HTMLInputElement;
        questionId.name = `Questions[${index}].Id`;

        const toDelete = question.querySelector('.question-delete') as HTMLInputElement;
        toDelete.name = `Questions[${index}].ToDelete`;

        const questionLabel = question.querySelector('.question-number') as HTMLLabelElement;
        if (toDelete.value.toLowerCase() === "false") {
            questionLabel.textContent = `Vraag ${questionNumber++}`;
        }

        const questionDescription = question.querySelector('.criteria-description') as HTMLInputElement;
        questionDescription.name = `Questions[${index}].Description`;

        if (!isDiscover) {
            const detailCheck = question.querySelector('.detail-check') as HTMLInputElement;
            const isDetailValue = (question.querySelector('.detail-value') as HTMLInputElement).value;
            detailCheck.checked = isDetailValue.toLowerCase() === "true";
            toggleAdviceOnQuestion(question);
        }

        /*const weightInput = question.querySelector('.question-weight') as HTMLInputElement;
        weightInput.name = `Questions[${index}].Weight`;*/

        const addAnswerBtn = question.querySelector('.btn-add-subcriteria') as HTMLButtonElement;
        addAnswerBtn.setAttribute('question-index', `${index}`);

        const answersList = question.querySelector('ul[id^="answers-list-"]') as HTMLUListElement;
        answersList.id = `answers-list-${index}`;

        updateAnswerIndices(answersList);
    });
}