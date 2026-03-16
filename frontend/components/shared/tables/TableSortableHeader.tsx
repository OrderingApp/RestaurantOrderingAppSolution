import { ArrowUpDown } from 'lucide-react';
import { ReactNode } from 'react';
import type { Column } from '@tanstack/react-table';
import clsx from 'clsx';

interface TableSortableHeaderProps<TRow extends object> {
    children: ReactNode;
    column: Column<TRow, unknown>;
    paymentDetails?: boolean;
}

const TableSortableHeader = <TRow extends object>({
    children,
    column,
    paymentDetails = false,
}: TableSortableHeaderProps<TRow>) => (
    <button
        onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
        className={clsx(
            'flex items-center gap-1',
            paymentDetails ? 'w-full text-center justify-center' : 'w-auto'
        )}
    >
        {children}
        <ArrowUpDown size={16} />
    </button>
);

export default TableSortableHeader;
