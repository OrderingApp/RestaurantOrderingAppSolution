'use client';

import DetailsAside from '@/components/shared/asides/Details';
import Menu from '../../pages/menu/Menu';
import {
    CURRENCIES,
    ORDER_TYPES,
    SEARCH_PARAMS_NAMES,
} from '@/helpers/constants/constants';
import { useOrdersContext } from '@/providers/OrdersContext';
// note: direct fetch is used for skipCustomerForm flow to avoid route navigation
import { useQueryClient } from '@tanstack/react-query';
import { BACKEND_URL } from '@/helpers/constants/constants';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import useOrderMutation from '@/helpers/queries/orders/useOrdersMutation';
import { toast } from 'sonner';
import { useState } from 'react';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { toggleQueryParam } from '@/helpers/utils/utils';
import CustomerInformationForm from '../../pages/orders/CustomerInformationForm';
import { ItemProps } from '@/components/shared/lists/Items/Item';
import { ButtonProps } from '@/components/shared/button/Button';
import { Currency } from '@/helpers/type/types';
import { useLanguage } from '@/providers/LanguageProvider';
import languagePacks from '@/helpers/constants/languagePacks';
import { OrderDto } from '@/helpers/interfaces/orders';
import { ORDERS_QUERY_KEY } from '@/helpers/queries/orders/useQueryOrders';

export interface BillProps {
    id: string;
    name: string;
    price: number;
    currency: Currency;
    nestedItems: ItemProps[];
}

const CreateOrder = ({
    toggleModal,
    skipCustomerForm = false,
    tableId,
}: {
    toggleModal: () => void;
    skipCustomerForm?: boolean;
    tableId?: string | undefined;
}) => {
    const { orders, clearOrders, deliveryPrice } = useOrdersContext();

    const queryClient = useQueryClient();
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

    const createDineinMutation = useOrderMutation('create', 'dinein', {
        redirectOnSettled: false,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: [
                    ...ORDERS_QUERY_KEY,
                    OrdersItems.BY_TYPE,
                    ORDER_TYPES.DINEIN,
                ],
            });

            toast.success(
                languagePacks[language].createOrderPage.confirmation ||
                    'Added to bill'
            );

            clearOrders();

            // remove modal-related query params and close modal
            const newParams = new URLSearchParams(searchParams.toString());

            newParams.delete(SEARCH_PARAMS_NAMES.MENU_ITEM_ID);
            newParams.delete(SEARCH_PARAMS_NAMES.USER_DATA);
            newParams.delete(SEARCH_PARAMS_NAMES.CATEGORY);
            newParams.delete(SEARCH_PARAMS_NAMES.SUBCATEGORY);
            newParams.delete(SEARCH_PARAMS_NAMES.TAG);

            toggleModal();
            router.push(`${pathname}?${newParams.toString()}`);
        },
        onError: (err) => {
            console.error(err);
            toast.error(
                languagePacks[language].createOrderPage.error ||
                    'Failed to add to bill'
            );
        },
    });

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
            onClick: async () => {
                if (orders.length === 0) return;

                if (skipCustomerForm) {
                    const today = new Date();
                    const dateStr = today.toISOString().split('T')[0];
                    const timeStr = today
                        .toTimeString()
                        .split(':')
                        .slice(0, 2)
                        .join(':');
                    const dateTimeStr = `${dateStr}T${timeStr}:00`;

                    const payloadOrderItems = orders.map((o) => ({
                        specialInstructions: '',
                        discount: o.discount || 0,
                        menuItemId: o.id,
                        extraIngredients: [],
                        removedIngredientIds: [],
                    }));

                    const orderDto: OrderDto & { tableId?: string } = {
                        createdAt: dateTimeStr,
                        discount: 0,
                        deliveryPrice: deliveryPrice || 0,
                        customerInformation: {
                            phoneNumber: '',
                            orderCompletionType: 'Immediate',
                            preferredPaymentMethod: 'Card',
                            additionalInstructions: '',
                            expectedOrderCompletion: dateTimeStr,
                        },
                        orderItems: payloadOrderItems.map((item) => ({
                            menuItemId: item.menuItemId,
                        })),
                    };

                    // include fields expected by the API (specialInstructions, discount, extras, removed ids)
                    // resolve a GUID for tableId: if current tableId is not a GUID, try fetching tables from backend
                    const isGuid = (s?: string) =>
                        !!s &&
                        /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/.test(
                            s
                        );

                    if (tableId) {
                        if (isGuid(tableId)) {
                            orderDto.tableId = tableId;
                        } else {
                            try {
                                const tablesRes = await fetch(
                                    `${BACKEND_URL}/tables`
                                );
                                if (tablesRes.ok) {
                                    const tables = await tablesRes.json();
                                    // try to find table by name containing the frontend id
                                    const match = tables.find(
                                        (t: { id: string; name?: string }) =>
                                            t.name
                                                ?.toLowerCase()
                                                .includes(tableId.toLowerCase())
                                    );
                                    const tableGuid = match
                                        ? match.id
                                        : tables[0]?.id;
                                    if (tableGuid) {
                                        orderDto.tableId = tableGuid;
                                    } else {
                                        console.warn(
                                            'No tables returned from backend; tableId not attached'
                                        );
                                    }
                                } else {
                                    console.warn(
                                        'Failed to fetch tables from backend'
                                    );
                                }
                            } catch (err) {
                                console.error('Error fetching tables', err);
                            }
                        }
                    }

                    // call generic mutation with proper shape using mutateAsync for better diagnostics
                    const payload = {
                        ...orderDto,
                        orderItems: payloadOrderItems,
                    };
                    console.log('Submitting dine-in order payload:', payload);
                    try {
                        await createDineinMutation.mutateAsync({
                            data: payload,
                        });
                        console.log('Mutation completed successfully');
                    } catch (err) {
                        console.error('Mutation error', err);
                        // onError handler will show toast; rethrowing not needed
                    }
                    return;
                }

                toogleUserDataModal();
            },
            variant: 'primary',
            disabled: orders.length === 0,
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
