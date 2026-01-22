'use client';

import DetailsAside from '@/components/shared/asides/Details';
import Menu from '../menu/Menu';
import { CURRENCIES, SEARCH_PARAMS_NAMES } from '@/helpers/constants/constants';
import { useOrdersContext } from '@/providers/OrdersContext';
import { useState } from 'react';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { toggleQueryParam } from '@/helpers/utils/utils';
import CustomerInformationForm from './CustomerInformationForm';
import { ItemProps } from '@/components/shared/lists/Items/Item';
import { ButtonProps } from '@/components/shared/button/Button';
import { Currency } from '@/helpers/type/types';
import { useLanguage } from '@/providers/LanguageProvider';
import languagePacks from '@/helpers/constants/languagePacks';

export interface BillProps {
    id: string;
    name: string;
    price: number;
    currency: Currency;
    nestedItems: ItemProps[];
}

const CreateOrder = ({ toggleModal }: { toggleModal: () => void }) => {
    const { orders, clearOrders } = useOrdersContext();
    const router = useRouter();
    const pathname = usePathname();
    const searchParams = useSearchParams();
    const menuItemId = searchParams.get(SEARCH_PARAMS_NAMES.MENU_ITEM_ID);
    const userData = searchParams.get(SEARCH_PARAMS_NAMES.USER_DATA);

    const [selectedId, setSelectedId] = useState('');

    const { language } = useLanguage();

    const {
        createOrderPage: {
            asideTitle,
            asideButtons: { accept, close, discount },
        },
    } = languagePacks[language];

    const handleClose = () => {
        const newParams = new URLSearchParams(searchParams.toString());

        newParams.delete(SEARCH_PARAMS_NAMES.MENU_ITEM_ID);
        newParams.delete(SEARCH_PARAMS_NAMES.USER_DATA);
        newParams.delete(SEARCH_PARAMS_NAMES.CATEGORY);
        newParams.delete(SEARCH_PARAMS_NAMES.SUBCATEGORY);
        newParams.delete(SEARCH_PARAMS_NAMES.TAG);

        toggleModal();
        clearOrders();
        router.push(`${pathname}?${newParams.toString()}`);
    };

    const buttons: ButtonProps[] = [
        {
            children: discount,
            variant: 'primary',
        },
        {
            children: accept,
            onClick: () => toogleUserDataModal(),
            variant: 'primary',
        },
        {
            children: close,
            onClick: handleClose,
            variant: 'tertiary',
        },
    ];

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

    const toogleUserDataModal = () => {
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.USER_DATA,
            'true',
            searchParams,
            router,
            pathname
        );
    };

    const orderItems = orders.map((order) => ({
        menuItemId: order.id,
    }));

    return !userData ? (
        <Menu variant="order">
            <DetailsAside
                title={asideTitle}
                items={bill}
                price={3}
                currency="pln"
                buttons={buttons}
            />
        </Menu>
    ) : (
        <CustomerInformationForm bill={bill} orderItems={orderItems} />
    );
};

export default CreateOrder;
