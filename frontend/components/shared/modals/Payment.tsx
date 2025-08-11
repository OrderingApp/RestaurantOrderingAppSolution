'use client';
import Image from 'next/image';

import paymentCard from '@/public/images/svg/payment-card.svg';
import paymentCash from '@/public/images/svg/payment-cash.svg';
import dollar from '@/public/images/svg/dollar.svg';

import Input from '../Input/Input';

import useLanguage from '@/helpers/hooks/useLanguage';
import languagePacks from '@/helpers/constants/languagePacks';

const Payment = ({
    onClick,
    totalAmount,
}: {
    onClick: () => void;
    totalAmount: number;
}) => {
    const { language } = useLanguage();
    const {
        paymentModal: { title, inputLabel, paymentByCard, paymentByCash },
    } = languagePacks[language];

    const btnStyle =
        'w-[198px] h-[98px] flex flex-col justify-center items-center gap-2 shadow-[0px_0px_6px_0px_#00000029] rounded-xl';

    return (
        <div className="w-[445px] h-[445px] bg-payment-modal-gradient rounded-2xl relative ">
            <button
                onClick={onClick}
                className="absolute top-5 right-5 text-xl"
            >
                X
            </button>
            <div className="bg-white mt-[0.4rem] h-full rounded-2xl p-5 flex flex-col gap-8">
                <div className="flex flex-col justify-center items-center pt-6">
                    <h2 className="text-xl font-bold text-center">{title}</h2>
                    <Image src={dollar} alt="dolar-icon" />
                </div>
                <div className="w-full px-10">
                    <Input
                        type="number"
                        label={inputLabel}
                        min={1}
                        defaultValue={totalAmount}
                        variant="secondary"
                        inputClassName="w-full"
                        labelClassName="text-xl font-bold text-center pb-4"
                    />
                </div>
                <ul className="flex gap-2 mt-2">
                    <li>
                        <button className={btnStyle}>
                            <p className="text-[10px] font-bold">
                                {paymentByCard}
                            </p>
                            <Image src={paymentCard} alt="card-icon" />
                        </button>
                    </li>
                    <li>
                        <button className={btnStyle}>
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

//TODO create flow
