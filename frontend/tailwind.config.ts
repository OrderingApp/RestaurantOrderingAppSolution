import type { Config } from 'tailwindcss';
import type { PluginAPI } from 'tailwindcss/types/config';

export default {
    content: [
        './pages/**/*.{js,ts,jsx,tsx,mdx}',
        './components/**/*.{js,ts,jsx,tsx,mdx}',
        './app/**/*.{js,ts,jsx,tsx,mdx}',
        './lib/styles/**/*.{js,ts,jsx,tsx,mdx}',
    ],
    theme: {
        extend: {
            backgroundImage: {
                'page-gradient':
                    'linear-gradient(135deg, #2C5364 0%, #0F2027 100%)',
            },
            colors: {
                primary: 'var(--primary)',
                secondary: 'var(--secondary)',
                tertiary: 'var(--tertiary)',
                danger: 'var(--danger)',
                'danger-dark': 'var(--danger-dark)',
                brown: 'var(--brown)',
                white: 'var(--white)',
                black: 'var(--black)',
                'light-gray': 'var(--light-gray)',
            },
            fontFamily: {
                serif: 'var(--serif)',
                sans: 'var(--sans)',
            },
        },
    },
    plugins: [
        ({ addVariant }: PluginAPI) => {
            addVariant('hocus', ['&:hover', '&:focus']);
        },
    ],
} satisfies Config;
