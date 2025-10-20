import { NotDineInOrder } from '@/helpers/interfaces/orders';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import { fetchWithToken } from '@/helpers/utils/utils';
import { useQuery } from '@tanstack/react-query';

export const useQueryOrdersByType = (type: string, statuses: string[]) => {
    const params = new URLSearchParams({
        orderType: type,
        date: new Date().toLocaleTimeString(),
    });

    statuses.forEach((status) => params.append('statuses', status));
    return useQuery({
        queryKey: [OrdersItems.BY_TYPE, type, statuses],
        queryFn: () =>
            fetchWithToken(
                'orders',
                `non-dinein-orders?${params.toString()}`
            ).then((response) => response as NotDineInOrder[]),
    });
};

export default useQueryOrdersByType;
