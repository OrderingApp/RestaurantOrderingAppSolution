'use client';

import { useState } from 'react';
import Image from 'next/image';

import Button from '../Button/Button';
import informationIcon from '@/public/images/svg/Info.svg';
import {
    CURRENCIES,
    MAX_ITEM_SELECT,
    MIN_ITEM_SELECT,
} from '@/helpers/constants/constants';
import clsx from 'clsx';
import { menuItemStyles } from '@/lib/styles/menuItem';

interface MenuItemProps {
    id: string;
    name: string;
    price: number;
    variant: 'card' | 'order';
    handleClick: (id: string) => void;
}

const MenuItem = ({ id, name, price, variant, handleClick }: MenuItemProps) => {
    const [inputValue, setInputValue] = useState<number | string>(1);

    const handleDecrease = () =>
        setInputValue((prev) => Math.max(1, +prev - 1));

    const handleIncrease = () => setInputValue((prev) => +prev + 1);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;

        if (!value) return setInputValue(value);
        setInputValue(isNaN(+value) || +value < 1 ? 1 : value);
    };

    return (
        <li
            className={clsx(
                'rounded-lg shadow-[0px_4px_4px_0px_#00000040] bg-white p-4 py-8 pb-4 relative flex flex-col',
                menuItemStyles.variants[variant]
            )}
        >
            <Image
                onClick={() => handleClick(id)}
                width={22}
                height={22}
                src={informationIcon}
                alt="informationIcon"
                className="absolute top-2 right-2"
            />
            <div>
                <h2 className="text-center text-[1.3rem] font-bold">{name}</h2>
                <p className="text-center text-sm text-[#2B5162] font-bold">
                    {price} {CURRENCIES.pln}
                </p>
            </div>
            {variant === 'order' && (
                <>
                    <label className="relative my-3">
                        <button
                            onClick={handleDecrease}
                            className="absolute top-1 left-1 w-4 h-4 bg-[#2B5162] rounded-full text-white flex justify-center items-center"
                        >
                            -
                        </button>
                        <input
                            className="bg-[#ECECEC] shadow-[0px_0px_5px_0px_#6A6A6A] rounded-md w-full text-center [&&::-webkit-inner-spin-button]:appearance-none"
                            type="number"
                            min={MIN_ITEM_SELECT}
                            onChange={handleChange}
                            value={inputValue}
                            max={MAX_ITEM_SELECT}
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
                </>
            )}
        </li>
    );
};

export default MenuItem;
