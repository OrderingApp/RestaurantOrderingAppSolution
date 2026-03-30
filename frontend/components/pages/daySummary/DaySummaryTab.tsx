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
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
    Cell,
} from 'recharts';
import { useMemo } from 'react';
import { Banknote, CreditCard, HandCoins } from 'lucide-react';
import { ColumnDef } from '@tanstack/react-table';

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
    // DUMMY DATA - Replace with orderTypeCounts.lokal and orderTypeCounts.wynos once backend is ready
    const DUMMY_LOKAL_COUNT = 45;
    const DUMMY_WYNOS_COUNT = 25;
    const { data: orders = [] } = useQuery({
        queryKey: ['orders'],
        queryFn: fetchOrders,
    });

    const columns: ColumnDef<ProductSale>[] = useMemo(
        () => [
            {
                accessorKey: 'name',
                header: 'Name',
            },
            {
                accessorKey: 'quantity',
                header: 'Qty',
                cell: ({ row }) => row.original.quantity,
            },
            {
                accessorKey: 'price',
                header: 'Price',
                cell: ({ row }) => `zł ${row.original.price.toFixed(2)}`,
            },
        ],
        []
    );

    const stats = useMemo(() => {
        const stats: RevenueStats = {
            cardRevenue: 0,
            cashRevenue: 0,
            totalRevenue: 0,
        };

        const productMap = new Map<string, ProductSale>();

        orders.forEach((order: Order) => {
            if (order.paymentStatus === 'Paid') {
                stats.totalRevenue += order.totalAmount;

                // Try to determine payment method from order type or default split
                // Since we don't have explicit payment method data in OrderReadDto,
                // we'll show total revenue
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

        return { stats, products: Array.from(productMap.values()) };
    }, [orders]);

    const topProducts = useMemo(() => {
        return stats.products
            .sort((a, b) => a.quantity - b.quantity)
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
        // TODO: Replace with real data from orderType counts
        // const lokal = orders.filter(
        //     (o: Order) =>
        //         o.orderStatus === 'Completed' && o.orderType === 'dinein'
        // ).length;
        // const wynos = orders.filter(
        //     (o: Order) =>
        //         o.orderStatus === 'Completed' &&
        //         (o.orderType === 'Takeaway' || o.orderType === 'Delivery')
        // ).length;

        return { lokal: DUMMY_LOKAL_COUNT, wynos: DUMMY_WYNOS_COUNT };
    }, []);

    const totalOrders = orderTypeCounts.lokal + orderTypeCounts.wynos;

    return (
        <aside className="w-full flex flex-col gap-6">
            <Card className="relative overflow-hidden p-5 text-revenue shadow-lg">
                <h2 className="text-3xl font-semibold z-10">Utarg</h2>
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
                                <span className="text-xl">Card</span>
                                <CreditCard size={42} />
                            </div>
                        </dt>
                        <dd className="text-3xl">
                            Zł {(stats.stats.totalRevenue * 0.5).toFixed(2)}
                        </dd>
                    </Card>
                    <Card className="flex flex-col-reverse p-4 text-center bg-transparent text-revenue">
                        <dt>
                            <div className="flex justify-between items-center">
                                <span className="text-xl">Cash</span>
                                <Banknote size={42} />
                            </div>
                        </dt>
                        <dd className="text-3xl">
                            Zł {(stats.stats.totalRevenue * 0.5).toFixed(2)}
                        </dd>
                    </Card>
                    <Card className="flex flex-col-reverse p-4 text-center bg-transparent text-revenue">
                        <dt>
                            <div className="flex justify-between items-center">
                                <span className="text-xl">Total</span>
                                <HandCoins size={42} />
                            </div>
                        </dt>
                        <dd className="text-3xl text-[#0598DA]">
                            Zł {(stats.stats.totalRevenue * 0.5).toFixed(2)}
                        </dd>
                    </Card>
                </dl>
            </Card>
            <div className="grid grid-cols-2 gap-6">
                <div className="bg-white row-span-2 flex flex-col h-full">
                    <h2 className="sr-only">Product Sales</h2>
                    <DataTable
                        columns={columns}
                        data={stats.products}
                        pagination
                        containerClassName="flex-1"
                    />
                </div>

                <Card className="p-4 bg-white">
                    <h2 className="font-semibold text-gray-800 mb-3 text-sm">
                        Most Sold Products
                    </h2>
                    <ResponsiveContainer width="100%" height={200}>
                        <BarChart data={chartData}>
                            <CartesianGrid
                                strokeDasharray="3 3"
                                stroke="#e5e7eb"
                            />
                            <XAxis
                                dataKey="name"
                                tick={{ fontSize: 12 }}
                                angle={-45}
                                textAnchor="end"
                                height={80}
                            />
                            <YAxis tick={{ fontSize: 12 }} />
                            <Tooltip
                                contentStyle={{
                                    borderRadius: 8,
                                    border: '1px solid #e5e7eb',
                                }}
                            />
                            <Bar
                                dataKey="count"
                                fill="#06b6d4"
                                radius={[8, 8, 0, 0]}
                            >
                                {chartData.map((_, index) => (
                                    <Cell
                                        key={`cell-${index}`}
                                        fill={colors[index % colors.length]}
                                    />
                                ))}
                            </Bar>
                        </BarChart>
                    </ResponsiveContainer>
                </Card>

                <Card className="p-4 bg-white flex flex-col items-center justify-center">
                    <h2 className="font-semibold text-gray-800 mb-4 text-sm w-full">
                        Total Orders
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
                                strokeDasharray={`${(orderTypeCounts.lokal / totalOrders) * 345.6} 345.6`}
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
                                strokeDasharray={`${(orderTypeCounts.wynos / totalOrders) * 345.6} 345.6`}
                                strokeLinecap="round"
                                transform={`rotate(${-90 + (orderTypeCounts.lokal / totalOrders) * 360} 60 60)`}
                            />
                        </svg>
                        <div className="absolute text-center">
                            <div className="text-2xl font-bold text-gray-800">
                                {totalOrders}
                            </div>
                            <div className="text-xs text-gray-600">orders</div>
                        </div>
                    </div>
                    <div className="flex gap-4 mt-4 text-xs">
                        <div className="flex items-center gap-2">
                            <div className="w-2 h-2 bg-red-500 rounded-full" />
                            <span className="text-gray-600">
                                Lokal ({orderTypeCounts.lokal})
                            </span>
                        </div>
                        <div className="flex items-center gap-2">
                            <div className="w-2 h-2 bg-cyan-500 rounded-full" />
                            <span className="text-gray-600">
                                Wynos/Dowóz ({orderTypeCounts.wynos})
                            </span>
                        </div>
                    </div>
                </Card>
            </div>
        </aside>
    );
};

export default DaySummaryTab;
