'use client';

import { CURRENCIES, FILTER_STATUS } from '@/helpers/constants/constants';
import ItemCard, { ItemCardProps } from './ItemCard';
import { formatDate } from '@/helpers/utils/dates';
import useLanguage from '@/helpers/hooks/useLanguage';
import languagePacks from '@/helpers/constants/languagePacks';
import { NotDineInOrder } from '@/helpers/interfaces/orders';

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
