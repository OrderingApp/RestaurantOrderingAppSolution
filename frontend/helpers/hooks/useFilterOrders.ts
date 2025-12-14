'use client';

import { useMemo } from 'react';
import { useSearchParams } from 'next/navigation';

import {
    ORDER_STATUSES,
    ordersTypes,
    SEARCH_PARAMS_NAMES,
} from '../constants/constants';

import useLanguage from './useLanguage';
import useQueryOrdersByType from '../queries/orders/useQueryOrdersByType';

const useFilterOrders = () => {
    const searchParams = useSearchParams();
    const { language } = useLanguage();
    const params = Object.fromEntries(searchParams.entries());
    const {
        [SEARCH_PARAMS_NAMES.ORDER_TYPE]: orderTypeParam,
        [SEARCH_PARAMS_NAMES.ORDER_STATUS]: orderStatusParam,
    } = params;

    const orderType = orderTypeParam || ordersTypes[language][0]?.id;

    const getOrderStatus = () => {
        if (orderStatusParam === 'Ongoing') {
            return [
                ORDER_STATUSES.ONGOING,
                ORDER_STATUSES.PAID_AND_READY_TO_PREPARE,
            ];
        }
        if (orderStatusParam) {
            return [orderStatusParam];
        }

        return [
            ORDER_STATUSES.ONGOING,
            ORDER_STATUSES.PAID_AND_READY_TO_PREPARE,
        ];
    };

    const { data } = useQueryOrdersByType(orderType, getOrderStatus());

    const inputValue = searchParams.get(SEARCH_PARAMS_NAMES.NAME);

    const filteredOrders = useMemo(() => {
        if (!data) return [];
        if (!inputValue) return data;

        const query = inputValue.toLowerCase();
        return data.filter(
            (order) =>
                order.phoneNumber?.includes(query) ||
                order.address?.toLowerCase().includes(query)
        );
    }, [data, inputValue]);

    return { filteredOrders };
};
export default useFilterOrders;
