import { BACKEND_PATHS } from '@/helpers/constants/constants';
import { NotDineInOrder } from '@/helpers/interfaces/orders';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import { fetchWithParams } from '@/helpers/utils/utils';
import { useQuery } from '@tanstack/react-query';

const useQueryOrdersByType = (type: string, statuses: string[]) => {
    const params = new URLSearchParams({
        orderType: type,
        date: new Date().toLocaleTimeString(),
    });

    statuses.forEach((status) => params.append('statuses', status));
    return useQuery({
        queryKey: [OrdersItems.BY_TYPE, type, statuses],
        queryFn: () =>
            fetchWithParams(
                BACKEND_PATHS.Orders,
                `non-dinein-orders?${params.toString()}`
            ).then((response) => response as NotDineInOrder[]),
    });
};

export default useQueryOrdersByType;
