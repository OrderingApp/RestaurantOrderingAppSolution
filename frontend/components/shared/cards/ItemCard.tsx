import { itemCardStyles } from '@/lib/styles/itemCard';
import clsx from 'clsx';

export interface ItemCardProps {
    title: string;
    subtitle?: string;
    variant: keyof typeof itemCardStyles.variants;
    onClick?: () => void;
    children: React.ReactNode;
    className?: string;
}

const ItemCard = ({
    title,
    subtitle,
    variant,
    onClick,
    children,
    className,
}: ItemCardProps) => (
    <button onClick={onClick}>
        <div
            className={clsx(
                'w-32 h-28 border border-black rounded-lg overflow-hidden relative',
                className
            )}
        >
            <div
                className={clsx(
                    'absolute left-[-2px] top-0 w-[130px] flex justify-between items-center px-2 h-8 border-b border-l border-r border-black text-white rounded-b-[10px]',
                    itemCardStyles.variants[variant]
                )}
            >
                <p className="text-[10px]">{title}</p>
                {subtitle && <p className="text-[10px]">{subtitle}</p>}
            </div>

            <div className="h-full bg-[#F5F5F5E5] bg-opacity-90 p-2 mt-8">
                {children}
            </div>
        </div>
    </button>
);

export default ItemCard;
