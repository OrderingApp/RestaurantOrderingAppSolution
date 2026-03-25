import { BACKEND_PATHS } from '@/helpers/constants/constants';
import { NotDineInOrder } from '@/helpers/interfaces/orders';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import { fetchWithParams } from '@/helpers/utils/utils';
import { useQuery } from '@tanstack/react-query';

// 1. Definiujemy interfejs dla parametrów
export interface UseQueryOrdersByTypeParams {
    type?: string;
    statuses?: string[];
    paymentStatuses?: string[];
}

// 2. Hook przyjmuje jeden obiekt z destrukturyzacją i domyślnymi wartościami
const useQueryOrdersByType = ({
    type,
    statuses = [],
    paymentStatuses = [],
}: UseQueryOrdersByTypeParams) => {
    const params = new URLSearchParams();

    if (type) params.append('orderType', type);

    // Zawsze dodajemy obecną datę w formacie zgodnym ze Swaggerem
    params.append('date', new Date().toISOString());

    statuses.forEach((status) => params.append('statuses', status));
    paymentStatuses.forEach((paymentStatus) =>
        params.append('paymentStatuses', paymentStatus)
    );

    return useQuery({
        // queryKey reaguje teraz na wszystkie przekazane filtry
        queryKey: [OrdersItems.BY_TYPE, type, statuses, paymentStatuses],
        queryFn: () =>
            fetchWithParams(
                BACKEND_PATHS.Orders,
                `non-dinein-orders?${params.toString()}`
            ).then((response) => response as NotDineInOrder[]),
    });
};

export default useQueryOrdersByType;
