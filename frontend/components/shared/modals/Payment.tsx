'use client';
import { useEffect, useMemo, useState } from 'react';
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
import { CURRENCIES } from '@/helpers/constants/constants';

interface PaymentProps {
    onClick: () => void;
    totalAmount: number;
    orderId: string;
    remainingAmount: number;
}

const MODAL_WIDTH = 445;
const MODAL_HEIGHT = 445;

const Payment = ({ onClick, orderId, remainingAmount }: PaymentProps) => {
    const router = useRouter();
    const { language } = useLanguage();
    const {
        paymentModal: {
            title,
            inputLabel,
            paymentByCard,
            paymentByCash,
            alreadyPaidMsg,
            amountRangeError,
            toastPaymentCreated,
            toastPaymentFailed,
        },
    } = languagePacks[language];

    const [amountInput, setAmountInput] = useState(
        String(Math.max(remainingAmount, 0))
    );

    useEffect(() => {
        setAmountInput(String(Math.max(remainingAmount, 0)));
    }, [remainingAmount]);

    const parsedAmount = useMemo(() => {
        const normalized = amountInput.replace(',', '.').trim();
        const value = Number(normalized);

        return Number.isFinite(value) ? value : NaN;
    }, [amountInput]);

    const hasOutstandingAmount = remainingAmount > 0;
    const isAmountValid =
        Number.isFinite(parsedAmount) &&
        parsedAmount > 0 &&
        parsedAmount <= remainingAmount;

    const { mutate: createPayment, isPending } = useCreatePaymentMutation({
        onSuccess: () => {
            toast.success(toastPaymentCreated);
            router.push('/orders');
        },
        onError: (error) => {
            const message =
                error instanceof Error ? error.message : toastPaymentFailed;
            toast.error(message);
        },
    });

    const onPay = (method: PaymentMethod) => {
        if (!hasOutstandingAmount || !isAmountValid || isPending) return;

        createPayment({
            orderId,
            amount: parsedAmount,
            paymentMethod: method,
        });
    };

    const btnStyle =
        'w-[198px] h-[98px] flex flex-col justify-center items-center gap-2 shadow-[0px_0px_6px_0px_#00000029] rounded-xl';

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
                <div className="w-full px-10">
                    <Input
                        type="number"
                        label={inputLabel}
                        min={0.01}
                        max={remainingAmount}
                        step="0.01"
                        value={amountInput}
                        onChange={(event) => setAmountInput(event.target.value)}
                        variant="secondary"
                        inputClassName="w-full"
                        labelClassName="text-xl font-bold text-center pb-4"
                    />
                    {!hasOutstandingAmount && (
                        <p className="text-center text-sm text-paid mt-3">
                            {alreadyPaidMsg}
                        </p>
                    )}
                    {hasOutstandingAmount && !isAmountValid && (
                        <p className="text-center text-sm text-danger mt-3">
                            {amountRangeError.replace(
                                '{max}',
                                remainingAmount.toFixed(2)
                            )}
                            {CURRENCIES.pln}
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
                                !isAmountValid ||
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
                                !isAmountValid ||
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
            </div>
        </div>
    );
};

export default Payment;
