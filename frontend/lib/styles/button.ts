import type { BasicStyles } from '../types/types';

const btnStyles: {
    variants: Omit<BasicStyles['variants'], 'secondary'> & {
        outline: string;
    };
    sizes: BasicStyles['sizes'];
} = {
    variants: {
        primary: 'bg-primary text-white',
        tertiary: 'bg-tertiary text-white',
        quaternary: 'bg-quaternary text-white',
        success: 'bg-[#2B622F] text-white',
        danger: 'bg-danger text-white ',
        outline: 'bg-white shadow-xl text-black ',
    },
    sizes: {
        xxs: 'p-3 px-5 text-sm rounded-xl',
        xs: 'py-3 px-[60px] text-sm rounded-xl shadow-[0px_0px_5px_0px_#6A6A6A]',
        sm: 'w-[10.5rem] h-[3.125rem] text-[1rem] shadow-[0px_4px_4px_0px_#00000040]',
        md: 'w-[11.25rem] h-[3.75rem] text-[1rem] rounded-lg shadow-[0px_4px_4px_0px_#00000040]',
        lg: 'w-[23.75rem] h-[3.125rem] text-xl rounded-xl shadow-[0px_4px_4px_0px_#00000040]',
        xl: 'px-[2.125rem] py-1 text-xl rounded-full shadow-[0px_4px_4px_0px_#00000040]',
    },
} as const;
//TODO: updates sizes and variants
export default btnStyles;
