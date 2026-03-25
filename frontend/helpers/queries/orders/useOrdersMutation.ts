'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { BACKEND_URL } from '@/helpers/constants/constants';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import { useRouter } from 'next/navigation';
import { Order, OrderDto, OrderKind } from '@/helpers/interfaces/orders';

type UseOrderMutationOptions = {
    redirectOnSettled?: boolean; // default true
    onSuccess?: (data: unknown) => void;
    onError?: (err: unknown) => void;
};

const useOrderMutation = (
    type: 'create' | 'update' | 'delete' | 'close',
    orderKind: OrderKind,
    options?: UseOrderMutationOptions
) => {
    const queryClient = useQueryClient();
    const router = useRouter();

    return useMutation({
        mutationFn: async ({ id, data }: { id?: string; data?: OrderDto }) => {
            let url = `${BACKEND_URL}/orders/${orderKind}`;
            let method = 'POST';

            if (type === 'update' && id) {
                url += `/${id}`;
                method = 'PUT';
            } else if (type === 'delete' && id) {
                url = `${BACKEND_URL}/orders/${id}`;
                method = 'DELETE';
            } else if (type === 'close' && id) {
                url = `${BACKEND_URL}/orders/${id}/close`;
                method = 'PATCH';
            }

            const response = await fetch(url, {
                method,
                headers: { 'Content-Type': 'application/json' },
                body:
                    type === 'create' || type === 'update'
                        ? JSON.stringify(data)
                        : undefined,
            });

            if (!response.ok)
                throw new Error(`Failed to ${type} ${orderKind} order`);

            if (response.status === 204) {
                return null;
            }

            const contentType = response.headers.get('content-type') || '';
            return contentType.includes('application/json')
                ? response.json()
                : null;
        },

        onMutate: async ({ id, data }: { id?: string; data?: OrderDto }) => {
            await queryClient.cancelQueries({
                queryKey: [OrdersItems.BY_TYPE, orderKind],
            });

            const previousOrders = queryClient.getQueryData<Order[]>([
                OrdersItems.BY_TYPE,
                orderKind,
            ]);

            queryClient.setQueryData(
                [OrdersItems.BY_TYPE, orderKind],
                (old: Order[] = []) => {
                    if (type === 'create' && data) {
                        return [
                            ...old,
                            { ...data, id: Date.now(), kind: orderKind },
                        ];
                    } else if (type === 'update' && id && data) {
                        return old.map((order) =>
                            order.id === id
                                ? { ...order, ...data, kind: orderKind }
                                : order
                        );
                    } else if (type === 'delete' && id) {
                        return old.filter((order) => order.id !== id);
                    } else if (type === 'close' && id) {
                        return old.map((order) =>
                            order.id === id
                                ? { ...order, orderStatus: 'Closed' }
                                : order
                        );
                    }
                    return old;
                }
            );

            return { previousOrders };
        },

        onError: (_, __, context) => {
            if (context?.previousOrders) {
                queryClient.setQueryData(
                    [OrdersItems.BY_TYPE, orderKind],
                    context.previousOrders
                );
            }

            options?.onError?.(_ as unknown);
        },

        onSettled: (data, error) => {
            queryClient.invalidateQueries({
                queryKey: [OrdersItems.BY_TYPE, orderKind],
            });

            if (options?.redirectOnSettled === false) {
                // don't redirect
            } else {
                router.push(`/orders`);
            }

            if (!error) options?.onSuccess?.(data as unknown);
        },
    });
};

export default useOrderMutation;
