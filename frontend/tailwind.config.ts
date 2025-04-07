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
                'modal-gradient':
                    'linear-gradient(135deg, #6E8894 0%, #334046 100%)',
                'payment-modal-gradient':
                    'linear-gradient(135deg, #CD5700 0%, #843901 100%)',
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
            boxShadow: {
                'inner-md': 'inset 0 0 5px rgba(0,0,0,0.5)',
                'inner-lg': 'inset 0 0 10px 0 #00000040',
            },
        },
    },
    plugins: [
        ({ addVariant }: PluginAPI) => {
            addVariant('hocus', ['&:hover', '&:focus']);
        },
    ],
} satisfies Config;
