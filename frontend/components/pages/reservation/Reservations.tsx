'use client';

import { useRouter } from 'next/navigation';

import useLanguage from '@/helpers/hooks/useLanguage';

import Button from '@/components/shared/Button/Button';
import DateCalendar from '@/components/shared/DateCalendar/DateCalendar';
import languagePacks from '@/helpers/constants/languagePacks';
import ReservationItem from '@/components/shared/ReservationItem/ReservationItem';

const Reservations = () => {
    const { language } = useLanguage();
    const router = useRouter();

    const {
        reservationsPage: {
            reservationTitle,
            upsertReservation,
            listOfReservations,
        },
    } = languagePacks[language];

    const handleDateSelect = (date: Date) => {
        console.log(date);
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
                <DateCalendar onDateSelect={handleDateSelect} />
            </div>

            <main className="flex flex-col">
                <h2 className="text-black text-3xl font-bold py-5 pl-10">
                    {listOfReservations}
                </h2>
                <div className="flex gap-4 p-3">
                    <ReservationItem />
                    <ReservationItem />
                    <ReservationItem />
                </div>
            </main>
        </section>
    );
};

export default Reservations;
