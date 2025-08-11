'use client';

import { useRouter, useSearchParams } from 'next/navigation';
import { ReactNode } from 'react';

import { SEARCH_PARAMS_NAMES } from '@/helpers/constants/constants';

import useQuerySingleOrder from '@/helpers/queries/orders/useQuerySingleOrder';

import Modal from './Modal';
import Payment from './Payment';
import languagePacks from '@/helpers/constants/languagePacks';
import useLanguage from '@/helpers/hooks/useLanguage';

const PaymentDetails = ({ children }: { children: ReactNode }) => {
    const { language } = useLanguage();
    const seachParams = useSearchParams();
    const router = useRouter();

    const orderId = useSearchParams().get(SEARCH_PARAMS_NAMES.ORDER_ID);
    const paymentMode = useSearchParams().get(SEARCH_PARAMS_NAMES.PAYMENT);

    const { data } = useQuerySingleOrder(orderId || '');
    const items = data?.orderItems || [];

    const {
        paymentDetails: { bill, product, quantity, productPrice, total },
    } = languagePacks[language];

    const closePaymentModal = () => {
        const params = new URLSearchParams(seachParams.toString());
        params.delete('payment');
        router.push(`/orders?${params.toString()}`);
    };

    return (
        <div className="bg-[#F6F6F6] w-full rounded-3xl h-full flex flex-row ">
            <div className="bg-gray-100 py-6 px-4 rounded-lg shadow-sm flex-1">
                <h2 className="text-xl font-bold mb-4 border-b border-gray-400 py-5 pt-2">
                    {bill}
                </h2>

                <table className="w-full border-collapse">
                    <thead>
                        <tr className="border-b-2 border-gray-400">
                            <th className="text-left py-3 px-2 font-semibold">
                                {product}
                            </th>
                            <th className="text-center py-3 px-2 font-semibold">
                                {quantity}
                            </th>
                            <th className="text-center py-3 px-2 font-semibold">
                                {productPrice}
                            </th>
                            <th className="text-center py-3 px-2 font-semibold">
                                {total}
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        {items.map((item, index) => (
                            <tr
                                key={index}
                                className="border-b border-gray-300 py-2"
                            >
                                <td className="py-5 px-2">
                                    {item.menuItem.name}
                                </td>
                                <td className="text-center py-3 px-2">{1}</td>
                                <td className="text-center py-3 px-2">
                                    {item.price}zł
                                </td>
                                <td className="text-center py-3 px-2">
                                    {item.price * 1}zł
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>

                <div className="mt-6 flex justify-end mr-10">
                    <div className="border-t-2 border-gray-400 pt-2">
                        <div className="flex justify-between items-center min-w-48">
                            <span className="font-bold text-lg">{total}</span>
                            <span className="font-bold text-lg">
                                {data?.totalAmount}zł
                            </span>
                        </div>
                    </div>
                </div>
            </div>
            {children}
            {paymentMode === 'true' && (
                <Modal onClose={closePaymentModal}>
                    <Payment
                        onClick={closePaymentModal}
                        totalAmount={data?.totalAmount || 0}
                    />
                </Modal>
            )}
        </div>
    );
};

export default PaymentDetails;

//TODO change currency
