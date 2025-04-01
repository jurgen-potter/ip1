window.addEventListener('DOMContentLoaded', (event) => {
    init();
})

function init() {
    const initialSelectedButton = document.querySelector(".account-type-btn.active");
    const selectedType = initialSelectedButton.getAttribute("data-type");
    showCorrectFields(selectedType);
    setEventListener();
}

function showCorrectFields(selectedType) {
    const panelmemberFields = document.getElementById("panelmemberFields");
    const organizationFields = document.getElementById("organizationFields");

    if (selectedType === "Panelmember") {
        organizationFields.classList.add("d-none");
        panelmemberFields.classList.remove("d-none");
    } else {
        organizationFields.classList.remove("d-none");
        panelmemberFields.classList.add("d-none");
    }
}

function setEventListener() {
    const accountTypeButtons = document.querySelectorAll(".account-type-btn");
    
    accountTypeButtons.forEach(button => {
        button.addEventListener("click", function () {
            accountTypeButtons.forEach(btn => btn.classList.remove("active"));
            
            this.classList.add("active");
            const selectedType = this.getAttribute("data-type");
            showCorrectFields(selectedType);
        });
    });
}