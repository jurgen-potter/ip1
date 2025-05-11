window.addEventListener('DOMContentLoaded', () => {
    loginInit();
});

type LoginType = "Account" | "Code";

function loginInit(): void {
    const selectedLogin = document.getElementById("selectedLogin") as HTMLInputElement;
    const raw = selectedLogin.value;
    const selectedType: LoginType = raw === "Code" ? "Code" : "Account";

    const accountFields = document.getElementById("accountFields") as HTMLDivElement;
    const codeFields = document.getElementById("codeFields") as HTMLDivElement;
    
    if (selectedType === "Account") {
        accountFields.classList.add("active");
    } else {
        codeFields.classList.add("active");
    }

    highlightButton(selectedType);
    showCorrectFields(selectedType);
    adjustFormContainerHeight(selectedType);
    setEventListener();
    
    // Add resize listener to handle window size changes
    window.addEventListener('resize', () => {
        adjustFormContainerHeight(selectedType);
    });
}

function setEventListener(): void {
    document.querySelectorAll<HTMLButtonElement>(".login-type-button")
        .forEach(button => {
            button.addEventListener("click", () => {
                const dt = button.getAttribute("data-type") as LoginType;
                (document.getElementById("selectedLogin") as HTMLInputElement).value = dt;

                highlightButton(dt);
                showCorrectFields(dt);
                requestAnimationFrame(() => {
                    adjustFormContainerHeight(dt);
                });
            });
        });
}

function highlightButton(selectedType: LoginType): void {
    const accountBtn = document.getElementById("accountBtn") as HTMLButtonElement;
    const codeBtn    = document.getElementById("codeBtn")    as HTMLButtonElement;

    // reset both
    [accountBtn, codeBtn].forEach(btn => {
        btn.classList.remove("bg-primary", "text-white");
        btn.classList.add("text-primary");
    });

    // highlight the one that's active
    const activeBtn = selectedType === "Account" ? accountBtn : codeBtn;
    activeBtn.classList.add("bg-primary", "text-white");
    activeBtn.classList.remove("text-primary");
}

function showCorrectFields(selectedType: LoginType): void {
    const accountFields = document.getElementById("accountFields") as HTMLDivElement;
    const codeFields    = document.getElementById("codeFields")    as HTMLDivElement;

    const isAccount = selectedType === "Account";

    // Account panel
    accountFields.classList.toggle("form-hidden",  !isAccount);
    accountFields.classList.toggle("active",   isAccount);

    // Code panel
    codeFields.classList.toggle("form-hidden",  isAccount);
    codeFields.classList.toggle("active",  !isAccount);
}

function adjustFormContainerHeight(selectedType: LoginType): void {
    const loginCard = document.querySelector(".login-card-container") as HTMLDivElement;
    const activeForm = document.querySelector(`#${selectedType.toLowerCase()}Fields`) as HTMLDivElement;
    
    if (loginCard && activeForm) {
        const topSectionHeight = activeForm.offsetTop;
        const formHeight = activeForm.offsetHeight;
        const padding = 32;
        const totalHeight = topSectionHeight + formHeight + padding;
        loginCard.style.minHeight = `${totalHeight}px`;
    }
}
