'use client';

import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { useState } from 'react';

import useLanguage from '@/helpers/hooks/useLanguage';

import Button from '@/components/shared/button/Button';
import DateCalendar from '@/components/shared/DateCalendar/DateCalendar';
import ReservationCard from '@/components/shared/cards/ReservationCard';
import languagePacks from '@/helpers/constants/languagePacks';
import Image from 'next/image';
import SearchInput from '@/components/shared/Input/SearchInput';
import { ICONS } from '@/helpers/constants/icons/icons';
import { toggleQueryParam } from '@/helpers/utils/utils';
import { SEARCH_PARAMS_NAMES } from '@/helpers/constants/constants';
import useFilterReservations from '@/helpers/hooks/useFilterReservations';

const Reservations = () => {
    const router = useRouter();
    const searchParams = useSearchParams();
    const pathname = usePathname();

    const { language } = useLanguage();
    const [selectedDate, setSelectedDate] = useState(new Date().toISOString());
    const { filteredReservations } = useFilterReservations(selectedDate);

    const {
        reservationsPage: { reservationsList, createReservation },
    } = languagePacks[language];

    const buttons = [
        {
            value: 'time',
            iconActive: ICONS.TIME_WHITE,
            icon: ICONS.TIME,
        },

        {
            value: 'guests',
            iconActive: ICONS.USER_WHITE,
            icon: ICONS.USER,
        },
    ];

    const isActive = (value: string) => {
        const param = searchParams.get(SEARCH_PARAMS_NAMES.FILTER_BY);
        if (!param || undefined) return 'time';
        if (param === value) return value;
    };

    return (
        <section className="p-4 px-10 relative h-full">
            <header className="flex justify-between items-center py-4">
                <h1 className="text-black text-4xl font-bold py-5 capitalize">
                    {reservationsList}
                </h1>
                <Button
                    onClick={() => router.push('/reservations/create')}
                    className="px-8 max-w-52"
                    size="md"
                >
                    {createReservation}
                </Button>
            </header>

            <div className="flex justify-around w-full mt-6 h-auto">
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
                <ul className="flex gap-4 justify-between px-2 py-3 flex-wrap">
                    {filteredReservations?.map((reservation) => (
                        <ReservationCard
                            key={reservation.id}
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
        </section>
    );
};

export default Reservations;
