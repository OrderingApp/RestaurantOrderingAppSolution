import type { Config } from 'tailwindcss';
import type { PluginAPI } from 'tailwindcss/types/config';

export default {
    darkMode: ['class'],
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
                'reservation-gradient':
                    'linear-gradient(135deg, #2B5162 0%, #203D4A 100%)',
                'modal-gradient':
                    'linear-gradient(135deg, #6E8894 0%, #334046 100%)',
                'payment-modal-gradient':
                    'linear-gradient(135deg, #CD5700 0%, #843901 100%)',
                'attention-gradient':
                    'linear-gradient(135deg, #DD8080 0%, #BB0101 100%)',
                'order-card-gradient':
                    'linear-gradient(135deg, #CD5700 0%, #843901 100%)',
                'order-paid-gradient':
                    'linear-gradient(135deg, #008080 0%, #005656 100%)',
            },
            colors: {
                primary: {
                    DEFAULT: 'hsl(var(--primary))',
                    foreground: 'hsl(var(--primary-foreground))',
                },
                secondary: {
                    DEFAULT: 'hsl(var(--secondary))',
                    foreground: 'hsl(var(--secondary-foreground))',
                },
                tertiary: 'var(--tertiary)',
                quaternary: 'var(--quaternary)',
                success: 'var(--success)',
                danger: 'var(--danger)',
                'danger-dark': 'var(--danger-dark)',
                'danger-light': 'var(--danger-light)',
                warning: 'var(--warning)',
                brown: 'var(--brown)',
                white: 'var(--white)',
                black: 'var(--black)',
                'lighter-gray': 'var(--lighter-gray)',
                'light-gray': 'var(--light-gray)',
                gray: 'var(--gray)',
                'dark-gray': 'var(--dark-gray)',
                served: 'var(--served)',
                ongoing: 'var(--ongoing)',
                paid: 'var(--paid)',
                background: 'hsl(var(--background))',
                foreground: 'hsl(var(--foreground))',
                card: {
                    DEFAULT: 'hsl(var(--card))',
                    foreground: 'hsl(var(--card-foreground))',
                },
                popover: {
                    DEFAULT: 'hsl(var(--popover))',
                    foreground: 'hsl(var(--popover-foreground))',
                },
                muted: {
                    DEFAULT: 'hsl(var(--muted))',
                    foreground: 'hsl(var(--muted-foreground))',
                },
                accent: {
                    DEFAULT: 'hsl(var(--accent))',
                    foreground: 'hsl(var(--accent-foreground))',
                },
                destructive: {
                    DEFAULT: 'hsl(var(--destructive))',
                    foreground: 'hsl(var(--destructive-foreground))',
                },
                border: 'hsl(var(--border))',
                input: 'hsl(var(--input))',
                ring: 'hsl(var(--ring))',
                chart: {
                    '1': 'hsl(var(--chart-1))',
                    '2': 'hsl(var(--chart-2))',
                    '3': 'hsl(var(--chart-3))',
                    '4': 'hsl(var(--chart-4))',
                    '5': 'hsl(var(--chart-5))',
                },
            },
            fontFamily: {
                serif: 'var(--serif)',
                sans: 'var(--sans)',
            },
            boxShadow: {
                'inner-sm': 'inset 0px 4px 4px 0px #00000040',
                'inner-md': 'inset 0 0 5px rgba(0,0,0,0.5)',
                'inner-lg': 'inset 0 0 10px 0 #00000040',
                'sm-left': '-4px 0px 4px 0px #00000040',
            },
            borderRadius: {
                lg: 'var(--radius)',
                md: 'calc(var(--radius) - 2px)',
                sm: 'calc(var(--radius) - 4px)',
            },
        },
    },
    plugins: [
        ({ addVariant }: PluginAPI) => {
            addVariant('hocus', ['&:hover', '&:focus']);
        },
        // eslint-disable-next-line @typescript-eslint/no-require-imports
        require('tailwindcss-animate'),
    ],
} satisfies Config;
