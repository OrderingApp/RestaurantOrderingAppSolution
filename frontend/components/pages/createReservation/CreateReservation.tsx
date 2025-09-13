'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
// test
import useLanguage from '@/helpers/hooks/useLanguage';
import useQueryReservations from '@/helpers/queries/reservations/useQueryReservations';

import ReservationForm from '@/components/reservations/ReservationForm';
import DateCalendar from '@/components/shared/DateCalendar/DateCalendar';
import ReservationCard from '@/components/shared/cards/ReservationCard';

import languagePacks from '@/helpers/constants/languagePacks';

const CreateReservation = () => {
    const router = useRouter();
    const { language } = useLanguage();
    const [selectedId, setSelectedId] = useState<string | null>(null);
    const [selectedDate, setSelectedDate] = useState<string>(
        new Date().toISOString()
    );

    const { data } = useQueryReservations(selectedDate);
    const {
        createReservationPage: { chooseReservation },
    } = languagePacks[language];

    const onDateSelectHandler = (date: Date) => {
        const formattedDate = date.toISOString();
        setSelectedDate(formattedDate);
    };

    const toggleSelected = (id: string) => {
        if (selectedId === id) {
            setSelectedId(null);
            router.push('/reservations/create');
        } else {
            setSelectedId(id);
            router.push(`/reservations/create?edit=${id}`);
        }
    };

    return (
        <section className="min-h-full flex">
            <ReservationForm />
            <div className="w-1/2 py-8">
                <h1 className="text-center text-black text-4xl font-bold py-5 capitalize">
                    {chooseReservation}
                </h1>
                <div className="py-2">
                    <DateCalendar
                        sliderSettings={{ slidesToShow: 5, slidesToScroll: 5 }}
                        onDateSelect={onDateSelectHandler}
                    />
                </div>
                <div className="flex gap-3 p-4 flex-wrap">
                    {data?.map((reservation) => (
                        <ReservationCard
                            onClick={() => toggleSelected(reservation.id)}
                            key={reservation.id}
                            {...reservation}
                            className={
                                selectedId == reservation.id ? 'scale-110' : ''
                            }
                        />
                    ))}
                </div>
            </div>
        </section>
    );
};

export default CreateReservation;

//TODO maybe ul idk
