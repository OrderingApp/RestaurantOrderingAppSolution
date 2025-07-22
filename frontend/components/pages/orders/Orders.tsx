'use client';

import { useState } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';

import {
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
import Menu from '../menu/Menu';
import DetailsAside from '@/components/shared/asides/Details';
import { useOrdersContext } from '@/providers/OrdersContext';
import OrderList from '@/components/shared/lists/orders/OrderList';

const Orders = () => {
    const { language } = useLanguage();
    const { orders } = useOrdersContext();
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

    const filterDataByStatus = (status: string) =>
        data?.filter((item) => item.orderStatus === status);

    const onGoingOrders = filterDataByStatus(FILTER_STATUS.ONGOING);
    const closedOrders = filterDataByStatus(FILTER_STATUS.CLOSED);

    const toggleSelected = (id: string) => {
        const params = new URLSearchParams(seachParams.toString());

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
        const params = new URLSearchParams(seachParams.toString());

        if (modal === 'true') {
            params.delete('modal');
        } else {
            params.set('modal', 'true');
        }

        router.push(`/orders?${params.toString()}`);
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

    return modal ? (
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
    ) : (
        <div className="flex flex-col h-full p-4 pb-0">
            <div className="flex p-4 justify-between items-center">
                <ToggleSwitch items={ordersTypes[language]} />
                <ul className="flex gap-4">
                    {orderId && <Button variant="danger">{deleteOrder}</Button>}
                    <Button onClick={toggleModal} variant="primary">
                        {orderId ? editOrder : createOrder}
                    </Button>
                    {orderId && <Button variant="primary">{payment}</Button>}
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

//TODO change buttons language detail aside title
