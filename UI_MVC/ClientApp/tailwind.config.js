/** @type {import('tailwindcss').Config} */
export default {
    content: [
        './src/**/*.{vue,js,ts,jsx,tsx,scss,css}',
        '../Views/**/*.cshtml',
        '../Pages/**/*.cshtml',
        '../Areas/**/*.cshtml'
    ],
    theme: {
        extend: {
            colors: {
                'text-black': '#080708',
                'brand-black': '#080708',
                'brand-cadet-gray': '#A5B5BF',
                'brand-ash-gray': '#ABC8C7',
                'brand-celadon': '#B8E2C8',
                'brand-celadon-light': '#BFF0D4',
                'primary': '#A5B5BF',
                'primary-hover': '#8E9FAA',
                'secondary': '#ABC8C7',
                'secondary-hover': '#97B3B2',
                'accent': '#B8E2C8',
                'accent-hover': '#A3CFAF',
                'background-page': '#FFFFFF',
                'surface': '#FFFFFF',
                'header-text': '#FFFFFF',
                'header-text-hover': '#E0E0E0',
                'header-nav-item-bg-hover': 'rgba(255, 255, 255, 0.1)',
                'red-500': '#EF4444',
                'red-600': '#DC2626',
                'red-700': '#B91C1C',
                'gray-50': '#F9FAFB',
                'gray-100': '#F3F4F6',
                'gray-200': '#E5E7EB',
                'gray-300': '#D1D5DB',
                'gray-400': '#9CA3AF',
                'gray-500': '#6B7280',
                'gray-600': '#4B5563',
                'gray-700': '#374151',
                'gray-800': '#1F2937',
                'gray-900': '#111827',
            },
            fontFamily: {
                sans: ['Bahnschrift', 'Segoe UI', 'Calibri', 'Roboto', 'Helvetica Neue', 'Arial', 'sans-serif'],
            }
        }
    },
    plugins: [
        require('@tailwindcss/forms'),
    ],
}