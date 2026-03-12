import { Fragment, useState, useMemo } from 'react';
import clsx from 'clsx';

import Item, { type ItemProps } from './Item';
import type { DistributiveOmitPartial } from '@/lib/types/types';
import ExpandableSection from '../../animations/Expendable';

interface List {
    id: string;
    isExpanded: boolean;
}

export type ToggleListProps = Omit<List, 'isExpanded'>;

export type ItemsListProps = {
    items: DistributiveOmitPartial<ItemProps, 'onClick'>[];
    onSelectItem?: (id: string) => void;
    selectedItemId?: string | null;
};

const ItemsList = ({ items, onSelectItem, selectedItemId }: ItemsListProps) => {
    const [expandedIds, setExpandedIds] = useState<Set<string>>(new Set());

    const itemIds = useMemo(
        () => items.filter((item) => item.id).map((item) => item.id!),
        [items]
    );

    const nestedLists = useMemo(
        () =>
            itemIds.map((id) => ({
                id,
                isExpanded: expandedIds.has(id),
            })),
        [itemIds, expandedIds]
    );

    const toggleExpansion = (id: string) => {
        setExpandedIds((prev) => {
            const newSet = new Set(prev);
            if (newSet.has(id)) {
                newSet.delete(id);
            } else {
                newSet.add(id);
            }
            return newSet;
        });
    };

    const handleSelect = (id: string, e: React.MouseEvent) => {
        e.stopPropagation();
        onSelectItem?.(id);
    };

    return (
        <dl className="grid gap-3 py-1 p-2 overflow-y-auto hide-scrollbar">
            {items.map((item, i) => {
                const nestedList = nestedLists.find(({ id }) => id === item.id);

                const isExpanded = nestedList?.isExpanded;
                const isSelected = selectedItemId === item.id;

                return (
                    <Fragment key={`${item.name}-${item.price}-${i}`}>
                        <div className="flex items-center gap-1">
                            {/* Selection indicator - only show if onSelectItem is provided and item has id */}
                            {onSelectItem && item.id && (
                                <button
                                    type="button"
                                    onClick={(e) => handleSelect(item.id!, e)}
                                    className={clsx(
                                        'flex-shrink-0 w-5 h-5 rounded-full border-2 transition-colors',
                                        isSelected
                                            ? 'border-primary bg-primary'
                                            : 'border-gray-400 bg-white hover:border-primary'
                                    )}
                                    aria-label={`Select ${item.name}`}
                                >
                                    {isSelected && (
                                        <span className="flex items-center justify-center w-full h-full">
                                            <span className="w-2 h-2 bg-white rounded-full" />
                                        </span>
                                    )}
                                </button>
                            )}
                            <div className="flex-1">
                                <Item
                                    {...{
                                        ...item,
                                        onClick: nestedList
                                            ? () => toggleExpansion(item.id!)
                                            : (item.onClick ?? (() => {})),
                                        className: `${item.className || ''} ${
                                            isExpanded
                                                ? 'rounded-t-md'
                                                : 'rounded-md'
                                        }`.trim(),
                                    }}
                                />
                            </div>
                        </div>

                        <ExpandableSection
                            isOpen={isExpanded}
                            className="-mt-3 shadow-[0px_2px_5px_0px_#6A6A6A] rounded-b-md "
                        >
                            <ul>
                                {item.nestedItems?.map((item, i, arr) => (
                                    <Item
                                        key={`${item.name}-${item.price}-${i}-${item.id}`}
                                        className={clsx(
                                            i % 2 === 0
                                                ? 'bg-gray'
                                                : 'bg-lighter-gray'
                                        )}
                                        isLastItem={i === arr.length - 1}
                                        {...item}
                                    />
                                ))}
                            </ul>
                        </ExpandableSection>
                    </Fragment>
                );
            })}
        </dl>
    );
};
export default ItemsList;
