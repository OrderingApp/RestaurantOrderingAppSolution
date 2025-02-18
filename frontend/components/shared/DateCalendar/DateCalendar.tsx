'use client';

import Slider from 'react-slick';
import 'slick-carousel/slick/slick.css';
import 'slick-carousel/slick/slick-theme.css';
import clsx from 'clsx';

import React, { useState, useEffect, useRef } from 'react';

interface DateCalendarProps {
    language?: string;
    endDateNumber?: number;
    variant?: 'primary';
    size?: 'sm' | 'md' | 'lg';
    sliderSettings?: object;
    className?: string;
    classNameText?: string;
    onDateSelect?: (date: Date) => void;
}

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

const sliderDefaultSettings = {
    speed: 300,
    slidesToShow: 7,
    slidesToScroll: 7,
    infinite: false,
    arrows: false,
    dots: false,
};

const DateCalendar = ({
    language = 'pl',
    endDateNumber = 2,
    variant = 'primary',
    size = 'sm',
    sliderSettings = sliderDefaultSettings,
    className,
    classNameText,
    onDateSelect,
}: DateCalendarProps) => {
    const sliderRef = useRef<Slider | null>(null);
    const [selectedDate, setSelectedDate] = useState<string | null>(null);
    const [dates, setDates] = useState<
        {
            date: string;
            day: string;
            month: string;
            year: string;
            fullDate: string;
            fullDateFormatted: Date;
        }[]
    >([]);

    useEffect(() => {
        const startDate = new Date();
        const endDate = new Date();

        endDate.setMonth(endDate.getMonth() + endDateNumber);

        const generatedDates = [];
        const currentDate = new Date(startDate);

        while (currentDate <= endDate) {
            const options: Intl.DateTimeFormatOptions = {
                weekday: 'short',
                day: '2-digit',
                month: 'short',
                year: 'numeric',
            };
            const formatter = new Intl.DateTimeFormat(language, options);
            const formattedDate = formatter.formatToParts(currentDate);

            const findDatePart = (
                type: keyof Intl.DateTimeFormatPartTypesRegistry
            ) => formattedDate.find((part) => part.type === type)?.value || '';
            const fullDateFormatted = formatter
                .format(currentDate)
                .replace(/\s/g, '');
            const dateObj = {
                date: findDatePart('weekday'),
                day: findDatePart('day'),
                month: findDatePart('month'),
                year: findDatePart('year'),
                fullDate: fullDateFormatted,
                fullDateFormatted: new Date(currentDate.getTime()),
            };

            generatedDates.push(dateObj);
            currentDate.setDate(currentDate.getDate() + 1);
        }

        setDates(generatedDates);

        const today = new Date();
        const todayFormatted = `${today.getDate()}${today.toLocaleString(language, { month: 'short' })}${today.getFullYear()}`;
        const index = generatedDates.findIndex(
            (d) => d.fullDate === todayFormatted
        );

        if (index !== -1) {
            setSelectedDate(todayFormatted);
            setTimeout(() => sliderRef.current?.slickGoTo(index), 100);
        }
    }, [language, endDateNumber]);

    const handleDateSelect = (
        fullDate: string,
        fullDateFormatted: Date,
        index: number
    ) => {
        setSelectedDate(fullDate);
        sliderRef.current?.slickGoTo(index);
        onDateSelect?.(fullDateFormatted);
    };

    return (
        <div data-testid="date-calendar">
            <Slider ref={sliderRef} {...sliderSettings}>
                {dates.map((date, index) => (
                    <div
                        data-testid={`date-${date.fullDate}`}
                        key={date.fullDate}
                        className={clsx(
                            'rounded-md cursor-pointer text-center',
                            sizeClasses[size].container,
                            selectedDate === date.fullDate
                                ? variantStyles[variant].selected
                                : variantStyles[variant].unselected,
                            className
                        )}
                        onClick={() =>
                            handleDateSelect(
                                date.fullDate,
                                date.fullDateFormatted,
                                index
                            )
                        }
                    >
                        <p
                            className={clsx(
                                sizeClasses[size].text,
                                classNameText
                            )}
                        >
                            {date.date}
                        </p>
                        <p
                            className={clsx(
                                sizeClasses[size].text,
                                classNameText
                            )}
                        >
                            {date.day}
                        </p>
                        <p
                            className={clsx(
                                sizeClasses[size].text,
                                classNameText
                            )}
                        >
                            {date.month} {date.year}
                        </p>
                    </div>
                ))}
            </Slider>
        </div>
    );
};

export default DateCalendar;

//TODO Make variants more flexible
