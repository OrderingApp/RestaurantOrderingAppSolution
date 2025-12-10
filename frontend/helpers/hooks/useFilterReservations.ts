import { useSearchParams } from 'next/navigation';
import { SEARCH_PARAMS_NAMES } from '../constants/constants';
import useQueryReservations from '../queries/reservations/useQueryReservations';
import { formatDate } from '../utils/dates';
import useLanguage from './useLanguage';

const useFilterReservations = (date: string) => {
    const { data } = useQueryReservations(date);
    const searchParams = useSearchParams();
    const page = searchParams.get(SEARCH_PARAMS_NAMES.PAGE);
    const name = searchParams.get(SEARCH_PARAMS_NAMES.NAME);
    const filterBy = searchParams.get(SEARCH_PARAMS_NAMES.FILTER_BY);

    const { language } = useLanguage();

    let filteredReservations = data || [];
    const totalItems = filteredReservations.length;
    const itemsPerPage = 8;

    if (filterBy === 'time') {
        filteredReservations = filteredReservations.sort((a, b) => {
            const { time: aTime } = formatDate(
                new Date(a.scheduledFor),
                language
            );
            const { time: bTime } = formatDate(
                new Date(b.scheduledFor),
                language
            );
            const toMinutes = (t: string) => {
                const [h, m] = t.split(':').map(Number);
                return h * 60 + m;
            };

            return toMinutes(aTime) - toMinutes(bTime);
        });
    }

    if (filterBy === 'guests') {
        filteredReservations = filteredReservations.sort(
            (a, b) => b.capacityNeeded - a.capacityNeeded
        );
    }

    if (name) {
        filteredReservations = filteredReservations.filter((item) =>
            item.phoneNumber.toLowerCase().includes(name.toLowerCase())
        );
    }

    if (page) {
        const pageNumber = parseInt(page, 10);
        const startIndex = (pageNumber - 1) * itemsPerPage;
        const endIndex = startIndex + itemsPerPage;
        filteredReservations = filteredReservations.slice(startIndex, endIndex);
    } else {
        filteredReservations = filteredReservations.slice(0, itemsPerPage);
    }

    return {
        filteredReservations,
        totalItems,
        itemsPerPage,
    };
};

export default useFilterReservations;
