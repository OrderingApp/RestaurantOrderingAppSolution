'use client';

import { useRouter, useSearchParams } from 'next/navigation';
import { ReactNode } from 'react';

import { SEARCH_PARAMS_NAMES } from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import useLanguage from '@/helpers/hooks/useLanguage';
import type { OrderKind } from '@/helpers/interfaces/orders';
import useQuerySingleOrder from '@/helpers/queries/orders/useQuerySingleOrder';
import { formatPriceStr } from '@/helpers/utils/prices';

import OrdersPaymentTable from '../tables/OrdersPaymentTable';
import Modal from './Modal';
import Payment from './Payment';

const currency = 'pln';

const PaymentDetails = ({ children }: { children: ReactNode }) => {
    const router = useRouter();
    const searchParams = useSearchParams();
    const { language } = useLanguage();

    const orderId = searchParams.get(SEARCH_PARAMS_NAMES.ORDER_ID) ?? '';
    const isPaymentOpen =
        searchParams.get(SEARCH_PARAMS_NAMES.PAYMENT) === 'true';

    const { data, isLoading, isError } = useQuerySingleOrder(orderId);
    const items = data?.orderItems ?? [];

    const totalAmount = data?.totalAmount ?? 0;

    const remainingRaw =
        data?.remainingAmount ??
        data?.unpaidAmount ??
        (data?.totalAmount != null ? data.totalAmount : 0);

    const remainingToPay =
        totalAmount > 0
            ? Math.min(Math.max(remainingRaw, 0), totalAmount)
            : Math.max(remainingRaw, 0);

    const paidSoFar = Math.max(totalAmount - remainingToPay, 0);

    const orderKind: OrderKind =
        data?.orderType === 'Delivery'
            ? 'Delivery'
            : data?.orderType === 'dinein'
              ? 'dinein'
              : 'Takeaway';

    const {
        paymentDetails: { bill },
        ordersPage: {
            orderOptionsModal: {
                summary: { paidAmount, remainingAmount },
            },
        },
    } = languagePacks[language];

    const closePaymentModal = () => {
        const params = new URLSearchParams(searchParams.toString());
        params.delete(SEARCH_PARAMS_NAMES.PAYMENT);

        const query = params.toString();
        router.push(query ? `/orders?${query}` : '/orders');
    };

    return (
        <>
            <Modal isOpen={isPaymentOpen} onClose={closePaymentModal}>
                <Payment
                    onClick={closePaymentModal}
                    orderId={orderId}
                    remainingAmount={remainingToPay}
                    orderKind={orderKind}
                />
            </Modal>

            <div className="bg-light-gray w-full rounded-3xl h-full flex flex-row">
                <div className="bg-gray-100 py-6 px-4 rounded-lg shadow-sm flex-1">
                    <h2 className="text-xl font-bold mb-2 py-5 pt-2">{bill}</h2>

                    <OrdersPaymentTable
                        items={items}
                        isLoading={isLoading}
                        isError={isError}
                    />

                    {!isLoading && !isError && (
                        <div className="min-w-60 pt-4">
                            <div className="ml-auto w-full max-w-sm rounded-2xl border border-gray-200 bg-white/70 p-4 shadow-sm">
                                <div className="flex items-center justify-between">
                                    <span className="text-sm font-medium text-dark-gray">
                                        {paidAmount}
                                    </span>
                                    <span className="font-bold text-lg text-green-700">
                                        {formatPriceStr({
                                            currency,
                                            price: paidSoFar,
                                        })}
                                    </span>
                                </div>

                                <div className="my-2 border-t border-gray-200" />

                                <div className="flex items-center justify-between">
                                    <span className="text-sm font-medium text-dark-gray">
                                        {remainingAmount}
                                    </span>
                                    <span className="font-bold text-lg text-orange-600">
                                        {formatPriceStr({
                                            currency,
                                            price: remainingToPay,
                                        })}
                                    </span>
                                </div>
                            </div>
                        </div>
                    )}
                </div>

                {children}
            </div>
        </>
    );
};

export default PaymentDetails;
