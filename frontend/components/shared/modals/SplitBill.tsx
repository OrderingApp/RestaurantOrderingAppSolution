'use client';

import { useState, useMemo, useCallback } from 'react';
import {
    DndContext,
    DragOverlay,
    closestCenter,
    useDraggable,
    useDroppable,
    PointerSensor,
    TouchSensor,
    useSensor,
    useSensors,
    type DragStartEvent,
    type DragEndEvent,
} from '@dnd-kit/core';

import useLanguage from '@/helpers/hooks/useLanguage';
import languagePacks from '@/helpers/constants/languagePacks';
import { BACKEND_URL } from '@/helpers/constants/constants';
import { formatPriceStr } from '@/helpers/utils/prices';
import type { OrderItem } from '@/helpers/interfaces/orders';
import { toast } from 'sonner';
import Button from '../button/Button';

/* ------------------------------------------------------------------ */
/*  Types                                                              */
/* ------------------------------------------------------------------ */

interface SplitBillItem {
    orderItemId: string;
    name: string;
    extras: string[];
    price: number;
    quantity: number;
}

interface Bill {
    id: string;
    label: string;
    items: SplitBillItem[];
}

interface SplitBillProps {
    onClose: () => void;
    orderId: string;
    orderItems: OrderItem[];
    onSplitSuccess?: () => void;
}

/* ------------------------------------------------------------------ */
/*  Helpers                                                            */
/* ------------------------------------------------------------------ */

const buildExtras = (item: OrderItem): string[] => {
    const extras: string[] = [];
    if (item.extraIngredients?.length) {
        extras.push(
            ...item.extraIngredients.map((e) => `+${e.ingredientName}`)
        );
    }
    if (item.removedIngredients?.length) {
        extras.push(
            ...item.removedIngredients.map((r) => `-${r.ingredientName}`)
        );
    }
    return extras;
};

const buildInitialBill = (orderItems: OrderItem[]): Bill => ({
    id: 'original',
    label: '',
    items: orderItems.map((oi) => ({
        orderItemId: oi.id,
        name: oi.menuItem.name,
        extras: buildExtras(oi),
        price: oi.price,
        quantity: 1,
    })),
});

/* ------------------------------------------------------------------ */
/*  Draggable item                                                     */
/* ------------------------------------------------------------------ */

function DraggableBillItem({
    item,
    billId,
    itemIndex,
}: {
    item: SplitBillItem;
    billId: string;
    itemIndex: number;
}) {
    const dragId = `${billId}::${item.orderItemId}`;
    const { attributes, listeners, setNodeRef, isDragging } = useDraggable({
        id: dragId,
        data: { billId, orderItemId: item.orderItemId },
    });

    return (
        <div
            ref={setNodeRef}
            {...listeners}
            {...attributes}
            style={{ touchAction: 'none' }}
            className={`flex items-start justify-between px-3 py-2 cursor-grab active:cursor-grabbing select-none transition-opacity ${
                itemIndex % 2 === 0 ? 'bg-[#EBEBEB]' : 'bg-[#D7D7D7]'
            } ${isDragging ? 'opacity-30' : 'opacity-100'}`}
        >
            <div className="flex-1 min-w-0">
                <p className="font-bold text-sm text-gray-900 truncate">
                    {item.name}
                </p>
                {item.extras.length > 0 && (
                    <p className="text-xs text-gray-500 truncate">
                        {item.extras.join(', ')}
                    </p>
                )}
            </div>
            <p className="font-bold text-sm text-gray-900 ml-3 shrink-0">
                {formatPriceStr({ currency: 'pln', price: item.price })}
            </p>
        </div>
    );
}

/* ------------------------------------------------------------------ */
/*  Droppable bill card                                                */
/* ------------------------------------------------------------------ */

function DroppableBillCard({
    bill,
    billIndex,
    labelPrefix,
    isSelected,
    onSelect,
}: {
    bill: Bill;
    billIndex: number;
    labelPrefix: string;
    isSelected: boolean;
    onSelect: () => void;
}) {
    const { isOver, setNodeRef } = useDroppable({ id: bill.id });
    const label = `${labelPrefix} ${billIndex + 1}`;

    return (
        <div
            ref={setNodeRef}
            onClick={onSelect}
            className={`rounded-2xl transition-all min-h-[180px] flex flex-col shadow-md ${
                isOver
                    ? 'bg-[#f2fbfb] shadow-lg'
                    : isSelected
                      ? 'bg-[#f7f7f7] shadow-lg'
                      : 'bg-white'
            }`}
        >
            {/* Header */}
            <button
                onClick={onSelect}
                className="w-full rounded-t-2xl bg-primary px-4 py-2 text-center text-sm font-semibold text-white flex items-center justify-center gap-1"
            >
                {label} <span className="text-xs">▾</span>
            </button>

            {/* Items */}
            <div className="flex-1 p-0 overflow-y-auto max-h-[300px]">
                {bill.items.length === 0 && (
                    <p className="text-gray-400 text-xs text-center py-4">—</p>
                )}
                {bill.items.map((item, idx) => (
                    <DraggableBillItem
                        key={item.orderItemId}
                        item={item}
                        billId={bill.id}
                        itemIndex={idx}
                    />
                ))}
            </div>
        </div>
    );
}

/* ------------------------------------------------------------------ */
/*  "Choose bill" drop placeholder                                     */
/* ------------------------------------------------------------------ */

function ChooseBillDropzone({ label }: { label: string }) {
    const { isOver, setNodeRef } = useDroppable({ id: '__choose__' });

    return (
        <div
            ref={setNodeRef}
            className={`rounded-2xl min-h-[180px] flex flex-col transition-all shadow-md ${
                isOver ? 'bg-[#00808010] shadow-lg' : 'bg-white'
            }`}
        >
            <div className="w-full rounded-t-2xl bg-[#008080] px-4 py-2 text-center text-sm font-semibold text-white">
                {label}
            </div>
            <div className="flex-1" />
        </div>
    );
}

/* ------------------------------------------------------------------ */
/*  Overlay during drag                                                */
/* ------------------------------------------------------------------ */

function DragOverlayContent({ item }: { item: SplitBillItem }) {
    return (
        <div className="bg-white border border-[#008080] rounded-lg px-3 py-2 shadow-xl flex items-start justify-between min-w-[200px]">
            <div className="flex-1 min-w-0">
                <p className="font-bold text-sm text-gray-900 truncate">
                    {item.name}
                </p>
                {item.extras.length > 0 && (
                    <p className="text-xs text-gray-500 truncate">
                        {item.extras.join(', ')}
                    </p>
                )}
            </div>
            <div className="text-right ml-3 shrink-0">
                <p className="font-bold text-sm text-gray-900">
                    {formatPriceStr({ currency: 'pln', price: item.price })}
                </p>
            </div>
        </div>
    );
}

/* ------------------------------------------------------------------ */
/*  Main component                                                     */
/* ------------------------------------------------------------------ */

export default function SplitBill({
    onClose,
    orderId,
    orderItems,
    onSplitSuccess,
}: SplitBillProps) {
    const { language } = useLanguage();
    const t = languagePacks[language].splitBillModal;

    // ---- state ----
    const [bills, setBills] = useState<Bill[]>(() => [
        buildInitialBill(orderItems),
    ]);
    const [selectedBillId, setSelectedBillId] = useState('original');
    const [activeItem, setActiveItem] = useState<SplitBillItem | null>(null);
    const [isSending, setIsSending] = useState(false);

    const billCounter = useMemo(() => bills.length, [bills]);

    const sensors = useSensors(
        useSensor(PointerSensor, { activationConstraint: { distance: 5 } }),
        useSensor(TouchSensor, {
            activationConstraint: { delay: 200, tolerance: 5 },
        })
    );

    // ---- tab helpers ----
    const activeBillIds = useMemo(() => bills.map((b) => b.id), [bills]);

    // ---- add new bill ----
    const handleAddBill = useCallback(() => {
        const newId = `new-${Date.now()}`;
        setBills((prev) => [...prev, { id: newId, label: '', items: [] }]);
        setSelectedBillId(newId);
    }, []);

    // ---- DnD handlers ----
    const handleDragStart = useCallback(
        (event: DragStartEvent) => {
            const { billId, orderItemId } = event.active.data.current as {
                billId: string;
                orderItemId: string;
            };
            const bill = bills.find((b) => b.id === billId);
            const item = bill?.items.find((i) => i.orderItemId === orderItemId);
            if (item) setActiveItem(item);
        },
        [bills]
    );

    const handleDragEnd = useCallback((event: DragEndEvent) => {
        setActiveItem(null);
        const { over, active } = event;
        if (!over) return;

        const { billId: fromBillId, orderItemId } = active.data.current as {
            billId: string;
            orderItemId: string;
        };
        const toBillId = over.id as string;

        if (toBillId === '__choose__' || fromBillId === toBillId) return;

        setBills((prev) => {
            const fromIdx = prev.findIndex((b) => b.id === fromBillId);
            const toIdx = prev.findIndex((b) => b.id === toBillId);
            if (fromIdx === -1 || toIdx === -1) return prev;

            const item = prev[fromIdx].items.find(
                (i) => i.orderItemId === orderItemId
            );
            if (!item) return prev;

            const next = [...prev];
            next[fromIdx] = {
                ...next[fromIdx],
                items: next[fromIdx].items.filter(
                    (i) => i.orderItemId !== orderItemId
                ),
            };
            next[toIdx] = {
                ...next[toIdx],
                items: [...next[toIdx].items, item],
            };
            return next;
        });
    }, []);

    // ---- submit split ----
    const handleSplit = useCallback(async () => {
        // Build splitGroups — every bill except 'original' becomes a group
        const splitGroups = bills
            .filter((b) => b.id !== 'original' && b.items.length > 0)
            .map((b) => ({
                orderItemIds: b.items.map((i) => i.orderItemId),
            }));

        if (splitGroups.length === 0) {
            toast.error(t.toastNoChanges);
            return;
        }

        setIsSending(true);
        try {
            const res = await fetch(`${BACKEND_URL}/orders/${orderId}/split`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ splitGroups }),
            });

            if (!res.ok) {
                const text = await res.text().catch(() => '');
                throw new Error(text || t.toastError);
            }

            toast.success(t.toastSuccess);
            onSplitSuccess?.();
            onClose();
        } catch (err) {
            toast.error(err instanceof Error ? err.message : t.toastError);
        } finally {
            setIsSending(false);
        }
    }, [bills, orderId, onClose, onSplitSuccess, t]);

    // ---- render ----
    return (
        <div className="p-6 h-full overflow-hidden flex flex-col">
            {/* Tabs */}
            <div className="flex items-center gap-2 mb-5 overflow-x-auto pb-1">
                {bills.map((bill, idx) => (
                    <button
                        key={bill.id}
                        onClick={() => setSelectedBillId(bill.id)}
                        className={`px-6 py-3 rounded-t-xl text-base font-semibold whitespace-nowrap transition-colors ${
                            selectedBillId === bill.id
                                ? 'bg-primary text-white'
                                : 'bg-white text-gray-600 hover:text-gray-900'
                        }`}
                    >
                        {t.billTab} {idx + 1}
                    </button>
                ))}
                <button
                    onClick={handleAddBill}
                    className="w-11 h-11 rounded-full bg-primary hover:opacity-90 text-white text-2xl leading-none flex items-center justify-center shrink-0 transition-colors"
                    title={t.addBill}
                >
                    +
                </button>
            </div>

            {/* Bill cards grid with DnD */}
            <DndContext
                sensors={sensors}
                collisionDetection={closestCenter}
                onDragStart={handleDragStart}
                onDragEnd={handleDragEnd}
            >
                <div className="grid grid-cols-2 gap-4 flex-1 overflow-y-auto overflow-x-visible min-h-0 pb-4 px-1">
                    {activeBillIds.map((billId) => {
                        const bill = bills.find((b) => b.id === billId)!;
                        const globalIdx = bills.indexOf(bill);
                        return (
                            <DroppableBillCard
                                key={billId}
                                bill={bill}
                                billIndex={globalIdx}
                                labelPrefix={t.billTab}
                                isSelected={selectedBillId === billId}
                                onSelect={() => setSelectedBillId(billId)}
                            />
                        );
                    })}

                    {/* If there's a "selected" bill that is empty and not visible, show placeholder */}
                    {bills.length < 2 && (
                        <ChooseBillDropzone label={t.chooseBill} />
                    )}
                </div>

                <DragOverlay>
                    {activeItem && <DragOverlayContent item={activeItem} />}
                </DragOverlay>
            </DndContext>

            {/* Action buttons */}
            <div className="flex justify-end gap-3 mt-5">
                <Button
                    onClick={onClose}
                    variant="tertiary"
                    size="md"
                    className="px-2 py-3.5 rounded-xl  text-secondary-foreground hover:opacity-90 text-base font-semibold transition-colors"
                >
                    {t.closeNoChanges}
                </Button>
                <Button
                    onClick={handleSplit}
                    disabled={isSending}
                    variant="primary"
                    size="md"
                    className="px-8 py-3.5 rounded-xl hover:opacity-90 disabled:opacity-50 text-white text-base font-semibold transition-colors"
                >
                    {isSending ? '...' : t.splitBill}
                </Button>
            </div>
        </div>
    );
}
