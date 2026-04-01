'use client';

import Image from 'next/image';
import { useQuery } from '@tanstack/react-query';
import { fetchOrders } from '@/helpers/queries/orders/useQueryOrders';
import { Order } from '@/helpers/interfaces/orders';
import { Card } from '@/components/ui/card';
import { DataTable } from '@/components/shared/tables/DataTable';
import daySummaryBg from '@/public/images/bgs/day-summary-tab.jpg';
import {
    BarChart,
    Bar,
    LabelList,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
    Rectangle,
} from 'recharts';
import { useMemo } from 'react';
import { Banknote, CreditCard, HandCoins } from 'lucide-react';
import { ColumnDef } from '@tanstack/react-table';
import TableSortableHeader from '@/components/shared/tables/TableSortableHeader';
import { useLanguage } from '@/providers/LanguageProvider';
import languagePacks from '@/helpers/constants/languagePacks';

interface ProductSale {
    name: string;
    quantity: number;
    price: number;
}

interface RevenueStats {
    cardRevenue: number;
    cashRevenue: number;
    totalRevenue: number;
}

const DaySummaryTab = () => {
    const { language } = useLanguage();
    const {
        daySummaryPage: {
            summary: {
                revenueTitle,
                revenueByPayment: { card, cash, total },
                productSales,
                mostSoldProducts,
                mostSoldCountLabel,
                totalOrdersLabel,
                ordersCountLabel,
                orderTypeLabels: { dineIn, takeawayDelivery },
                tableColumns: { name, qty, price },
            },
        },
    } = languagePacks[language];

    const { data: orders = [] } = useQuery({
        queryKey: ['orders'],
        queryFn: fetchOrders,
    });

    const columns: ColumnDef<ProductSale>[] = useMemo(
        () => [
            {
                accessorKey: 'name',
                header: ({ column }) => (
                    <TableSortableHeader column={column}>
                        {name}
                    </TableSortableHeader>
                ),
            },
            {
                accessorKey: 'quantity',
                header: ({ column }) => (
                    <TableSortableHeader column={column}>
                        {qty}
                    </TableSortableHeader>
                ),
                cell: ({ row }) => row.original.quantity,
            },
            {
                accessorKey: 'price',
                header: ({ column }) => (
                    <TableSortableHeader column={column}>
                        {price}
                    </TableSortableHeader>
                ),
                cell: ({ row }) => `zł ${row.original.price.toFixed(2)}`,
            },
        ],
        [name, qty, price]
    );

    const stats = useMemo(() => {
        const stats: RevenueStats = {
            cardRevenue: 0,
            cashRevenue: 0,
            totalRevenue: 0,
        };

        const productMap = new Map<string, ProductSale>();

        orders.forEach((order: Order) => {
            const isSettledOrder =
                order.paymentStatus === 'Paid' ||
                order.orderStatus === 'Closed' ||
                order.orderStatus === 'Completed';

            if (isSettledOrder) {
                stats.totalRevenue += order.totalAmount;
            }

            order.orderItems.forEach((item) => {
                const itemName = item.menuItem.name;
                const current = productMap.get(itemName) || {
                    name: itemName,
                    quantity: 0,
                    price: 0,
                };
                current.quantity += 1;
                current.price += item.price;
                productMap.set(itemName, current);
            });
        });

        // Fallback for environments where payment/order status is not finalized yet.
        if (stats.totalRevenue === 0) {
            stats.totalRevenue = orders
                .filter((order: Order) => order.orderStatus !== 'Cancelled')
                .reduce((sum, order) => sum + order.totalAmount, 0);
        }

        // Until backend exposes payment method breakdown, keep a temporary 50/50 split.
        stats.cardRevenue = stats.totalRevenue * 0.5;
        stats.cashRevenue = stats.totalRevenue * 0.5;

        return { stats, products: Array.from(productMap.values()) };
    }, [orders]);

    const topProducts = useMemo(() => {
        return stats.products
            .sort((a, b) => b.quantity - a.quantity)
            .slice(0, 5);
    }, [stats.products]);

    const chartData = useMemo(() => {
        return topProducts.map((product) => ({
            name:
                product.name.length > 15
                    ? product.name.substring(0, 15) + '...'
                    : product.name,
            count: product.quantity,
            fullName: product.name,
        }));
    }, [topProducts]);

    const colors = ['#06b6d4', '#0891b2', '#0e7490', '#164e63', '#083344'];

    const orderTypeCounts = useMemo(() => {
        return orders.reduce(
            (acc, order: Order) => {
                if (order.orderStatus === 'Cancelled') {
                    return acc;
                }

                if (order.orderType === 'dinein') {
                    acc.lokal += 1;
                } else if (
                    order.orderType === 'Takeaway' ||
                    order.orderType === 'Delivery'
                ) {
                    acc.wynos += 1;
                }

                return acc;
            },
            { lokal: 0, wynos: 0 }
        );
    }, [orders]);

    const totalOrders = orderTypeCounts.lokal + orderTypeCounts.wynos;
    const totalOrdersSafe = totalOrders || 1;

    return (
        <aside className="w-full flex flex-col gap-6">
            <Card className="relative overflow-hidden p-5 text-revenue shadow-lg">
                <h2 className="text-3xl font-semibold z-10">{revenueTitle}</h2>
                <Image
                    src={daySummaryBg}
                    alt="Day summary background"
                    fill
                    className="absolute inset-0 object-cover"
                    quality={75}
                    priority={false}
                />
                <dl className="grid grid-cols-3 gap-4 relative font-semibold z-10">
                    <Card className="flex flex-col-reverse p-4 text-center bg-transparent text-revenue">
                        <dt>
                            <div className="flex justify-between items-center">
                                <span className="text-xl">{card}</span>
                                <CreditCard size={42} />
                            </div>
                        </dt>
                        <dd className="text-3xl">
                            Zł {stats.stats.cardRevenue.toFixed(2)}
                        </dd>
                    </Card>
                    <Card className="flex flex-col-reverse p-4 text-center bg-transparent text-revenue">
                        <dt>
                            <div className="flex justify-between items-center">
                                <span className="text-xl">{cash}</span>
                                <Banknote size={42} />
                            </div>
                        </dt>
                        <dd className="text-3xl">
                            Zł {stats.stats.cashRevenue.toFixed(2)}
                        </dd>
                    </Card>
                    <Card className="flex flex-col-reverse p-4 text-center bg-transparent text-revenue">
                        <dt>
                            <div className="flex justify-between items-center">
                                <span className="text-xl">{total}</span>
                                <HandCoins size={42} />
                            </div>
                        </dt>
                        <dd className="text-3xl text-[#0598DA]">
                            Zł {stats.stats.totalRevenue.toFixed(2)}
                        </dd>
                    </Card>
                </dl>
            </Card>
            <div className="grid grid-cols-3 gap-6 items-start">
                <div className="bg-white">
                    <h2 className="sr-only">{productSales}</h2>
                    <DataTable
                        columns={columns}
                        data={stats.products}
                        pagination
                    />
                </div>

                <Card className="p-4 bg-white self-start">
                    <h2 className="font-semibold text-gray-800 mb-3 text-sm">
                        {mostSoldProducts}
                    </h2>
                    <div className="w-full aspect-[4/3] min-h-[220px]">
                        <ResponsiveContainer width="100%" height="100%">
                            <BarChart
                                data={chartData}
                                layout="vertical"
                                margin={{
                                    top: 0,
                                    right: 0,
                                    left: 0,
                                    bottom: 0,
                                }}
                            >
                                <CartesianGrid
                                    strokeDasharray="3 3"
                                    stroke="#e5e7eb"
                                />
                                <XAxis type="number" tick={{ fontSize: 12 }} />
                                <YAxis
                                    dataKey="name"
                                    type="category"
                                    tick={{ fontSize: 12 }}
                                    width={72}
                                />
                                <Tooltip
                                    contentStyle={{
                                        borderRadius: 8,
                                        border: '1px solid #e5e7eb',
                                    }}
                                />
                                <Bar
                                    dataKey="count"
                                    name={mostSoldCountLabel}
                                    shape={(props) => {
                                        const barProps = props as {
                                            index?: number;
                                            radius?: [
                                                number,
                                                number,
                                                number,
                                                number,
                                            ];
                                        };

                                        return (
                                            <Rectangle
                                                {...props}
                                                fill={
                                                    colors[
                                                        (barProps.index ?? 0) %
                                                            colors.length
                                                    ]
                                                }
                                                radius={
                                                    barProps.radius ?? [
                                                        0, 8, 8, 0,
                                                    ]
                                                }
                                            />
                                        );
                                    }}
                                >
                                    <LabelList
                                        dataKey="count"
                                        position="right"
                                        fill="#374151"
                                        fontSize={12}
                                    />
                                </Bar>
                            </BarChart>
                        </ResponsiveContainer>
                    </div>
                </Card>

                <Card className="p-4 bg-white flex flex-col items-center self-start">
                    <h2 className="font-semibold text-gray-800 mb-4 text-sm w-full">
                        {totalOrdersLabel}
                    </h2>
                    <div className="relative w-32 h-32 flex items-center justify-center">
                        <svg className="w-full h-full" viewBox="0 0 120 120">
                            <circle
                                cx="60"
                                cy="60"
                                r="55"
                                fill="none"
                                stroke="#e5e7eb"
                                strokeWidth="8"
                            />
                            {/* Lokal (red) segment */}
                            <circle
                                cx="60"
                                cy="60"
                                r="55"
                                fill="none"
                                stroke="#ef4444"
                                strokeWidth="8"
                                strokeDasharray={`${(orderTypeCounts.lokal / totalOrdersSafe) * 345.6} 345.6`}
                                strokeLinecap="round"
                                transform="rotate(-90 60 60)"
                            />
                            {/* Wynos/Dowóz (cyan) segment */}
                            <circle
                                cx="60"
                                cy="60"
                                r="55"
                                fill="none"
                                stroke="#06b6d4"
                                strokeWidth="8"
                                strokeDasharray={`${(orderTypeCounts.wynos / totalOrdersSafe) * 345.6} 345.6`}
                                strokeLinecap="round"
                                transform={`rotate(${-90 + (orderTypeCounts.lokal / totalOrdersSafe) * 360} 60 60)`}
                            />
                        </svg>
                        <div className="absolute text-center">
                            <div className="text-2xl font-bold text-gray-800">
                                {totalOrders}
                            </div>
                            <div className="text-xs text-gray-600">
                                {ordersCountLabel}
                            </div>
                        </div>
                    </div>
                    <div className="flex gap-4 mt-4 text-xs">
                        <div className="flex items-center gap-2">
                            <div className="w-2 h-2 bg-red-500 rounded-full" />
                            <span className="text-gray-600">
                                {dineIn} ({orderTypeCounts.lokal})
                            </span>
                        </div>
                        <div className="flex items-center gap-2">
                            <div className="w-2 h-2 bg-cyan-500 rounded-full" />
                            <span className="text-gray-600">
                                {takeawayDelivery} ({orderTypeCounts.wynos})
                            </span>
                        </div>
                    </div>
                </Card>
            </div>
        </aside>
    );
};

export default DaySummaryTab;
