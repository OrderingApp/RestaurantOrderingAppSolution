'use client';

import { usePathname, useRouter, useSearchParams } from 'next/navigation';

import Image from 'next/image';

import {
    ordersTypes,
    SEARCH_PARAMS_NAMES,
} from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import useLanguage from '@/helpers/hooks/useLanguage';
import { setQueryParams, toggleQueryParam } from '@/helpers/utils/utils';
import { ICONS } from '@/helpers/constants/icons/icons';
import useFilterOrders from '@/helpers/hooks/useFilterOrders';

import Button, { ButtonProps } from '@/components/shared/button/Button';
import OverviewModal from '@/components/shared/modals/OverviewModal';
import ToggleSwitch from '@/components/shared/toggleSwitch/ToggleSwitch';
import DetailsAside from '@/components/shared/asides/Details';
import OrderList from '@/components/shared/lists/orders/OrderList';
import PaymentDetails from '@/components/shared/modals/PaymentDetails';
import EditOrder from './EditOrder';
import CreateOrder from '../../shared/modals/CreateOrder';
import SearchInput from '@/components/shared/Input/SearchInput';
import clsx from 'clsx';
import Modal from '@/components/shared/modals/Modal';
import OrderOptionsModal from '@/components/shared/modals/OrderOptionsModal';

const Orders = () => {
    const { language } = useLanguage();
    const searchParams = useSearchParams();
    const pathname = usePathname();
    const router = useRouter();

    const params = Object.fromEntries(searchParams.entries());

    const {
        [SEARCH_PARAMS_NAMES.ORDER_ID]: orderId,
        [SEARCH_PARAMS_NAMES.MODAL]: modal,
        [SEARCH_PARAMS_NAMES.CLOSE_ORDER]: closeOrder,
    } = params;

    const { filteredOrders } = useFilterOrders();

    const {
        ordersPage: { createOrder, asideTitle, noOrdersFoundFallback },
        generic: { searchPlaceholder },
    } = languagePacks[language];

    // Modal state booleans
    const isCreateOrderModalOpen = modal === 'true' && !orderId;
    const isEditOrderModalOpen = modal === 'true' && !!orderId;
    const isPaymentModalOpen = closeOrder === 'true';
    const isOrderOptionsModalOpen = !!orderId && !isPaymentModalOpen;

    const toggleModal = () => {
        const params = new URLSearchParams(searchParams.toString());
        const isOpen = params.get(SEARCH_PARAMS_NAMES.MODAL) === 'true';

        if (isOpen) {
            params.delete(SEARCH_PARAMS_NAMES.MODAL);
            params.delete(SEARCH_PARAMS_NAMES.ORDER_ID);
            params.delete(SEARCH_PARAMS_NAMES.MENU_ITEM_ID);
            params.delete(SEARCH_PARAMS_NAMES.USER_DATA);
            params.delete(SEARCH_PARAMS_NAMES.CATEGORY);
            params.delete(SEARCH_PARAMS_NAMES.SUBCATEGORY);
            params.delete(SEARCH_PARAMS_NAMES.TAG);
            params.delete(SEARCH_PARAMS_NAMES.ORDER_MENU_PAGE);
        } else {
            params.set(SEARCH_PARAMS_NAMES.MODAL, 'true');
        }

        const queryString = params.toString();
        router.push(queryString ? `${pathname}?${queryString}` : pathname);
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

    const buttons = [
        {
            value: 'All',
            param: SEARCH_PARAMS_NAMES.ORDER_STATUS,
            iconActive: ICONS.LIST_WHITE,
            icon: ICONS.LIST,
        },
        {
            value: 'Ongoing',
            param: SEARCH_PARAMS_NAMES.ORDER_STATUS,
            iconActive: ICONS.MENU_OPEN_WHITE,
            icon: ICONS.MENU_OPEN,
        },
        {
            value: 'Paid',
            param: SEARCH_PARAMS_NAMES.PAYMENT_STATUS,
            iconActive: ICONS.CLOSE_WHITE,
            icon: ICONS.CLOSE,
        },
        {
            value: 'Closed',
            param: SEARCH_PARAMS_NAMES.ORDER_STATUS,
            iconActive: ICONS.TIME_WHITE,
            icon: ICONS.TIME,
        },
    ];

    const orderStatus = searchParams.get(SEARCH_PARAMS_NAMES.ORDER_STATUS);
    const paymentStatus = searchParams.get(SEARCH_PARAMS_NAMES.PAYMENT_STATUS);
    const activeFilter = paymentStatus ?? orderStatus ?? 'All';

    const onFilterClick = (paramName: string, value: string) =>
        setQueryParams(
            value === 'All'
                ? {
                      [SEARCH_PARAMS_NAMES.ORDER_STATUS]: undefined,
                      [SEARCH_PARAMS_NAMES.PAYMENT_STATUS]: undefined,
                  }
                : {
                      [SEARCH_PARAMS_NAMES.ORDER_STATUS]:
                          paramName === SEARCH_PARAMS_NAMES.ORDER_STATUS
                              ? value
                              : undefined,
                      [SEARCH_PARAMS_NAMES.PAYMENT_STATUS]:
                          paramName === SEARCH_PARAMS_NAMES.PAYMENT_STATUS
                              ? value
                              : undefined,
                  },
            searchParams,
            router,
            pathname
        );

    const openOrderOptionsModal = (id: string) => {
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.ORDER_ID,
            id,
            searchParams,
            router,
            pathname
        );
    };

    const closeOrderOptionsModal = () => {
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.ORDER_ID,
            undefined,
            searchParams,
            router,
            pathname
        );
    };

    return (
        <>
            <div className="flex flex-col h-full p-4 pb-0">
                <div className="flex p-4 justify-between items-center h-auto">
                    <ToggleSwitch items={ordersTypes[language]} />
                    <div className="flex gap-4">
                        <Button
                            onClick={toggleModal}
                            variant="primary"
                            className="hocus:scale-95"
                        >
                            {createOrder}
                        </Button>
                    </div>
                </div>
                <div className="flex justify-around w-full h-auto mt-4">
                    <div className="flex items-start justify-between w-full">
                        <div className="flex gap-4 mb-4 w-2/5 ">
                            {buttons.map((btn) => {
                                const isActive = activeFilter === btn.value;

                                return (
                                    <button
                                        className={clsx(
                                            'p-3 rounded-lg shadow-xl transition-all',
                                            isActive
                                                ? 'bg-primary'
                                                : 'bg-[#F6F6F6]'
                                        )}
                                        onClick={() =>
                                            onFilterClick(btn.param, btn.value)
                                        }
                                        key={btn.value}
                                    >
                                        <Image
                                            src={
                                                isActive
                                                    ? btn.iconActive
                                                    : btn.icon
                                            }
                                            alt="iconList"
                                            className={clsx(
                                                'transition-all',
                                                !isActive &&
                                                    'group-hover:brightness-0 group-hover:invert'
                                            )}
                                        />
                                    </button>
                                );
                            })}
                        </div>
                        <div className="w-3/5 mr-5">
                            <SearchInput
                                placeholder={searchPlaceholder}
                                className="w-full"
                            />
                        </div>
                    </div>
                </div>
                <div className="flex-1 text-center">
                    {filteredOrders.length > 0 ? (
                        <OrderList
                            orders={filteredOrders}
                            openModal={openOrderOptionsModal}
                        />
                    ) : (
                        <p className="text-2xl text-danger">
                            {noOrdersFoundFallback}
                        </p>
                    )}
                </div>
            </div>

            {/* Modals */}
            <OverviewModal isOpen={isCreateOrderModalOpen}>
                <CreateOrder toggleModal={toggleModal} />
            </OverviewModal>

            <OverviewModal isOpen={isEditOrderModalOpen}>
                <EditOrder toggleModal={toggleModal} />
            </OverviewModal>

            {/* Change detailaside props or change aside ui */}

            <OverviewModal isOpen={isPaymentModalOpen}>
                <PaymentDetails>
                    <DetailsAside
                        title={asideTitle}
                        items={[]}
                        served={true}
                        buttons={buttonsPayment}
                    />
                </PaymentDetails>
            </OverviewModal>

            <Modal
                isOpen={isOrderOptionsModalOpen}
                onClose={closeOrderOptionsModal}
            >
                <OrderOptionsModal onClose={closeOrderOptionsModal} />
            </Modal>
        </>
    );
};

export default Orders;
