// src/components/SortableEmptySeat.tsx
import React from 'react';
import { useSortable } from '@dnd-kit/sortable'; // Use primary useSortable
import { CSS } from '@dnd-kit/utilities';
import { cn } from '@/lib/utils'; // Assuming this utility is available

interface SortableEmptySeatProps {
    id: string; // Unique ID for the empty slot
}

const SortableEmptySeat = ({ id }: SortableEmptySeatProps) => {
    const {
        attributes,
        listeners,
        setNodeRef,
        transform,
        transition,
        isOver, // isOver directly from useSortable is useful for visual feedback
    } = useSortable({ id });

    const style = {
        transform: CSS.Transform.toString(transform),
        transition,
        zIndex: 0, // Empty slots stay in the background
    };

    return (
        <div
            ref={setNodeRef}
            style={style}
            {...attributes}
            {...listeners}
            className={cn(
                'aspect-square h-[52px] border border-dashed rounded-xl flex items-center justify-center transition-colors',
                'bg-gray-50', // Default empty background
                isOver ? 'border-primary-500 bg-primary-100' : 'border-gray-300' // Highlight when over
            )}
            aria-label="Empty seat slot" // Accessibility
        >
            {/* Optional: 'Empty' text or an icon */}
        </div>
    );
};

export default SortableEmptySeat;
