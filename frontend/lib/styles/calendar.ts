import { BasicStyles } from '../types/types';

export const calendarStyles: {
    variants: {
        [K in keyof Omit<
            BasicStyles['variants'],
            'tertiary' | 'success' | 'danger' | 'quaternary'
        >]: {
            selected: string;
            unselected: string;
        };
    };
    sizes: {
        [K in keyof Omit<BasicStyles['sizes'], 'xxs' | 'xs' | 'xl'>]: {
            container: string;
            text: string;
        };
    };
} = {
    variants: {
        primary: {
            selected: 'bg-[#2B5162] text-white',
            unselected: 'bg-[#E2E2E2] text-[#2B5162]',
        },
        secondary: {
            selected: 'bg-primary text-white',
            unselected: 'bg-[#E2E2E2] text-primary',
        },
    },
    sizes: {
        sm: { container: 'py-2', text: 'text-xs' },
        md: { container: 'py-2', text: 'text-base' },
        lg: { container: 'py-2', text: 'text-lg' },
    },
};

export interface CalendarStyles {
    variant?: keyof typeof calendarStyles.variants;
    size?: keyof typeof calendarStyles.sizes;
}
