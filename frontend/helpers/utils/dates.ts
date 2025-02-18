export const formatDate = (date: Date, language: string) => {
    const options: Intl.DateTimeFormatOptions = {
        weekday: 'short',
        day: '2-digit',
        month: 'short',
        year: 'numeric',
    };
    const formatter = new Intl.DateTimeFormat(language, options);
    const formattedDate = formatter.formatToParts(date);

    const findDatePart = (type: keyof Intl.DateTimeFormatPartTypesRegistry) =>
        formattedDate.find((part) => part.type === type)?.value || '';

    const fullDateFormatted = formatter.format(date).replace(/\s/g, '');

    return {
        date: findDatePart('weekday'),
        day: findDatePart('day'),
        month: findDatePart('month'),
        year: findDatePart('year'),
        fullDate: fullDateFormatted,
        fullDateFormatted: new Date(date.getTime()),
    };
};

export const generateDates = (
    startDate: Date,
    endDate: Date,
    language: string
) => {
    const generatedDates = [];
    const currentDate = new Date(startDate);

    while (currentDate <= endDate) {
        generatedDates.push(formatDate(currentDate, language));
        currentDate.setDate(currentDate.getDate() + 1);
    }

    return generatedDates;
};
