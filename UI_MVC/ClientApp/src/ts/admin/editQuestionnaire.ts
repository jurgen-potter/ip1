let allCollapsed: boolean = false;
let dragSrcEl: HTMLElement | null = null;
let sourceListId: string = "";
let dragType: "question" | "answer" | "" = "";
let isDiscover: boolean;

window.addEventListener('DOMContentLoaded', () => {
    editQuestionnaireInit();
});

// Set up all event handlers
function editQuestionnaireInit(): void {
    const questionnaireIdInput = document.getElementById("questionnaire-id") as HTMLInputElement;
    isDiscover = questionnaireIdInput.value === "1"

    const questionItems = document.querySelectorAll<HTMLLIElement>('.question-item');
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

    const toggleAllBtn = document.getElementById('toggle-all-btn') as HTMLButtonElement;
    toggleAllBtn.addEventListener('click', () => {
        toggleExpandAll(toggleAllBtn);
    });
}

// Add all event handlers for a question and its answers
function addQuestionHandlers(question: HTMLLIElement): void {
    const expandButton = question.querySelector('.expand-btn') as HTMLButtonElement;
    expandButton.addEventListener('click', function () {
        toggleArrow(this);
    });

    const removeQuestionButton = question.querySelector(".remove-question-btn") as HTMLButtonElement;
    removeQuestionButton.addEventListener("click", () => {
        removeQuestion(question);
    });

    const addAnswerButton = question.querySelector(".add-answer-btn") as HTMLButtonElement;
    addAnswerButton.addEventListener("click", () => {
        const questionIndex = addAnswerButton.getAttribute('question-index');
        if (questionIndex !== null) {
            addAnswer(questionIndex);
        }
    });

    const questionDescription = question.querySelector(".question-description") as HTMLInputElement;
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

    const answerItems = question.querySelectorAll<HTMLLIElement>('.answer-item');
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
    const removeAnswerButton = answer.querySelector(".remove-answer-btn") as HTMLButtonElement;
    removeAnswerButton.addEventListener("click", () => {
        removeAnswer(answer);
    });

    const answerDescription = answer.querySelector(".answer-description") as HTMLInputElement;
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
    const answerItems = question.querySelectorAll<HTMLLIElement>('.answer-item');
    answerItems.forEach((answer) => {
        toggleAdviceOnAnswer(answer, detailCheck);
    })
}

function toggleAdviceOnAnswer(answer: HTMLLIElement, detailCheck: HTMLInputElement): void {
    const advice = answer.querySelector('.answer-advice') as HTMLInputElement;
    const label = answer.querySelector('.advice-label') as HTMLLabelElement;
    if (detailCheck.checked) {
        advice.classList.add('d-none');
        label.classList.add('d-none');
    } else {
        advice.classList.remove('d-none');
        label.classList.remove('d-none');
    }
}

// Change arrow direction when question is expanded/collapsed
function toggleArrow(button: HTMLButtonElement): void {
    const arrow = button.querySelector('i')!;
    arrow.classList.toggle('bi-chevron-up');
    arrow.classList.toggle('bi-chevron-down');
}

// Expand/collapse all questions
function toggleExpandAll(button: HTMLButtonElement): void {
    const collapseQuestions = document.querySelectorAll<HTMLDivElement>('.question-body');
    collapseQuestions.forEach(c => {
        const bsCollapse = (window as any).bootstrap.Collapse.getOrCreateInstance(c);
        if (allCollapsed) {
            bsCollapse.show();
        } else {
            bsCollapse.hide();
        }
    });

    allCollapsed = !allCollapsed;
    button.innerHTML = allCollapsed ? 'Alles uitvouwen' : 'Alles invouwen';

    const expandButtons = document.querySelectorAll<HTMLButtonElement>('.expand-btn');
    expandButtons.forEach(expandButton => {
        const arrow = expandButton.querySelector('i')!;
        if (arrow.classList.contains('bi-chevron-up') && allCollapsed) {
            toggleArrow(expandButton);
        } else if (arrow.classList.contains('bi-chevron-down') && !allCollapsed) {
            toggleArrow(expandButton);
        }
    });
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
        <div class="col-auto">
            <div class="form-check">
                <input type="hidden" value="false" class="detail-value" />
                <input name="Questions[${questionIndex}].IsDetail" class="form-check-input detail-check" type="checkbox" />
                <label class="form-check-label">Panel Detail</label>
            </div>
        </div>`;

    const newQuestion = document.createElement("li");
    newQuestion.classList.add("card", "mb-3", "question-item");
    newQuestion.innerHTML = `
            <input name="Questions[${questionIndex}].Id" type="hidden" class="question-id" value="0" />
            <input name="Questions[${questionIndex}].ToDelete" type="hidden" class="question-delete" value="false" />
            <div class="card-header d-flex align-items-start">
                <div class="fs-5 me-3">
                    <span class="drag-handle">&#x2630;</span>
                </div>
                <div class="flex-grow-1">
                    <label class="form-label fw-bold question-number">Vraag ${questionIndex + 1}</label>
                    <div class="d-flex align-items-start gap-2">
                        <div class="flex-grow-1 w-100">
                            <div class="flex-grow-1">
                                <input name="Questions[${questionIndex}].Description" class="form-control mb-0 question-description"/>
                                <span class="text-danger small field-validation-valid" data-valmsg-for="Questions[${questionIndex}].Description" data-valmsg-replace="true"></span>
                            </div>
                        </div>
                        <button type="button" class="btn btn-danger btn-sm ms-2 remove-question-btn">
                            <i class="bi bi-trash"></i>
                        </button>
                        ${isDetailHtml}
                    </div>
                </div>
                <button class="btn btn-sm expand-btn ms-2 mt-1" type="button" data-bs-toggle="collapse" data-bs-target="#question-body-${questionIndex}" aria-expanded="true" aria-controls="question-body-@i">
                    <i class="bi bi-chevron-up"></i>
                </button>
            </div>
            <div class="collapse show question-body" id="question-body-${questionIndex}">
                <div class="card-body">
<!--                    <label class="form-label fw-bold">Weging</label>
                    <input name="Questions[${questionIndex}].Weight" type="range" min="1" max="10" class="form-range mb-2 question-weight" value="1"/>
                    <div class="d-flex justify-content-between px-1 text-muted small">
                        <span>1</span>
                        <span>2</span>
                        <span>3</span>
                        <span>4</span>
                        <span>5</span>
                        <span>6</span>
                        <span>7</span>
                        <span>8</span>
                        <span>9</span>
                        <span>10</span>
                    </div>-->
                    <label class="form-label fw-bold">Antwoorden</label>
                    <ul id="answers-list-${questionIndex}">
                    </ul>
                    <button type="button" class="btn btn-sm btn-secondary add-answer-btn" question-index=${questionIndex}>Voeg antwoordoptie toe</button>
                </div>
            </div>
        `;
    return newQuestion;
}

// Generate HTML for a new answer given the question and answer index
function generateAnswerHtml(questionIndex: string | number, answerIndex: number): HTMLLIElement {
    const isCriticalHtml = isDiscover
        ? `
        <div class="col-auto">
            <div class="form-check">
                <input type="hidden" value="false" class="critical-value" />
                <input name="Questions[${questionIndex}].Answers[${answerIndex}].IsCritical" class="form-check-input critical-check" type="checkbox" />
                <label class="form-check-label">Breekpunt</label>
            </div>
        </div>`
        : "";

    const newAnswer = document.createElement("li");
    newAnswer.classList.add("row", "answer-item", "mb-2", "align-items-center", "p-2");
    newAnswer.innerHTML = `
            <input name="Questions[${questionIndex}].Answers[${answerIndex}].Id" type="hidden" class="answer-id" value="0" />
            <input name="Questions[${questionIndex}].Answers[${answerIndex}].ToDelete" type="hidden" class="answer-delete" value="false" />
            <div class="col-auto fs-5 p-0">
                <span class="drag-handle">&#x2630</span>
            </div>
            <div class="col">
                <div class="form-floating mb-2">
                    <input name="Questions[${questionIndex}].Answers[${answerIndex}].Description" id="Description-@i-@j" class="form-control answer-description" />
                    <label for="Description-${questionIndex}-${answerIndex}">Beschrijving</label>
                </div>
                <span class="text-danger small field-validation-valid" data-valmsg-for="Questions[${questionIndex}].Answers[${answerIndex}].Description" data-valmsg-replace="true"></span>
                <div class="form-floating mb-2">
                    <input name="Questions[${questionIndex}].Answers[${answerIndex}].Advice" id="Advice-@i-@j" class="form-control answer-advice" />
                    <label for="Advice-${questionIndex}-${answerIndex}" class="advice-label">Aanbeveling</label>
                </div>
                <span class="text-danger small field-validation-valid" data-valmsg-for="Questions[${questionIndex}].Answers[${answerIndex}].Advice" data-valmsg-replace="true"></span>
            </div>
            <div class="col-auto">
                <button type="button" class="btn btn-danger btn-sm remove-answer-btn">Verwijder</button>
            </div>
            ${isCriticalHtml}
        `;
    return newAnswer;
}

// Sets a question to be deleted and ensures it is not visible anymore
function removeQuestion(question: HTMLLIElement): void {
    const toDeleteInput = question.querySelector('.question-delete') as HTMLInputElement;
    toDeleteInput.value = "true";
    question.classList.add('d-none');
    updateQuestionIndices();
}

// Sets an answer to be deleted and ensures it is not visible anymore
function removeAnswer(answer: HTMLLIElement): void {
    const toDeleteInput = answer.querySelector('.answer-delete') as HTMLInputElement;
    toDeleteInput.value = "true";
    answer.classList.add('d-none');
    const answersList = answer.closest("ul")!;
    updateAnswerIndices(answersList);
}

// Add drag and drop event handlers for a question
function addQuestionDnDHandlers(question: HTMLLIElement): void {
    const handle = question.querySelector('.card-header .drag-handle') as HTMLDivElement;
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

    const isAnswer = dragType === "answer" && this.classList.contains('answer-item');
    const isQuestion = dragType === "question" && this.classList.contains('question-item');

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

    const overClass = dragType === "answer" ? '.answer-item.over' : '.question-item.over';
    document.querySelectorAll<HTMLElement>(overClass).forEach(el => el.classList.remove('over'));
    dragType = "";
}

// Set all data of the corresponding question of the model behind the page to the correct values after changing the order of the answers
function updateAnswerIndices(answerList: HTMLUListElement): void {
    const answers = answerList.querySelectorAll<HTMLLIElement>('.answer-item');
    const questionIndex = answerList.id.split('-')[2];

    answers.forEach((answer, index) => {
        const id = answer.querySelector('.answer-id') as HTMLInputElement;
        id.name = `Questions[${questionIndex}].Answers[${index}].Id`;

        const toDelete = answer.querySelector('.answer-delete') as HTMLInputElement;
        toDelete.name = `Questions[${questionIndex}].Answers[${index}].ToDelete`;

        const description = answer.querySelector('.answer-description') as HTMLInputElement;
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
    const questions = document.querySelectorAll<HTMLLIElement>('.question-item');
    let questionNumber = 1;

    questions.forEach((question, index) => {
        const collapseBody = question.querySelector('.question-body') as HTMLDivElement;
        collapseBody.id = `question-body-${index}`;

        const expandButton = question.querySelector('.expand-btn') as HTMLButtonElement;
        expandButton.setAttribute('data-bs-target', `#question-body-${index}`);
        expandButton.setAttribute('aria-controls', `question-body-${index}`);

        const questionId = question.querySelector('.question-id') as HTMLInputElement;
        questionId.name = `Questions[${index}].Id`;

        const toDelete = question.querySelector('.question-delete') as HTMLInputElement;
        toDelete.name = `Questions[${index}].ToDelete`;

        const questionLabel = question.querySelector('.question-number') as HTMLLabelElement;
        if (toDelete.value.toLowerCase() === "false") {
            questionLabel.textContent = `Vraag ${questionNumber++}`;
        }

        const questionDescription = question.querySelector('.question-description') as HTMLInputElement;
        questionDescription.name = `Questions[${index}].Description`;

        if (!isDiscover) {
            const detailCheck = question.querySelector('.detail-check') as HTMLInputElement;
            const isDetailValue = (question.querySelector('.detail-value') as HTMLInputElement).value;
            detailCheck.checked = isDetailValue.toLowerCase() === "true";
            toggleAdviceOnQuestion(question);
        }

        /*const weightInput = question.querySelector('.question-weight') as HTMLInputElement;
        weightInput.name = `Questions[${index}].Weight`;*/

        const addAnswerBtn = question.querySelector('.add-answer-btn') as HTMLButtonElement;
        addAnswerBtn.setAttribute('question-index', `${index}`);

        const answersList = question.querySelector('ul[id^="answers-list-"]') as HTMLUListElement;
        answersList.id = `answers-list-${index}`;

        updateAnswerIndices(answersList);
    });
}