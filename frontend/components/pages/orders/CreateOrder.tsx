'use client';

import DetailsAside from '@/components/shared/asides/Details';
import Menu from '../menu/Menu';
import { CURRENCIES, SEARCH_PARAMS_NAMES } from '@/helpers/constants/constants';
import { useOrdersContext } from '@/providers/OrdersContext';
import { useState } from 'react';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { toggleQueryParam } from '@/helpers/utils/utils';
import { ItemProps } from '@/components/shared/lists/Items/Item';
import { ButtonProps } from '@/components/shared/button/Button';
import { Currency } from '@/helpers/type/types';

export interface BillProps {
    id: string;
    name: string;
    price: number;
    currency: Currency;
    nestedItems: ItemProps[];
}

const CreateOrder = ({ toggleModal }: { toggleModal: () => void }) => {
    const { orders } = useOrdersContext();
    const router = useRouter();
    const pathname = usePathname();
    const searchParams = useSearchParams();
    const menuItemId = searchParams.get(SEARCH_PARAMS_NAMES.MENU_ITEM_ID);
    
    const buttons: ButtonProps[] = [
        {
            children: 'Dodaj zniżkę',
            variant: 'primary',
        },
        {
            children: 'Zatwierdź',
            onClick: () => toogleUserDataModal(),
            variant: 'primary',
        },
        {
            children: 'Zamknij bez zmian',
            onClick: () => toggleModal(),
            variant: 'tertiary',
        },
    ];

    const [selectedId, setSelectedId] = useState<string>('');

    const toggleSelect = (id: string) => {
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.MENU_ITEM_ID,
            id,
            searchParams,
            router,
            pathname
        );

        if (selectedId.includes(id)) {
            setSelectedId('');
            return;
        }
        setSelectedId(id);
    };

    const bill: BillProps[] = [
        {
            id: '1st',
            name: 'Rachunek 1',
            price: orders.reduce(
                (acc, item) => acc + item.price * item.quantity,
                0
            ),
            currency: 'pln' as keyof typeof CURRENCIES,
            nestedItems: orders.map((order) => ({
                name: order.name,
                price: order.price,
                currency: 'pln',
                quantity: order.quantity,
                onClick: () => toggleSelect(order.id),
                className: menuItemId === order.id ? 'bg-red-200' : '',
            })),
        },
    ];

    const toogleUserDataModal = () => {
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.USER_DATA,
            'true',
            searchParams,
            router,
            pathname
        );
    };

    return (
        <Menu variant="order">
            <DetailsAside
                title={'Zamówienie'}
                items={bill}
                price={3}
                currency="pln"
                buttons={buttons}
            />
        </Menu>
    );
};

export default CreateOrder;
