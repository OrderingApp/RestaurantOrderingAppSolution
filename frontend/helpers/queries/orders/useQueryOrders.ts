import { BACKEND_PATHS, BACKEND_URL } from '@/helpers/constants/constants';
import { Order } from '@/helpers/interfaces/orders';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import { useQuery } from '@tanstack/react-query';

export const fetchOrders = async (): Promise<Order[]> => {
    const res = await fetch(`${BACKEND_URL}/${BACKEND_PATHS.Orders}`);

    if (!res.ok) throw new Error('Failed to fetch orders');

    return res.json();
};

export const ORDERS_QUERY_KEY = [OrdersItems.ALL];

const useQueryOrders = () =>
    useQuery({
        queryKey: ORDERS_QUERY_KEY,
        queryFn: fetchOrders,
    });

export default useQueryOrders;
