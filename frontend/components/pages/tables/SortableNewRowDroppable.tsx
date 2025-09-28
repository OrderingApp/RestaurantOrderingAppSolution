import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { cn } from '@/lib/utils'; // Assuming this utility is available

interface SortableNewRowDroppableProps {
    id: string;
}

const SortableNewRowDroppable: React.FC<SortableNewRowDroppableProps> = ({
    id,
}) => {
    const {
        attributes,
        listeners,
        setNodeRef,
        transform,
        transition,
        isOver, // Get isOver from useSortable directly
    } = useSortable({ id });

    const style = {
        transform: CSS.Transform.toString(transform),
        transition,
        zIndex: 1, // Ensure it's not hidden
    };

    return (
        <div
            ref={setNodeRef}
            style={style}
            {...attributes}
            {...listeners}
            className={cn(
                'h-[52px] border border-dashed rounded-2xl flex items-center justify-center text-dark-gray transition-colors text-sm mt-6',
                'min-w-[52px]', // Min width so it's always visible
                isOver
                    ? 'border-primary-500 bg-primary-50 text-primary-700'
                    : 'border-gray-300 bg-gray-50'
            )}
        >
            {isOver
                ? 'Drop here to create a new row'
                : 'Drag a seat here to create a new row'}
        </div>
    );
};

export default SortableNewRowDroppable;
