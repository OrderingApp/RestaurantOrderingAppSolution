import BasicStyles from '../types/types';

const btnStyles: {
    variants: Omit<BasicStyles['variants'], 'secondary' | 'tertiary'> & {
        outline: string;
    };
    sizes: BasicStyles['sizes'] & { xl: string };
} = {
    variants: {
        primary: 'bg-[#2B5162] text-white',
        success: 'bg-[#2B622F] text-white',
        danger: 'bg-[#F20707] text-white ',
        outline: 'bg-white shadow-lg text-black border border-gray-200',
    },
    sizes: {
        sm: 'px-2 py-1 text-sm',
        md: 'px-4 py-2 text-base',
        lg: 'px-6 py-6 text-2xl rounded-3xl',
        xl: 'px-[2.125rem] py-1 text-xl rounded-full',
    },
};

export default btnStyles;
