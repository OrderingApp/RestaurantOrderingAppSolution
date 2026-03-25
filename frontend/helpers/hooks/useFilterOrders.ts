'use client';

import { useMemo } from 'react';
import { useSearchParams } from 'next/navigation';

import {
    ORDER_STATUSES,
    ordersTypes,
    PAYMENT_STATUSES,
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
        [SEARCH_PARAMS_NAMES.PAYMENT_STATUS]: paymentStatusParam,
    } = params;

    const orderType = orderTypeParam || ordersTypes[language][0]?.id;
    const allOrderStatuses = Object.values(ORDER_STATUSES);
    const allPaymentStatuses = Object.values(PAYMENT_STATUSES);

    const getFilters = () => {
        if (paymentStatusParam === PAYMENT_STATUSES.PAID) {
            return {
                statuses: allOrderStatuses.filter(
                    (status) => status !== ORDER_STATUSES.CLOSED
                ),
                paymentStatuses: [PAYMENT_STATUSES.PAID],
            };
        }

        if (orderStatusParam === 'All') {
            return {
                statuses: [ORDER_STATUSES.ONGOING],
                paymentStatuses: allPaymentStatuses,
            };
        }

        if (orderStatusParam === ORDER_STATUSES.ONGOING) {
            return {
                statuses: [ORDER_STATUSES.ONGOING],
                paymentStatuses: [
                    PAYMENT_STATUSES.UNPAID,
                    PAYMENT_STATUSES.PARTIALPAID,
                ],
            };
        }

        if (orderStatusParam) {
            return {
                statuses: [orderStatusParam],
                paymentStatuses: paymentStatusParam
                    ? [paymentStatusParam]
                    : allPaymentStatuses,
            };
        }

        return {
            statuses: [ORDER_STATUSES.ONGOING],
            paymentStatuses: allPaymentStatuses,
        };
    };

    const { statuses, paymentStatuses } = getFilters();
    const { data } = useQueryOrdersByType({
        type: orderType,
        statuses,
    });

    const inputValue = searchParams.get(SEARCH_PARAMS_NAMES.NAME);

    const filteredOrders = useMemo(() => {
        if (!data) return [];

        const paymentFiltered =
            paymentStatuses.length === allPaymentStatuses.length
                ? data
                : data.filter(
                      (order) =>
                          !!order.paymentStatus &&
                          paymentStatuses.includes(order.paymentStatus)
                  );

        if (!inputValue) return paymentFiltered;

        const query = inputValue.toLowerCase().trim();
        const queryDigits = query.replace(/\D/g, '');

        return paymentFiltered.filter((order) => {
            const address = order.address?.toLowerCase() ?? '';
            const phone = order.phoneNumber ?? '';
            const phoneDigits = phone.replace(/\D/g, '');

            return (
                address.includes(query) ||
                phone.toLowerCase().includes(query) ||
                (queryDigits.length > 0 && phoneDigits.includes(queryDigits))
            );
        });
    }, [allPaymentStatuses.length, data, inputValue, paymentStatuses]);

    return { filteredOrders };
};

export default useFilterOrders;
