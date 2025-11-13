'use client';

import { CURRENCIES, FILTER_STATUS } from '@/helpers/constants/constants';
import ItemCard, { ItemCardProps } from './ItemCard';
import { formatDate } from '@/helpers/utils/dates';
import useLanguage from '@/helpers/hooks/useLanguage';
import languagePacks from '@/helpers/constants/languagePacks';
import { NotDineInOrder } from '@/helpers/interfaces/orders';
import { formatPhoneNumber } from '@/helpers/utils/utils';

export interface OrderProps
    extends Omit<ItemCardProps, 'variant' | 'children' | 'title'>,
        NotDineInOrder {}

const OrderCard = ({
    orderType,
    orderStatus,
    totalAmount,
    phoneNumber,
    address,
    expectedOrderCompletion,
    className,
    variantClassName,
    onClick,
}: OrderProps) => {
    const { language } = useLanguage();
    const {
        ordersPage: {
            orderCard: { pickup, delivery },
        },
    } = languagePacks[language];

    return (
        <ItemCard
            title={orderType === 'Takeaway' ? pickup : delivery}
            subtitle={
                formatDate(new Date(expectedOrderCompletion), language).time
            }
            variant={
                orderStatus === FILTER_STATUS.ONGOING
                    ? 'orderActive'
                    : 'orderCompleted'
            }
            className={className}
            variantClassName={variantClassName}
            onClick={onClick}
        >
            <div>
                <p className="text-left text-[11px] font-bold pb-0.5">
                    {totalAmount}
                    {CURRENCIES.pln}
                </p>
                <p className="text-left text-[11px] font-bold">
                    {formatPhoneNumber(phoneNumber)}
                </p>
                {address && (
                    <p className="text-left text-[11px] font-bold">{address}</p>
                )}
            </div>
        </ItemCard>
    );
};

export default OrderCard;
