'use client';

import Image from 'next/image';

import informationIcon from '@/public/images/svg/Info.svg';
import { CURRENCIES } from '@/helpers/constants/constants';

import { useOrdersContext } from '@/providers/OrdersContext';

export type AddItemHandler = (item: {
    id: string;
    name: string;
    price: number;
    discount: number;
    quantity: number;
    currency: 'pln';
}) => void;

interface MenuItemProps {
    id: string;
    name: string;
    price: number;
    onOpenMenuItemInformation: (id: string) => void;
    onAddItem?: AddItemHandler;
}

const MenuItem = ({
    id,
    name,
    price,
    onOpenMenuItemInformation,
    onAddItem,
}: MenuItemProps) => {
    const { addOrder } = useOrdersContext();

    const newItem = {
        id,
        name,
        price,
        discount: 0,
        quantity: 1,
        currency: 'pln',
    } as const;

    const handleAddItem = () => {
        // If onAddItem prop is provided, use it (for CreateOrder modal)
        // Otherwise fall back to global orders context
        if (onAddItem) {
            onAddItem(newItem);
        } else {
            addOrder(newItem);
        }
    };

    return (
        <li
            onClick={handleAddItem}
            className="rounded-lg shadow-[0px_4px_4px_0px_#00000040] bg-white p-4 py-5 pb-4 relative flex flex-col min-h-32 justify-center"
        >
            <button onClick={() => onOpenMenuItemInformation(id)}>
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
        </li>
    );
};

export default MenuItem;
