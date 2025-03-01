import clsx from 'clsx';

import { type ComponentStyles } from '@/helpers/types/ui-types';
import { calendarStyles } from '@/lib/styles/calendar';

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
    const { container, text } = calendarStyles.sizes[size];

    return (
        <div
            data-testid={`date-${fullDate}`}
            className={clsx(
                'rounded-md cursor-pointer text-center',
                container,
                isSelected
                    ? calendarStyles.variants[variant].selected
                    : calendarStyles.variants[variant].unselected,
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
