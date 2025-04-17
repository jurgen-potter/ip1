let allCollapsed = false;
let dragSrcEl = null;
let sourceListId = "";
let dragType = "";

window.addEventListener('DOMContentLoaded', (event) => {
    init();
})

// Set up all event handlers
function init() {
    const questionItems = document.querySelectorAll('.question-item');
    questionItems.forEach(question => {
        addQuestionHandlers(question);
    });

    const addQuestionButton = document.getElementById('add-question-btn');
    addQuestionButton.addEventListener('click', () => {
        addQuestion();
    });

    const toggleAllBtn = document.getElementById('toggle-all-btn');
    toggleAllBtn.addEventListener('click', () => {
        toggleExpandAll(toggleAllBtn);
    });
}

// Add all event handlers for a question and its answers
function addQuestionHandlers(question) {
    const expandButton = question.querySelector('.expand-btn');
    expandButton.addEventListener('click', function () {
        toggleArrow(this);
    });

    const removeQuestionButton = question.querySelector(".remove-question-btn");
    removeQuestionButton.addEventListener("click", function () {
        removeQuestion(question);
    })

    const addAnswerButton = question.querySelector(".add-answer-btn");
    addAnswerButton.addEventListener("click", function () {
        const questionIndex = addAnswerButton.getAttribute('question-index');
        addAnswer(questionIndex);
    });

    const questionDescription = question.querySelector(".question-description");
    questionDescription.addEventListener('input', () => {
        questionDescription.setAttribute('value', questionDescription.value);
    })

    addQuestionDnDHandlers(question);

    const answerItems = document.querySelectorAll('.answer-item');
    answerItems.forEach(answer => {
        addAnswerHandlers(answer);
    });
}

// Add all event handlers for an answer
function addAnswerHandlers(answer) {
    const removeAnswerButton = answer.querySelector(".remove-answer-btn");
    removeAnswerButton.addEventListener("click", function () {
        removeAnswer(answer);
    });

    const answerDescription = answer.querySelector(".answer-description");
    answerDescription.addEventListener('input', () => {
        answerDescription.setAttribute('value', answerDescription.value);
    })

    const criticalCheck = answer.querySelector(".critical-check");
    criticalCheck.addEventListener('change', () => {
        const isCritical = answer.querySelector('.critical-value');
        isCritical.value = criticalCheck.checked;
    })

    addAnswerDnDHandlers(answer);
}

// Change arrow direction when question is expanded/collapsed
function toggleArrow(button) {
    const arrow = button.querySelector('i');
    arrow.classList.toggle('bi-chevron-up');
    arrow.classList.toggle('bi-chevron-down');
}

// Expand/collapse all questions
function toggleExpandAll(button) {
    const collapseQuestions = document.querySelectorAll('.question-body');
    collapseQuestions.forEach(c => {
        const bsCollapse = bootstrap.Collapse.getOrCreateInstance(c);
        if (allCollapsed) {
            bsCollapse.show();
        } else {
            bsCollapse.hide();
        }
    });

    allCollapsed = !allCollapsed;
    button.innerHTML = allCollapsed
        ? 'Alles uitvouwen'
        : 'Alles invouwen';

    const expandButtons = document.querySelectorAll('.expand-btn');
    expandButtons.forEach(expandButton => {
        const arrow = expandButton.querySelector('i');
        if (arrow.classList.contains('bi-chevron-up') && allCollapsed) {
            toggleArrow(expandButton);
        } else if (arrow.classList.contains('bi-chevron-down') && !allCollapsed) {
            toggleArrow(expandButton);
        }
    });
}

// Create a new question
function addQuestion() {
    const questions = document.getElementById(`questions-list`);
    const index = questions.children.length;

    const newQuestion = generateQuestionHtml(index);
    questions.appendChild(newQuestion);
    addAnswer(index);
    addQuestionHandlers(newQuestion);
}

// Create a new answer
function addAnswer(questionIndex) {
    const answers = document.getElementById(`answers-list-${questionIndex}`);
    const index = answers.children.length;

    const newAnswer = generateAnswerHtml(questionIndex, index);
    addAnswerHandlers(newAnswer);
    answers.appendChild(newAnswer);
}

// Generate HTML for a new question given the question index
function generateQuestionHtml(questionIndex) {
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
                    <div class="d-flex align-items-center">
                        <div class="flex-grow-1 w-100">
                            <input name="Questions[${questionIndex}].Description" class="form-control mb-0 question-description"/>
                            <span class="text-danger small field-validation-valid" data-valmsg-for="Questions[${questionIndex}].Description" data-valmsg-replace="true"></span>
                        </div>
                        <button type="button" class="btn btn-danger btn-sm ms-2 remove-question-btn">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </div>
                <button class="btn btn-sm expand-btn ms-2 mt-1" type="button" data-bs-toggle="collapse" data-bs-target="#question-body-${questionIndex}" aria-expanded="true" aria-controls="question-body-@i">
                    <i class="bi bi-chevron-up"></i>
                </button>
            </div>
            <div class="collapse show question-body" id="question-body-${questionIndex}">
                <div class="card-body">
                    <label class="form-label fw-bold">Weging</label>
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
                    </div>
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
function generateAnswerHtml(questionIndex, answerIndex) {
    const newAnswer = document.createElement("li");
    newAnswer.classList.add("row", "answer-item", "mb-2", "align-items-center", "p-2");
    newAnswer.innerHTML = `
            <input name="Questions[${questionIndex}].Answers[${answerIndex}].Id" type="hidden" class="answer-id" value="0" />
            <input name="Questions[${questionIndex}].Answers[${answerIndex}].ToDelete" type="hidden" class="answer-delete" value="false" />
            <div class="col-auto fs-5 p-0">
                <span class="drag-handle">&#x2630</span>
            </div>
            <div class="col">
                <input name="Questions[${questionIndex}].Answers[${answerIndex}].Description" class="form-control answer-description" />
                <span class="text-danger small field-validation-valid" data-valmsg-for="Questions[${questionIndex}].Answers[${answerIndex}].Description" data-valmsg-replace="true"></span>
            </div>
            <div class="col-auto">
                <button type="button" class="btn btn-danger btn-sm remove-answer-btn">Verwijder</button>
            </div>
            <div class="col-auto">
                <div class="form-check">
                    <input type="hidden" value="false" class="critical-value" />
                    <input name="Questions[${questionIndex}].Answers[${answerIndex}].IsCritical" class="form-check-input critical-check" type="checkbox" />
                    <label class="form-check-label">Breekpunt</label>
                </div>
            </div>
        `;
    return newAnswer;
}

// Sets a question to be deleted and ensures it is not visible anymore
function removeQuestion(question) {
    const toDeleteInput = question.querySelector('.question-delete');
    toDeleteInput.value = "true";
    question.classList.add('d-none');
    updateQuestionIndices();
}

// Sets an answer to be deleted and ensures it is not visible anymore
function removeAnswer(answer) {
    const toDeleteInput = answer.querySelector('.answer-delete');
    toDeleteInput.value = "true";
    answer.classList.add('d-none');
    const answersList = answer.closest("ul");
    updateAnswerIndices(answersList);
}

// Add drag and drop event handlers for a question
function addQuestionDnDHandlers(question) {
    const handle = question.querySelector('.card-header .drag-handle');
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
function addAnswerDnDHandlers(answer) {
    const handle = answer.querySelector('.drag-handle');
    answer.draggable = false;

    handle.addEventListener('mousedown', (e) => {
        answer.draggable = true;
    });

    handle.addEventListener('mouseup', (e) => {
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
function handleDragStart(e) {
    dragSrcEl = this;
    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData('text/html', this.outerHTML);
    sourceListId = this.closest('ul').id;
    this.classList.add('dragged');
}

// Set behaviour when dragging another element over this element
function handleDragOver(e) {
    if (e.preventDefault) {
        e.preventDefault();
    }

    if (dragType === "answer" && this.classList.contains('answer-item')) {
        const currentListId = this.closest('ul').id;
        if (sourceListId === currentListId) {
            this.classList.add('over');
            e.dataTransfer.dropEffect = 'move';
        } else {
            e.dataTransfer.dropEffect = 'none';
        }
    } else if (dragType === "question" && this.classList.contains('question-item')) {
        this.classList.add('over');
        e.dataTransfer.dropEffect = 'move';
    } else {
        e.dataTransfer.dropEffect = 'none';
    }

    return false;
}

// Reset behaviour when dragging another element away from this element
function handleDragLeave(e) {
    this.classList.remove('over');
}

// Drop the currently dragged element and set add all data to the correct position in the page
function handleDrop(e) {
    if (e.stopPropagation) {
        e.stopPropagation();
    }

    // Check if the drop target is valid
    if (dragSrcEl !== this && dragType === "answer" && this.classList.contains('answer-item')) {
        const currentListId = this.closest('ul').id;
        if (sourceListId !== currentListId) {
            this.classList.remove('over');
            return false;
        }
        
        const dropHTML = e.dataTransfer.getData('text/html');
        dragSrcEl.parentNode.removeChild(dragSrcEl);
        this.insertAdjacentHTML('beforebegin', dropHTML);
        const droppedElem = this.previousSibling;

        addAnswerHandlers(droppedElem);
        updateAnswerIndices(this.closest("ul"));
    } else if (dragSrcEl !== this && dragType === "question" && this.classList.contains('question-item')) {
        const dropHTML = e.dataTransfer.getData('text/html');
        dragSrcEl.parentNode.removeChild(dragSrcEl);
        this.insertAdjacentHTML('beforebegin', dropHTML);
        const droppedElem = this.previousSibling;

        addQuestionHandlers(droppedElem);
        updateQuestionIndices();
    }

    this.classList.remove('over');
    return false;
}

// Reset everything related to the drag
function handleDragEnd(e) {
    this.draggable = false;
    this.classList.remove('dragged');

    if (dragType === "answer") {
        document.querySelectorAll('.answer-item.over').forEach(el => el.classList.remove('over'));
    } else if (dragType === "question") {
        document.querySelectorAll('.question-item.over').forEach(el => el.classList.remove('over'));
    }

    // Reset drag type
    dragType = "";
}

// Set all data of the corresponding question of the model behind the page to the correct values after changing the order of the answers
function updateAnswerIndices(answerList) {
    const answers = answerList.querySelectorAll('.answer-item');
    const questionIndex = answerList.id.split('-')[2];
    
    answers.forEach((answer, index) => {
        const id = answer.querySelector('.answer-id');
        id.name = `Questions[${questionIndex}].Answers[${index}].Id`;

        const toDelete = answer.querySelector('.answer-delete');
        toDelete.name = `Questions[${questionIndex}].Answers[${index}].ToDelete`;
        
        const description = answer.querySelector('.answer-description');
        description.name = `Questions[${questionIndex}].Answers[${index}].Description`;

        const criticalCheck = answer.querySelector('.critical-check');
        const isCriticalValue = answer.querySelector('.critical-value').value;
        criticalCheck.checked = isCriticalValue === "true";
    });
}

// Set all data of the model behind the page to the correct values after changing the order of the questions
function updateQuestionIndices() {
    const questions = document.querySelectorAll('.question-item');
    let questionNumber = 1;
    
    questions.forEach((question, index) => {
        const collapseBody = question.querySelector('.question-body');
        collapseBody.id = `question-body-${index}`;
        
        const expandButton = question.querySelector('.expand-btn');
        expandButton.setAttribute('data-bs-target', `#question-body-${index}`);
        expandButton.setAttribute('aria-controls', `question-body-${index}`);
        
        const questionId = question.querySelector('.question-id');
        questionId.name = `Questions[${index}].Id`;

        const toDelete = question.querySelector('.question-delete');
        toDelete.name = `Questions[${index}].ToDelete`;
        
        const questionLabel = question.querySelector('.question-number');
        if (toDelete.value.toLowerCase() === "false") {
            questionLabel.textContent = `Vraag ${questionNumber++}`;
        }

        const questionDescription = question.querySelector('.question-description');
        questionDescription.name = `Questions[${index}].Description`;
        
        const weightInput = question.querySelector('.question-weight');
        weightInput.name = `Questions[${index}].Weight`;

        const addAnswerBtn = question.querySelector('.add-answer-btn');
        addAnswerBtn.setAttribute('question-index', `${index}`);
        
        const answersList = question.querySelector('ul[id^="answers-list-"]');
        answersList.id = `answers-list-${index}`;
        
        updateAnswerIndices(answersList);
    });
}