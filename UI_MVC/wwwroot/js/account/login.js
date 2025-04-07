window.addEventListener('DOMContentLoaded', (event) => {
    init();
})

function init() {
    let selectedType = document.getElementById("selectedLogin").value;
    let selectedBtn = document.getElementById("accountBtn");
    if (selectedType !== "Account") {
        selectedType = "Code";
        selectedBtn = document.getElementById("codeBtn");
    }
    
    selectedBtn.classList.add("active");
    showCorrectFields(selectedType);
    setEventListener();
}

function showCorrectFields(selectedType) {
    const accountFields = document.getElementById("accountFields");
    const codeFields = document.getElementById("codeFields");
    const selectedLogin = document.getElementById("selectedLogin")

    if (selectedType === "Account") {
        codeFields.classList.add("d-none");
        accountFields.classList.remove("d-none");
        selectedLogin.value = "PanelMember";
    } else {
        codeFields.classList.remove("d-none");
        accountFields.classList.add("d-none");
        selectedLogin.value = "Organization";
    }
}

function setEventListener() {
    const loginTypeButtons = document.querySelectorAll(".login-type-btn");

    loginTypeButtons.forEach(button => {
        button.addEventListener("click", function () {
            loginTypeButtons.forEach(btn => btn.classList.remove("active"));
            
            this.classList.add("active");
            const selectedType = this.getAttribute("data-type");
            
            showCorrectFields(selectedType);
        });
    });
}