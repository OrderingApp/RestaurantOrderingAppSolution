import { NotDineInOrder } from '@/helpers/interfaces/orders';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import { fetchWithToken } from '@/helpers/utils/utils';
import { useQuery } from '@tanstack/react-query';

export const useQueryOrdersByType = (type: string) =>
    useQuery({
        queryKey: [OrdersItems.BY_TYPE, type],
        queryFn: () =>
            fetchWithToken(
                'orders',
                `non-dinein-orders?orderType=${type}&date=${new Date().toLocaleTimeString()}`
            ).then((response) => response as NotDineInOrder[]),
    });

export default useQueryOrdersByType;
