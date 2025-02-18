import clsx from 'clsx';

import { type ComponentStyles } from '@/helpers/types/ui-types';

interface DateItemProps extends ComponentStyles {
    date: string;
    day: string;
    month: string;
    year: string;
    fullDate: string;
    isSelected: boolean;
    onClick: () => void;
    className?: string;
    classNameText?: string;
}

const DateItem = ({
    date,
    day,
    month,
    year,
    fullDate,
    isSelected,
    onClick,
    variant,
    size,
    className,
    classNameText,
}: DateItemProps) => {
    const { container, text } = sizeClasses[size];

    return (
        <div
            data-testid={`date-${fullDate}`}
            className={clsx(
                'rounded-md cursor-pointer text-center',
                container,
                isSelected
                    ? variantStyles[variant].selected
                    : variantStyles[variant].unselected,
                className
            )}
            onClick={onClick}
        >
            <p className={clsx(text, classNameText)}>{date}</p>
            <p className={clsx(text, classNameText)}>{day}</p>
            <p className={clsx(text, classNameText)}>
                {month} {year}
            </p>
        </div>
    );
};

export default DateItem;

const variantStyles = {
    primary: {
        selected: 'bg-[#2B5162] text-white',
        unselected: 'bg-[#E2E2E2] text-[#2B5162]',
    },
};

const sizeClasses = {
    sm: { container: 'py-1', text: 'text-sm' },
    md: { container: 'py-2', text: 'text-base' },
    lg: { container: 'py-2', text: 'text-lg' },
};
