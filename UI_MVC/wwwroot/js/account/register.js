window.addEventListener('DOMContentLoaded', (event) => {
    init();
})

function init() {
    let selectedType = document.getElementById("selectedAccount").value;
    let selectedBtn = document.getElementById("panelMemberBtn");
    if (selectedType !== "PanelMember") {
        selectedType = "Organization";
        selectedBtn = document.getElementById("organizationBtn");
    }
    
    selectedBtn.classList.add("active");
    showCorrectFields(selectedType);
    setEventListener();
}

function showCorrectFields(selectedType) {
    const panelMemberFields = document.getElementById("panelMemberFields");
    const organizationFields = document.getElementById("organizationFields");
    const selectedAccount = document.getElementById("selectedAccount")

    if (selectedType === "PanelMember") {
        organizationFields.classList.add("d-none");
        panelMemberFields.classList.remove("d-none");
        selectedAccount.value = "PanelMember";
    } else {
        organizationFields.classList.remove("d-none");
        panelMemberFields.classList.add("d-none");
        selectedAccount.value = "Organization";
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