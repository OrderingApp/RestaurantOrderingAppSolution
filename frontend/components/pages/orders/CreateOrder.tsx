'use client';

import DetailsAside from '@/components/shared/asides/Details';
import Menu from '../menu/Menu';
import { CURRENCIES, SEARCH_PARAMS_NAMES } from '@/helpers/constants/constants';
import { useOrdersContext } from '@/providers/OrdersContext';
import { useState } from 'react';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { toggleQueryParam } from '@/helpers/utils/utils';
import OrderOptionsModal from '@/components/shared/modals/OrderOptionsModal';

const CreateOrder = ({ toggleModal }: { toggleModal: () => void }) => {
    const { orders } = useOrdersContext();
    const router = useRouter();
    const pathname = usePathname();
    const searchParams = useSearchParams();

    const menuItemId = searchParams.get(SEARCH_PARAMS_NAMES.MENU_ITEM_ID);

    const buttons = [
        {
            children: 'Dodaj zniżkę',
            variant: 'primary',
        },
        {
            children: 'Zatwierdź',
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

    const bill = [
        {
            id: '1st',
            name: 'Rachunek 1',
            price: orders.reduce(
                (acc, item) => acc + item.price * item.quantity,
                0
            ),
            currency: 'pln' as keyof typeof CURRENCIES,
            nestedItems: orders.map((order) => ({
                ...order,
                isServed: true,
                onClick: () => toggleSelect(order.id),
                className: menuItemId === order.id ? 'bg-red-200' : '',
            })),
        },
    ];

    const toggleOptionsModal = () => {
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.MENU_ITEM_ID,
            undefined,
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
            {menuItemId && <OrderOptionsModal onClose={toggleOptionsModal} />}
        </Menu>
    );
};

export default CreateOrder;
