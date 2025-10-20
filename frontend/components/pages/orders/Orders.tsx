'use client';

import { useState } from 'react';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';

import Image from 'next/image';

import {
    ordersTypes,
    SEARCH_PARAMS_NAMES,
} from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import useLanguage from '@/helpers/hooks/useLanguage';
import { toggleQueryParam } from '@/helpers/utils/utils';
import { ICONS } from '@/helpers/constants/icons/icons';
import useFilterOrders from '@/helpers/hooks/useFilterOrders';

import Button, { ButtonProps } from '@/components/shared/button/Button';
import OverviewModal from '@/components/shared/modals/OverviewModal';
import ToggleSwitch from '@/components/shared/toggleSwitch/ToggleSwitch';
import DetailsAside from '@/components/shared/asides/Details';
import OrderList from '@/components/shared/lists/orders/OrderList';
import PaymentDetails from '@/components/shared/modals/PaymentDetails';
import EditOrder from './EditOrder';
import CreateOrder from './CreateOrder';
import SearchInput from '@/components/shared/Input/SearchInput';

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
    } = params;

    const { filteredOrders } = useFilterOrders();

    const {
        ordersPage: { createOrder, editOrder, payment, asideTitle },
    } = languagePacks[language];

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

    const buttonsPayment: ButtonProps[] = [
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
                        items={[]}
                        served={true}
                        buttons={buttonsPayment}
                    />
                </PaymentDetails>
            </OverviewModal>
        );
    }

    const buttons = [
        {
            value: 'Ongoing',
            iconActive: ICONS.LIST_WHITE,
            icon: ICONS.LIST,
        },

        {
            value: 'Closed',
            iconActive: ICONS.CLOSE_WHITE,
            icon: ICONS.CLOSE,
        },
    ];

    const isActive = (value: string) => {
        const param = searchParams.get('orderStatus');
        if (!param || undefined) return 'Ongoing';
        if (param === value) return value;
    };

    return (
        <div className="flex flex-col h-full p-4 pb-0">
            <div className="flex p-4 justify-between items-center h-auto">
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
            <div className="flex justify-around w-full mt-20  h-auto">
                <div className="flex items-start justify-between w-full">
                    <div className="flex gap-4 mb-4 w-2/5 ">
                        {buttons.map((btn) => (
                            <button
                                className={`${isActive(btn.value) === btn.value ? 'bg-primary' : 'bg-[#F6F6F6]'} p-3 rounded-lg shadow-xl`}
                                onClick={() =>
                                    toggleQueryParam(
                                        'orderStatus',
                                        btn.value,
                                        searchParams,
                                        router,
                                        pathname
                                    )
                                }
                                key={btn.value}
                            >
                                <Image
                                    src={
                                        isActive(btn.value) === btn.value
                                            ? btn.iconActive
                                            : btn.icon
                                    }
                                    alt="iconList"
                                />
                            </button>
                        ))}
                    </div>
                    <div className="w-3/5 mr-5">
                        <SearchInput placeholder="Wyszukaj" />
                    </div>
                </div>
            </div>
            <div className="flex-1 text-center text-5xl">
                <OrderList
                    orders={filteredOrders}
                    toggleSelected={toggleSelected}
                    selectedId={selectedId}
                />
            </div>
        </div>
    );
};

export default Orders;

//TODO  = add pagination
