'use client';

import { CURRENCIES, ORDER_STATUSES } from '@/helpers/constants/constants';
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
            orderCard: {
                pickup,
                delivery,
                price,
                phone,
                address: addressLabel,
            },
        },
    } = languagePacks[language];

    return (
        <ItemCard
            title={orderType === 'Takeaway' ? pickup : delivery}
            subtitle={
                formatDate(new Date(expectedOrderCompletion), language).time
            }
            variant={
                orderStatus === ORDER_STATUSES.ONGOING
                    ? 'orderActive'
                    : 'orderCompleted'
            }
            className={className}
            variantClassName={variantClassName}
            onClick={onClick}
        >
            <div className="flex flex-col gap-1 items-center justify-center py-2">
                <p className="text-left text-xs">
                    <span>{price}:</span>
                    <span className="font-bold">
                        {totalAmount}
                        {CURRENCIES.pln}
                    </span>
                </p>
                <p className="text-left text-xs ">
                    <span>{phone}:</span>
                    <span className="font-bold">
                        {formatPhoneNumber(phoneNumber)}
                    </span>
                </p>
                {address && (
                    <p className="text-left text-xs flex flex-row justify-center">
                        <span>{addressLabel}:</span>
                        <span className="font-bold capitalize">{address}</span>
                    </p>
                )}
            </div>
        </ItemCard>
    );
};

export default OrderCard;
