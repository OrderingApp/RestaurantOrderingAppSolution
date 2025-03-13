'use client';

import { useState } from 'react';
import Image from 'next/image';

import Button from '../Button/Button';
import informationIcon from '@/public/images/svg/Info.svg';

interface MenuItemProps {
    name: string;
    price: number;
}

const MenuItem = ({ name, price }: MenuItemProps) => {
    const [inputValue, setInputValue] = useState<number | string>(1);

    const handleDecrease = () => {
        setInputValue((prev) => Math.max(1, +prev - 1));
    };

    const handleIncrease = () => {
        setInputValue((prev) => +prev + 1);
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;

        if (value === '') {
            setInputValue(value);
            return;
        }

        const numericValue = parseInt(value, 10);
        setInputValue(
            isNaN(numericValue) || numericValue < 1 ? 1 : numericValue
        );
    };

    return (
        <div className="w-52 min-h-56 rounded-lg shadow-[0px_4px_4px_0px_#00000040] bg-white p-4 py-8 pb-4 relative flex flex-col justify-between">
            <Image
                width={22}
                height={22}
                src={informationIcon}
                alt="informationIcon"
                className="absolute top-2 right-2"
            />
            <div>
                <h2 className="text-center text-[1.3rem] font-bold">{name}</h2>
                <p className="text-center text-sm text-[#2B5162] font-bold">
                    {price}zł
                </p>
            </div>
            <label className="relative my-3">
                <button
                    onClick={handleDecrease}
                    className="absolute top-1 left-1 w-4 h-4 bg-[#2B5162] rounded-full text-white flex justify-center items-center"
                >
                    <span>-</span>
                </button>
                <input
                    className="bg-[#ECECEC] shadow-[0px_0px_5px_0px_#6A6A6A] rounded-md w-full text-center [&&::-webkit-inner-spin-button]:appearance-none"
                    type="number"
                    name=""
                    id=""
                    min={1}
                    onChange={handleChange}
                    value={inputValue}
                    max={99}
                />
                <button
                    onClick={handleIncrease}
                    className="absolute top-1 right-1 w-4 h-4 bg-[#2B5162] rounded-full text-white flex justify-center items-center"
                >
                    +
                </button>
            </label>
            <div className="flex flex-col gap-2">
                <Button className="w-full rounded-lg py-4" size="md">
                    Dodaj
                </Button>
            </div>
        </div>
    );
};

export default MenuItem;
