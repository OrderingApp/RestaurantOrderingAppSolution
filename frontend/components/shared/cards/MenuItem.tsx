'use client';

import { useState } from 'react';
import Image from 'next/image';

import Button from '../button/Button';
import informationIcon from '@/public/images/svg/Info.svg';
import {
    CURRENCIES,
    MAX_ITEM_SELECT,
    MIN_ITEM_SELECT,
} from '@/helpers/constants/constants';
import clsx from 'clsx';
import { menuItemStyles } from '@/lib/styles/menuItem';
import { useOrdersContext } from '@/providers/OrdersContext';
import { useLanguage } from '@/providers/LanguageProvider';
import languagePacks from '@/helpers/constants/languagePacks';

interface MenuItemProps {
    id: string;
    name: string;
    price: number;
    variant: 'card' | 'order';
    handleClick: (id: string) => void;
}

const MenuItem = ({ id, name, price, variant, handleClick }: MenuItemProps) => {
    const { language } = useLanguage();
    const { createOrderPage } = languagePacks[language];
    const [inputValue, setInputValue] = useState<number | string>(1);
    const { addOrder } = useOrdersContext();

    const handleDecrease = () =>
        setInputValue((prev) => Math.max(1, +prev - 1));

    const handleIncrease = () => setInputValue((prev) => +prev + 1);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;

        if (!value) return setInputValue(value);
        setInputValue(isNaN(+value) || +value < 1 ? 1 : value);
    };

    const newItem = {
        id,
        name,
        price,
        discount: 0,
        quantity: inputValue,
        currency: 'pln',
    };

    return (
        <li
            className={clsx(
                'rounded-lg shadow-[0px_4px_4px_0px_#00000040] bg-white p-4 py-8 pb-4 relative flex flex-col',
                menuItemStyles.variants[variant]
            )}
        >
            <button onClick={() => handleClick(id)}>
                <Image
                    width={22}
                    height={22}
                    src={informationIcon}
                    alt="informationIcon"
                    className="absolute top-2 right-2 transition-transform hocus:scale-90"
                />
            </button>
            <div className="h-[60px] flex flex-col justify-center">
                <h2 className="text-center font-bold text-[1.1rem] leading-tight break-words">
                    {name}
                </h2>
                <p className="text-center text-sm text-[#2B5162] font-bold">
                    {price} {CURRENCIES.pln}
                </p>
            </div>
            {variant === 'order' && (
                <>
                    <div className="relative my-3">
                        <button
                            onClick={handleDecrease}
                            className="absolute top-1/2 -translate-y-1/2 left-1 w-4 h-4 bg-[#2B5162] rounded-full text-white flex justify-center items-center transition-transform hocus:scale-90"
                            onMouseLeave={(e) => e.stopPropagation()}
                        >
                            -
                        </button>
                        <input
                            className="px-8 bg-[#ECECEC] shadow-[0px_0px_5px_0px_#6A6A6A] rounded-md w-full text-center [&::-webkit-inner-spin-button]:appearance-none py-1"
                            type="number"
                            min={MIN_ITEM_SELECT}
                            onChange={handleChange}
                            value={inputValue}
                            max={MAX_ITEM_SELECT}
                        />
                        <button
                            onClick={handleIncrease}
                            className="absolute top-1/2 -translate-y-1/2 right-1 w-4 h-4 bg-[#2B5162] rounded-full text-white flex justify-center items-center transition-transform hocus:scale-90"
                            onMouseLeave={(e) => e.stopPropagation()}
                        >
                            +
                        </button>
                    </div>
                    <div className="flex flex-col gap-2">
                        <Button
                            className="w-full rounded-lg py-1 transition-transform hocus:scale-95"
                            onClick={() => addOrder(newItem)}
                            size="sm"
                        >
                            {createOrderPage.addOrder}
                        </Button>
                    </div>
                </>
            )}
        </li>
    );
};

export default MenuItem;
