import { itemCardStyles } from '@/lib/styles/itemCard';
import { cn } from '@/lib/utils';

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
}: ItemCardProps) => {
    const styles = itemCardStyles.variants[variant];

    return (
        <li>
            <button onClick={onClick}>
                <div
                    className={cn(
                        'w-44 h-28 rounded-lg overflow-hidden relative',
                        styles.container,
                        className
                    )}
                >
                    <div
                        className={cn(
                            'absolute left-[-2px] top-0 w-44 flex justify-between items-center px-2 h-8 border-b border-l border-r border-black',
                            styles.appearance,
                            variantClassName
                        )}
                    >
                        <p className="text-[11px]">{title}</p>
                        {subtitle && <p className="text-[11px]">{subtitle}</p>}
                    </div>

                    <div className="h-full p-2 mt-10">{children}</div>
                </div>
            </button>
        </li>
    );
};

export default ItemCard;
