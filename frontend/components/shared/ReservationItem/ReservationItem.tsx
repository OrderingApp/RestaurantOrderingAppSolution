import React from 'react';

const ReservationItem = () => {
    return (
        <div className="w-36 h-28 border border-black rounded-lg overflow-hidden relative">
            <div className="bg-[#2C5364] absolute left-[-2px] top-0 w-[146px] flex justify-between items-center px-2 h-8 border-b border-l border-r border-black text-white rounded-b-[10px]">
                <p className="text-[12px]">Rezerwacja</p>
                <p className="text-[12px]">14:30</p>
            </div>

            <div className="h-full bg-[#F5F5F5E5] bg-opacity-90 p-2 mt-8">
                <p className="text-sm ">Jan Kowalski</p>
                <p className="text-sm">4 osoby</p>
            </div>
        </div>
    );
};

export default ReservationItem;
