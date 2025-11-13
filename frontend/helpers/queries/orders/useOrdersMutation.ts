import { useMutation, useQueryClient } from '@tanstack/react-query';
import { BACKEND_URL } from '@/helpers/constants/constants';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import { useRouter } from 'next/navigation';
import { Order, OrderDto, OrderKind } from '@/helpers/interfaces/orders';

const useOrderMutation = (
    type: 'create' | 'update' | 'delete',
    orderKind: OrderKind
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
        },

        onSettled: () => {
            queryClient.invalidateQueries({
                queryKey: [OrdersItems.BY_TYPE, orderKind],
            });
            router.push(`/orders`);
        },
    });
};

export default useOrderMutation;
