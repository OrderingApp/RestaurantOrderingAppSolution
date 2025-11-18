import DaySummary from '@/components/pages/daySummary/DaySummary';
import {
    fetchOrders,
    ORDERS_QUERY_KEY,
} from '@/helpers/queries/orders/useQueryOrders';
import getQueryClient from '@/lib/react-query/getQueryClient';

import { dehydrate, HydrationBoundary } from '@tanstack/react-query';

const DaySummaryPage = async () => {
    const queryClient = getQueryClient();

    await queryClient.prefetchQuery({
        queryKey: ORDERS_QUERY_KEY,
        queryFn: fetchOrders,
    });

    return (
        <HydrationBoundary state={dehydrate(queryClient)}>
            <DaySummary />;
        </HydrationBoundary>
    );
};

export default DaySummaryPage;
