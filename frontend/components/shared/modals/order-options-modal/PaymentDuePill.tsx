import clsx from 'clsx';

import { CURRENCIES } from '@/helpers/constants/constants';

const PaymentDuePill = ({
    label,
    isPaid,
    amount,
}: {
    label: string;
    isPaid: boolean;
    amount: number;
}) => (
    <div className="bg-[#F6F6F6] flex justify-center items-center p-2 px-3 gap-3 mr-8 rounded-lg shadow-sm border border-gray-100">
        <span className="font-bold text-sm">{label}</span>
        <span
            className={clsx('font-bold', {
                'text-paid': isPaid,
                'text-ongoing': !isPaid,
            })}
        >
            {amount} {CURRENCIES.pln}
        </span>
    </div>
);

export default PaymentDuePill;
