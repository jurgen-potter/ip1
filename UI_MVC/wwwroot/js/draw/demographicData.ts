let allCriteriaCollapsed: boolean = false;
const MAX_CRITERIA = 5;
const MAX_SUBCRITERIA = 5;
window.addEventListener('DOMContentLoaded', () => {
    editCriteriaInit();
    initSubcriteriaSums();
});

// ----------------------
// Initialization
// ----------------------
function editCriteriaInit(): void {
    // Bestaande criteria initialiseren
    document
        .querySelectorAll<HTMLLIElement>('.criteria-item')
        .forEach((item) => {
            addCriteriaHandlers(item);
            item
                .querySelectorAll<HTMLLIElement>('.subcriteria-item')
                .forEach((sub) => addSubCriteriaHandlers(sub));
        });

    const addBtn = document.getElementById('add-criteria-btn') as HTMLButtonElement | null;
    addBtn?.addEventListener('click', () => addCriteria());

    const toggleAllBtn = document.getElementById('toggle-all-btn') as HTMLButtonElement | null;
    toggleAllBtn?.addEventListener('click', () => cToggleExpandAll(toggleAllBtn));

    updateCriteriaLimitUI(); // <- nieuw: direct checken bij init
}



// ----------------------
// Criteria handlers
// ----------------------
function addCriteriaHandlers(criteria: HTMLLIElement): void {
    // uitklap pijltje
    const expandBtn = criteria.querySelector<HTMLButtonElement>('.expand-btn');
    expandBtn?.addEventListener('click', () => cToggleArrow(expandBtn!));

    // verwijder criteria
    const removeBtn = criteria.querySelector<HTMLButtonElement>('.remove-criteria-btn');
    removeBtn?.addEventListener('click', () => {
        removeCriteria(criteria);
        initSubcriteriaSums();
    });

    // voeg subcriteria toe
    const addSubBtn = criteria.querySelector<HTMLButtonElement>('.add-subcriteria-btn');
    addSubBtn?.addEventListener('click', () => {
        const idx = addSubBtn.getAttribute('criteria-index');
        if (idx !== null) {
            addSubCriteria(idx);
            initSubcriteriaSums();
        }
    });
}

// ----------------------
// Subcriteria handlers
// ----------------------
function addSubCriteriaHandlers(sub: HTMLLIElement): void {
    // verwijder subcriteria
    const removeSub = sub.querySelector<HTMLButtonElement>('.remove-subcriteria-btn');
    removeSub?.addEventListener('click', () => {
        removeSubCriteria(sub);
        initSubcriteriaSums();
    });
}

// ----------------------
// Collapse / Expand
// ----------------------
function cToggleArrow(btn: HTMLButtonElement | null): void {
    if (!btn) return;
    const icon = btn.querySelector('i');
    icon?.classList.toggle('bi-chevron-up');
    icon?.classList.toggle('bi-chevron-down');
}

function cToggleExpandAll(btn: HTMLButtonElement | null): void {
    document.querySelectorAll<HTMLDivElement>('.criteria-body').forEach((body) => {
        const collapse = (window as any).bootstrap.Collapse.getOrCreateInstance(body);
        allCriteriaCollapsed ? collapse.show() : collapse.hide();
    });

    allCriteriaCollapsed = !allCriteriaCollapsed;
    if (btn) btn.innerHTML = allCriteriaCollapsed ? 'Alles uitvouwen' : 'Alles invouwen';

    document.querySelectorAll<HTMLButtonElement>('.expand-btn').forEach((b) => {
        const icon = b.querySelector('i');
        if (!icon) return;
        icon.classList.toggle('bi-chevron-up', !allCriteriaCollapsed);
        icon.classList.toggle('bi-chevron-down', allCriteriaCollapsed);
    });
}

// ----------------------
// Create / Remove items
// ----------------------
function addCriteria(): void {
    const list = document.getElementById('criterias-list') as HTMLUListElement | null;
    if (!list) return;

    const ci = list.children.length;
    if (ci >= MAX_CRITERIA) {
        updateCriteriaLimitUI();
        return;
    }

    const newLi = generateCriteriaHtml(ci);
    list.appendChild(newLi);
    addCriteriaHandlers(newLi);
    addSubCriteria(ci.toString());

    updateCriteriaLimitUI();
}


function removeCriteria(criteria: HTMLLIElement): void {
    criteria.remove();
    updateCriteriaLimitUI();
}

function addSubCriteria(ci: string): void {
    const subList = document.getElementById(`subcriterias-list-${ci}`) as HTMLUListElement | null;
    if (!subList) return;

    const sci = subList.children.length;
    if (sci >= MAX_SUBCRITERIA) {
        updateSubCriteriaButton(ci);
        return;
    }

    const newSub = generateSubCriteriaHtml(ci, sci);
    subList.appendChild(newSub);
    addSubCriteriaHandlers(newSub);
    initSubcriteriaSums();

    updateSubCriteriaButton(ci);
}

function removeSubCriteria(sub: HTMLLIElement): void {
    const parent = sub.closest('ul') as HTMLUListElement | null;
    sub.remove();
    validateAll();

    if (parent?.id.startsWith('subcriterias-list-')) {
        const ci = parent.id.replace('subcriterias-list-', '');
        updateSubCriteriaButton(ci);
    }
}

// HTML Generators
function generateCriteriaHtml(ci: number): HTMLLIElement {
    const li = document.createElement('li');
    li.className = 'card mb-3 criteria-item';
    li.innerHTML = `
    <input name="Criteria[${ci}].Id" type="hidden" class="criteria-id" value="0" />
    <div class="card-header d-flex align-items-start">
      <div class="flex-grow-1">
        <label class="form-label fw-bold criteria-number">Criteria</label>
        <div class="d-flex align-items-center w-100">
          <input name="Criteria[${ci}].Name"
                 class="form-control mb-0 criteria-description"
                 placeholder="Criteria omschrijving" />
          <button type="button"
                  class="btn btn-danger btn-sm ms-2 remove-criteria-btn">
            <i class="bi bi-trash"></i>
          </button>
        </div>
      </div>
      <button class="btn btn-sm expand-btn ms-2"
              data-bs-toggle="collapse"
              aria-expanded="true"
              aria-controls="criteria-body-@i"
              data-bs-target="#criteria-body-${ci}">
        <i class="bi bi-chevron-up"></i>
      </button>
    </div>
    <div class="collapse show criteria-body" id="criteria-body-${ci}">
      <div class="card-body">
        <label class="form-label fw-bold">Subcriteria</label>
        <ul id="subcriterias-list-${ci}" class="list-unstyled"></ul>
        <div class="d-flex justify-content-between align-items-center mt-2">
          <button type="button"
                  class="btn btn-sm btn-secondary add-subcriteria-btn"
                  criteria-index="${ci}">
            Voeg een subcriteria toe
          </button>
          <div class="subcriteria-total mt-2 text-end small">
            Totaal: <span class="subcriteria-sum">0</span>%
            <span class="text-danger subcriteria-warning" style="display:none;">
              (moet 100%)
            </span>
          </div>
        </div>
      </div>
    </div>
  `;
    return li;
}

function generateSubCriteriaHtml(ci: string, sci: number): HTMLLIElement {
    const li = document.createElement('li');
    li.className = 'row subcriteria-item mb-2 align-items-center p-2';
    li.innerHTML = `
    <input name="Criteria[${ci}].SubCriteria.Index" value="${sci}"
           type="hidden" class="subcriteria-id"" />
    <div class="col">
      <input name="Criteria[${ci}].SubCriteria[${sci}].Name"
             class="form-control subcriteria-description"
             placeholder="Subcriteria omschrijving" />  
    </div>
    <div class="col-auto">
      <div class="input-group">
        <input name="Criteria[${ci}].SubCriteria[${sci}].Percentage"
               type="number" min="0" max="100"
               class="form-control subcriteria-percentage"
               value="0" />
        <span class="input-group-text">%</span>
      </div>
    </div>
    <div class="col-auto">
      <button type="button"
              class="btn btn-danger btn-sm remove-subcriteria-btn">
        Verwijder
      </button>
    </div>
  `;
    return li;
}

// ----------------------
// Live total check
// ----------------------
function initSubcriteriaSums(): void {
    document.querySelectorAll<HTMLLIElement>('.criteria-item').forEach(criteria => {
        const subList = criteria.querySelector<HTMLUListElement>('[id^="subcriterias-list-"]');
        if (!subList) return;

        const sumDisplay = criteria.querySelector<HTMLSpanElement>('.subcriteria-sum')!;
        const warning = criteria.querySelector<HTMLElement>('.subcriteria-warning')!;

        const updateSum = () => {
            const inputs = Array.from(subList.querySelectorAll<HTMLInputElement>('.subcriteria-percentage'));
            const total = inputs.reduce((acc, i) => acc + Number(i.value), 0);
            sumDisplay.textContent = String(total);
            warning.style.display = (total === 100) ? 'none' : 'inline';
            validateAll();  
        };

        // delegatie: luister op de parent-lijst
        subList.addEventListener('input', (ev) => {
            const tgt = ev.target as HTMLInputElement;
            if (tgt.classList.contains('subcriteria-percentage')) {
                updateSum();
            }
        });

        updateSum();
    });

    validateAll();

    
}

function updateCriteriaLimitUI(): void {
    const list = document.getElementById('criterias-list') as HTMLUListElement | null;
    const addBtn = document.getElementById('add-criteria-btn') as HTMLButtonElement | null;

    const count = list?.querySelectorAll('.criteria-item').length ?? 0;
    const limitReached = count >= MAX_CRITERIA;

    if (addBtn) addBtn.disabled = limitReached;
}

function updateSubCriteriaButton(ci: string): void {
    const subList = document.getElementById(`subcriterias-list-${ci}`) as HTMLUListElement | null;
    const addBtn = document.querySelector<HTMLButtonElement>(`.add-subcriteria-btn[criteria-index="${ci}"]`);
    if (!subList || !addBtn) return;

    const count = subList.children.length;
    addBtn.disabled = count >= MAX_SUBCRITERIA;
}

function validateAll() {
    const calculateBtn = document.getElementById('calculate-btn') as HTMLButtonElement;
    // check per criteria-item of sum = 100
    const allValid = Array.from(document.querySelectorAll<HTMLLIElement>('.criteria-item'))
        .every(item => Number(item.querySelector<HTMLSpanElement>('.subcriteria-sum')!.textContent) === 100);

    calculateBtn.disabled = !allValid;
}