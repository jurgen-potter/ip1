let allCollapsed: boolean = false;
const MAX_CRITERIA = 5;
const MAX_SUBCRITERIA = 5;

window.addEventListener('DOMContentLoaded', () => {
    editCriteriaInit();
    initSubcriteriaSums(); // Deze functie roept validateAll aan

    const toggleAllBtn = document.getElementById('toggle-all-btn') as HTMLButtonElement | null;
    if (toggleAllBtn) {
        // Bepaal initiële staat voor de knoptekst
        const firstCriteriaBody = document.querySelector<HTMLDivElement>('.criteria-card-body-wrapper');
        if (firstCriteriaBody && firstCriteriaBody.classList.contains('is-collapsed')) {
            allCollapsed = true;
        }
        toggleAllBtn.textContent = allCollapsed ? 'Alles uitvouwen' : 'Alles invouwen';
    }

    const totalAvailableInput = document.getElementById('TotalAvailablePotentialPanelmembers') as HTMLInputElement | null;
    totalAvailableInput?.addEventListener('input', () => {
        validateAllFormInputs();
        const rawValue = totalAvailableInput.value;
        const value = parseInt(rawValue, 10);

        if (isNaN(value)) {
            return;
        }

        const min = 100;
        const max = 1_000_000;

        // Show or hide warning based on range
        warning.style.display = (value < min || value > max) ? 'block' : 'none';

    });

    const warning = document.querySelector('.panelmember-warning') as HTMLElement;
});

// ----------------------
// Initialization
// ----------------------
function editCriteriaInit(): void {
    document
        .querySelectorAll<HTMLLIElement>('.criteria-item-card') // Gebruik de nieuwe SCSS klasse
        .forEach((item) => {
            addCriteriaHandlers(item);
            item
                .querySelectorAll<HTMLLIElement>('.subcriteria-item-row') // Gebruik de nieuwe SCSS klasse
                .forEach((sub) => addSubCriteriaHandlers(sub));

            // Stel de initiële staat van de pijltjes in
            const expandBtn = item.querySelector<HTMLButtonElement>('.criteria-expand-button');
            const criteriaBody = item.querySelector<HTMLDivElement>('.criteria-card-body-wrapper');
            if (expandBtn && criteriaBody) {
                const isOpen = !criteriaBody.classList.contains('is-collapsed'); // CSHTML start nu open
                updateExpandButtonIcon(expandBtn, isOpen);
                expandBtn.setAttribute('aria-expanded', isOpen.toString());
            }
        });

    const addBtn = document.getElementById('add-criteria-btn') as HTMLButtonElement | null;
    addBtn?.addEventListener('click', () => addCriteria());

    const toggleAllBtn = document.getElementById('toggle-all-btn') as HTMLButtonElement | null;
    toggleAllBtn?.addEventListener('click', () => cToggleExpandAll(toggleAllBtn));

    updateCriteriaLimitUI();
}

// ----------------------
// Criteria handlers
// ----------------------
function addCriteriaHandlers(criteriaItemCard: HTMLLIElement): void {
    const expandBtn = criteriaItemCard.querySelector<HTMLButtonElement>('.criteria-expand-button');
    expandBtn?.addEventListener('click', () => {
        // De target voor collapse is de .criteria-card-body-wrapper binnen dezelfde .criteria-item-card
        const targetElement = criteriaItemCard.querySelector<HTMLDivElement>('.criteria-card-body-wrapper');
        if (targetElement) {
            toggleCollapse(targetElement, expandBtn);
        }
    });

    const removeBtn = criteriaItemCard.querySelector<HTMLButtonElement>('.btn-remove-criteria');
    removeBtn?.addEventListener('click', () => {
        removeCriteria(criteriaItemCard);
        // initSubcriteriaSums(); // Wordt in removeCriteria aangeroepen
    });

    const addSubBtn = criteriaItemCard.querySelector<HTMLButtonElement>('.btn-add-subcriteria');
    addSubBtn?.addEventListener('click', () => {
        const idx = addSubBtn.getAttribute('criteria-index');
        if (idx !== null) {
            addSubCriteria(idx);
            // initSubcriteriaSums(); // Wordt in addSubCriteria aangeroepen
        }
    });
}

// ----------------------
// Subcriteria handlers
// ----------------------
function addSubCriteriaHandlers(subcriteriaItemRow: HTMLLIElement): void {
    const removeSub = subcriteriaItemRow.querySelector<HTMLButtonElement>('.btn-remove-subcriteria');
    removeSub?.addEventListener('click', () => {
        removeSubCriteria(subcriteriaItemRow);
        // initSubcriteriaSums(); // Wordt in removeSubCriteria aangeroepen
    });

    const percentageInput = subcriteriaItemRow.querySelector<HTMLInputElement>('.subcriteria-percentage');
    percentageInput?.addEventListener('input', () => {
        const criteriaCard = subcriteriaItemRow.closest('.criteria-item-card');
        if (criteriaCard) {
            updateSumForCriteria(criteriaCard as HTMLLIElement);
        }
    });
}

// ----------------------
// Collapse / Expand (Custom Logic)
// ----------------------
function toggleCollapse(targetElement: HTMLDivElement, button: HTMLButtonElement): void {
    const isCurrentlyOpen = !targetElement.classList.contains('is-collapsed');
    targetElement.classList.toggle('is-collapsed');
    updateExpandButtonIcon(button, !isCurrentlyOpen);
    button.setAttribute('aria-expanded', (!isCurrentlyOpen).toString());
}

function updateExpandButtonIcon(btn: HTMLButtonElement, isOpen: boolean): void {
    const icon = btn.querySelector('i');
    if (icon) {
        icon.classList.remove('fa-chevron-up', 'fa-chevron-down');
        icon.classList.add(isOpen ? 'fa-chevron-up' : 'fa-chevron-down');
    }
}

function cToggleExpandAll(btn: HTMLButtonElement | null): void {
    allCollapsed = !allCollapsed;

    document.querySelectorAll<HTMLDivElement>('.criteria-card-body-wrapper').forEach((body) => {
        const expandButton = body.closest('.criteria-item-card')?.querySelector<HTMLButtonElement>('.criteria-expand-button');
        if (expandButton) {
            const shouldBeOpen = !allCollapsed;
            if (shouldBeOpen) {
                body.classList.remove('is-collapsed');
            } else {
                body.classList.add('is-collapsed');
            }
            updateExpandButtonIcon(expandButton, shouldBeOpen);
            expandButton.setAttribute('aria-expanded', shouldBeOpen.toString());
        }
    });

    if (btn) btn.textContent = allCollapsed ? 'Alles uitvouwen' : 'Alles invouwen';
}

// ----------------------
// Create / Remove items
// ----------------------
function addCriteria(): void {
    const list = document.getElementById('criterias-list') as HTMLUListElement | null;
    if (!list) return;

    const ci = list.querySelectorAll('.criteria-item-card').length; // Gebruik nieuwe klasse
    if (ci >= MAX_CRITERIA) {
        updateCriteriaLimitUI();
        return;
    }

    const newLi = generateCriteriaHtml(ci); // ci is de nieuwe index (0-based)
    list.appendChild(newLi);
    addCriteriaHandlers(newLi);
    addSubCriteria(ci.toString()); // Voegt een eerste subcriteria toe

    updateCriteriaLimitUI();
    // initSubcriteriaSums(); // Wordt aangeroepen via addSubCriteria -> updateSumForCriteria
}

function removeCriteria(criteriaItemCard: HTMLLIElement): void {
    criteriaItemCard.remove();
    updateCriteriaLimitUI();
    initSubcriteriaSums(); // Herbereken alles na verwijderen van een hoofdcriteria
}

function addSubCriteria(ci: string): void { // ci is de criteria-index (string)
    const subList = document.getElementById(`subcriterias-list-${ci}`) as HTMLUListElement | null;
    if (!subList) return;

    const sci = subList.children.length; // sci is de nieuwe subcriteria-index (0-based)
    if (sci >= MAX_SUBCRITERIA) {
        updateSubCriteriaButtonState(ci); // Hernoemd
        return;
    }

    const newSub = generateSubCriteriaHtml(ci, sci);
    subList.appendChild(newSub);
    addSubCriteriaHandlers(newSub);

    const criteriaCard = subList.closest('.criteria-item-card');
    if (criteriaCard) {
        updateSumForCriteria(criteriaCard as HTMLLIElement);
    }
    updateSubCriteriaButtonState(ci); // Hernoemd
}

function removeSubCriteria(subcriteriaItemRow: HTMLLIElement): void {
    const parentList = subcriteriaItemRow.closest('ul.subcriteria-list') as HTMLUListElement | null;
    const criteriaCard = parentList?.closest('.criteria-item-card') as HTMLLIElement | null;
    const criteriaIndex = criteriaCard?.querySelector<HTMLButtonElement>('.btn-add-subcriteria')?.getAttribute('criteria-index');

    subcriteriaItemRow.remove();

    if (criteriaCard && criteriaIndex) { // Zorg dat criteriaIndex bestaat
        updateSubCriteriaButtonState(criteriaIndex);
        updateSumForCriteria(criteriaCard); // Update de som van het parent criteria
    } else if (criteriaCard) { // Fallback als criteriaIndex niet direct gevonden wordt
        updateSumForCriteria(criteriaCard);
    }
    // validateAllFormInputs(); // Wordt in updateSumForCriteria gedaan
}

// HTML Generators (Aangepast aan nieuwe klassen en Font Awesome)
function generateCriteriaHtml(ci: number): HTMLLIElement { // ci is de 0-based index
    const li = document.createElement('li');
    li.className = 'criteria-item-card';
    const criteriaBodyId = `criteria-body-${ci}`;
    // Nieuw toegevoegde criteria zijn altijd bewerkbaar en verwijderbaar
    li.innerHTML = `
    <input name="Criteria[${ci}].Id" type="hidden" class="criteria-id" value="0" />
    <div class="criteria-card-header">
      <div class="criteria-header-content">
        <label class="criteria-list-label">Criteria</label>
        <div class="criteria-header-input-group">
          <input name="Criteria[${ci}].Name"
                 class="auth-input-field criteria-description"
                 placeholder="Criteria omschrijving" required />
          <button type="button" class="btn btn-danger btn-sm-equivalent btn-remove-criteria">
            <i class="fas fa-trash"></i>
          </button>
        </div>
        <span data-valmsg-for="Criteria[${ci}].Name" class="auth-validation-message"></span>
      </div>
      <button type="button" class="criteria-expand-button"
              data-bs-target="#${criteriaBodyId}" 
              aria-expanded="true"
              aria-controls="${criteriaBodyId}">
        <i class="fas fa-chevron-up"></i>
      </button>
    </div>
    <div class="criteria-card-body-wrapper" id="${criteriaBodyId}">
      <div class="criteria-card-body">
        <label class="subcriteria-list-label">Subcriteria</label>
        <ul id="subcriterias-list-${ci}" class="subcriteria-list"></ul>
        <div class="subcriteria-footer">
          <button type="button"
                  class="btn btn-secondary btn-add-subcriteria"
                  criteria-index="${ci}">
            Voeg een subcriteria toe
          </button>
          <div class="subcriteria-total-display">
            Totaal: <span class="subcriteria-sum">0</span>%
            <span class="subcriteria-warning" style="display:none;">
              (moet 100%)
            </span>
          </div>
        </div>
      </div>
    </div>
  `;
    return li;
}

function generateSubCriteriaHtml(ci: string, sci: number): HTMLLIElement { // ci is criteria-index (string), sci is subcriteria-index (number)
    const li = document.createElement('li');
    li.className = 'subcriteria-item-row';
    li.innerHTML = `
    <input name="Criteria[${ci}].SubCriteria[${sci}].Id" type="hidden" class="subcriteria-id-hidden" value="0" />
    <input name="Criteria[${ci}].SubCriteria.Index" value="${sci}" type="hidden" />
    <div class="subcriteria-input-name-wrapper">
      <input name="Criteria[${ci}].SubCriteria[${sci}].Name"
             class="auth-input-field subcriteria-description"
             placeholder="Subcriteria omschrijving" required />
      <span data-valmsg-for="Criteria[${ci}].SubCriteria[${sci}].Name" class="auth-validation-message"></span>
    </div>
    <div class="subcriteria-input-percentage-wrapper">
      <div class="subcriteria-percentage-group">
        <input name="Criteria[${ci}].SubCriteria[${sci}].Percentage"
               type="number" min="0" max="100"
               class="subcriteria-percentage-group subcriteria-percentage"
               value="0" required />
        <span class="percentage-suffix">%</span>
      </div>
    </div>
    <div class="subcriteria-remove-button-wrapper">
      <button type="button" class="btn btn-danger btn-sm-equivalent btn-remove-subcriteria">
            <i class="fas fa-trash"></i>
      </button>
    </div>
  `;
    return li;
}

// ----------------------
// Live total check & UI Updates
// ----------------------
function updateSumForCriteria(criteriaItemCard: HTMLLIElement): void {
    const subList = criteriaItemCard.querySelector<HTMLUListElement>('ul.subcriteria-list');
    if (!subList) return;

    const sumDisplay = criteriaItemCard.querySelector<HTMLSpanElement>('.subcriteria-sum');
    const warningDisplay = criteriaItemCard.querySelector<HTMLElement>('.subcriteria-warning');

    if (!sumDisplay || !warningDisplay) return;

    const inputs = Array.from(subList.querySelectorAll<HTMLInputElement>('.subcriteria-percentage'));
    const total = inputs.reduce((acc, input) => acc + Number(input.value), 0);
    sumDisplay.textContent = String(total);
    warningDisplay.style.display = (total === 100 || inputs.length === 0) ? 'none' : 'inline';
    validateAllFormInputs(); // Hernoemd van validateAll
}

function initSubcriteriaSums(): void {
    document.querySelectorAll<HTMLLIElement>('.criteria-item-card').forEach(criteriaItemCard => {
        updateSumForCriteria(criteriaItemCard);
        criteriaItemCard.querySelectorAll<HTMLInputElement>('.subcriteria-percentage').forEach(input => {
            input.addEventListener('input', () => {
                updateSumForCriteria(criteriaItemCard);
            });
        });
    });
    validateAllFormInputs(); // Hernoemd van validateAll
}

function updateCriteriaLimitUI(): void {
    const list = document.getElementById('criterias-list') as HTMLUListElement | null;
    const addBtn = document.getElementById('add-criteria-btn') as HTMLButtonElement | null;
    if (!list || !addBtn) return;

    const count = list.querySelectorAll('.criteria-item-card').length;
    addBtn.disabled = count >= MAX_CRITERIA;
}

function updateSubCriteriaButtonState(ci: string): void { // Hernoemd
    const subList = document.getElementById(`subcriterias-list-${ci}`) as HTMLUListElement | null;
    const addBtn = document.querySelector<HTMLButtonElement>(`.btn-add-subcriteria[criteria-index="${ci}"]`);
    if (!subList || !addBtn) return;

    const count = subList.children.length;
    addBtn.disabled = count >= MAX_SUBCRITERIA;
}

function validateAllFormInputs(): void {
    const calculateBtn = document.getElementById('calculate-btn') as HTMLButtonElement | null;
    const totalAvailableInput = document.getElementById('TotalAvailablePotentialPanelmembers') as HTMLInputElement | null;
    const panelWarning = document.querySelector<HTMLElement>('.panelmember-warning');

    if (!calculateBtn || !totalAvailableInput || !panelWarning) return;

    let allSumsAreValid = true;

    document.querySelectorAll<HTMLLIElement>('.criteria-item-card').forEach(item => {
        const inputs = item.querySelectorAll<HTMLInputElement>('.subcriteria-percentage');
        if (inputs.length > 0) {
            const sumText = item.querySelector<HTMLSpanElement>('.subcriteria-sum')?.textContent;
            if (!sumText || Number(sumText) !== 100) {
                allSumsAreValid = false;
            }
        }
    });

    const allDescriptionsFilled = Array.from(document.querySelectorAll<HTMLInputElement>('input.criteria-description, input.subcriteria-description'))
        .every(input => {
            if (input.hasAttribute('readonly')) return true;
            return input.value.trim() !== '';
        });

    const totalAvailable = Number(totalAvailableInput.value);
    const totalAvailableValid = !isNaN(totalAvailable) && totalAvailable >= 100;

    panelWarning.style.display = totalAvailableValid ? 'none' : 'block';

    calculateBtn.disabled = !allSumsAreValid || !allDescriptionsFilled || !totalAvailableValid;
}
