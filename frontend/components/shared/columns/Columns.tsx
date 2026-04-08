import { memo, useMemo } from 'react';
import {
    CURRENCIES,
    ORDER_TYPES,
    PAYMENT_STATUSES,
} from '@/helpers/constants/constants';
import { NotDineInOrder, Order } from '@/helpers/interfaces/orders';
import { formatDate, formatElapsedTime } from '@/helpers/utils/dates';
import { formatPhoneNumber, paymentBorderColor } from '@/helpers/utils/utils';
import { Badge, DashboardCard } from '@/components/shared/cards/DashboardCard';
import { aggregatePaymentStatus } from '@/helpers/utils/orderTransforms';
import { Reservation } from '@/helpers/queries/reservations/useQueryReservations';
import { useDashboardClock } from '@/providers/DashboardClockProvider';

const DineInElapsedTime = memo(
    ({ earliestCreatedAtMs }: { earliestCreatedAtMs: number }) => {
        const nowMs = useDashboardClock();
        const elapsedSeconds = Math.max(
            0,
            Math.floor((nowMs - earliestCreatedAtMs) / 1000)
        );

        return <>{formatElapsedTime(elapsedSeconds)}</>;
    }
);

DineInElapsedTime.displayName = 'DineInElapsedTime';

type NonDineInType = ORDER_TYPES.DELIVERY | ORDER_TYPES.TAKEAWAY;
const TWO_MINUTES_IN_MS = 2 * 60_000;

const PAYMENT_BADGE_COLORS: Record<PAYMENT_STATUSES, string> = {
    [PAYMENT_STATUSES.UNPAID]: 'bg-warning',
    [PAYMENT_STATUSES.PARTIALPAID]: 'bg-warning',
    [PAYMENT_STATUSES.PAID]: 'bg-primary',
};

const getPaymentBadgeColor = (status?: string) => {
    if (!status) return PAYMENT_BADGE_COLORS[PAYMENT_STATUSES.UNPAID];

    return (
        PAYMENT_BADGE_COLORS[status as PAYMENT_STATUSES] ??
        PAYMENT_BADGE_COLORS[PAYMENT_STATUSES.UNPAID]
    );
};

export const Column = ({
    title,
    count,
    children,
}: {
    title: string;
    count: number;
    children: React.ReactNode;
}) => (
    <section className="flex flex-col flex-1 min-w-0">
        <h2 className="font-bold text-md mb-3">
            {title}{' '}
            <span className="text-dark-gray font-normal">({count})</span>
        </h2>
        <div className="flex flex-col gap-2 overflow-y-auto hide-scrollbar pr-1">
            {children}
        </div>
    </section>
);

export const DineInOrdersColumn = ({
    orders,
    tables,
    title,
    billCountLabel,
    onCardClick,
}: {
    orders: Order[];
    tables: Map<string, string>;
    title: string;
    billCountLabel: string;
    onCardClick: () => void;
}) => {
    const tableSummaries = useMemo(() => {
        const groupedOrders = new Map<string, Order[]>();

        for (const order of orders) {
            const tableId = order.tableId ?? 'unknown';
            const list = groupedOrders.get(tableId) ?? [];
            list.push(order);
            groupedOrders.set(tableId, list);
        }

        return [...groupedOrders.entries()].map(([tableId, tableOrders]) => {
            const tableName = tables.get(tableId) ?? tableId.slice(0, 4);
            const totalAmount = tableOrders.reduce(
                (sum, order) => sum + order.totalAmount,
                0
            );
            const billCount = tableOrders.length;
            const earliestCreatedAtMs = tableOrders.reduce((min, order) => {
                const createdAtMs = new Date(order.createdAt).getTime();
                return createdAtMs < min ? createdAtMs : min;
            }, Number.POSITIVE_INFINITY);
            const paymentStatus = aggregatePaymentStatus(tableOrders);

            return {
                tableId,
                tableName,
                totalAmount,
                billCount,
                earliestCreatedAtMs,
                paymentStatus,
            };
        });
    }, [orders, tables]);

    return (
        <Column title={title} count={tableSummaries.length}>
            {tableSummaries.map((summary) => {
                return (
                    <DashboardCard
                        key={summary.tableId}
                        borderColor={paymentBorderColor(summary.paymentStatus)}
                        onClick={onCardClick}
                    >
                        <span className="flex flex-col gap-0.5 text-xs leading-tight">
                            <span className="font-bold text-sm">
                                {summary.tableName}
                            </span>
                            <span>
                                {billCountLabel}: {summary.billCount}
                            </span>
                            <span className="text-dark-gray">
                                ⏱{' '}
                                <DineInElapsedTime
                                    earliestCreatedAtMs={
                                        summary.earliestCreatedAtMs
                                    }
                                />
                            </span>
                        </span>
                        <Badge
                            label={`${summary.totalAmount.toFixed(2)} ${CURRENCIES.pln}`}
                            className={getPaymentBadgeColor(
                                summary.paymentStatus
                            )}
                        />
                    </DashboardCard>
                );
            })}
        </Column>
    );
};

export const NonDineInColumn = ({
    orders,
    title,
    type,
    asapLabel,
    language,
    onCardClick,
}: {
    orders: NotDineInOrder[];
    title: string;
    type: NonDineInType;
    asapLabel: string;
    language: string;
    onCardClick: (id: string) => void;
}) => {
    const isDelivery = type === ORDER_TYPES.DELIVERY;

    return (
        <Column title={title} count={orders.length}>
            {orders.map((order) => {
                const expectedDate = new Date(order.expectedOrderCompletion);
                const isWithinTwoMinutes =
                    Math.abs(
                        expectedDate.getTime() -
                            new Date(order.createdAt).getTime()
                    ) < TWO_MINUTES_IN_MS;
                const timeLabel = isWithinTwoMinutes
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
                            className={getPaymentBadgeColor(
                                order.paymentStatus
                            )}
                        />
                    </DashboardCard>
                );
            })}
        </Column>
    );
};

export const ReservationsColumn = ({
    reservations,
    title,
    language,
    onCardClick,
}: {
    reservations: Reservation[];
    title: string;
    language: string;
    onCardClick: (id: string) => void;
}) => {
    const now = Date.now();

    return (
        <Column title={title} count={reservations.length}>
            {reservations.map((r) => {
                const isPast = new Date(r.scheduledFor).getTime() <= now;
                const borderColor = isPast ? 'bg-danger' : 'bg-primary';
                const badgeColor = isPast ? 'bg-danger' : 'bg-primary';
                const time = formatDate(
                    new Date(r.scheduledFor),
                    language
                ).time;

                return (
                    <DashboardCard
                        key={r.id}
                        borderColor={borderColor}
                        onClick={() => onCardClick(r.id)}
                    >
                        <span className="flex flex-col gap-0.5 text-xs leading-tight">
                            <span className="font-bold text-sm">{r.name}</span>
                            {r.capacityNeeded && (
                                <span>{r.capacityNeeded} os.</span>
                            )}
                            {r.tableName && <span>{r.tableName}</span>}
                            <span>{formatPhoneNumber(r.phoneNumber)}</span>
                        </span>
                        <Badge label={time} className={badgeColor} />
                    </DashboardCard>
                );
            })}
        </Column>
    );
};

export default ReservationsColumn;
