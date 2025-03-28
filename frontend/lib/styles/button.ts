import BasicStyles from '../types/types';

const btnStyles: {
    variants: Omit<BasicStyles['variants'], 'secondary'> & {
        outline: string;
    };
    sizes: BasicStyles['sizes'] & { xl: string };
} = {
    variants: {
        primary: 'bg-primary text-white',
        tertiary: 'bg-tertiary text-white',
        success: 'bg-[#2B622F] text-white',
        danger: 'bg-danger text-white ',
        outline: 'bg-white shadow-lg text-black border border-gray-200',
    },
    sizes: {
        sm: 'w-[10.5rem] h-[3.125rem] text-[1rem]',
        md: 'w-[11.25rem] h-[3.75rem] text-[1rem] rounded-lg',
        lg: 'w-[23.75rem] h-[3.125rem] text-xl rounded-xl',
        xl: 'px-[2.125rem] py-1 text-xl rounded-full',
    },
};
//TODO: updates sizes and variants
export default btnStyles;
