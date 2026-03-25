'use client';

import { useState } from 'react';

import Button from '../button/Button';
import Input from '../Input/Input';

import useLanguage from '@/helpers/hooks/useLanguage';
import languagePacks from '@/helpers/constants/languagePacks';

import { DISCOUNTS } from '@/helpers/constants/constants';

const Discount = ({ onClick }: { onClick: () => void }) => {
    const { language } = useLanguage();
    const [isCustomDiscount, setIsCustomDiscount] = useState(false);
    const [discount, setDiscount] = useState('');
    const {
        discountModal: { disscountTitle },
    } = languagePacks[language];

    return (
        <div className="w-[720px] h-[377px] rounded-md bg-[#F8F8F8] p-4 py-7 relative flex flex-col justify-between items-center">
            <button
                onClick={onClick}
                className="absolute top-5 right-5 text-xl"
            >
                X
            </button>
            <h2 className="text-2xl font-bold text-center">{disscountTitle}</h2>
            {!isCustomDiscount && (
                <ul className="flex justify-between w-full px-10 gap-4">
                    {DISCOUNTS.map((discount) => (
                        <li key={discount.id}>
                            <button
                                onClick={() => setDiscount(discount.id)}
                                className="w-24 h-24 bg-white shadow-[0_0_5px_rgba(0,0,0,0.22)] rounded-lg font-bold text-2xl"
                            >
                                {discount.name}
                            </button>
                        </li>
                    ))}
                    <li>
                        <button
                            onClick={() => setIsCustomDiscount(true)}
                            className="w-24 h-24 bg-white shadow-[0_0_5px_rgba(0,0,0,0.22)] rounded-lg font-bold text-2xl"
                        >
                            ...
                        </button>
                    </li>
                </ul>
            )}

            {isCustomDiscount && (
                <Input
                    type="number"
                    value={discount}
                    inputClassName="py-2 w-full"
                    variant="secondary"
                    onChange={(e) => setDiscount(e.target.value)}
                />
            )}

            <Button>{disscountTitle}</Button>
        </div>
    );
};

export default Discount;
