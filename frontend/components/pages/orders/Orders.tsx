'use client';

import { useState } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';

import {
    CURRENCIES,
    FILTER_STATUS,
    ordersTypes,
    SEARCH_PARAMS_NAMES,
} from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import useLanguage from '@/helpers/hooks/useLanguage';
import useQueryOrdersByType from '@/helpers/queries/orders/useQueryOrders';

import Button from '@/components/shared/Button/Button';
import OverviewModal from '@/components/shared/modals/OverviewModal';
import ToggleSwitch from '@/components/shared/toggleSwitch/ToggleSwitch';
import Menu from '../menu/Menu';
import DetailsAside from '@/components/shared/asides/Details';
import { useOrdersContext } from '@/providers/OrdersContext';
import OrderList from '@/components/shared/lists/orders/OrderList';
import PaymentDetails from '@/components/shared/modals/PaymentDetails';

const Orders = () => {
    const { language } = useLanguage();
    const { orders } = useOrdersContext();
    const searchParams = useSearchParams();
    const router = useRouter();

    const [selectedId, setSelectedId] = useState<string | null>(null);

    const orderId = useSearchParams().get(SEARCH_PARAMS_NAMES.ORDER_ID);
    const modal = useSearchParams().get(SEARCH_PARAMS_NAMES.MODAL);
    const closeOrder = useSearchParams().get('closeOrder');

    const orderType =
        useSearchParams().get(SEARCH_PARAMS_NAMES.ORDER_TYPE) ||
        ordersTypes[language][0]?.id;

    const { data } = useQueryOrdersByType(orderType);

    const {
        ordersPage: {
            ordersActiveTitle,
            ordersClosedTitle,
            createOrder,
            editOrder,
            payment,
        },
    } = languagePacks[language];

    const filterDataByStatus = (status: string) =>
        data?.filter((item) => item.orderStatus === status);

    const onGoingOrders = filterDataByStatus(FILTER_STATUS.ONGOING);
    const closedOrders = filterDataByStatus(FILTER_STATUS.CLOSED);

    const toggleQueryParam = (
        paramName: string,
        value: string | null = 'true'
    ) => {
        const params = new URLSearchParams(searchParams.toString());

        if (params.get(paramName) === value) {
            params.delete(paramName);
        } else {
            if (value !== null) {
                params.set(paramName, value);
            }
        }

        router.push(`/orders?${params.toString()}`);
    };
    const toggleSelected = (id: string) => {
        const params = new URLSearchParams(searchParams.toString());

        if (selectedId === id) {
            setSelectedId(null);
            params.delete('orderId');
        } else {
            setSelectedId(id);
            params.set('orderId', id);
        }
        router.push(`/orders?${params.toString()}`);
    };

    const toggleModal = () => {
        toggleQueryParam('modal');
    };

    const closeOrderHandler = () => {
        toggleQueryParam('closeOrder');
    };

    const finalizePayment = () => {
        toggleQueryParam('payment');
    };

    const buttons = [
        {
            children: 'Dodaj zniżkę',
        },
        {
            children: 'Zatwierdź',
        },
        {
            children: 'Zamknij bez zmian',
            onClick: () => toggleModal(),
            variant: 'tertiary',
        },
    ];

    const buttonsPayment = [
        {
            children: 'Zamknij bez zmian',
            onClick: () => closeOrderHandler(),
            variant: 'tertiary',
        },
        {
            children: 'Płatność',
            onClick: () => finalizePayment(),
        },
    ];

    const items = [
        {
            id: '1st',
            name: 'Rachunek 1',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            nestedItems: [
                {
                    name: 'rosa 1',
                    price: 33,
                    currency: 'pln' as keyof typeof CURRENCIES,
                    quantity: 1,
                    onClick: () => {
                        console.log(`item 1 clicked`);
                    },
                    isServed: true,
                },
                {
                    name: 'rosa 2',
                    price: 33,
                    currency: 'pln' as keyof typeof CURRENCIES,
                    quantity: 2,
                    annotation: ['+mozarella', '-mozzarella', '+mozarella'],
                    onClick: () => {
                        console.log(`item 2 clicked`);
                    },
                },
                {
                    name: 'pizza Margheritta',
                    price: 33,
                    currency: 'pln' as keyof typeof CURRENCIES,
                    quantity: 1,
                    onClick: () => {
                        console.log(`item 3 clicked`);
                    },
                    isServed: true,
                },
                {
                    name: 'rosa 3',
                    price: 33,
                    currency: 'pln' as keyof typeof CURRENCIES,
                    quantity: 2,
                    annotation: ['+mozarella', '-mozzarella'],
                    onClick: () => {
                        console.log(`item 4 clicked`);
                    },
                },
            ],
        },
        {
            id: '2nd',
            name: 'Rachunek 2',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            nestedItems: [
                {
                    name: 'rosa 1',
                    price: 33,
                    currency: 'pln' as keyof typeof CURRENCIES,
                    quantity: 1,
                    onClick: () => {
                        console.log(`item 1 clicked`);
                    },
                    isServed: true,
                },
                {
                    name: 'rosa 2',
                    price: 33,
                    currency: 'pln' as keyof typeof CURRENCIES,
                    quantity: 2,
                    annotation: ['+mozarella', '-mozzarella', '+mozarella'],
                    onClick: () => {
                        console.log(`item 2 clicked`);
                    },
                },
                {
                    name: 'pizza Margheritta',
                    price: 33,
                    currency: 'pln' as keyof typeof CURRENCIES,
                    quantity: 1,
                    onClick: () => {
                        console.log(`item 3 clicked`);
                    },
                    isServed: true,
                },
                {
                    name: 'rosa 3',
                    price: 33,
                    currency: 'pln' as keyof typeof CURRENCIES,
                    quantity: 2,
                    annotation: ['+mozarella', '-mozzarella'],
                    onClick: () => {
                        console.log(`item 4 clicked`);
                    },
                },
            ],
        },
        {
            id: '3rd',
            name: 'Rachunek 3',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            nestedItems: [
                {
                    name: 'rosa 1',
                    price: 33,
                    currency: 'pln' as keyof typeof CURRENCIES,
                    quantity: 1,
                    onClick: () => {
                        console.log(`item 1 clicked`);
                    },
                    isServed: true,
                },
                {
                    name: 'rosa 2',
                    price: 33,
                    currency: 'pln' as keyof typeof CURRENCIES,
                    quantity: 2,
                    annotation: ['+mozarella', '-mozzarella', '+mozarella'],
                    onClick: () => {
                        console.log(`item 2 clicked`);
                    },
                },
                {
                    name: 'pizza Margheritta',
                    price: 33,
                    currency: 'pln' as keyof typeof CURRENCIES,
                    quantity: 1,
                    onClick: () => {
                        console.log(`item 3 clicked`);
                    },
                    isServed: true,
                },
                {
                    name: 'rosa 3',
                    price: 33,
                    currency: 'pln' as keyof typeof CURRENCIES,
                    quantity: 2,
                    annotation: ['+mozarella', '-mozzarella'],
                    onClick: () => {
                        console.log(`item 4 clicked`);
                    },
                },
            ],
        },
    ];

    const invoiceItems = [
        {
            product: 'Pizza Margheritta',
            quantity: 3,
            pricePerUnit: 33,
            total: 99,
        },
        {
            product: 'Pizza Margheritta',
            quantity: 3,
            pricePerUnit: 33,
            total: 99,
        },
        {
            product: 'Pizza Margheritta',
            quantity: 3,
            pricePerUnit: 33,
            total: 99,
        },
    ];

    if (modal === 'true') {
        return (
            <OverviewModal>
                <Menu variant="order">
                    <DetailsAside
                        title="Odbiór"
                        items={orders}
                        price={3}
                        currency="pln"
                        buttons={buttons}
                    />
                </Menu>
            </OverviewModal>
        );
    }

    if (closeOrder === 'true') {
        return (
            <OverviewModal>
                <PaymentDetails items={invoiceItems}>
                    <DetailsAside
                        title="stolik b2"
                        items={items}
                        served={true}
                        buttons={buttonsPayment}
                    />
                </PaymentDetails>
            </OverviewModal>
        );
    }

    return (
        <div className="flex flex-col h-full p-4 pb-0">
            <div className="flex p-4 justify-between items-center">
                <ToggleSwitch items={ordersTypes[language]} />
                <ul className="flex gap-4">
                    <Button onClick={toggleModal} variant="primary">
                        {orderId ? editOrder : createOrder}
                    </Button>
                    {orderId && (
                        <Button onClick={closeOrderHandler} variant="primary">
                            {payment}
                        </Button>
                    )}
                </ul>
            </div>
            <div className="flex justify-around w-full mt-20 h-full">
                <div className="flex-1 text-center text-5xl">
                    <h2 className="text-2xl font-bold">{ordersActiveTitle}</h2>
                    <OrderList
                        orders={onGoingOrders}
                        toggleSelected={toggleSelected}
                        selectedId={selectedId}
                    />
                </div>
                <hr className="w-[0.1rem] h-full bg-black" />
                <div className="flex-1 text-center text-5xl">
                    <h2 className="text-2xl font-bold">{ordersClosedTitle}</h2>
                    <OrderList
                        orders={closedOrders}
                        toggleSelected={toggleSelected}
                        selectedId={selectedId}
                    />
                </div>
            </div>
        </div>
    );
};

export default Orders;
