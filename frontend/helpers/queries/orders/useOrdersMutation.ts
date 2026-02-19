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
    type: 'create' | 'update' | 'delete',
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
                url += `/${id}`;
                method = 'DELETE';
            }

            // diagnostic log
            console.log('[useOrdersMutation] mutationFn called', {
                type,
                orderKind,
                id,
                data,
                url,
                method,
            });

            // detect DOM nodes or circular-prone objects in data
            const seenPaths: string[] = [];
            const findDom = (obj: unknown, path = '', depth = 0): boolean => {
                if (depth > 4 || obj === null || typeof obj !== 'object')
                    return false;
                const ctorName =
                    obj &&
                    (obj as { constructor?: { name?: string } }).constructor
                        ? (obj as { constructor?: { name?: string } })
                              .constructor!.name || ''
                        : '';
                if (ctorName && /HTML|Element|Node/.test(ctorName)) {
                    seenPaths.push(path || 'root');
                    return true;
                }
                try {
                    for (const key of Object.keys(obj as object)) {
                        if (
                            findDom(
                                (obj as Record<string, unknown>)[key],
                                path ? `${path}.${key}` : key,
                                depth + 1
                            )
                        )
                            return true;
                    }
                } catch {
                    // ignore traversal errors
                }
                return false;
            };

            if (findDom(data)) {
                console.error(
                    '[useOrdersMutation] Found DOM-like value in payload at paths:',
                    seenPaths
                );
            }

            const response = await fetch(url, {
                method,
                headers: { 'Content-Type': 'application/json' },
                body: type !== 'delete' ? JSON.stringify(data) : undefined,
            });

            if (!response.ok)
                throw new Error(`Failed to ${type} ${orderKind} order`);

            return response.json();
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
