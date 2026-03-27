'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';

import { BACKEND_URL } from '@/helpers/constants/constants';
import { OrdersItems } from '@/helpers/utils/queryKeys';

import { ORDERS_QUERY_KEY } from './useQueryOrders';

export interface SplitOrderGroupInput {
    orderItemIds: string[];
}

export interface SplitOrderMutationInput {
    orderId: string;
    splitGroups: SplitOrderGroupInput[];
}

export interface UseSplitOrderMutationOptions {
    onSuccess?: () => void | Promise<void>;
    onError?: (error: unknown) => void;
}

const useSplitOrderMutation = (options?: UseSplitOrderMutationOptions) => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async ({
            orderId,
            splitGroups,
        }: SplitOrderMutationInput) => {
            if (!orderId) {
                throw new Error('Missing order id');
            }

            const response = await fetch(
                `${BACKEND_URL}/orders/${orderId}/split`,
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ splitGroups }),
                }
            );

            if (!response.ok) {
                const contentType = response.headers.get('content-type') || '';

                if (contentType.includes('application/json')) {
                    const body = await response.json();
                    throw new Error(
                        body?.errorMessage || 'Failed to split order.'
                    );
                }

                const text = await response.text();
                throw new Error(text || 'Failed to split order.');
            }

            const contentType = response.headers.get('content-type') || '';
            return contentType.includes('application/json')
                ? response.json()
                : null;
        },
        onSuccess: async (_, variables) => {
            await queryClient.invalidateQueries({
                queryKey: [OrdersItems.BY_ID, variables.orderId],
            });

            await queryClient.invalidateQueries({
                queryKey: [OrdersItems.BY_TYPE],
            });

            await queryClient.invalidateQueries({
                queryKey: ORDERS_QUERY_KEY,
            });

            await options?.onSuccess?.();
        },
        onError: (error) => {
            options?.onError?.(error);
        },
    });
};

export default useSplitOrderMutation;
