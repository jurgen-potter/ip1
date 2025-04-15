let allCollapsed = false;
let dragSrcEl = null;
let sourceListId = "";
let dragType = "";

window.addEventListener('DOMContentLoaded', (event) => {
    initQuestions();
    initAnswers();
})

function initQuestions() {
    // Let arrow of button point up/down depending on collapsed/expanded
    document.querySelectorAll('.toggle-btn').forEach(button => {
        button.addEventListener('click', function () {
            toggleArrow(button);
        });
    });

    document.querySelector('.add-question-btn').addEventListener('click', () => {
        addQuestion();
    });

    document.querySelectorAll('.remove-question-btn').forEach(button => {
        button.addEventListener('click', function() {
            removeQuestion(this);
        });
    });

    // Let all fields be collapsed/expanded by pressing toggle all
    const toggleAllBtn = document.getElementById('toggle-all-btn');
    toggleAllBtn.addEventListener('click', () => {
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
        toggleAllBtn.innerHTML = allCollapsed
            ? 'Alles uitvouwen'
            : 'Alles invouwen';

        const toggleButtons = document.querySelectorAll('.toggle-btn');
        toggleButtons.forEach(button => {
            const icon = button.querySelector('i');

            if (icon && icon.classList.contains('bi-chevron-up') && allCollapsed) {
                toggleArrow(button);
            } else if (icon && icon.classList.contains('bi-chevron-down') && !allCollapsed) {
                toggleArrow(button);
            }
        });
    });

    document.querySelectorAll('.question-item').forEach(addQuestionDnDHandlers);
}

function initAnswers() {
    document.querySelectorAll('.add-answer-btn').forEach(button => {
        button.addEventListener('click', () => {
            const questionIndex = button.getAttribute('question-index');
            addAnswer(questionIndex);
        });
    });

    document.querySelectorAll('.remove-answer-btn').forEach(button => {
        button.addEventListener('click', () => {
            removeAnswer(button);
        })
    })

    document.querySelectorAll('.answer-item').forEach(addAnswerDnDHandlers);

    // Have HTML synced with input
    document.querySelectorAll('input').forEach((input) => {
        input.addEventListener('input', () => {
            input.setAttribute('value', input.value);
        })
    })
}

function toggleArrow(button) {
    const arrow = button.querySelector('i');
    arrow.classList.toggle('bi-chevron-up');
    arrow.classList.toggle('bi-chevron-down');
}

function addQuestion() {
    const questions = document.getElementById(`questions-list`);
    const index = questions.children.length;

    const newQuestion = document.createElement("li");
    newQuestion.classList.add("card", "mb-3", "question-item");
    newQuestion.innerHTML = `
            <input name="Questions[${index}].Id" type="hidden" class="question-id" value="0" />
            <div class="card-header d-flex align-items-start">
                <div class="fs-5 me-3">
                    <span class="drag-handle">&#x2630;</span>
                </div>
                <div class="flex-grow-1">
                    <label class="form-label fw-bold">Vraag ${index + 1}</label>
                    <div class="d-flex align-items-center">
                        <input name="Questions[${index}].Description" class="form-control mb-0 question-description"/>
                        <button type="button" class="btn btn-danger btn-sm ms-2 remove-question-btn">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </div>
                <button class="btn btn-sm toggle-btn ms-2 mt-1" type="button" data-bs-toggle="collapse" data-bs-target="#question-body-${index}" aria-expanded="true" aria-controls="question-body-@i">
                    <i class="bi bi-chevron-up"></i>
                </button>
            </div>
            <div class="collapse show question-body" id="question-body-${index}">
                <div class="card-body">
                    <label class="form-label fw-bold">Weging</label>
                    <input name="Questions[${index}].Weight" type="range" min="1" max="10" class="form-range mb-2" value="1"/>
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
                    <ul id="answers-list-${index}">
                        <li class="row answer-item mb-2 align-items-center p-2">
                            <input name="Questions[${index}].Answers[0].Id" type="hidden" class="answer-id" value="0" />
                            <div class="col-auto fs-5 p-0">
                                <span class="drag-handle">&#x2630;</span>
                            </div>
                            <div class="col">
                                <input name="Questions[${index}].Answers[0].Description" class="form-control answer-description" />
                            </div>
                            <div class="col-auto">
                                <button type="button" class="btn btn-danger btn-sm remove-answer-btn">Verwijder</button>
                            </div>
                        </li>
                    </ul>
                    <button type="button" class="btn btn-sm btn-secondary add-answer-btn" question-index=${index}>Voeg antwoordoptie toe</button>
                </div>
            </div>
        `;
    addQuestionDnDHandlers(newQuestion);
    addQuestionHandlers(newQuestion);
    questions.appendChild(newQuestion);
}

function removeQuestion(button) {
    const questionCard = button.closest('.question-item');
    questionCard.remove();

    updateQuestionIndices();
}

function addQuestionHandlers(newQuestion) {
    const addAnswerButton = newQuestion.querySelector(".add-answer-btn");
    addAnswerButton.addEventListener("click", function () {
        const questionIndex = addAnswerButton.getAttribute('question-index');
        addAnswer(questionIndex);
    });
    
    const removeAnswerButton = newQuestion.querySelector(".remove-answer-btn");
    removeAnswerButton.addEventListener("click", function () {
        removeAnswer(removeAnswerButton);
    });
    
    const removeQuestionButton = newQuestion.querySelector(".remove-question-btn");
    removeQuestionButton.addEventListener("click", function () {
        removeQuestion(removeQuestionButton);
    })

    newQuestion.querySelectorAll('.answer-item').forEach(addAnswerDnDHandlers);

    newQuestion.querySelectorAll('input').forEach((input) => {
        input.addEventListener('input', () => {
            input.setAttribute('value', input.value);
        })
    })
}

function addAnswer(questionIndex) {
    const answers = document.getElementById(`answers-list-${questionIndex}`);
    const index = answers.children.length;

    const newAnswer = document.createElement("li");
    newAnswer.classList.add("row", "answer-item", "mb-2", "align-items-center", "p-2");
    newAnswer.innerHTML = `
            <input name="@Model.Questions[@i].Answers[@j].Id" type="hidden" class="answer-id" value="0" />
            <div class="col-auto fs-5 p-0">
                <span class="drag-handle">&#x2630</span>
            </div>
            <div class="col">
                <input name="Questions[${questionIndex}].Answers[${index}].Description" class="form-control answer-description" />
            </div>
            <div class="col-auto">
                <button type="button" class="btn btn-danger btn-sm remove-answer-btn" onclick="removeAnswer(this)">Verwijder</button>
            </div>
        `;
    addAnswerDnDHandlers(newAnswer);

    newAnswer.querySelectorAll('input.answer-description').forEach((input) => {
        input.addEventListener('input', () => {
            input.setAttribute('value', input.value);
        })
    })

    const removeAnswerButton = newAnswer.querySelector(".remove-answer-btn");
    removeAnswerButton.addEventListener("click", function () {
        removeAnswer(removeAnswerButton);
    });

    answers.appendChild(newAnswer);
}

function removeAnswer(button) {
    button.closest("li").remove();
}

function addAnswerDnDHandlers(answer) {
    const handle = answer.querySelector('.drag-handle');
    if (!handle) return;

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

function addQuestionDnDHandlers(question) {
    const handle = question.querySelector('.card-header .drag-handle');
    if (!handle) return;

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

function handleDragStart(e) {
    dragSrcEl = this;
    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData('text/html', this.outerHTML);
    sourceListId = this.closest('ul').id;
    this.classList.add('dragged');
}

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

function handleDragLeave(e) {
    this.classList.remove('over');
}

function handleDrop(e) {
    if (e.stopPropagation) {
        e.stopPropagation();
    }

    // Check if the drop target is valid based on drag type
    if (dragSrcEl !== this && dragType === "answer" && this.classList.contains('answer-item')) {
        const currentListId = this.closest('ul').id;
        if (sourceListId !== currentListId) {
            this.classList.remove('over');
            return false;
        }

        // Handle answer dropping
        const dropHTML = e.dataTransfer.getData('text/html');
        dragSrcEl.parentNode.removeChild(dragSrcEl);
        this.insertAdjacentHTML('beforebegin', dropHTML);

        const droppedElem = this.previousSibling;
        addAnswerDnDHandlers(droppedElem);

        // Ensure input listener is reattached
        droppedElem.querySelector('input')?.addEventListener('input', function () {
            this.setAttribute('value', this.value);
        });

        updateAnswerIndices(this.closest("ul"));
    } else if (dragSrcEl !== this && dragType === "question" && this.classList.contains('question-item')) {
        // Handle question dropping
        const dropHTML = e.dataTransfer.getData('text/html');
        dragSrcEl.parentNode.removeChild(dragSrcEl);
        this.insertAdjacentHTML('beforebegin', dropHTML);

        const droppedElem = this.previousSibling;
        addQuestionDnDHandlers(droppedElem);

        // Reattach all event handlers within the question
        setupDroppedQuestion(droppedElem);

        // Update all question indices
        updateQuestionIndices();
    }

    this.classList.remove('over');
    return false;
}

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

function updateAnswerIndices(ul) {
    const liItems = ul.querySelectorAll('li');
    const questionIndex = ul.id.split('-')[2];
    liItems.forEach((li, idx) => {
        const description = li.querySelector('input.answer-description');
        description.name = `Questions[${questionIndex}].Answers[${idx}].Description`;

        const id = li.querySelector('input.answer-id');
        id.name = `Questions[${questionIndex}].Answers[${idx}].Id`;
    });
}

function updateQuestionIndices() {
    const questions = document.querySelectorAll('.question-item');

    questions.forEach((question, qIndex) => {
        // Update question index in label
        const questionLabel = question.querySelector('.flex-grow-1 label');
        if (questionLabel) {
            questionLabel.textContent = `Vraag ${qIndex + 1}`;
        }
        
        // Update question id
        const questionId = question.querySelector('input.question-id');
        if (questionId) {
            questionId.name = `Questions[${qIndex}].Id`;
        }

        // Update question description
        const questionDescription = question.querySelector('input.question-description');
        if (questionDescription) {
            questionDescription.name = `Questions[${qIndex}].Description`;
        }

        // Update weight input
        const weightInput = question.querySelector('input[type="range"]');
        if (weightInput) {
            weightInput.name = `Questions[${qIndex}].Weight`;
        }

        // Update the question body ID and data-bs-target reference
        const collapseBody = question.querySelector('.question-body');
        if (collapseBody) {
            collapseBody.id = `question-body-${qIndex}`;
        }

        const toggleBtn = question.querySelector('.toggle-btn');
        if (toggleBtn) {
            toggleBtn.setAttribute('data-bs-target', `#question-body-${qIndex}`);
            toggleBtn.setAttribute('aria-controls', `question-body-${qIndex}`);
        }

        // Update answers list ID
        const answersList = question.querySelector('ul[id^="answers-list-"]');
        if (answersList) {
            answersList.id = `answers-list-${qIndex}`;

            // Update add answer button question-index
            const addAnswerBtn = question.querySelector('.add-answer-btn');
            if (addAnswerBtn) {
                addAnswerBtn.setAttribute('question-index', qIndex);
            }

            // Update all answer indices
            updateAnswerIndices(answersList);
        }
    });
}

// Setup all event handlers for a dropped question
function setupDroppedQuestion(question) {
    // Setup toggle button
    const toggleBtn = question.querySelector('.toggle-btn');
    if (toggleBtn) {
        toggleBtn.addEventListener('click', function () {
            toggleArrow(this);
        });
    }

    // Setup add answer button
    const addAnswerBtn = question.querySelector('.add-answer-btn');
    if (addAnswerBtn) {
        addAnswerBtn.addEventListener('click', function () {
            const questionIndex = this.getAttribute('question-index');
            addAnswer(questionIndex);
        });
    }

    // Setup remove answer buttons
    question.querySelectorAll('.remove-answer-btn').forEach(button => {
        button.addEventListener('click', function () {
            removeAnswer(this);
        });
    });

    // Setup input listeners
    question.querySelectorAll('input').forEach(input => {
        input.addEventListener('input', function () {
            this.setAttribute('value', this.value);
        });
    });

    // Setup answer drag and drop
    question.querySelectorAll('.answer-item').forEach(addAnswerDnDHandlers);
}
