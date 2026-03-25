import { Order } from '@/helpers/interfaces/orders';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import { fetchWithParams } from '@/helpers/utils/utils';
import { useQuery } from '@tanstack/react-query';

export const useQuerySingleOrder = (id: string) =>
    useQuery({
        queryKey: [OrdersItems.BY_ID, id],
        enabled: !!id,
        queryFn: () =>
            fetchWithParams('orders', id).then((response) => response as Order),
    });

export default useQuerySingleOrder;
