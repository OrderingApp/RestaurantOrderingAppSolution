import { ArrowUpDown } from 'lucide-react';
import { ReactNode } from 'react';
import type { Column } from '@tanstack/react-table';
import type { ColumnOrder } from '../../OrdersTable';

interface TableSortableHeaderProps {
    children: ReactNode;
    column: Column<ColumnOrder, unknown>;
}

const TableSortableHeader = ({
    children,
    column,
}: TableSortableHeaderProps) => (
    <button
        onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
        className="flex items-center gap-1"
    >
        {children}
        <ArrowUpDown size={16} />
    </button>
);

export default TableSortableHeader;
