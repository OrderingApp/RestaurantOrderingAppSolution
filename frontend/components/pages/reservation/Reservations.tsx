'use client';

import { useRouter } from 'next/navigation';
import { useState } from 'react';

import useLanguage from '@/helpers/hooks/useLanguage';
import useQueryReservations from '@/helpers/queries/reservations/useQueryReservations';

import Button from '@/components/shared/Button/Button';
import DateCalendar from '@/components/shared/DateCalendar/DateCalendar';
import { ReservationCard } from '@/components/shared/cards/ReservationCard';
import languagePacks from '@/helpers/constants/languagePacks';

const Reservations = () => {
    const router = useRouter();
    const { language } = useLanguage();
    const [selectedDate, setSelectedDate] = useState(
        new Date().toISOString().split('T')[0]
    );
    const { data } = useQueryReservations(selectedDate);

    const {
        reservationsPage: {
            reservationTitle,
            upsertReservation,
            listOfReservations,
        },
    } = languagePacks[language];

    const onDateSelectHandler = (date: Date) => {
        const formattedDate = date.toISOString().split('T')[0];
        setSelectedDate(formattedDate);
    };

    return (
        <section className="py-3">
            <header className="flex justify-between items-center px-10 py-4">
                <h1 className="text-black text-4xl font-bold py-5 capitalize">
                    {reservationTitle}
                </h1>
                <Button
                    onClick={() => router.push('/reservations/create')}
                    className="px-8 max-w-52"
                    size="md"
                >
                    {upsertReservation}
                </Button>
            </header>

            <div className="pb-4">
                <DateCalendar onDateSelect={onDateSelectHandler} />
            </div>

            <main className="flex flex-col">
                <h2 className="text-black text-3xl font-bold py-5 pl-10">
                    {listOfReservations}
                </h2>
                <div className="flex gap-4 p-3">
                    {data?.map((reservation) => (
                        <ReservationCard
                            key={reservation.id}
                            {...reservation}
                        />
                    ))}
                </div>
            </main>
        </section>
    );
};

export default Reservations;

//TODO maybe ul list idk
