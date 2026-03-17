'use client';

import { useEffect, useMemo, useState } from 'react';

import { COMPANYS_CURRENCY } from '@/helpers/constants/constants';
import type { Currency } from '@/helpers/type/types';
import type { AddItemHandler } from '@/components/shared/cards/MenuItem';

export type LocalBillItem = {
    id: string; // local line item id (unique per row in the receipt)
    menuItemId: string; // menu item id
    name: string;
    price: number;
    currency: Currency;
    quantity: number;
    discount?: number;
};

export type LocalBill = {
    id: string; // either existing bill id or 'new-X' for new bills
    isNew: boolean;
    orderItems: LocalBillItem[];
    pendingItems: LocalBillItem[]; // items added from menu, not yet synced to backend
    totalAmount: number;
};

type ExistingOrderForTable = {
    id: string;
    totalAmount?: number;
    orderItems: Array<{
        price: number;
        quantity?: number;
        discount?: number;
        menuItem: {
            id: string;
            name: string;
        };
    }>;
};

const createLocalLineItemId = () => {
    if (typeof crypto !== 'undefined' && 'randomUUID' in crypto) {
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        return (crypto as any).randomUUID() as string;
    }

    return `${Date.now()}-${Math.random().toString(16).slice(2)}`;
};

export const useCreateOrderLocalBills = ({
    existingOrdersForTable,
    isAsideOrdersLoading,
    canAddMultipleBills,
    getHasExtras,
    getExtrasSignature,
}: {
    existingOrdersForTable: ExistingOrderForTable[];
    isAsideOrdersLoading: boolean;
    canAddMultipleBills: boolean;
    getHasExtras: (orderItemId: string) => boolean;
    getExtrasSignature: (orderItemId: string) => string;
}) => {
    const [selectedBill, setSelectedBill] = useState<string | null>(null);
    const [localBills, setLocalBills] = useState<LocalBill[]>([]);
    const [newBillCounter, setNewBillCounter] = useState(0);

    // Auto-select first existing bill or 'new' if none exist
    useEffect(() => {
        if (selectedBill === null) {
            if (localBills.length > 0) {
                setSelectedBill(localBills[0].id);
            } else if (!isAsideOrdersLoading && existingOrdersForTable.length) {
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
                const initialized: LocalBill[] = existingOrdersForTable.map(
                    (order) => ({
                        id: order.id,
                        isNew: false,
                        orderItems: order.orderItems.map((item) => ({
                            id: item.menuItem.id,
                            menuItemId: item.menuItem.id,
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
                return;
            }

            if (!isAsideOrdersLoading) {
                setLocalBills([
                    {
                        id: 'new',
                        isNew: true,
                        orderItems: [],
                        pendingItems: [],
                        totalAmount: 0,
                    },
                ]);
            }
        }
    }, [existingOrdersForTable, localBills.length, isAsideOrdersLoading]);

    const addItemToSelectedBill: AddItemHandler = (item) => {
        setLocalBills((prev) => {
            if (!selectedBill) return prev;

            const newPendingItem: LocalBillItem = {
                id: createLocalLineItemId(),
                menuItemId: item.id,
                name: item.name,
                price: item.price,
                currency: item.currency,
                quantity: item.quantity,
                discount: item.discount,
            };

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
                        pendingItems: [newPendingItem],
                        totalAmount: 0,
                    },
                ];
            }

            if (billIndex === -1) return prev;

            const updatedBills = [...prev];

            // Merge into an existing pending row ONLY if it has no extras.
            // If a row has extras, adding the same dish should create a new row.
            const existingNoExtrasIndex = updatedBills[
                billIndex
            ].pendingItems.findIndex(
                (i) =>
                    i.menuItemId === newPendingItem.menuItemId &&
                    !getHasExtras(i.id)
            );

            if (existingNoExtrasIndex !== -1) {
                updatedBills[billIndex] = {
                    ...updatedBills[billIndex],
                    pendingItems: updatedBills[billIndex].pendingItems.map(
                        (i, idx) =>
                            idx === existingNoExtrasIndex
                                ? {
                                      ...i,
                                      quantity:
                                          i.quantity + newPendingItem.quantity,
                                  }
                                : i
                    ),
                };

                return updatedBills;
            }

            updatedBills[billIndex] = {
                ...updatedBills[billIndex],
                pendingItems: [
                    ...updatedBills[billIndex].pendingItems,
                    newPendingItem,
                ],
            };

            return updatedBills;
        });
    };

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

    const resetBillsState = () => {
        setLocalBills([]);
        setSelectedBill(null);
        setNewBillCounter(0);
    };

    const mergeDuplicatePendingItemsForBill = (billId: string) => {
        const billIndex = localBills.findIndex((b) => b.id === billId);
        if (billIndex === -1) return [] as string[];

        const bill = localBills[billIndex];
        if (!bill.pendingItems.length) return [] as string[];

        const removedOrderItemIds: string[] = [];
        const removedSet = new Set<string>();
        const firstIdByKey = new Map<string, string>();
        const aggregatedQtyByFirstId = new Map<string, number>();

        for (const item of bill.pendingItems) {
            const extrasSig = getExtrasSignature(item.id);
            const key = `${item.menuItemId}::${extrasSig}`;

            const firstId = firstIdByKey.get(key);
            if (!firstId) {
                firstIdByKey.set(key, item.id);
                aggregatedQtyByFirstId.set(item.id, item.quantity);
                continue;
            }

            aggregatedQtyByFirstId.set(
                firstId,
                (aggregatedQtyByFirstId.get(firstId) ?? 0) + item.quantity
            );
            removedOrderItemIds.push(item.id);
            removedSet.add(item.id);
        }

        if (removedOrderItemIds.length === 0) return [] as string[];

        const nextBills = [...localBills];
        nextBills[billIndex] = {
            ...bill,
            pendingItems: bill.pendingItems
                .filter((x) => !removedSet.has(x.id))
                .map((x) => ({
                    ...x,
                    quantity: aggregatedQtyByFirstId.get(x.id) ?? x.quantity,
                })),
        };

        setLocalBills(nextBills);
        return removedOrderItemIds;
    };

    const mergeDuplicatePendingItemsForSelectedBill = () => {
        if (!selectedBill) return [] as string[];
        return mergeDuplicatePendingItemsForBill(selectedBill);
    };

    const selectedBillData = useMemo(
        () => localBills.find((b) => b.id === selectedBill),
        [localBills, selectedBill]
    );

    return {
        localBills,
        setLocalBills,
        selectedBill,
        setSelectedBill,
        selectedBillData,
        addItemToSelectedBill,
        handleAddNewBill,
        resetBillsState,
        mergeDuplicatePendingItemsForBill,
        mergeDuplicatePendingItemsForSelectedBill,
    };
};
