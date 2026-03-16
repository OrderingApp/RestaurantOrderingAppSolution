import { useMemo } from 'react';
import { Ellipsis } from 'lucide-react';

import { useLanguage } from '@/providers/LanguageProvider';
import OrderStatusBadge from '@/components/shared/badges/OrderStatus';
import { DataTable } from '@/components/shared/tables/DataTable';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuLabel,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import Badge from '@/components/shared/badges/Badge';
import TableSortableHeader from '@/components/shared/tables/TableSortableHeader';
import TableCell from '../../shared/tables/TableCell';

import { formatPriceStr } from '@/helpers/utils/prices';
import { ORDER_STATUSES, ORDER_TYPES } from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import { Order } from '@/helpers/interfaces/orders';
import { getOrderStatus, getOrderType } from '@/helpers/utils/order';
import { copyToClipboard } from '@/helpers/utils/utils';

import type { ColumnDef } from '@tanstack/react-table';
import useQueryOrders from '@/helpers/queries/orders/useQueryOrders';

export interface ColumnOrder
    extends Omit<Order, 'tableId' | 'customerInformation' | 'orderItems'> {
    number: number;
}
const currency = 'pln';

const OrdersTable = () => {
    const { language } = useLanguage();
    const { data, isLoading, isError, error } = useQueryOrders({});

    const {
        ordersTable: {
            actions: {
                label: actionsLabel,
                copyId: {
                    label: copyIdLabel,
                    notification: copyIdNotification,
                },
                viewOrder,
                viewReceipt,
            },
            searchPlaceholder,
            noResults,
        },
        entities: {
            order: {
                descriptive: { id, price, status, type, discount },
            },
        },
        generic: {
            options: { yes, no },
        },
    } = languagePacks[language];

    const columns: ColumnDef<ColumnOrder>[] = [
        {
            accessorKey: 'number',
            header: ({ column }) => (
                <TableSortableHeader column={column}>#</TableSortableHeader>
            ),
            cell: ({ row }) => <TableCell>{row.getValue('number')}</TableCell>,
        },
        {
            accessorKey: 'id',
            header: ({ column }) => (
                <TableSortableHeader column={column}>{id}</TableSortableHeader>
            ),
            cell: ({ row }) => (
                <TableCell className="truncate max-w-32 block">
                    {row.getValue('id')}
                </TableCell>
            ),
        },
        {
            accessorKey: 'totalAmount',
            header: ({ column }) => (
                <TableSortableHeader column={column}>
                    {price}
                </TableSortableHeader>
            ),
            cell: ({ row }) => (
                <TableCell>
                    {formatPriceStr({
                        currency,
                        price: row.getValue('totalAmount'),
                    })}
                </TableCell>
            ),
        },
        {
            accessorKey: 'orderStatus',
            header: ({ column }) => (
                <TableSortableHeader column={column}>
                    {status}
                </TableSortableHeader>
            ),
            cell: ({ row }) => {
                const status: ORDER_STATUSES = row.getValue('orderStatus');

                return (
                    <TableCell renderChildrenAsTitle={false}>
                        <OrderStatusBadge status={status}>
                            {getOrderStatus(status, language)}
                        </OrderStatusBadge>
                    </TableCell>
                );
            },
        },
        {
            accessorKey: 'orderType',
            header: ({ column }) => (
                <TableSortableHeader column={column}>
                    {type}
                </TableSortableHeader>
            ),
            cell: ({ row }) => {
                const type: ORDER_TYPES = row.getValue('orderType');

                return <TableCell>{getOrderType(type, language)}</TableCell>;
            },
        },
        {
            accessorKey: 'discount',
            header: ({ column }) => (
                <TableSortableHeader column={column}>
                    {discount}
                </TableSortableHeader>
            ),
            cell: ({ row }) => {
                const isDiscount = !!row.getValue('discount');

                return (
                    <TableCell renderChildrenAsTitle={false}>
                        <Badge variant={isDiscount ? 'success' : 'danger'}>
                            {isDiscount ? yes : no}
                        </Badge>
                    </TableCell>
                );
            },
        },
        {
            id: 'actions',
            header: '',
            cell: ({ row }) => (
                <DropdownMenu>
                    <DropdownMenuTrigger className="flex justify-center w-full">
                        <Ellipsis />
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                        <DropdownMenuLabel>{actionsLabel}</DropdownMenuLabel>
                        <DropdownMenuItem
                            className="cursor-pointer"
                            onClick={() =>
                                copyToClipboard({
                                    text: row.original.id,
                                    notification: copyIdNotification,
                                })
                            }
                        >
                            {copyIdLabel}
                        </DropdownMenuItem>
                        <DropdownMenuItem className="cursor-pointer">
                            {viewOrder}
                        </DropdownMenuItem>
                        <DropdownMenuItem className="cursor-pointer">
                            {viewReceipt}
                        </DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            ),
        },
    ];

    const transformedData = useMemo(() => {
        if (!data) return [];

        const sorted = [...data].sort(
            (a, b) =>
                new Date(a.createdAt).getTime() -
                new Date(b.createdAt).getTime()
        );

        return sorted.map((order, i) => {
            const number = i + 1;

            return {
                ...order,
                number,
                _search: [
                    number,
                    order.id,
                    order.totalAmount,
                    getOrderStatus(
                        order.orderStatus as ORDER_STATUSES,
                        language
                    ),
                    getOrderType(order.orderType as ORDER_TYPES, language),
                    order.discount ? yes : no,
                ]
                    .join(' ')
                    .toLowerCase(),
            };
        });
    }, [data, language, no, yes]);

    return (
        <DataTable
            columns={columns}
            data={transformedData}
            searchPlaceholder={searchPlaceholder}
            noResultsFallback={noResults}
            pagination
            isLoading={isLoading}
            isError={isError}
            error={error?.message}
        />
    );
};

export default OrdersTable;

// TODO: show order and receipt functionalities
