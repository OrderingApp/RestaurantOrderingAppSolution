'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';

import { ORDER_TYPES } from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import { useLanguage } from '@/providers/LanguageProvider';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import { ORDERS_QUERY_KEY } from './useQueryOrders';
import {
    ConfirmOrderBillsInput,
    syncOrderBills,
    BillForSync,
    BillItemForSync,
    OrderItemStatusUpdateForSync,
    OrderItemUpdateForSync,
} from './orderBillsSyncService';

export type {
    BillForSync,
    BillItemForSync,
    ConfirmOrderBillsInput,
    OrderItemStatusUpdateForSync,
    OrderItemUpdateForSync,
};

export interface ConfirmOrderBillsMutationInput
    extends Omit<ConfirmOrderBillsInput, 'orderType'> {
    orderType?: ORDER_TYPES;
}

export interface UseConfirmOrderBillsMutationOptions {
    orderType?: ORDER_TYPES;
    onSuccess?: () => void | Promise<void>;
    onError?: (error: unknown) => void;
}

const useConfirmOrderBillsMutation = (
    options?: UseConfirmOrderBillsMutationOptions
) => {
    const queryClient = useQueryClient();
    const { language } = useLanguage();

    return useMutation({
        mutationFn: async ({
            orderType,
            ...rest
        }: ConfirmOrderBillsMutationInput) => {
            const resolvedOrderType =
                orderType || options?.orderType || ORDER_TYPES.DINEIN;

            await syncOrderBills({
                ...rest,
                orderType: resolvedOrderType,
            });

            return resolvedOrderType;
        },
        onSuccess: async (resolvedOrderType) => {
            await queryClient.invalidateQueries({
                queryKey: [
                    ...ORDERS_QUERY_KEY,
                    OrdersItems.BY_TYPE,
                    resolvedOrderType,
                ],
            });

            await queryClient.invalidateQueries({
                queryKey: [OrdersItems.BY_TYPE, resolvedOrderType],
            });

            await queryClient.invalidateQueries({
                queryKey: ORDERS_QUERY_KEY,
            });

            if (resolvedOrderType === ORDER_TYPES.DINEIN) {
                await queryClient.invalidateQueries({ queryKey: ['tables'] });
            }

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

export default useConfirmOrderBillsMutation;
