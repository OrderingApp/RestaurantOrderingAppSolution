'use client';

import DetailsAside from '@/components/shared/asides/Details';
import Menu from '../../pages/menu/Menu';
import {
    COMPANYS_CURRENCY,
    ORDER_TYPES,
    ordersTypes,
    SEARCH_PARAMS_NAMES,
} from '@/helpers/constants/constants';
import { useOrdersContext } from '@/providers/OrdersContext';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import { useState, useEffect, useMemo } from 'react';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { toggleQueryParam } from '@/helpers/utils/utils';
import CustomerInformationForm from '../../pages/orders/CustomerInformationForm';
import { ItemProps } from '@/components/shared/lists/Items/Item';
import { ButtonProps } from '@/components/shared/button/Button';
import { Currency } from '@/helpers/type/types';
import { useLanguage } from '@/providers/LanguageProvider';
import languagePacks from '@/helpers/constants/languagePacks';
import useQueryOrders from '@/helpers/queries/orders/useQueryOrders';
import useQueryTables from '@/helpers/queries/tables/useQueryTables';
import { getAggregatedDineInOrdersForTable } from '@/helpers/utils/orderTransforms';
import useConfirmDineinBillsMutation from '@/helpers/queries/orders/useConfirmDineinBillsMutation';

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
    orderType,
    allowMultipleBills,
}: {
    toggleModal: () => void;
    skipCustomerForm?: boolean;
    tableId?: string | undefined;
    orderType?: ORDER_TYPES;
    allowMultipleBills?: boolean;
}) => {
    const { clearOrders, deliveryPrice } = useOrdersContext();

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

    const { data: allOrders, isLoading: isOrdersLoading } = useQueryOrders({
        queryKeys: [OrdersItems.BY_TYPE, ORDER_TYPES.DINEIN],
    });
    const { data: tables } = useQueryTables();

    const tableName =
        tableId && tables
            ? tables.find((t) => t.id === tableId)?.name || ''
            : '';

    const existingOrdersForTable = useMemo(() => {
        return getAggregatedDineInOrdersForTable(allOrders || [], tableId);
    }, [allOrders, tableId]);

    const isAsideOrdersLoading =
        !!tableId && isOrdersLoading && localBills.length === 0;

    // Auto-select first existing bill or 'new' if none exist
    useEffect(() => {
        if (selectedBill === null) {
            if (localBills.length > 0) {
                setSelectedBill(localBills[0].id);
            } else if (
                !isAsideOrdersLoading &&
                existingOrdersForTable.length > 0
            ) {
                setSelectedBill(existingOrdersForTable[0].id);
            } else if (!isAsideOrdersLoading) {
                setSelectedBill('new');
            }
        }
    }, [
        existingOrdersForTable,
        selectedBill,
        localBills,
        isAsideOrdersLoading,
    ]);

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
                            currency: COMPANYS_CURRENCY,
                            quantity: item.quantity || 1,
                            discount: item.discount,
                        })),
                        pendingItems: [],
                        totalAmount: order.totalAmount || 0,
                    })
                );
                setLocalBills(initialized);
            } else if (!isAsideOrdersLoading) {
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
    }, [existingOrdersForTable, localBills.length, isAsideOrdersLoading]);

    const addItemToSelectedBill = (item: LocalBillItem) => {
        setLocalBills((prev) => {
            // Nothing selected yet: ignore safely.
            if (!selectedBill) return prev;

            const targetBillId = selectedBill === 'new' ? 'new' : selectedBill;
            const billIndex = prev.findIndex((b) => b.id === targetBillId);

            // Placeholder bill selected but not present yet: create it on first add.
            if (selectedBill === 'new' && billIndex === -1) {
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

            // Selected bill vanished (e.g. race/state reset): ignore safely.
            if (billIndex === -1) return prev;

            const updatedBills = [...prev];
            const existingItemIndex = updatedBills[
                billIndex
            ].pendingItems.findIndex((i) => i.id === item.id);

            // Existing pending item: increment quantity.
            if (existingItemIndex !== -1) {
                updatedBills[billIndex] = {
                    ...updatedBills[billIndex],
                    pendingItems: updatedBills[billIndex].pendingItems.map(
                        (i, idx) =>
                            idx === existingItemIndex
                                ? {
                                      ...i,
                                      quantity: i.quantity + item.quantity,
                                  }
                                : i
                    ),
                };

                return updatedBills;
            }

            // New pending item for selected bill.
            updatedBills[billIndex] = {
                ...updatedBills[billIndex],
                pendingItems: [...updatedBills[billIndex].pendingItems, item],
            };

            return updatedBills;
        });
    };

    const { detailsAside } = languagePacks[language];
    const {
        createOrderPage: {
            asideButtons: { accept, close, discount },
        },
    } = languagePacks[language];

    const effectiveOrderType =
        orderType ?? (tableId ? ORDER_TYPES.DINEIN : ORDER_TYPES.TAKEAWAY);
    const canAddMultipleBills = allowMultipleBills ?? !!tableId;

    const selectedOrderTypeLabel =
        ordersTypes[language].find((type) => type.id === effectiveOrderType)
            ?.name ??
        ordersTypes[language][0]?.name ??
        detailsAside.receipt;

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
        newParams.delete(SEARCH_PARAMS_NAMES.ORDER_MENU_PAGE);

        router.push(`${pathname}?${newParams.toString()}`);
        toggleModal();
    };

    const confirmDineinBillsMutation = useConfirmDineinBillsMutation({
        onSuccess: handleClose,
    });

    // Compute if accept button should be enabled:
    // only enable when there are unsynced changes (pending items) in any bill.
    // This keeps it disabled when nothing changed, or when a new bill is empty.
    const selectedBillData = localBills.find((b) => b.id === selectedBill);
    const hasPendingChanges = localBills.some(
        (bill) => bill.pendingItems.length > 0
    );
    const canAcceptBill = !!selectedBill && hasPendingChanges;

    const buttons: ButtonProps[] = [
        {
            children: discount,
            variant: 'primary',
        },
        {
            children: accept,
            variant: 'primary',
            disabled: !canAcceptBill || confirmDineinBillsMutation.isPending,
            onClick: async () => {
                // Guard: must have a selected bill
                if (!selectedBill) return;

                if (skipCustomerForm || tableId) {
                    // For dine-in orders (tableId present), submit directly to backend.
                    // When skipCustomerForm is true, also submit directly without opening the customer form.
                    return await confirmDineinBillsMutation.mutateAsync({
                        bills: localBills,
                        tableId,
                        deliveryPrice: deliveryPrice || 0,
                    });
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

    const getBillStyles = (isSelected: boolean) => ({
        className: isSelected ? 'bg-primary text-white' : '',
        priceStrClassName: isSelected ? 'text-white' : '',
    });

    const handleAddNewBill = () => {
        if (!canAddMultipleBills) return;

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
            currency: COMPANYS_CURRENCY,
            nestedItems: [
                ...localBill.orderItems.map((it) => ({
                    name: it.name,
                    price: it.price,
                    currency: COMPANYS_CURRENCY,
                    quantity: it.quantity || 1,
                    onClick: () => {},
                })),
                ...localBill.pendingItems.map((item) => ({
                    name: item.name,
                    price: item.price,
                    currency: COMPANYS_CURRENCY,
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
                        : selectedOrderTypeLabel
                }
                items={isAsideOrdersLoading ? [] : bill}
                buttons={buttons}
                onSelectItem={setSelectedBill}
                selectedItemId={selectedBill}
                onAddNewOrder={
                    canAddMultipleBills ? handleAddNewBill : undefined
                }
                isItemsLoading={isAsideOrdersLoading}
            />
        </Menu>
    ) : (
        <CustomerInformationForm bill={bill} orderItems={orderItems} />
    );
};

export default CreateOrder;
