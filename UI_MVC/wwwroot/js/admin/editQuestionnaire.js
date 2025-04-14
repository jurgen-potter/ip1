let allCollapsed = false;
let dragSrcEl = null;

window.addEventListener('DOMContentLoaded', (event) => {
    initQuestions();
    initAnswers();
})

function initQuestions() {
    // Let arrow of button point up/down depending on collapsed/expanded
    document.querySelectorAll('.toggle-btn').forEach(button => {
        button.addEventListener('click', function() {
            toggleArrow(button);
        });
    });
    
    // Let all fields be collapsed/expanded by pressing toggle all
    const toggleAllBtn = document.getElementById('toggle-all-btn');
    toggleAllBtn.addEventListener('click', () => {
        const collapseQuestions = document.querySelectorAll('.question-item');
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

    document.querySelectorAll('.answer-item').forEach(addDnDHandlers);
    
    // Have HTML synced with input
    document.querySelectorAll('input').forEach((input) => {
        input.addEventListener('input', () => {
            input.setAttribute('value', input.value);
        })
    })
}

function toggleArrow(button) {
    const arrow = button.querySelector('i');

    if (arrow.classList.contains('bi-chevron-up')) {
        arrow.classList.remove('bi-chevron-up');
        arrow.classList.add('bi-chevron-down');
    } else {
        arrow.classList.remove('bi-chevron-down');
        arrow.classList.add('bi-chevron-up');
    }
}

function addAnswer(questionIndex) {
    const answers = document.getElementById(`answers-list-${questionIndex}`);
    const index = answers.children.length;

    const newAnswer = document.createElement("li");
    newAnswer.classList.add("row", "answer-item", "mb-2", "align-items-center", "p-2");
    newAnswer.draggable = true;
    newAnswer.innerHTML = `
            <div class="col-auto fs-5 p-0">
                <span class="drag-handle">&#x2630</span>
            </div>
            <div class="col">
                <input name="Questions[${questionIndex}].Answers[${index}].Description" class="form-control d-inline-block"/>
            </div>
            <div class="col-auto">
                <button type="button" class="btn btn-danger btn-sm" onclick="removeAnswer(this)">Verwijder</button>
            </div>
        `;
    addDnDHandlers(newAnswer);

    newAnswer.querySelector('input').addEventListener('input', function () {
        this.setAttribute('value', this.value);
    });
    
    answers.appendChild(newAnswer);
}

function removeAnswer(button) {
    button.closest("li").remove();
}

function addDnDHandlers(li) {
    const handle = li.querySelector('.drag-handle');
    if (!handle) return;

    handle.addEventListener('mousedown', (e) => {
        li.draggable = true;
    });

    handle.addEventListener('mouseup', (e) => {
        li.draggable = false;
    });

    li.addEventListener('dragstart', handleDragStart, false);
    li.addEventListener('dragover', handleDragOver, false);
    li.addEventListener('dragleave', handleDragLeave, false);
    li.addEventListener('drop', handleDrop, false);
    li.addEventListener('dragend', handleDragEnd, false);
}

function handleDragStart(e) {
    dragSrcEl = this;
    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData('text/html', this.outerHTML);
    this.classList.add('dragged');
}

function handleDragOver(e) {
    if (e.preventDefault) {
        e.preventDefault();
    }

    this.classList.add('over');
    e.dataTransfer.dropEffect = 'move';

    return false;
}

function handleDragLeave(e) {
    this.classList.remove('over');
}

function handleDrop(e) {
    if (e.stopPropagation) {
        e.stopPropagation();
    }

    if (dragSrcEl !== this) {
        const dropHTML = e.dataTransfer.getData('text/html');
        dragSrcEl.parentNode.removeChild(dragSrcEl);
        this.insertAdjacentHTML('beforebegin', dropHTML);

        const droppedElem = this.previousSibling;
        addDnDHandlers(droppedElem);
    }

    this.classList.remove('over');
    updateAnswerIndices(this.closest("ul"));
    return false;
}

function handleDragEnd(e) {
    this.classList.remove('dragElem');
    document.querySelectorAll('.answer-item.over').forEach(el => el.classList.remove('over'));
}

function updateAnswerIndices(ul) {
    const liItems = ul.querySelectorAll('li');
    const questionIndex = ul.id.split('-')[2]; // answers-list-<i>
    liItems.forEach((li, idx) => {
        const input = li.querySelector('input');
        input.name = `Questions[${questionIndex}].Answers[${idx}].Description`;
    });
}