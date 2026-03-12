export const formatDate = (date: Date, language: string) => {
    if (isNaN(date.getTime())) {
        return {
            date: '',
            day: '',
            month: '',
            year: '',
            time: '--:--',
            fullDate: '',
            fullDateFormatted: new Date(),
            monthNumber: '',
        };
    }

    const options: Intl.DateTimeFormatOptions = {
        weekday: 'short',
        day: '2-digit',
        month: 'short',
        year: 'numeric',
        hour: 'numeric',
        minute: 'numeric',
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
        time: `${findDatePart('hour')}:${findDatePart('minute')}`,
        fullDate: fullDateFormatted,
        fullDateFormatted: new Date(date.getTime()),
        monthNumber: (date.getMonth() + 1).toString(),
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

export const checkMaxAndMinDate = () => {
    const today = new Date();
    const threeMonthsLater = new Date();
    threeMonthsLater.setMonth(today.getMonth() + 3);
    const minDateString = today.toISOString().split('T')[0];
    const maxDateString = threeMonthsLater.toISOString().split('T')[0];

    return { minDateString, maxDateString };
};

export const getFutureTime = (minutes: number): string => {
    const now = new Date();
    now.setMinutes(now.getMinutes() + minutes);
    return now.toLocaleTimeString([], {
        hour: '2-digit',
        minute: '2-digit',
        hour12: false,
    });
};

export const parseIsoDateAndTime = (isoDateString?: string | null) => {
    if (!isoDateString || !isoDateString.includes('T')) {
        return { date: '', time: '' };
    }

    try {
        const [datePart, timePartWithSeconds] = isoDateString.split('T');
        const timePart = timePartWithSeconds.slice(0, 5);

        return {
            date: datePart,
            time: timePart,
        };
    } catch (error) {
        console.error('Błąd podczas parsowania daty:', isoDateString);
        return { date: '', time: '' };
    }
};
export const getElapsedSecondsFromTimestamp = (timestamp?: string) => {
    if (!timestamp) return 0;

    const startedAt = new Date(timestamp);
    if (Number.isNaN(startedAt.getTime())) return 0;

    const elapsed = Math.floor((Date.now() - startedAt.getTime()) / 1000);
    return Math.max(0, elapsed);
};

export const getRemainingSecondsUntilTimestamp = (timestamp?: string) => {
    if (!timestamp) return 0;

    const targetDate = new Date(timestamp);
    if (Number.isNaN(targetDate.getTime())) return 0;

    const remaining = Math.floor((targetDate.getTime() - Date.now()) / 1000);
    return Math.max(0, remaining);
};

export const formatElapsedTime = (elapsedInSeconds: number) => {
    const hours = Math.floor(elapsedInSeconds / 3600)
        .toString()
        .padStart(2, '0');
    const minutes = Math.floor((elapsedInSeconds % 3600) / 60)
        .toString()
        .padStart(2, '0');
    const seconds = Math.floor(elapsedInSeconds % 60)
        .toString()
        .padStart(2, '0');

    return `${hours}:${minutes}:${seconds}`;
};

export const formatReservationHour = (dateValue?: string) => {
    if (!dateValue) return '--:--';

    const date = new Date(dateValue);
    if (Number.isNaN(date.getTime())) return '--:--';

    return new Intl.DateTimeFormat(undefined, {
        hour: '2-digit',
        minute: '2-digit',
    }).format(date);
};
