import { BasicStyles } from '../types/types';

export const inputStyles: {
    variants: Omit<
        BasicStyles['variants'],
        'success' | 'danger' | 'tertiary' | 'quaternary'
    >;
    sizes: {
        [K in keyof Omit<BasicStyles['sizes'], 'xxs' | 'xs' | 'xl'>]: {
            input: string;
            label: string;
            error: string;
        };
    };
} = {
    variants: {
        primary: 'bg-[#E6E6E6] text-[#2B5162]',
        secondary:
            'bg-white shadow-inner-md rounded-lg text-center hide-input-number-icon',
    },
    sizes: {
        sm: {
            label: 'text-sm',
            input: 'px-4 py-2 text-md',
            error: 'text-sm',
        },
        md: {
            label: 'text-md',
            input: 'px-4 py-2 text-md',
            error: 'text-sm',
        },
        lg: {
            label: 'text-md',
            input: 'px-4 py-3 text-md',
            error: 'text-md',
        },
    },
} as const;
