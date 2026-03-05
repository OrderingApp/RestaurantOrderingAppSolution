'use client';

import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { useState } from 'react';
import Image from 'next/image';

import useLanguage from '@/helpers/hooks/useLanguage';
import useFilterReservations from '@/helpers/hooks/useFilterReservations';

import { PaginationWithLinks } from '@/components/ui/pagination-with-links';

import Button from '@/components/shared/button/Button';
import DateCalendar from '@/components/shared/DateCalendar/DateCalendar';
import ReservationCard from '@/components/shared/cards/ReservationCard';
import SearchInput from '@/components/shared/Input/SearchInput';
import Modal from '@/components/shared/modals/Modal';
import UpsertReservation from '@/components/shared/modals/UpsertReservation';

import languagePacks from '@/helpers/constants/languagePacks';
import { ICONS } from '@/helpers/constants/icons/icons';
import { setQueryParams, toggleQueryParam } from '@/helpers/utils/utils';
import { SEARCH_PARAMS_NAMES } from '@/helpers/constants/constants';
import { useReservationContext } from '@/providers/ReservationsContext';

const Reservations = () => {
    const router = useRouter();
    const searchParams = useSearchParams();
    const pathname = usePathname();

    const [selectedDate, setSelectedDate] = useState(new Date().toISOString());

    const {
        filteredReservations,
        totalItems,
        ITEMS_PER_PAGE: itemsPerPage,
        totalPages,
    } = useFilterReservations(selectedDate);
    const { language } = useLanguage();

    const modal = searchParams.get(SEARCH_PARAMS_NAMES.MODAL);
    const page = searchParams.get(SEARCH_PARAMS_NAMES.PAGE);

    const {
        reservationsPage: { reservationsList, createReservation },
    } = languagePacks[language];

    const { hasUnsavedChanges, clearLocalReservations } =
        useReservationContext();

    const buttons = [
        {
            value: 'time',
            iconActive: ICONS.TIME_WHITE,
            icon: ICONS.TIME,
        },

        {
            value: 'occupatied',
            iconActive: ICONS.TABLE_WHITE_OCCUPIED,
            icon: ICONS.TABLE_OCCUPIED,
        },
        {
            value: 'free',
            iconActive: ICONS.FREE_TABLE_WHITE,
            icon: ICONS.FREE_TABLE,
        },
    ];

    const isActive = (value: string) => {
        const param = searchParams.get(SEARCH_PARAMS_NAMES.FILTER_BY);
        if (!param || undefined) return 'time';
        if (param === value) return value;
    };

    const toggleModal = () => {
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.MODAL,
            'true',
            searchParams,
            router,
            pathname
        );
    };

    const editReservationHandler = (id: string) => {
        setQueryParams(
            {
                [SEARCH_PARAMS_NAMES.MODAL]: 'true',
                [SEARCH_PARAMS_NAMES.RESERVATION]: id,
            },
            searchParams,
            router,
            pathname
        );
    };

    const closeModal = () => {
        setQueryParams(
            {
                [SEARCH_PARAMS_NAMES.MODAL]: undefined,
                [SEARCH_PARAMS_NAMES.RESERVATION]: undefined,
            },
            searchParams,
            router,
            pathname
        );
    };

    const handleSafeClose = () => {
        if (hasUnsavedChanges) {
            const confirmed = window.confirm(
                'Masz przypisane rezerwacje, które nie zostały jeszcze wysłane do bazy. Czy na pewno chcesz zamknąć okno i utracić te dane?'
            );
            if (!confirmed) return;
        }

        closeModal();
        clearLocalReservations();
    };

    return (
        <section className="p-4 px-10 relative h-full">
            <header className="flex justify-between items-center py-4">
                <h1 className="text-black text-4xl font-bold py-5 capitalize">
                    {reservationsList}
                </h1>
                <Button
                    onClick={toggleModal}
                    className="px-8 max-w-52"
                    size="md"
                >
                    {createReservation}
                </Button>
            </header>

            <div className="flex justify-around w-full mt-6 h-auto ">
                <div className="flex items-start justify-between w-full">
                    <div className="flex gap-4 mb-4 w-2/5 ">
                        {buttons.map((btn) => (
                            <button
                                className={`${isActive(btn.value) === btn.value ? 'bg-primary' : 'bg-[#F6F6F6]'} p-3 rounded-lg shadow-xl`}
                                onClick={() =>
                                    toggleQueryParam(
                                        SEARCH_PARAMS_NAMES.FILTER_BY,
                                        btn.value,
                                        searchParams,
                                        router,
                                        pathname
                                    )
                                }
                                key={btn.value}
                            >
                                <Image
                                    src={
                                        isActive(btn.value) === btn.value
                                            ? btn.iconActive
                                            : btn.icon
                                    }
                                    alt="iconList"
                                />
                            </button>
                        ))}
                    </div>
                    <div className="w-3/5 mr-5">
                        <SearchInput
                            placeholder="Wyszukaj"
                            className="w-full"
                        />
                    </div>
                </div>
            </div>

            <main className="flex flex-col">
                <ul className="flex gap-x-8 gap-y-4  px-2 py-3 flex-wrap">
                    {filteredReservations?.map((reservation) => (
                        <ReservationCard
                            key={reservation.id}
                            onClick={() =>
                                editReservationHandler(reservation.id)
                            }
                            {...reservation}
                        />
                    ))}
                </ul>
            </main>
            <div className="absolute bottom-0 left-0 w-full">
                <DateCalendar
                    onDateSelect={(date) => setSelectedDate(date.toISOString())}
                />
            </div>

            <Modal isOpen={modal === 'true'} onClose={handleSafeClose}>
                <UpsertReservation onClose={handleSafeClose} />
            </Modal>

            {totalPages > 1 && (
                <div
                    className={`absolute bottom-20 left-1/2  -translate-x-1/2`}
                >
                    <PaginationWithLinks
                        page={page ? parseInt(page, 10) : 1}
                        pageSize={itemsPerPage || 12}
                        totalCount={totalItems || 0}
                        navigationMode="router"
                    />
                </div>
            )}
        </section>
    );
};

export default Reservations;
