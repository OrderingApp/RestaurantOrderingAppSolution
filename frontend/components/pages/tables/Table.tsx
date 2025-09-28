// src/components/Table.tsx

import React, { useState, useMemo, useCallback } from 'react';
import {
    DndContext,
    closestCenter,
    KeyboardSensor,
    PointerSensor,
    TouchSensor,
    useSensor,
    useSensors,
    DragEndEvent,
    DragStartEvent,
    DragCancelEvent,
} from '@dnd-kit/core';
import {
    SortableContext,
    verticalListSortingStrategy,
    horizontalListSortingStrategy,
    arrayMove,
    sortableKeyboardCoordinates,
} from '@dnd-kit/sortable';

import SortableSeat from './SortableSeat';
import SortableEmptySeat from './SortableEmptySeat';
import SortableNewRowDroppable from './SortableNewRowDroppable';
import { cn } from '@/lib/utils';

type SeatItem = string | null;

interface TableProps {
    id: string;
    layout: number[];
    onSeatDragChange: (isDragging: boolean) => void;
}

const Table = ({ id, layout: initialLayout, onSeatDragChange }: TableProps) => {
    const [rows, setRows] = useState<SeatItem[][]>(() => {
        // Initialize with default maximum columns for any row based on initialLayout
        // This ensures rows are created with enough empty slots initially
        const maxCols = Math.max(...initialLayout, 1); // At least 1 column
        return initialLayout.map((seatCount, rowIndex) => {
            const row: SeatItem[] = Array.from({ length: maxCols }).fill(null); // All nulls initially
            for (let i = 0; i < seatCount; i++) {
                row[i] = `${id}-r${rowIndex}-c${i}`; // Place seats
            }
            return row;
        });
    });

    const sensors = useSensors(
        useSensor(PointerSensor, { activationConstraint: { distance: 8 } }),
        useSensor(TouchSensor, {
            activationConstraint: {
                delay: 200,
                distance: 10,
            },
        }),
        useSensor(KeyboardSensor, {
            coordinateGetter: sortableKeyboardCoordinates,
        })
    );

    const handleDragStart = useCallback(
        (event: DragStartEvent) => {
            onSeatDragChange(true);
        },
        [onSeatDragChange]
    );

    const handleDragCancel = useCallback(
        (event: DragCancelEvent) => {
            onSeatDragChange(false);
        },
        [onSeatDragChange]
    );

    const handleDragEnd = useCallback(
        (event: DragEndEvent) => {
            onSeatDragChange(false);
            const { active, over } = event;

            if (!over) {
                setRows(rows); // Revert if dropped nowhere valid
                return;
            }

            const activeId = String(active.id);
            const overId = String(over.id);

            let newRows = rows.map((row) => [...row]); // Deep copy
            let activeRowIndex: number = -1;
            let activeColIndex: number = -1;

            // Find active item's position
            for (let r = 0; r < newRows.length; r++) {
                const cIdx = newRows[r].indexOf(activeId);
                if (cIdx !== -1) {
                    activeRowIndex = r;
                    activeColIndex = cIdx;
                    break;
                }
            }

            if (activeRowIndex === -1) {
                console.warn(`Active seat ${activeId} not found.`);
                return;
            }

            const movedSeatId = newRows[activeRowIndex][activeColIndex];
            // Vacate the original spot *before* trying to place it elsewhere
            newRows[activeRowIndex][activeColIndex] = null;

            let dropHandled = false;

            // --- Determine the drop target ---
            // SCENARIO 1: Dropped on the "new row" droppable
            if (overId === `new-row-droppable-${id}`) {
                if (movedSeatId) {
                    // Only add if it was an actual seat
                    newRows.push([movedSeatId]);
                }
                dropHandled = true;
            }
            // SCENARIO 2: Dropped on an empty slot or another seat
            else {
                let overRowIndex: number = -1;
                let overColIndex: number = -1;
                let isOverEmptySlot: boolean = false;

                for (let r = 0; r < newRows.length; r++) {
                    const seatCIdx = newRows[r].indexOf(overId);
                    if (seatCIdx !== -1) {
                        overRowIndex = r;
                        overColIndex = seatCIdx;
                        break;
                    } else {
                        const emptySlotCIdx = newRows[r].findIndex(
                            (item, colIdx) =>
                                item === null &&
                                overId === `${id}-empty-${r}-${colIdx}`
                        );
                        if (emptySlotCIdx !== -1) {
                            overRowIndex = r;
                            overColIndex = emptySlotCIdx;
                            isOverEmptySlot = true;
                            break;
                        }
                    }
                }

                if (overRowIndex !== -1 && overColIndex !== -1) {
                    if (isOverEmptySlot) {
                        // Place into the empty slot
                        if (movedSeatId) {
                            newRows[overRowIndex][overColIndex] = movedSeatId;
                        }
                    } else {
                        // Dropped on an occupied seat (within same row or different)
                        if (activeRowIndex === overRowIndex) {
                            // Same row, horizontal reorder
                            const rowSeats = newRows[activeRowIndex].filter(
                                (item): item is string => item !== null
                            );
                            const overSeatIndexInFiltered =
                                rowSeats.indexOf(overId);

                            if (movedSeatId && overSeatIndexInFiltered !== -1) {
                                // Find the position where `movedSeatId` would be if it were in `rowSeats` already
                                // and shift based on that, essentially moving it relative to `overId`
                                const targetIndex = overSeatIndexInFiltered;
                                rowSeats.splice(targetIndex, 0, movedSeatId);
                                // Then remove the original instance of movedSeatId if it was already in `rowSeats` (which it wouldn't be since we removed it)

                                // To reconstruct: clear the original row and then fill with reordered seats
                                newRows[activeRowIndex].fill(null);
                                rowSeats.forEach((seat, idx) => {
                                    if (idx < newRows[activeRowIndex].length) {
                                        // Ensure within bounds
                                        newRows[activeRowIndex][idx] = seat;
                                    }
                                });
                            }
                        } else {
                            // Different row, insert into target row
                            if (movedSeatId) {
                                newRows[overRowIndex].splice(
                                    overColIndex,
                                    0,
                                    movedSeatId
                                );
                                // If this makes the row too long, consider removing a null from the end or expanding row
                                // For now, let it expand if needed. We'll clean up empty rows later.
                            }
                        }
                    }
                    dropHandled = true;
                }
            }

            if (!dropHandled) {
                // If the drop wasn't handled by any specific scenario, revert the active item
                newRows[activeRowIndex][activeColIndex] = movedSeatId;
                setRows(rows);
                return;
            }

            // --- Post-drag cleanup ---
            // 1. Ensure all rows have the same maximum number of columns,
            // or a reasonable default (e.g., max of current rows, or min 3)
            const currentMaxCols = Math.max(
                3,
                ...newRows.map((row) => row.length)
            );
            newRows = newRows.map((row) => {
                while (row.length < currentMaxCols) {
                    row.push(null);
                }
                // Trim excess nulls from the end of a row if it ends up with too many
                // while (row.length > 0 && row[row.length - 1] === null && row.filter(Boolean).length < row.length -1) {
                //      row.pop();
                // }
                return row;
            });

            // 2. Remove rows that are entirely empty (contain only nulls)
            newRows = newRows.filter((row) =>
                row.some((item) => item !== null)
            );

            // If all rows become empty, ensure at least one empty row for dropping
            if (newRows.length === 0) {
                newRows.push(Array(currentMaxCols).fill(null));
            }

            setRows(newRows);
        },
        [id, onSeatDragChange, rows]
    );

    // Function to delete a seat
    const handleDeleteSeat = useCallback((seatIdToDelete: string) => {
        setRows((prevRows) => {
            let updatedRows = prevRows.map((row) => [...row]);
            for (let r = 0; r < updatedRows.length; r++) {
                const cIdx = updatedRows[r].indexOf(seatIdToDelete);
                if (cIdx !== -1) {
                    updatedRows[r][cIdx] = null; // Turn into an empty slot
                    break;
                }
            }
            // Clean up: remove rows that are now entirely empty
            updatedRows = updatedRows.filter((row) =>
                row.some((item) => item !== null)
            );
            if (updatedRows.length === 0) {
                // If all removed, add a default empty row
                const currentMaxCols = Math.max(
                    3,
                    ...prevRows.map((row) => row.length)
                );
                updatedRows.push(Array(currentMaxCols).fill(null));
            }
            return updatedRows;
        });
    }, []);

    // Memoized list of sortable items (row IDs and the new-row-droppable ID)
    const verticalSortableItems = useMemo(() => {
        const rowIds = rows.flatMap((row, i) =>
            row.some((item) => item !== null) ? [`row-${id}-${i}`] : []
        );
        return [...rowIds, `new-row-droppable-${id}`];
    }, [rows, id]);

    // Memoized list of horizontal sortable items for each row
    const getHorizontalSortableItems = useCallback(
        (rowSeats: SeatItem[], rowIndex: number) => {
            return rowSeats.map(
                (seatId, colIndex) =>
                    seatId || `${id}-empty-${rowIndex}-${colIndex}`
            );
        },
        [id]
    );

    return (
        <ul className="p-6 border border-dashed border-dark-gray rounded-2xl bg-white shadow-md w-fit flex flex-col gap-6">
            <DndContext
                sensors={sensors}
                collisionDetection={closestCenter}
                onDragStart={handleDragStart}
                onDragEnd={handleDragEnd}
                onDragCancel={handleDragCancel}
            >
                <SortableContext
                    items={verticalSortableItems}
                    strategy={verticalListSortingStrategy}
                >
                    {rows.map((rowSeats, rowIndex) => (
                        <li
                            key={`row-${id}-${rowIndex}`}
                            className="flex gap-6 items-center"
                        >
                            <SortableContext
                                items={getHorizontalSortableItems(
                                    rowSeats,
                                    rowIndex
                                )}
                                strategy={horizontalListSortingStrategy}
                            >
                                {rowSeats.map((seatId, colIndex) =>
                                    seatId ? (
                                        <SortableSeat
                                            key={seatId}
                                            id={seatId}
                                            onDelete={handleDeleteSeat} // Pass delete handler
                                        />
                                    ) : (
                                        <SortableEmptySeat
                                            key={`${id}-empty-${rowIndex}-${colIndex}`}
                                            id={`${id}-empty-${rowIndex}-${colIndex}`}
                                        />
                                    )
                                )}
                            </SortableContext>
                        </li>
                    ))}

                    <SortableNewRowDroppable id={`new-row-droppable-${id}`} />
                </SortableContext>
            </DndContext>
        </ul>
    );
};

export default Table;

// issues:
// empty seats at cols whjere no seats are at all
// on touch devices last time checked it didnt work
// still to do tables repositioning (x, y)
// if e.g. 3 rows 3 cols the 2nd row seat doesnt haev the ability to go into 2nd col  ???
//  restructure the seats i suppose as they seem to be a lot
