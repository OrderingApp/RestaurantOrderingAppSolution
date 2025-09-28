// src/components/SortableSeat.tsx
import React from 'react';
import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { cn } from '@/lib/utils'; // Assuming cn utility
import { X } from 'lucide-react'; // Example delete icon, install lucide-react if not present: npm i lucide-react

// Original Seat component (now a base for the clickable/draggable seat)
const Seat = () => (
    <div className="aspect-square h-[52px] border border-dark-gray rounded-xl flex items-center justify-center">
        {/* Optional: Seat number or symbol */}
        Seat
    </div>
);

interface SortableSeatProps {
    id: string;
    onDelete: (seatId: string) => void; // New prop for delete
}

const SortableSeat = ({ id, onDelete }: SortableSeatProps) => {
    const {
        attributes,
        listeners,
        setNodeRef,
        transform,
        transition,
        isDragging,
    } = useSortable({ id });

    const style = {
        transform: CSS.Transform.toString(transform),
        transition,
        zIndex: isDragging ? 20 : 0, // Higher z-index for dragging seat
        position: 'relative',
        opacity: isDragging ? 0.7 : 1, // Visual feedback for dragging
    };

    return (
        <div
            ref={setNodeRef}
            style={style}
            className="relative draggable-seat group"
        >
            <div {...attributes} {...listeners}>
                {' '}
                {/* Listeners applied here for dragging */}
                <Seat />
            </div>
            {/* Delete button, only visible when not dragging and maybe on hover */}
            {!isDragging && (
                <button
                    onClick={() => onDelete(id)}
                    className={cn(
                        'absolute -top-2 -right-2 bg-red-500 text-white rounded-full p-0.5',
                        'h-5 w-5 flex items-center justify-center text-xs opacity-0 group-hover:opacity-100 transition-opacity'
                    )}
                    aria-label={`Delete seat ${id}`}
                >
                    <X size={12} />
                </button>
            )}
        </div>
    );
};

export default SortableSeat;
