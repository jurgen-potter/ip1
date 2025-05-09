/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        './Areas/**/*.cshtml',
        './wwwroot/css/components.css'
    ],
    theme: {
        extend: {
            colors: {
                'brand-black': '#333333',
                'brand-cadet-gray': '#91A3B0',
                'background-page': '#F8F9FA',
                'text-black': '#333333',
                'header-text': '#FFFFFF',
                'header-text-hover': '#CCCCCC',
                'header-nav-item-bg-hover': 'rgba(255, 255, 255, 0.1)'
            },
            fontFamily: {
                'sans': ['Inter', 'ui-sans-serif', 'system-ui', '-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'Roboto', 'Helvetica Neue', 'Arial', 'sans-serif']
            }
        },
    },
    plugins: [
        require('@tailwindcss/forms')
    ],
}