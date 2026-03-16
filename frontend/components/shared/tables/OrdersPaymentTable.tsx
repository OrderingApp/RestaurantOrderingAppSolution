import type { ColumnDef } from '@tanstack/react-table';

import { useLanguage } from '@/providers/LanguageProvider';
import { DataTable } from '@/components/shared/tables/DataTable';
import TableCell from '@/components/shared/tables/TableCell';
import Badge from '@/components/shared/badges/Badge';

import { formatPriceStr } from '@/helpers/utils/prices';
import languagePacks from '@/helpers/constants/languagePacks';
import type { OrderItem } from '@/helpers/interfaces/orders';
import { aggregateAndSortOrderItems } from '@/helpers/utils/orderTransforms';
import TableSortableHeader from './TableSortableHeader';

interface ProductItem {
    id: string;
    productNumber: string;
    productName: string;
    category: string;
    price: number;
    quantity: number;
    discount: number;
    totalPrice: number;
}

interface OrdersPaymentTableProps {
    items: OrderItem[];
    isLoading?: boolean;
    isError?: boolean;
    error?: string;
}

const currency = 'pln';

const OrdersPaymentTable = ({
    items,
    isLoading,
    isError,
    error,
}: OrdersPaymentTableProps) => {
    const { language } = useLanguage();
    const {
        paymentDetails: {
            productNumber,
            product: productName,
            productPrice,
            quantity,
            discount,
            total,
        },
    } = languagePacks[language];

    const columns: ColumnDef<ProductItem>[] = [
        {
            accessorKey: 'productNumber',
            header: ({ column }) => (
                <TableSortableHeader paymentDetails={true} column={column}>
                    {productNumber}
                </TableSortableHeader>
            ),
            cell: ({ row }) => (
                <TableCell className="block text-center py-2.5 text-sm">
                    {row.getValue('productNumber')}
                </TableCell>
            ),
        },
        {
            accessorKey: 'productName',
            header: ({ column }) => (
                <TableSortableHeader paymentDetails={true} column={column}>
                    {productName}
                </TableSortableHeader>
            ),
            cell: ({ row }) => (
                <TableCell className="block text-center py-2.5 text-sm font-medium">
                    {row.getValue('productName')}
                </TableCell>
            ),
        },
        {
            accessorKey: 'price',
            header: ({ column }) => (
                <TableSortableHeader paymentDetails={true} column={column}>
                    {productPrice}
                </TableSortableHeader>
            ),
            cell: ({ row }) => (
                <TableCell className="block text-center py-2.5 text-sm">
                    {formatPriceStr({
                        currency,
                        price: row.getValue('price'),
                    })}
                </TableCell>
            ),
        },
        {
            accessorKey: 'quantity',
            header: ({ column }) => (
                <TableSortableHeader paymentDetails={true} column={column}>
                    {quantity}
                </TableSortableHeader>
            ),
            cell: ({ row }) => (
                <TableCell className="block text-center py-2.5 text-sm">
                    {row.getValue('quantity')}
                </TableCell>
            ),
        },
        {
            accessorKey: 'discount',
            header: ({ column }) => (
                <TableSortableHeader paymentDetails={true} column={column}>
                    {discount}
                </TableSortableHeader>
            ),
            cell: ({ row }) => {
                const discountAmount = row.getValue('discount') as number;
                const hasDiscount = discountAmount > 0;

                return (
                    <TableCell
                        renderChildrenAsTitle={false}
                        className="block text-center py-2.5 text-sm"
                    >
                        <Badge variant={hasDiscount ? 'success' : 'warning'}>
                            {hasDiscount ? `-${discountAmount}%` : 'Brak'}
                        </Badge>
                    </TableCell>
                );
            },
        },
        {
            accessorKey: 'totalPrice',
            header: ({ column }) => (
                <TableSortableHeader paymentDetails={true} column={column}>
                    {total}
                </TableSortableHeader>
            ),
            cell: ({ row }) => (
                <TableCell className="block text-center py-2.5 text-sm font-bold">
                    {formatPriceStr({
                        currency,
                        price: row.getValue('totalPrice'),
                    })}
                </TableCell>
            ),
        },
    ];

    const transformedData = () => {
        const aggregatedItems = aggregateAndSortOrderItems(items);

        return aggregatedItems.map((item, index) => {
            const quantity = item.quantity;
            const price = item.price;
            const productNumber = String(index + 1);
            const category = 'Brak kategorii';
            const discount = item.discount || 0;
            const totalPrice = price * quantity * (1 - discount / 100);

            return {
                id: item.id,
                productNumber,
                productName: item.menuItem.name,
                category,
                price,
                quantity,
                discount,
                totalPrice,
            } satisfies ProductItem;
        });
    };

    return (
        <DataTable
            columns={columns}
            data={transformedData()}
            isError={isError}
            error={error}
            isLoading={isLoading}
        />
    );
};

export default OrdersPaymentTable;
