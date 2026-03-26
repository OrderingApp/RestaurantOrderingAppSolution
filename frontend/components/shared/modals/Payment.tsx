'use client';
import { useEffect, useMemo, useRef, useState } from 'react';
import { useRouter } from 'next/navigation';
import Image from 'next/image';
import { toast } from 'sonner';

import paymentCard from '@/public/images/svg/payment-card.svg';
import paymentCash from '@/public/images/svg/payment-cash.svg';
import dollar from '@/public/images/svg/dollar.svg';

import Input from '../Input/Input';

import useLanguage from '@/helpers/hooks/useLanguage';
import languagePacks from '@/helpers/constants/languagePacks';
import useCreatePaymentMutation, {
    PaymentMethod,
} from '@/helpers/queries/payments/useCreatePaymentMutation';
import useOrderMutation from '@/helpers/queries/orders/useOrdersMutation';
import type { OrderKind } from '@/helpers/interfaces/orders';
import { formatPriceStr } from '@/helpers/utils/prices';

interface PaymentProps {
    onClick: () => void;
    orderId: string;
    remainingAmount: number;
    orderKind: OrderKind;
}

const MODAL_WIDTH = 445;
const MODAL_HEIGHT = 445;
type PaymentView = 'selectMethod' | 'cashSummary';

interface CashSummarySnapshot {
    customerAmount: number;
    dueAmount: number;
    changeDue: number;
}

const Payment = ({
    onClick,
    orderId,
    remainingAmount,
    orderKind,
}: PaymentProps) => {
    const router = useRouter();
    const { language } = useLanguage();
    const {
        paymentModal: {
            title,
            inputLabel,
            paymentByCard,
            paymentByCash,
            customerAmountLabel,
            orderPriceLabel,
            changeDueLabel,
            confirmCloseOrder,
            exitSummary,
            alreadyPaidMsg,
            amountRangeError,
            toastPaymentCreated,
            toastPaymentFailed,
        },
        ordersPage: {
            orderOptionsModal: {
                toasts: { closeSuccess, closeError },
            },
        },
    } = languagePacks[language];

    const [amountInput, setAmountInput] = useState(
        String(Math.max(remainingAmount, 0))
    );
    const [view, setView] = useState<PaymentView>('selectMethod');
    const activeMethodRef = useRef<PaymentMethod | null>(null);
    const [paymentRecorded, setPaymentRecorded] = useState(false);
    const [cashSummarySnapshot, setCashSummarySnapshot] =
        useState<CashSummarySnapshot | null>(null);

    useEffect(() => {
        if (view === 'cashSummary') return;

        setAmountInput(String(Math.max(remainingAmount, 0)));
        setView('selectMethod');
        activeMethodRef.current = null;
        setPaymentRecorded(false);
        setCashSummarySnapshot(null);
    }, [remainingAmount, view]);

    const parsedAmount = useMemo(() => {
        const normalized = amountInput.replace(',', '.').trim();
        const value = Number(normalized);

        return Number.isFinite(value) ? value : NaN;
    }, [amountInput]);

    const hasOutstandingAmount = remainingAmount > 0;
    const isPositiveAmount = Number.isFinite(parsedAmount) && parsedAmount > 0;
    const isCardAmountOverLimit =
        isPositiveAmount && parsedAmount > remainingAmount;

    const changeDue =
        isPositiveAmount && parsedAmount > remainingAmount
            ? parsedAmount - remainingAmount
            : 0;

    const amountForBackend = Math.min(parsedAmount, remainingAmount);

    const formattedRemaining = formatPriceStr({
        currency: 'pln',
        price: remainingAmount,
    });

    const { mutate: createPayment, isPending } = useCreatePaymentMutation({
        onSuccess: () => {
            toast.success(toastPaymentCreated);

            if (activeMethodRef.current) {
                setPaymentRecorded(true);
                return;
            }
        },
        onError: (error) => {
            const message =
                error instanceof Error ? error.message : toastPaymentFailed;
            toast.error(message);

            if (activeMethodRef.current) {
                setView('selectMethod');
                setPaymentRecorded(false);
                setCashSummarySnapshot(null);
                activeMethodRef.current = null;
            }
        },
    });

    const { mutate: closeOrderMutation, isPending: isCloseOrderPending } =
        useOrderMutation('close', orderKind, {
            redirectOnSettled: false,
            onSuccess: () => {
                toast.success(closeSuccess);
                router.push('/orders');
            },
            onError: () => {
                toast.error(closeError);
            },
        });

    const closeOrder = () => {
        if (!orderId || !paymentRecorded || isCloseOrderPending) return;

        closeOrderMutation({ id: orderId });
    };

    const onPay = (method: PaymentMethod) => {
        if (!hasOutstandingAmount || isPending) return;

        if (method === 'Card') {
            if (!isPositiveAmount) return;

            if (isCardAmountOverLimit) {
                toast.error(
                    amountRangeError.replace('{max}', formattedRemaining)
                );
                return;
            }

            activeMethodRef.current = 'Card';
            setPaymentRecorded(false);
            setCashSummarySnapshot({
                customerAmount: parsedAmount,
                dueAmount: remainingAmount,
                changeDue: 0,
            });
            setView('cashSummary');

            createPayment({
                orderId,
                amount: parsedAmount,
                paymentMethod: 'Card',
            });

            return;
        }

        if (method === 'Cash') {
            if (!isPositiveAmount) return;

            activeMethodRef.current = 'Cash';
            setPaymentRecorded(false);
            setCashSummarySnapshot({
                customerAmount: parsedAmount,
                dueAmount: remainingAmount,
                changeDue,
            });
            setView('cashSummary');

            createPayment({
                orderId,
                amount: amountForBackend,
                paymentMethod: 'Cash',
            });

            return;
        }
    };

    const btnStyle =
        'w-[198px] h-[98px] flex flex-col justify-center items-center gap-2 shadow-[0px_0px_6px_0px_#00000029] rounded-xl';

    const summaryCustomerAmount =
        cashSummarySnapshot?.customerAmount ?? parsedAmount;
    const summaryDueAmount = cashSummarySnapshot?.dueAmount ?? remainingAmount;
    const summaryChangeDue = cashSummarySnapshot?.changeDue ?? changeDue;

    return (
        <div
            className="bg-payment-modal-gradient pt-[0.4rem] rounded-2xl relative"
            style={{ width: MODAL_WIDTH, height: MODAL_HEIGHT }}
        >
            <button
                onClick={onClick}
                className="absolute top-5 right-5 text-xl"
            >
                X
            </button>
            <div className="bg-white  h-full rounded-2xl p-5 flex flex-col gap-8">
                <div className="flex flex-col justify-center items-center pt-6">
                    <h2 className="text-xl font-bold text-center">{title}</h2>
                    <Image src={dollar} alt="dolar-icon" />
                </div>

                {view === 'selectMethod' ? (
                    <>
                        <div className="w-full px-10">
                            <Input
                                type="number"
                                label={inputLabel}
                                min={0.01}
                                step="0.01"
                                value={amountInput}
                                onChange={(event) =>
                                    setAmountInput(event.target.value)
                                }
                                variant="secondary"
                                inputClassName="w-full"
                                labelClassName="text-xl font-bold text-center pb-4"
                            />

                            {!hasOutstandingAmount && (
                                <p className="text-center text-sm text-paid mt-3">
                                    {alreadyPaidMsg}
                                </p>
                            )}
                        </div>

                        <ul className="flex gap-2 mt-2">
                            <li>
                                <button
                                    className={btnStyle}
                                    onClick={() => onPay('Card')}
                                    disabled={
                                        !hasOutstandingAmount ||
                                        !isPositiveAmount ||
                                        isPending
                                    }
                                >
                                    <p className="text-[10px] font-bold">
                                        {paymentByCard}
                                    </p>
                                    <Image src={paymentCard} alt="card-icon" />
                                </button>
                            </li>
                            <li>
                                <button
                                    className={btnStyle}
                                    onClick={() => onPay('Cash')}
                                    disabled={
                                        !hasOutstandingAmount ||
                                        !isPositiveAmount ||
                                        isPending
                                    }
                                >
                                    <p className="text-[10px] font-bold">
                                        {paymentByCash}
                                    </p>
                                    <Image src={paymentCash} alt="cash-icon" />
                                </button>
                            </li>
                        </ul>
                    </>
                ) : (
                    <div className="mt-1 px-6 pb-1 flex-1 flex flex-col justify-between min-h-0">
                        <div className="rounded-xl border-2 border-gray-300 bg-gray-50 overflow-hidden text-base shadow-sm">
                            <div className="grid grid-cols-2 border-b border-gray-300">
                                <span className="px-4 py-2 font-semibold">
                                    {customerAmountLabel}
                                </span>
                                <span className="px-4 py-2 text-right font-bold text-lg">
                                    {formatPriceStr({
                                        currency: 'pln',
                                        price: summaryCustomerAmount,
                                    })}
                                </span>
                            </div>
                            <div className="grid grid-cols-2 border-b border-gray-300">
                                <span className="px-4 py-2 font-semibold">
                                    {orderPriceLabel}
                                </span>
                                <span className="px-4 py-2 text-right font-bold text-lg">
                                    {formatPriceStr({
                                        currency: 'pln',
                                        price: summaryDueAmount,
                                    })}
                                </span>
                            </div>
                            <div className="grid grid-cols-2">
                                <span className="px-4 py-2 font-semibold text-orange-600">
                                    {changeDueLabel}
                                </span>
                                <span className="px-4 py-2 text-right font-extrabold text-xl text-orange-600">
                                    {formatPriceStr({
                                        currency: 'pln',
                                        price: summaryChangeDue,
                                    })}
                                </span>
                            </div>
                        </div>

                        <div className="grid grid-cols-2 gap-3">
                            <button
                                className="h-12 w-full rounded-lg border border-gray-300 bg-white text-sm font-semibold shadow-sm"
                                onClick={closeOrder}
                                disabled={
                                    isPending ||
                                    !paymentRecorded ||
                                    isCloseOrderPending
                                }
                            >
                                {confirmCloseOrder}
                            </button>
                            <button
                                className="h-12 w-full rounded-lg border border-gray-300 bg-white text-sm font-semibold shadow-sm"
                                onClick={() => router.push('/orders')}
                                disabled={isPending || isCloseOrderPending}
                            >
                                {exitSummary}
                            </button>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
};

export default Payment;
