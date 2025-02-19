'use client';

import useLanguage from '@/helpers/hooks/useLanguage';

import ReservationForm from '@/components/reservations/ReservationForm';
import DateCalendar from '@/components/shared/DateCalendar/DateCalendar';
import languagePacks from '@/helpers/constants/languagePacks';
import ReservationItem from '@/components/shared/ReservationItem/ReservationItem';

const CreateReservation = () => {
    const { language } = useLanguage();
    const {
        createReservationPage: { chooseReservation },
    } = languagePacks[language];

    return (
        <section className="min-h-full flex">
            <ReservationForm />
            <div className="w-1/2 py-8">
                <h1 className="text-center text-black text-4xl font-bold py-5 capitalize">
                    {chooseReservation}
                </h1>
                <div className="py-2">
                    <DateCalendar
                        size="sm"
                        variant="primary"
                        sliderSettings={{ slidesToShow: 5, slidesToScroll: 5 }}
                    />
                </div>
                <div className="flex gap-1 p-2">
                    <ReservationItem />
                    <ReservationItem />
                    <ReservationItem />
                </div>
            </div>
        </section>
    );
};

export default CreateReservation;
