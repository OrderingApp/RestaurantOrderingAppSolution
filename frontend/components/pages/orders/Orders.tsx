'use client';

import Button from '@/components/shared/Button/Button';
import OrderCard from '@/components/shared/cards/OrderCard';
import ToggleSwitch from '@/components/shared/toggleSwitch/ToggleSwitch';

const Orders = () => {
    return (
        <div className="flex flex-col h-full">
            <div className="flex m-4 justify-between">
                <ToggleSwitch />

                <Button className="w-52 rounded-xl" variant="primary" size="xl">
                    Utwórz
                </Button>
            </div>
            <div className="flex justify-around w-full mt-20 h-full">
                <div className="flex-1 text-center text-5xl">
                    <span className="text-center">Otwarte</span>
                    <ul className="flex justify-evenly mt-5">
                        <OrderCard
                            type="pickup"
                            time="20:30"
                            price="20"
                            phoneNumber="555-321-321"
                            status="active"
                        />
                    </ul>
                </div>
                <hr className="w-1 h-full bg-black" />
                <div className="flex-1 text-center text-5xl">
                    <span>Zamknięte</span>
                    <ul className="flex justify-evenly mt-5">
                        <OrderCard
                            type="pickup"
                            time="20:30"
                            price="20"
                            phoneNumber="555-321-321"
                            status="completed"
                        />
                    </ul>
                </div>
            </div>
        </div>
    );
};

export default Orders;
