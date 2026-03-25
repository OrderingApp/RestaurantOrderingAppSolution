import type { aggregateAndSortOrderItems } from '@/helpers/utils/orderTransforms';
import { CURRENCIES } from '@/helpers/constants/constants';

const formatAmount = (amount: number) => amount.toFixed(2).replace('.', ',');

const SummaryView = ({
    title,
    statusLabel,
    paidLabel,
    unpaidLabel,
    paidAmountLabel,
    remainingAmountLabel,
    isPaid,
    totalAmount,
    totalPaidAmount,
    remainingAmountValue,
    summaryOrderItems,
}: {
    title: string;
    statusLabel: string;
    paidLabel: string;
    unpaidLabel: string;
    paidAmountLabel: string;
    remainingAmountLabel: string;
    isPaid: boolean;
    totalAmount: number;
    totalPaidAmount: number;
    remainingAmountValue: number;
    summaryOrderItems: ReturnType<typeof aggregateAndSortOrderItems>;
}) => (
    <>
        <h2 className="text-xl font-bold mb-8">{title}</h2>

        <div className="border border-gray-300 rounded-2xl p-2 py-4 shadow-xl">
            <div className="max-h-44 overflow-y-auto pr-1">
                <ul className="flex flex-col px-2">
                    {summaryOrderItems.map((item, index) => (
                        <li
                            key={`${item.id}-${index}`}
                            className="py-2 pb-1 border-b border-gray-300 text-md text-black"
                        >
                            <div className="flex justify-between items-center font-bold">
                                <span>
                                    {item.menuItem.name}{' '}
                                    <span className="text-dark-gray font-normal text-sm ml-1">
                                        x{item.quantity}
                                    </span>
                                </span>
                                <span>
                                    {formatAmount(item.price * item.quantity)}{' '}
                                    {CURRENCIES.pln}
                                </span>
                            </div>

                            {!!item.extraIngredients?.length && (
                                <ul className="mt-1 pl-1">
                                    {item.extraIngredients.map(
                                        (extraIngredient, extraIndex) => (
                                            <li
                                                key={`${item.id}-extra-${extraIngredient.ingredientId}-${extraIndex}`}
                                                className="text-xs text-dark-gray"
                                            >
                                                +
                                                {extraIngredient.ingredientName}
                                                {extraIngredient.quantity > 1 &&
                                                    ` x${extraIngredient.quantity}`}
                                            </li>
                                        )
                                    )}
                                </ul>
                            )}
                        </li>
                    ))}
                </ul>
            </div>

            <div className="px-2 pt-2 space-y-1 text-3.5">
                <div className="flex items-center justify-between border-b border-gray-300 pb-1 font-bold text-[#EA580C]">
                    <span>Suma:</span>
                    <span>
                        {formatAmount(totalAmount)} {CURRENCIES.pln}
                    </span>
                </div>

                <div className="flex items-center justify-between border-b border-gray-300 pb-1 font-bold text-black">
                    <span>{paidAmountLabel}:</span>
                    <span>
                        {formatAmount(totalPaidAmount)} {CURRENCIES.pln}
                    </span>
                </div>

                <div className="flex items-center justify-between border-b border-gray-300 pb-1 font-bold text-dark-gray">
                    <span>{remainingAmountLabel}:</span>
                    <span>
                        {formatAmount(remainingAmountValue)} {CURRENCIES.pln}
                    </span>
                </div>
            </div>

            <div className="mt-5 bg-gray font-semibold text-center py-3 rounded-xl text-xl text-black">
                <span>{statusLabel}: </span>
                <span className={isPaid ? 'text-paid' : 'text-ongoing'}>
                    {isPaid ? paidLabel : unpaidLabel}
                </span>
            </div>
        </div>
    </>
);

export default SummaryView;
