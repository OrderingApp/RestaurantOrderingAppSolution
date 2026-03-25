import Image from 'next/image';
import clsx from 'clsx';

import { inputStyles } from '@/lib/styles/input';

import PaymentDuePill from './PaymentDuePill';
import type { InformationInput } from './types';

const InfoView = ({
    title,
    paymentDueLabel,
    isPaid,
    totalAmount,
    informationInputs,
}: {
    title: string;
    paymentDueLabel: string;
    isPaid: boolean;
    totalAmount: number;
    informationInputs: InformationInput[];
}) => (
    <>
        <div className="flex justify-between items-start">
            <h2 className="text-xl font-bold">{title}</h2>
            <PaymentDuePill
                label={paymentDueLabel}
                isPaid={isPaid}
                amount={totalAmount}
            />
        </div>
        <ul className="flex flex-col gap-2 mt-2">
            {informationInputs.map((input) => (
                <li key={input.label}>
                    <h4 className="font-bold ml-2 text-sm">{input.label}</h4>
                    <div
                        className={clsx(
                            inputStyles.variants.primary,
                            'flex h-11 rounded-2xl p-3 justify-between items-center'
                        )}
                    >
                        <span className="text-sm">{input.value}</span>
                        {input.icon && (
                            <Image
                                className="aspect-square"
                                src={input.icon}
                                alt={input.alt ?? ''}
                                width={22}
                                height={22}
                            />
                        )}
                    </div>
                </li>
            ))}
        </ul>
    </>
);

export default InfoView;
