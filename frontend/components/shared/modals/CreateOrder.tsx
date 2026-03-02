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
import { useState, useMemo, useEffect, useCallback } from 'react';
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
import useQueryOrders from '@/helpers/queries/orders/useQueryOrders';
import useQueryTables from '@/helpers/queries/tables/useQueryTables';
import useAddOrderItemsMutation from '@/helpers/queries/orders/useAddOrderItemsMutation';

export interface BillProps {
    id: string;
    name: string;
    price: number;
    currency: Currency;
    nestedItems: ItemProps[];
    className?: string;
}

type LocalBillItem = {
    id: string; // menu item id
    name: string;
    price: number;
    currency: Currency;
    quantity: number;
    discount?: number;
};

type LocalBill = {
    id: string; // either existing bill id or 'new-X' for new bills
    isNew: boolean;
    orderItems: LocalBillItem[];
    pendingItems: LocalBillItem[]; // items added from menu, not yet synced to backend
    totalAmount: number;
};

const CreateOrder = ({
    toggleModal,
    skipCustomerForm = false,
    tableId,
}: {
    toggleModal: () => void;
    skipCustomerForm?: boolean;
    tableId?: string | undefined;
}) => {
    const { clearOrders, deliveryPrice } = useOrdersContext();

    const queryClient = useQueryClient();
    const router = useRouter();
    const pathname = usePathname();
    const searchParams = useSearchParams();
    const menuItemId = searchParams.get(SEARCH_PARAMS_NAMES.MENU_ITEM_ID);
    const userData = searchParams.get(SEARCH_PARAMS_NAMES.USER_DATA);

    const [selectedId, setSelectedId] = useState('');
    const [selectedBill, setSelectedBill] = useState<string | null>(null);
    const [localBills, setLocalBills] = useState<LocalBill[]>([]);
    const [newBillCounter, setNewBillCounter] = useState(0);

    const { language } = useLanguage();

    const { data: allOrders } = useQueryOrders({
        queryKeys: [OrdersItems.BY_TYPE, ORDER_TYPES.DINEIN],
    });
    const { data: tables } = useQueryTables();

    const dineInOrders = useMemo(
        () =>
            (allOrders || []).filter(
                (o) =>
                    o.orderType.toLowerCase() ===
                    ORDER_TYPES.DINEIN.toLowerCase()
            ),
        [allOrders]
    );

    const tableName = useMemo(() => {
        if (!tableId || !tables) return '';
        const table = tables.find((t) => t.id === tableId);
        return table?.name || '';
    }, [tableId, tables]);

    const existingOrdersForTable = useMemo(() => {
        return tableId ? dineInOrders.filter((o) => o.tableId === tableId) : [];
    }, [dineInOrders, tableId]);

    // Auto-select first existing bill or 'new' if none exist
    useEffect(() => {
        if (selectedBill === null) {
            if (existingOrdersForTable.length > 0) {
                setSelectedBill(existingOrdersForTable[0].id);
            } else {
                setSelectedBill('new');
            }
        }
    }, [existingOrdersForTable, selectedBill]);

    // Initialize localBills from existingOrdersForTable, or create a default new bill
    useEffect(() => {
        if (localBills.length === 0) {
            if (existingOrdersForTable.length > 0) {
                // Initialize from existing orders
                const initialized: LocalBill[] = existingOrdersForTable.map(
                    (order) => ({
                        id: order.id,
                        isNew: false,
                        orderItems: order.orderItems.map((item) => ({
                            id: item.menuItem.id,
                            name: item.menuItem.name,
                            price: item.price,
                            currency: 'pln' as keyof typeof CURRENCIES,
                            quantity: 1,
                            discount: item.discount,
                        })),
                        pendingItems: [],
                        totalAmount: order.totalAmount || 0,
                    })
                );
                setLocalBills(initialized);
            } else {
                // No existing orders - create a default new bill
                const defaultBill: LocalBill = {
                    id: 'new',
                    isNew: true,
                    orderItems: [],
                    pendingItems: [],
                    totalAmount: 0,
                };
                setLocalBills([defaultBill]);
            }
        }
    }, [existingOrdersForTable, localBills.length]);

    // Direct callback to add items to the selected bill - no useEffect needed
    const addItemToSelectedBill = useCallback(
        (item: LocalBillItem) => {
            setLocalBills((prev) => {
                const billIndex = prev.findIndex((b) => b.id === selectedBill);

                if (billIndex !== -1) {
                    // Add to existing bill
                    const updatedBills = [...prev];
                    const existingItemIndex = updatedBills[
                        billIndex
                    ].pendingItems.findIndex((i) => i.id === item.id);

                    if (existingItemIndex !== -1) {
                        // Item already exists, update quantity
                        updatedBills[billIndex] = {
                            ...updatedBills[billIndex],
                            pendingItems: updatedBills[
                                billIndex
                            ].pendingItems.map((i, idx) =>
                                idx === existingItemIndex
                                    ? {
                                          ...i,
                                          quantity: i.quantity + item.quantity,
                                      }
                                    : i
                            ),
                        };
                    } else {
                        // New item, add to pending
                        updatedBills[billIndex] = {
                            ...updatedBills[billIndex],
                            pendingItems: [
                                ...updatedBills[billIndex].pendingItems,
                                item,
                            ],
                        };
                    }
                    return updatedBills;
                } else if (selectedBill === 'new') {
                    // Create new bill with this item
                    return [
                        ...prev,
                        {
                            id: 'new',
                            isNew: true,
                            orderItems: [],
                            pendingItems: [item],
                            totalAmount: 0,
                        },
                    ];
                }
                return prev;
            });
        },
        [selectedBill]
    );

    const { detailsAside } = languagePacks[language];
    const {
        createOrderPage: {
            asideButtons: { accept, close, discount },
        },
    } = languagePacks[language];

    const createDineinMutation = useOrderMutation('create', 'dinein', {
        redirectOnSettled: false,
        onSuccess: () => {
            // Manual invalidation is handled in the accept button logic
            // This ensures we only invalidate after all mutations are complete
        },
        onError: (err) => {
            console.error(err);
            toast.error(
                languagePacks[language].createOrderPage.error ||
                    'Failed to add to bill'
            );
        },
    });

    const addOrderItemsMutation = useAddOrderItemsMutation({
        onSuccess: () => {
            // Don't close modal here - let the accept button handler manage it
            // Mutation already invalidates queries
        },
    });

    const handleClose = () => {
        clearOrders();
        setLocalBills([]);
        setSelectedBill(null);

        const newParams = new URLSearchParams(searchParams.toString());
        newParams.delete(SEARCH_PARAMS_NAMES.MENU_ITEM_ID);
        newParams.delete(SEARCH_PARAMS_NAMES.USER_DATA);
        newParams.delete(SEARCH_PARAMS_NAMES.CATEGORY);
        newParams.delete(SEARCH_PARAMS_NAMES.SUBCATEGORY);
        newParams.delete(SEARCH_PARAMS_NAMES.TAG);

        router.push(`${pathname}?${newParams.toString()}`);
        toggleModal();
    };

    // Compute if accept button should be enabled
    const selectedBillData = localBills.find((b) => b.id === selectedBill);
    const itemsInSelectedBill = selectedBillData?.orderItems.length ?? 0;
    const pendingInSelectedBill = selectedBillData?.pendingItems.length ?? 0;
    const canAcceptBill =
        selectedBill && (itemsInSelectedBill > 0 || pendingInSelectedBill > 0);

    const buttons: ButtonProps[] = [
        {
            children: discount,
            variant: 'primary',
        },
        {
            children: accept,
            variant: 'primary',
            disabled: !canAcceptBill,
            onClick: async () => {
                // Guard: must have a selected bill
                if (!selectedBill) return;

                if (skipCustomerForm || tableId) {
                    // For dine-in orders (tableId present), sync directly to backend
                    // For orders with skipCustomerForm, also skip the form
                    try {
                        const today = new Date();
                        const dateStr = today.toISOString().split('T')[0];
                        const timeStr = today
                            .toTimeString()
                            .split(':')
                            .slice(0, 2)
                            .join(':');
                        const dateTimeStr = `${dateStr}T${timeStr}:00`;

                        const isGuid = (s?: string) =>
                            !!s &&
                            /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/.test(
                                s
                            );

                        let resolvedTableId = tableId || '';

                        if (tableId && !isGuid(tableId)) {
                            try {
                                const tablesRes = await fetch(
                                    `${BACKEND_URL}/tables`
                                );
                                if (tablesRes.ok) {
                                    const tables = await tablesRes.json();
                                    const match = tables.find(
                                        (t: { id: string; name?: string }) =>
                                            t.name
                                                ?.toLowerCase()
                                                .includes(tableId.toLowerCase())
                                    );
                                    resolvedTableId = match
                                        ? match.id
                                        : tables[0]?.id;
                                }
                            } catch (err) {
                                console.error(
                                    'Failed to resolve table GUID',
                                    err
                                );
                            }
                        } else if (tableId && isGuid(tableId)) {
                            resolvedTableId = tableId;
                        }

                        // Use current localBills state to sync to backend
                        for (const localBill of localBills) {
                            if (
                                localBill.isNew &&
                                localBill.pendingItems.length > 0
                            ) {
                                // Create new bill with pending items
                                // Expand items by quantity since backend expects separate entries
                                const payloadOrderItems =
                                    localBill.pendingItems.flatMap(
                                        (item: LocalBillItem) =>
                                            Array.from(
                                                { length: item.quantity },
                                                () => ({
                                                    specialInstructions: '',
                                                    discount:
                                                        item.discount || 0,
                                                    menuItemId: item.id,
                                                    extraIngredients: [],
                                                    removedIngredientIds: [],
                                                })
                                            )
                                    );

                                await createDineinMutation.mutateAsync({
                                    data: {
                                        createdAt: dateTimeStr,
                                        discount: 0,
                                        deliveryPrice: deliveryPrice || 0,
                                        customerInformation: {
                                            phoneNumber: '',
                                            orderCompletionType: 'Immediate',
                                            preferredPaymentMethod: 'Card',
                                            additionalInstructions: '',
                                            expectedOrderCompletion:
                                                dateTimeStr,
                                        },
                                        orderItems: payloadOrderItems,
                                        tableId: resolvedTableId,
                                    } as OrderDto & { tableId: string },
                                });
                            } else if (
                                !localBill.isNew &&
                                localBill.pendingItems.length > 0
                            ) {
                                // Add pending items to existing bill
                                // Expand items by quantity since backend expects separate entries
                                const payloadOrderItems =
                                    localBill.pendingItems.flatMap(
                                        (item: LocalBillItem) =>
                                            Array.from(
                                                { length: item.quantity },
                                                () => ({
                                                    specialInstructions: '',
                                                    discount:
                                                        item.discount || 0,
                                                    menuItemId: item.id,
                                                    extraIngredients: [],
                                                    removedIngredientIds: [],
                                                })
                                            )
                                    );

                                await addOrderItemsMutation.mutateAsync({
                                    orderId: localBill.id,
                                    orderItems: payloadOrderItems,
                                });
                            }
                        }

                        // All mutations succeeded, show success and close modal
                        toast.success(
                            languagePacks[language].createOrderPage
                                .confirmation || 'Bill confirmed successfully'
                        );

                        // Force refresh of queries to ensure immediate UI update
                        await queryClient.invalidateQueries({
                            queryKey: [
                                ...ORDERS_QUERY_KEY,
                                OrdersItems.BY_TYPE,
                                ORDER_TYPES.DINEIN,
                            ],
                        });

                        // Also invalidate the tables query in case table status changed
                        await queryClient.invalidateQueries({
                            queryKey: ['tables'],
                        });

                        handleClose();
                    } catch (err) {
                        console.error('Failed to sync bills', err);
                        toast.error(
                            languagePacks[language].createOrderPage.error ||
                                'Failed to confirm bill'
                        );
                    }
                    return;
                }

                // For non-dine-in or when skipCustomerForm is false, show customer form
                toggleUserDataModal();
            },
        },
        {
            children: close,
            onClick: handleClose,
            variant: 'tertiary',
        },
    ];

    const BILL_CURRENCY = 'pln' as keyof typeof CURRENCIES;

    const getBillStyles = (isSelected: boolean) => ({
        className: isSelected ? 'bg-primary text-white' : '',
        priceStrClassName: isSelected ? 'text-white' : '',
    });

    const handleAddNewBill = () => {
        const newId = `new-${newBillCounter}`;
        setNewBillCounter((prev) => prev + 1);

        const newBill: LocalBill = {
            id: newId,
            isNew: true,
            orderItems: [],
            pendingItems: [],
            totalAmount: 0,
        };

        setLocalBills((prev) => [...prev, newBill]);
        setSelectedBill(newId);
    };

    const bill: BillProps[] = localBills.map((localBill, index) => {
        const isSelected = selectedBill === localBill.id;
        const pendingPrice = localBill.pendingItems.reduce(
            (acc, item) => acc + item.price * item.quantity,
            0
        );
        return {
            id: localBill.id,
            name: localBill.isNew
                ? `${detailsAside.receipt} ${localBills.filter((b) => b.isNew).indexOf(localBill) + 1 + existingOrdersForTable.length} (nowy)`
                : `${detailsAside.receipt} ${index + 1}`,
            price: localBill.totalAmount + pendingPrice,
            currency: BILL_CURRENCY,
            nestedItems: [
                ...localBill.orderItems.map((it) => ({
                    name: it.name,
                    price: it.price,
                    currency: BILL_CURRENCY,
                    quantity: it.quantity || 1,
                    onClick: () => {},
                })),
                ...localBill.pendingItems.map((item) => ({
                    name: item.name,
                    price: item.price,
                    currency: BILL_CURRENCY,
                    quantity: item.quantity,
                    onClick: () => toggleSelect(item.id),
                    className: menuItemId === item.id ? 'bg-red-200' : '',
                })),
            ],
            ...getBillStyles(isSelected),
        };
    });

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

    const toggleUserDataModal = () => {
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.USER_DATA,
            'true',
            searchParams,
            router,
            pathname
        );
    };

    // Build orderItems from the selected bill's pending items for CustomerInformationForm
    const orderItems = selectedBillData
        ? selectedBillData.pendingItems.flatMap((item) =>
              Array.from({ length: item.quantity }, () => ({
                  menuItemId: item.id,
              }))
          )
        : [];

    return !userData ? (
        <Menu variant="order" onAddItem={addItemToSelectedBill}>
            <DetailsAside
                title={
                    tableName
                        ? `${detailsAside.table} ${tableName}`
                        : detailsAside.receipt
                }
                items={bill}
                buttons={buttons}
                onSelectItem={setSelectedBill}
                selectedItemId={selectedBill}
                onAddNewOrder={handleAddNewBill}
            />
        </Menu>
    ) : (
        <CustomerInformationForm bill={bill} orderItems={orderItems} />
    );
};

export default CreateOrder;
