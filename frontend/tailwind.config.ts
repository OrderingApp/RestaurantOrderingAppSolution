import type { Config } from 'tailwindcss';

export default {
    content: [
        './pages/**/*.{js,ts,jsx,tsx,mdx}',
        './components/**/*.{js,ts,jsx,tsx,mdx}',
        './app/**/*.{js,ts,jsx,tsx,mdx}',
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
                brown: 'var(--brown)',
                white: 'var(--white)',
                black: 'var(--black)',
            },
            fontFamily: {
                serif: 'var(--serif)',
                sans: 'var(--sans)',
            },
        },
    },
    plugins: [],
} satisfies Config;
