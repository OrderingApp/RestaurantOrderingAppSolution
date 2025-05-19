import { Fragment, useState } from 'react';
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
};

const ItemsList = ({ items }: ItemsListProps) => {
    const [nestedLists, setNestedLists] = useState<List[]>(() =>
        items.reduce((acc, item) => {
            if (item.id) {
                acc.push({
                    id: item.id,
                    isExpanded: false,
                });
            }
            return acc;
        }, [] as List[])
    );

    const toggleList = (id: string) =>
        setNestedLists((prev) =>
            prev.map((list) =>
                list.id === id
                    ? { ...list, isExpanded: !list.isExpanded }
                    : list
            )
        );

    return (
        <dl className="grid gap-3 py-1 p-2 overflow-y-auto hide-scrollbar">
            {items.map((item, i) => {
                const nestedList = nestedLists.find(({ id }) => id === item.id);

                const isExpanded = nestedList?.isExpanded;

                return (
                    <Fragment key={`${item.name}-${item.price}-${i}`}>
                        <Item
                            {...{
                                ...item,
                                onClick: nestedList
                                    ? toggleList
                                    : (item.onClick ?? (() => {})),
                                className: isExpanded
                                    ? 'rounded-t-md'
                                    : 'rounded-md',
                            }}
                        />

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
