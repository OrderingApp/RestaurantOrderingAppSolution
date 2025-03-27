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
import OrderCard from '@/components/shared/cards/OrderCard';
import OverviewModal from '@/components/shared/modals/OverviewModal';
import ToggleSwitch from '@/components/shared/toggleSwitch/ToggleSwitch';
import Menu from '../menu/Menu';
import DetailsAside from '@/components/shared/asides/Details';

const Orders = () => {
    const { language } = useLanguage();
    const seachParams = useSearchParams();
    const router = useRouter();
    const orderType =
        useSearchParams().get(SEARCH_PARAMS_NAMES.ORDER_TYPE) ||
        ordersTypes[language][0]?.id;
    const orderId = useSearchParams().get(SEARCH_PARAMS_NAMES.ORDER_ID);
    const modal = useSearchParams().get(SEARCH_PARAMS_NAMES.MODAL);
    const [selectedId, setSelectedId] = useState<string | null>(null);
    const { data } = useQueryOrdersByType(orderType);
    const {
        ordersPage: {
            ordersActiveTitle,
            ordersClosedTitle,
            createOrder,
            editOrder,
            deleteOrder,
            payment,
        },
    } = languagePacks[language];

    const filterDataByStatus = (status: string) => {
        return data?.filter((item) => item.orderStatus === status);
    };

    const onGoingOrders = filterDataByStatus(FILTER_STATUS.ONGOING);
    const closedOrders = filterDataByStatus(FILTER_STATUS.CLOSED);

    const toggleSelected = (id: string) => {
        const params = new URLSearchParams(seachParams.toString());

        selectedId === id
            ? (setSelectedId(null), params.delete('orderId'))
            : (setSelectedId(id), params.set('orderId', id));

        router.push(`/orders?${params.toString()}`);
    };

    const toggleModal = () => {
        const params = new URLSearchParams(seachParams.toString());

        modal === 'true' ? params.delete('modal') : params.set('modal', 'true');

        router.push(`/orders?${params.toString()}`);
    };

    const items = [
        {
            name: 'rosa 1',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            quantity: 1,
        },

        {
            name: 'Pizza Margheritta',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            quantity: 2,
            annotation: 'dodatkowe składniki',
        },
        {
            name: 'pizza Margheritta',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            quantity: 1,
        },

        {
            name: 'Pizza Margheritta',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            quantity: 2,
            annotation: 'dodatkowe składniki',
        },
        {
            name: 'pizza Margheritta',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            quantity: 1,
        },

        {
            name: 'Pizza Margheritta',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            quantity: 2,
            annotation: 'dodatkowe składniki',
        },
        {
            name: 'pizza Margheritta',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            quantity: 1,
        },

        {
            name: 'Pizza Margheritta',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            quantity: 2,
            annotation: 'dodatkowe składniki',
        },
        {
            name: 'pizza Margheritta',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            quantity: 1,
        },

        {
            name: 'Pizza Margheritta',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            quantity: 2,
            annotation: 'dodatkowe składniki',
        },
        {
            name: 'pizza Margheritta',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            quantity: 1,
        },

        {
            name: 'Pizza Margheritta',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            quantity: 2,
            annotation: 'dodatkowe składniki',
        },
        {
            name: 'pizza Margheritta',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            quantity: 1,
        },

        {
            name: 'Pizza Margheritta',
            price: 33,
            currency: 'pln' as keyof typeof CURRENCIES,
            quantity: 2,
            annotation: 'dodatkowe składniki',
        },
    ];
    const buttons = [
        {
            children: 'Zatwierdź',
        },
        { children: 'Zamknij bez zmian', onClick: () => toggleModal() },
    ];

    return modal ? (
        <OverviewModal>
            <Menu variant="order">
                <DetailsAside
                    title="Odbiór"
                    items={items}
                    price={3}
                    currency="pln"
                    buttons={buttons}
                />
            </Menu>
        </OverviewModal>
    ) : (
        <div className="flex flex-col h-full p-4 pb-0">
            <div className="flex p-4 justify-between items-center">
                <ToggleSwitch items={ordersTypes[language]} />
                <div className="flex gap-4">
                    {orderId && <Button variant="danger">{deleteOrder}</Button>}
                    <Button onClick={toggleModal} variant="primary">
                        {orderId ? editOrder : createOrder}
                    </Button>
                    {orderId && <Button variant="primary">{payment}</Button>}
                </div>
            </div>
            <div className="flex justify-around w-full mt-20 h-full">
                <div className="flex-1 text-center text-5xl">
                    <h2 className="text-2xl font-bold">{ordersActiveTitle}</h2>
                    <ul className="flex mt-5 gap-2">
                        {onGoingOrders?.map((order) => (
                            <OrderCard
                                onClick={() => toggleSelected(order.id)}
                                key={order.id}
                                {...order}
                                className={
                                    selectedId == order.id ? 'scale-110' : ''
                                }
                            />
                        ))}
                    </ul>
                </div>
                <hr className="w-[0.1rem] h-full bg-black" />
                <div className="flex-1 text-center text-5xl">
                    <h2 className="text-2xl font-bold">{ordersClosedTitle}</h2>
                    <ul className="flex gap-2 mt-5 mx-2">
                        {closedOrders?.map((order) => (
                            <OrderCard
                                onClick={() => toggleSelected(order.id)}
                                key={order.id}
                                {...order}
                                className={
                                    selectedId == order.id ? 'scale-110' : ''
                                }
                            />
                        ))}
                    </ul>
                </div>
            </div>
        </div>
    );
};

export default Orders;
