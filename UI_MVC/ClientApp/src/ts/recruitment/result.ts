window.addEventListener('DOMContentLoaded', () => {
    const input = document.getElementById('panelmember-count') as HTMLInputElement;
    const warning = document.querySelector('.panelmember-warning') as HTMLElement;

    input.addEventListener('input', () => {
        const rawValue = input.value;
        const value = parseInt(rawValue, 10);

        if (isNaN(value)) {
            return;
        }

        const min = 5;
        const max = 500;

        // Show or hide warning based on range
        warning.style.display = (value < min || value > max) ? 'block' : 'none';
    });

});
