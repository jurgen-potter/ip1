/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./UI_MVC/Views/**/*.cshtml",
    "./UI_MVC/Pages/**/*.cshtml"
  ],
  theme: {
    extend: {
      colors: {
        'text-black': '#080708',
        'brand-black': '#080708', // Voor de donkere header achtergrond
        'brand-cadet-gray': '#A5B5BF',
        'brand-ash-gray': '#ABC8C7', // Nieuwe hero achtergrond
        'brand-celadon': '#B8E2C8',
        'brand-celadon-light': '#BFF0D4',
        'primary': '#A5B5BF',      // Cadet Grijs
        'primary-hover': '#8E9FAA', // Iets donkerder voor hover
        'secondary': '#ABC8C7',    // Ash Grijs
        'secondary-hover': '#97B3B2',// Iets donkerder voor hover
        'accent': '#B8E2C8',       // Celadon
        'accent-hover': '#A3CFAF',  // Iets donkerder voor hover
        'background-page': '#FFFFFF', // Witte achtergrond voor de pagina
        'surface': '#FFFFFF', // Wit voor kaarten, modals etc.
        'header-text': '#FFFFFF', // Witte tekst voor op donkere header
        'header-text-hover': '#E0E0E0', // Iets gedempter wit voor hover in header
        'header-nav-item-bg-hover': 'rgba(255, 255, 255, 0.1)', // Subtiele hover achtergrond voor nav items
      },
      fontFamily: {
        sans: ['Bahnschrift', 'Segoe UI', 'Calibri', 'Roboto', 'Helvetica Neue', 'Arial', 'sans-serif'],
      }
    }
  },
  plugins: [],
} 