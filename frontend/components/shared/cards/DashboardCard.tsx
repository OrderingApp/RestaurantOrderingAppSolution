import clsx from 'clsx';
import { Badge as ShadcnBadge } from '@/components/ui/badge';

export const DashboardCard = ({
    borderColor,
    children,
    onClick,
}: {
    borderColor: string;
    children: React.ReactNode;
    onClick?: () => void;
}) => (
    <button
        onClick={onClick}
        className={clsx(
            'w-full flex items-stretch rounded-lg overflow-hidden bg-white shadow-sm text-left',
            'hover:shadow-md transition-shadow'
        )}
    >
        <span className={clsx('w-2 shrink-0 rounded-l-lg', borderColor)} />
        <span className="flex-1 flex items-center justify-between px-3 py-2 min-h-[70px] gap-2">
            {children}
        </span>
    </button>
);

export const Badge = ({
    label,
    className,
}: {
    label: string;
    className?: string;
}) => (
    <ShadcnBadge className={clsx('rounded-md', className)}>{label}</ShadcnBadge>
);
