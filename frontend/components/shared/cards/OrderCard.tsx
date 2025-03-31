'use client';

import { CURRENCIES, FILTER_STATUS } from '@/helpers/constants/constants';
import ItemCard, { ItemCardProps } from './ItemCard';
import { formatDate } from '@/helpers/utils/dates';
import useLanguage from '@/helpers/hooks/useLanguage';
import languagePacks from '@/helpers/constants/languagePacks';

export interface OrderProps
    extends Omit<ItemCardProps, 'variant' | 'children' | 'title'> {
    id: string;
    orderType: 'Takeaway' | 'Delivery';
    orderStatus: 'Ongoing' | 'Closed';
    dateTime: Date;
    totalAmount: string;
    phoneNumber: number;
    address?: string;
}

const OrderCard = ({
    orderType,
    dateTime,
    orderStatus,
    totalAmount,
    phoneNumber,
    address,
    className,
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
            subtitle={formatDate(new Date(dateTime), language).time}
            variant={
                orderStatus === FILTER_STATUS.ONGOING
                    ? 'orderActive'
                    : 'orderCompleted'
            }
            className={className}
            onClick={onClick}
        >
            <div>
                <p className="text-left text-[11px] font-bold">
                    {totalAmount}
                    {CURRENCIES.pln}
                </p>
                <p className="text-left text-[11px] font-bold">{phoneNumber}</p>
                {address && (
                    <p className="text-left text-[11px] font-bold">{address}</p>
                )}
            </div>
        </ItemCard>
    );
};

export default OrderCard;

//TODO CHANGE NAMES FROM LANGUAGE PACKS AND CURRENCIES
