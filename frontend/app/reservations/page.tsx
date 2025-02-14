import ReservationForm from '@/components/reservations/ReservationForm';

import React from 'react';

const Reservations = () => {
    return (
        <section className="flex justify-center items-center min-h-screen min-w-screen">
            <div className="w-[95%] min-h-screen py-3 flex rounded-lg">
                <aside className="w-[120px] min-h-full bg-blue-100"></aside>
                <div className="w-full  flex">
                    <ReservationForm />
                    <div className="w-1/2 bg-blue-100"></div>
                </div>
            </div>
        </section>
    );
};

export default Reservations;
