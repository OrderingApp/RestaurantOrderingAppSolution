import { itemCardStyles } from '@/lib/styles/itemCard';
import { twMerge } from 'tailwind-merge';

export interface ItemCardProps {
    title: string;
    subtitle?: string;
    variant: keyof typeof itemCardStyles.variants;
    onClick?: () => void;
    children: React.ReactNode;
    className?: string;
    variantClassName?: string;
}

const ItemCard = ({
    title,
    subtitle,
    variant,
    onClick,
    children,
    className,
    variantClassName,
}: ItemCardProps) => (
    <li>
        <button onClick={onClick}>
            <div
                className={twMerge(
                    'w-32 h-28 border border-black rounded-lg overflow-hidden relative',
                    className
                )}
            >
                <div
                    className={twMerge(
                        'absolute left-[-2px] top-0 w-[130px] flex justify-between items-center px-2 h-8 border-b border-l border-r border-black text-white rounded-b-[10px]',
                        itemCardStyles.variants[variant],
                        variantClassName
                    )}
                >
                    <p className="text-[10px]">{title}</p>
                    {subtitle && <p className="text-[10px]">{subtitle}</p>}
                </div>

                <div className="h-full p-2 mt-8">{children}</div>
            </div>
        </button>
    </li>
);

export default ItemCard;
