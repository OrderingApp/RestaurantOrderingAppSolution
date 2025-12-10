import clsx from 'clsx';

import { calendarStyles } from '@/lib/styles/calendar';

interface DateItemProps {
    date: string;
    day: string;
    year: string;
    fullDate: string;
    monthNumber: string;
    variant: keyof typeof calendarStyles.variants;
    size: keyof typeof calendarStyles.sizes;
    isSelected: boolean;
    onClick: () => void;
    className?: string;
    classNameText?: string;
}

const DateItem = ({
    date,
    day,
    monthNumber,
    year,
    fullDate,
    isSelected,
    onClick,
    variant,
    size,
    className,
    classNameText,
}: DateItemProps) => {
    const { container, text } = calendarStyles.sizes[size];

    return (
        <div
            data-testid={`date-${fullDate}`}
            className={clsx(
                'cursor-pointer text-center',
                container,
                isSelected
                    ? calendarStyles.variants[variant].selected
                    : calendarStyles.variants[variant].unselected,
                className
            )}
            onClick={onClick}
        >
            <p className={clsx(text, classNameText)}>{date}</p>
            <p className={clsx(text, classNameText)}>
                {day}.{monthNumber}
            </p>
            <p className={clsx(text, classNameText)}>{year}</p>
        </div>
    );
};

export default DateItem;
