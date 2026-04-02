import { CURRENCIES } from '@/helpers/constants/constants';
import { NotDineInOrder } from '@/helpers/interfaces/orders';
import { formatDate } from '@/helpers/utils/dates';
import { formatPhoneNumber, paymentBorderColor } from '@/helpers/utils/utils';

import Column from './Column';
import { DashboardCard, Badge } from './DashboardCard';

const NonDineInColumn = ({
    orders,
    title,
    type,
    asapLabel,
    language,
    onCardClick,
}: {
    orders: NotDineInOrder[];
    title: string;
    type: 'delivery' | 'pickup';
    asapLabel: string;
    language: string;
    onCardClick: (id: string) => void;
}) => {
    const isDelivery = type === 'delivery';
    const badgeColor = isDelivery ? 'bg-[#CD5700]' : 'bg-[#2C5364]';

    return (
        <Column title={title} count={orders.length}>
            {orders.map((order) => {
                const expectedDate = new Date(order.expectedOrderCompletion);
                const isSameMinute =
                    Math.abs(
                        expectedDate.getTime() -
                            new Date(order.createdAt).getTime()
                    ) < 120_000;
                const timeLabel = isSameMinute
                    ? asapLabel
                    : formatDate(expectedDate, language).time;

                return (
                    <DashboardCard
                        key={order.id}
                        borderColor={paymentBorderColor(order.paymentStatus)}
                        onClick={() => onCardClick(order.id)}
                    >
                        <span className="flex flex-col gap-0.5 text-xs leading-tight">
                            {isDelivery && order.address && (
                                <span className="font-bold text-sm capitalize">
                                    {order.address}
                                </span>
                            )}
                            <span>{formatPhoneNumber(order.phoneNumber)}</span>
                            <span className="text-dark-gray">{timeLabel}</span>
                        </span>
                        <Badge
                            label={`${order.totalAmount.toFixed(2)} ${CURRENCIES.pln}`}
                            className={badgeColor}
                        />
                    </DashboardCard>
                );
            })}
        </Column>
    );
};

export default NonDineInColumn;
