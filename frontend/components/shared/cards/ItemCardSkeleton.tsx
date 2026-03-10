import { Skeleton } from '@/components/ui/skeleton';

import { ITEM_CARD_HEIGHT, ITEM_CARD_WIDTH } from './ItemCard';

const ItemCardSkeleton = ({ index }: { index: number }) => {
    return (
        <div
            style={{
                width: ITEM_CARD_WIDTH,
                height: ITEM_CARD_HEIGHT,
            }}
            key={index}
            className="flex flex-col gap-3 p-4 border rounded-xl shadow-sm"
        >
            <Skeleton className="w-24 h-4" />
            <Skeleton className="w-full h-6 mt-auto" />
            <Skeleton className="w-full h-6" />
        </div>
    );
};

export default ItemCardSkeleton;
