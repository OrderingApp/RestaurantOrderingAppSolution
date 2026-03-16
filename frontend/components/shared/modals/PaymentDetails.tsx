'use client';

import { useRouter, useSearchParams } from 'next/navigation';
import { ReactNode } from 'react';

import { SEARCH_PARAMS_NAMES } from '@/helpers/constants/constants';

import useQuerySingleOrder from '@/helpers/queries/orders/useQuerySingleOrder';

import Modal from './Modal';
import Payment from './Payment';
import languagePacks from '@/helpers/constants/languagePacks';
import useLanguage from '@/helpers/hooks/useLanguage';
import OrdersPaymentTable from '../tables/OrdersPaymentTable';
import { formatPriceStr } from '@/helpers/utils/prices';

const currency = 'pln';

const PaymentDetails = ({ children }: { children: ReactNode }) => {
    const { language } = useLanguage();
    const seachParams = useSearchParams();
    const router = useRouter();

    const orderId = seachParams.get(SEARCH_PARAMS_NAMES.ORDER_ID);
    const paymentMode = seachParams.get(SEARCH_PARAMS_NAMES.PAYMENT);

    const { data, isLoading, isError } = useQuerySingleOrder(orderId || '');
    const items = data?.orderItems || [];

    const {
        paymentDetails: { bill, total },
        generic: { errorMsg },
    } = languagePacks[language];

    const closePaymentModal = () => {
        const params = new URLSearchParams(seachParams.toString());
        params.delete('payment');
        router.push(`/orders?${params.toString()}`);
    };

    return (
        <>
            <Modal isOpen={paymentMode === 'true'} onClose={closePaymentModal}>
                <Payment
                    onClick={closePaymentModal}
                    totalAmount={data?.totalAmount || 0}
                />
            </Modal>

            <div className="bg-light-gray w-full rounded-3xl h-full flex flex-row ">
                <div className="bg-gray-100 py-6 px-4 rounded-lg shadow-sm flex-1">
                    <h2 className="text-xl font-bold mb-2 py-5 pt-2">{bill}</h2>

                    <OrdersPaymentTable
                        items={items}
                        isLoading={isLoading}
                        isError={!orderId || isError}
                        error={errorMsg}
                    />

                    {isError && (
                        <div className="mt-4 p-4 bg-red-100 text-red-700 rounded">
                            {errorMsg}
                        </div>
                    )}

                    {!isLoading && !isError && (
                        <div className="mt-6 flex justify-end mr-10">
                            <div className="flex justify-between items-center min-w-48 border-t-2 border-gray-400 pt-2">
                                <span className="font-bold text-lg">
                                    {total}
                                </span>
                                <span className="font-bold text-lg">
                                    {formatPriceStr({
                                        currency,
                                        price: data?.totalAmount || 0,
                                    })}
                                </span>
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
