'use client';

import { useState } from 'react';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';

import {
    CURRENCIES,
    FILTER_STATUS,
    ordersTypes,
    SEARCH_PARAMS_NAMES,
} from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import useLanguage from '@/helpers/hooks/useLanguage';
import useQueryOrdersByType from '@/helpers/queries/orders/useQueryOrders';

import Button from '@/components/shared/button/Button';
import OverviewModal from '@/components/shared/modals/OverviewModal';
import ToggleSwitch from '@/components/shared/toggleSwitch/ToggleSwitch';

import DetailsAside from '@/components/shared/asides/Details';

import OrderList from '@/components/shared/lists/orders/OrderList';
import PaymentDetails from '@/components/shared/modals/PaymentDetails';
import EditOrder from './EditOrder';
import CreateOrder from './CreateOrder';
import { toggleQueryParam } from '@/helpers/utils/utils';

const Orders = () => {
    const { language } = useLanguage();
    const searchParams = useSearchParams();
    const pathname = usePathname();
    const router = useRouter();

    const [selectedId, setSelectedId] = useState<string | null>(null);

    const params = Object.fromEntries(searchParams.entries());

    const {
        [SEARCH_PARAMS_NAMES.ORDER_ID]: orderId,
        [SEARCH_PARAMS_NAMES.MODAL]: modal,
        [SEARCH_PARAMS_NAMES.CLOSE_ORDER]: closeOrder,
        [SEARCH_PARAMS_NAMES.ORDER_TYPE]: orderTypeParam,
    } = params;

    const orderType = orderTypeParam || ordersTypes[language][0]?.id;

    const { data } = useQueryOrdersByType(orderType);

    const {
        ordersPage: {
            ordersActiveTitle,
            ordersClosedTitle,
            createOrder,
            editOrder,
            payment,
            asideTitle,
        },
    } = languagePacks[language];

    const filterDataByStatus = (status: string) =>
        data?.filter((item) => item.orderStatus === status);

    const onGoingOrders = filterDataByStatus(FILTER_STATUS.ONGOING);
    const closedOrders = filterDataByStatus(FILTER_STATUS.CLOSED);

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
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.MODAL,
            'true',
            searchParams,
            router,
            pathname
        );
    };

    const closeOrderHandler = () => {
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.CLOSE_ORDER,
            'true',
            searchParams,
            router,
            pathname
        );
    };

    const finalizePayment = () => {
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.PAYMENT,
            'true',
            searchParams,
            router,
            pathname
        );
    };

    const buttonsPayment = [
        {
            children: 'Zamknij bez zmian',
            onClick: () => closeOrderHandler(),
            variant: 'tertiary',
        },
        {
            children: 'Płatność',
            onClick: () => finalizePayment(),
            variant: 'primary',
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

    if (modal === 'true' && !orderId) {
        return (
            <OverviewModal>
                <CreateOrder toggleModal={toggleModal} />
            </OverviewModal>
        );
    }

    if (modal === 'true' && orderId) {
        return (
            <OverviewModal>
                <EditOrder toggleModal={toggleModal} />
            </OverviewModal>
        );
    }

    if (closeOrder === 'true') {
        return (
            <OverviewModal>
                <PaymentDetails>
                    <DetailsAside
                        title={asideTitle}
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
                <div className="flex gap-4">
                    <Button onClick={toggleModal} variant="primary">
                        {orderId ? editOrder : createOrder}
                    </Button>
                    {orderId && (
                        <Button onClick={closeOrderHandler} variant="primary">
                            {payment}
                        </Button>
                    )}
                </div>
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

//TODO change details aside
