'use client';

import {
    CURRENCIES,
    ORDER_STATUSES,
    ORDER_TYPES,
} from '@/helpers/constants/constants';
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
            title={orderType === ORDER_TYPES.TAKEAWAY ? pickup : delivery}
            subtitle={
                formatDate(new Date(expectedOrderCompletion), language).time
            }
            variant={
                orderStatus === ORDER_STATUSES.ONGOING
                    ? 'orderActive'
                    : 'orderCompleted'
            }
            className={`${className} ${orderType === ORDER_TYPES.DELIVERY ? '!h-[120px]' : ''}`}
            variantClassName={variantClassName}
            onClick={onClick}
        >
            <span className="flex flex-col gap-1 items-center justify-center">
                <span className="text-left text-xs">
                    <span>{price}:</span>
                    <span className="font-bold">
                        {totalAmount}
                        {CURRENCIES.pln}
                    </span>
                </span>
                <span className="text-left text-xs">
                    <span>{phone}:</span>
                    <span className="font-bold">
                        {formatPhoneNumber(phoneNumber)}
                    </span>
                </span>
                {address && (
                    <span className="text-left text-xs flex flex-row justify-center">
                        <span>{addressLabel}:</span>
                        <span className="font-bold capitalize text-center mb-2">
                            {address}
                        </span>
                    </span>
                )}
            </span>
        </ItemCard>
    );
};

export default OrderCard;
