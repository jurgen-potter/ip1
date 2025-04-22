window.addEventListener('DOMContentLoaded', () => {
    loginInit();
});

function loginInit(): void {
    const selectedLogin = document.getElementById("selectedLogin") as HTMLInputElement;
    let selectedType = selectedLogin.value;
    let selectedBtn = document.getElementById("accountBtn") as HTMLButtonElement;
    if (selectedType !== "Account") {
        selectedType = "Code";
        selectedBtn = document.getElementById("codeBtn") as HTMLButtonElement;
    }

    selectedBtn.classList.add("active");
    showCorrectFields(selectedType);
    setEventListener();
}

function showCorrectFields(selectedType: string): void {
    const accountFields = document.getElementById("accountFields") as HTMLDivElement;
    const codeFields = document.getElementById("codeFields") as HTMLDivElement;
    const selectedLogin = document.getElementById("selectedLogin") as HTMLInputElement;

    if (accountFields && codeFields && selectedLogin) {
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
}

function setEventListener(): void {
    const loginTypeButtons = document.querySelectorAll<HTMLButtonElement>(".login-type-btn");

    loginTypeButtons.forEach(button => {
        button.addEventListener("click", function (this: Element) {
            loginTypeButtons.forEach(btn => btn.classList.remove("active"));

            this.classList.add("active");
            const selectedType = this.getAttribute("data-type") ?? "Account";

            showCorrectFields(selectedType);
        });
    });
}
