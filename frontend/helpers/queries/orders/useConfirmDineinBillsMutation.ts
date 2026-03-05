'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';

import { BACKEND_URL, ORDER_TYPES } from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import { useLanguage } from '@/providers/LanguageProvider';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import { ORDERS_QUERY_KEY } from './useQueryOrders';

interface BillItemForSync {
    id: string;
    quantity: number;
    discount?: number;
}

interface BillForSync {
    id: string;
    isNew: boolean;
    pendingItems: BillItemForSync[];
}

interface ConfirmDineinBillsInput {
    bills: BillForSync[];
    tableId?: string;
    deliveryPrice: number;
}

interface UseConfirmDineinBillsMutationOptions {
    onSuccess?: () => void | Promise<void>;
    onError?: (error: unknown) => void;
}

const toOrderItemsPayload = (items: BillItemForSync[]) =>
    items.flatMap((item) =>
        Array.from({ length: item.quantity }, () => ({
            specialInstructions: '',
            discount: item.discount || 0,
            menuItemId: item.id,
            extraIngredients: [],
            removedIngredientIds: [],
        }))
    );

const getCurrentOrderDateTime = () => {
    const now = new Date();
    const dateStr = now.toISOString().split('T')[0];
    const timeStr = now.toTimeString().split(':').slice(0, 2).join(':');

    return `${dateStr}T${timeStr}:00`;
};

const useConfirmDineinBillsMutation = (
    options?: UseConfirmDineinBillsMutationOptions
) => {
    const queryClient = useQueryClient();
    const { language } = useLanguage();

    return useMutation({
        mutationFn: async ({
            bills,
            tableId,
            deliveryPrice,
        }: ConfirmDineinBillsInput) => {
            const dateTimeStr = getCurrentOrderDateTime();
            const resolvedTableId = tableId || '';

            const billsWithPendingItems = bills.filter(
                (bill) => bill.pendingItems.length > 0
            );

            await Promise.all(
                billsWithPendingItems.map(async (bill) => {
                    const payloadOrderItems = toOrderItemsPayload(
                        bill.pendingItems
                    );

                    if (bill.isNew) {
                        const createRes = await fetch(
                            `${BACKEND_URL}/orders/dinein`,
                            {
                                method: 'POST',
                                headers: { 'Content-Type': 'application/json' },
                                body: JSON.stringify({
                                    createdAt: dateTimeStr,
                                    discount: 0,
                                    deliveryPrice,
                                    customerInformation: {
                                        phoneNumber: '',
                                        orderCompletionType: 'Immediate',
                                        preferredPaymentMethod: 'Card',
                                        additionalInstructions: '',
                                        expectedOrderCompletion: dateTimeStr,
                                    },
                                    orderItems: payloadOrderItems,
                                    tableId: resolvedTableId,
                                }),
                            }
                        );

                        if (!createRes.ok) {
                            const text = await createRes.text().catch(() => '');
                            throw new Error(
                                text || 'Failed to create dine-in bill'
                            );
                        }

                        return;
                    }

                    const addItemsRes = await fetch(
                        `${BACKEND_URL}/orders/${bill.id}/order-items`,
                        {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify(payloadOrderItems),
                        }
                    );

                    if (!addItemsRes.ok) {
                        const text = await addItemsRes.text().catch(() => '');
                        throw new Error(text || 'Failed to add items to bill');
                    }
                })
            );
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: [
                    ...ORDERS_QUERY_KEY,
                    OrdersItems.BY_TYPE,
                    ORDER_TYPES.DINEIN,
                ],
            });

            await queryClient.invalidateQueries({ queryKey: ['tables'] });

            toast.success(
                languagePacks[language].createOrderPage.confirmation ||
                    'Bill confirmed successfully'
            );

            await options?.onSuccess?.();
        },
        onError: (err) => {
            console.error('Failed to sync bills', err);
            toast.error(
                languagePacks[language].createOrderPage.error ||
                    'Failed to confirm bill'
            );

            options?.onError?.(err);
        },
    });
};

export default useConfirmDineinBillsMutation;
