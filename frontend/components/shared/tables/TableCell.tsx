import { ReactNode } from 'react';

interface TableCellProps {
    children: ReactNode;
    className?: string;
    renderChildrenAsTitle?: boolean;
}

const TableCell = ({
    children,
    className,
    renderChildrenAsTitle = true,
}: TableCellProps) => {
    const props = renderChildrenAsTitle ? { title: children as string } : {};

    return (
        <span
            className={`leading-[100%] font-semibold ${className}`}
            {...props}
        >
            {children}
        </span>
    );
};

export default TableCell;
