'use client';

import { useState, useEffect, useRef, useMemo } from 'react';
import Slider, { type Settings as SliderSettings } from 'react-slick';
import 'slick-carousel/slick/slick.css';
import 'slick-carousel/slick/slick-theme.css';

import useLanguage from '@/helpers/hooks/useLanguage';

import DateItem from './components/DateItem/DateItem';
import { formatDate, generateDates } from '@/helpers/utils/dates';
import { type ComponentStyles } from '@/helpers/types/ui-types';

interface DateCalendarProps extends Partial<ComponentStyles> {
    endDateNumber?: number;
    sliderSettings?: SliderSettings;
    className?: string;
    classNameText?: string;
    onDateSelect?: (date: Date) => void;
}

/**
 * A calendar component that displays a range of dates in a slider.
 *
 * @param {DateCalendarProps} props - The component props.
 * @param {number} [props.endDateNumber=2] - The number of months to display from the current date.
 * @param {string} [props.variant='primary'] - The variant of the calendar (e.g., 'primary').
 * @param {string} [props.size='sm'] - The size of the calendar (e.g., 'sm', 'md', 'lg').
 * @param {SliderSettings} [props.sliderSettings] - Custom settings for the slider.
 * @param {string} [props.className] - Additional CSS class for the calendar container.
 * @param {string} [props.classNameText] - Additional CSS class for the text inside the calendar.
 * @param {(date: Date) => void} [props.onDateSelect] - Callback function when a date is selected.
 * @returns {JSX.Element} The DateCalendar component.
 */
const DateCalendar = ({
    endDateNumber = 2,
    variant = 'primary',
    size = 'sm',
    sliderSettings = {},
    className,
    classNameText,
    onDateSelect,
}: DateCalendarProps) => {
    const sliderRef = useRef<Slider | null>(null);
    const { language } = useLanguage();
    const [selectedDate, setSelectedDate] = useState<string>();

    const dates = useMemo(() => {
        const currentDate = new Date();
        const endDate = new Date();

        endDate.setMonth(endDate.getMonth() + endDateNumber);

        return generateDates(currentDate, endDate, language);
    }, [language, endDateNumber]);

    useEffect(() => {
        const today = new Date();
        const todayFormatted = formatDate(today, language).fullDate;

        setSelectedDate(todayFormatted);
    }, [language]);

    useEffect(() => {
        if (!sliderRef.current || !selectedDate) return;

        const index = dates.findIndex((d) => d.fullDate === selectedDate);

        if (index !== -1) sliderRef.current.slickGoTo(index);
    }, [dates, selectedDate]);

    const handleDateSelect = (
        fullDate: string,
        fullDateFormatted: Date,
        index: number
    ) => {
        sliderRef.current?.slickGoTo(index - 3);
        onDateSelect?.(fullDateFormatted);

        setSelectedDate(fullDate);
    };

    return (
        <div data-testid="date-calendar">
            <Slider
                ref={sliderRef}
                {...{ ...sliderDefaultSettings, ...sliderSettings }}
            >
                {dates.map((date, index) => (
                    <DateItem
                        key={date.fullDate}
                        {...{
                            ...date,
                            variant,
                            size,
                            className,
                            classNameText,
                        }}
                        isSelected={selectedDate === date.fullDate}
                        onClick={() =>
                            handleDateSelect(
                                date.fullDate,
                                date.fullDateFormatted,
                                index
                            )
                        }
                    />
                ))}
            </Slider>
        </div>
    );
};

const sliderDefaultSettings: SliderSettings = {
    speed: 300,
    slidesToShow: 7,
    slidesToScroll: 7,
    infinite: false,
    arrows: false,
    dots: false,
};

export default DateCalendar;

//TODO Make variants more flexible
