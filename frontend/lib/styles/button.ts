import BasicStyles from '../types/types';

const btnStyles: {
    variants: Omit<BasicStyles['variants'], 'secondary' | 'tertiary'> & {
        outline: string;
    };
    sizes: BasicStyles['sizes'] & { xl: string };
} = {
    variants: {
        primary: 'bg-primary text-white',
        success: 'bg-[#2B622F] text-white',
        danger: 'bg-[#F20707] text-white ',
        outline: 'bg-white shadow-lg text-black border border-gray-200',
    },
    sizes: {
        sm: 'px-2 py-1 text-sm',
        md: 'px-6 py-2 text-base rounded-lg',
        lg: 'px-6 py-4 text-xl rounded-xl',
        xl: 'px-[2.125rem] py-1 text-xl rounded-full',
    },
};
//TODO: updates sizes and variants
export default btnStyles;
