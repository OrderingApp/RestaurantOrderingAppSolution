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
    const allPaymentStatuses: string[] =
        Object.values(PAYMENT_STATUSES).map(String);

    const normalizedPaymentStatuses: string[] = paymentStatusParam
        ? paymentStatusParam
              .split(',')
              .map((v) => v.trim())
              .filter(Boolean)
              .map((value) =>
                  allPaymentStatuses.find(
                      (s) => s.toLowerCase() === value.toLowerCase()
                  )
              )
              .filter((v): v is string => !!v)
        : [];

    const getFilters = () => {
        if (orderStatusParam === 'All') {
            return {
                statuses: [ORDER_STATUSES.ONGOING, ORDER_STATUSES.COMPLETED],
                paymentStatuses:
                    normalizedPaymentStatuses.length > 0
                        ? normalizedPaymentStatuses
                        : allPaymentStatuses,
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

        if (orderStatusParam === ORDER_STATUSES.COMPLETED) {
            return {
                statuses: [ORDER_STATUSES.COMPLETED],
                paymentStatuses: [PAYMENT_STATUSES.PAID],
            };
        }

        if (orderStatusParam) {
            return {
                statuses: [orderStatusParam],
                paymentStatuses:
                    normalizedPaymentStatuses.length > 0
                        ? normalizedPaymentStatuses
                        : allPaymentStatuses,
            };
        }

        if (normalizedPaymentStatuses.length > 0) {
            return {
                statuses: [ORDER_STATUSES.ONGOING, ORDER_STATUSES.COMPLETED],
                paymentStatuses: normalizedPaymentStatuses,
            };
        }

        return {
            statuses: [ORDER_STATUSES.ONGOING, ORDER_STATUSES.COMPLETED],
            paymentStatuses: allPaymentStatuses,
        };
    };

    const { statuses, paymentStatuses } = getFilters();
    const { data } = useQueryOrdersByType({
        type: orderType,
        statuses,
        paymentStatuses,
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
