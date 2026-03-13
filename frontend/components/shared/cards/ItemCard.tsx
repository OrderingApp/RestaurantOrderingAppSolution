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

export const ITEM_CARD_WIDTH = 176;
export const ITEM_CARD_HEIGHT = 110;

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
                <span
                    style={{ width: ITEM_CARD_WIDTH, height: ITEM_CARD_HEIGHT }}
                    className={cn(
                        'block rounded-lg overflow-hidden relative',
                        styles.container,
                        className
                    )}
                >
                    <span
                        className={cn(
                            'absolute left-[-2px] top-0 w-44 flex justify-between items-center px-2 h-8 border-b border-l border-r border-black',
                            styles.appearance,
                            variantClassName
                        )}
                    >
                        <span className="text-[11px]">{title}</span>

                        {subtitle && (
                            <span className="text-[11px]">{subtitle}</span>
                        )}
                    </span>

                    <span className="block h-full p-2 pt-10">{children}</span>
                </span>
            </button>
        </li>
    );
};

export default ItemCard;
