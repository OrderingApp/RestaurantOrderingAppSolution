'use client';

import { useState, useEffect } from 'react';

import { formatElapsedTime } from '@/helpers/utils/dates';
import { CURRENCIES } from '@/helpers/constants/constants';
import { Order } from '@/helpers/interfaces/orders';
import { paymentBorderColor } from '@/helpers/utils/utils';
import { aggregatePaymentStatus } from '@/helpers/utils/orderTransforms';

import Column from './Column';
import { DashboardCard, Badge } from './DashboardCard';

const DineInOrdersColumn = ({
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
    const [, setTick] = useState(0);

    useEffect(() => {
        const interval = setInterval(() => setTick((t) => t + 1), 1000);
        return () => clearInterval(interval);
    }, []);

    const ordersByTable = (() => {
        const map = new Map<string, Order[]>();
        for (const order of orders) {
            const tableId = order.tableId ?? 'unknown';
            const list = map.get(tableId) ?? [];
            list.push(order);
            map.set(tableId, list);
        }
        return map;
    })();

    return (
        <Column title={title} count={ordersByTable.size}>
            {[...ordersByTable.entries()].map(([tableId, tableOrders]) => {
                const tableName = tables.get(tableId) ?? tableId.slice(0, 4);
                const totalAmount = tableOrders.reduce(
                    (sum, o) => sum + o.totalAmount,
                    0
                );
                const billCount = tableOrders.length;
                const earliest = tableOrders.reduce(
                    (min, o) =>
                        new Date(o.createdAt).getTime() <
                        new Date(min).getTime()
                            ? o.createdAt
                            : min,
                    tableOrders[0].createdAt
                );
                const aggStatus = aggregatePaymentStatus(tableOrders);

                return (
                    <DashboardCard
                        key={tableId}
                        borderColor={paymentBorderColor(aggStatus)}
                        onClick={onCardClick}
                    >
                        <span className="flex flex-col gap-0.5 text-xs leading-tight">
                            <span className="font-bold text-sm">
                                {tableName}
                            </span>
                            <span>
                                {billCountLabel}: {billCount}
                            </span>
                            <span className="text-dark-gray">
                                ⏱{' '}
                                {formatElapsedTime(
                                    Math.floor(
                                        (Date.now() -
                                            new Date(earliest).getTime()) /
                                            1000
                                    )
                                )}
                            </span>
                        </span>
                        <Badge
                            label={`${totalAmount.toFixed(2)} ${CURRENCIES.pln}`}
                            className="bg-[#CD5700]"
                        />
                    </DashboardCard>
                );
            })}
        </Column>
    );
};

export default DineInOrdersColumn;
