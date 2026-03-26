'use client';

import clsx from 'clsx';
import { ItemProps } from './Item';
import { capitalizeFirstLetter } from '@/helpers/utils/utils';
import SwipeableRow from '@/components/shared/rows/SwipeableRow';

type Dish = Omit<
    ItemProps,
    'button' | 'nestedItems' | 'id' | 'price' | 'currency' | 'isIngredient'
> & { priceStr: string };

const Dish = ({
    onClick,
    onSwipeLeft,
    onSwipeRight,
    className,
    name,
    priceStr,
    quantity,
    annotation,
    annotationClassName,
    isServed,
    isLastItem,
}: Dish) => {
    return (
        <SwipeableRow onSwipeLeft={onSwipeLeft} onSwipeRight={onSwipeRight}>
            {({ shouldShowAction }) => (
                <button
                    onClick={onClick}
                    className={clsx(
                        'relative grid grid-cols-[1fr_auto_auto] items-center gap-x-4 gap-y-0.5 w-full text-sm font-semibold py-1 pl-3 pr-2',
                        className
                    )}
                >
                    <span
                        className={clsx(
                            'absolute top-0 left-0 w-1 h-full',
                            isServed ? 'bg-served' : 'bg-danger',
                            shouldShowAction && 'opacity-0',
                            isLastItem && 'rounded-bl-md'
                        )}
                    />

                    <span className="justify-self-start text-left">
                        {capitalizeFirstLetter(name)}
                    </span>

                    {quantity && (
                        <span className="text-xs font-normal">x{quantity}</span>
                    )}

                    <span className="text-primary">{priceStr}</span>

                    {annotation && (
                        <ul
                            className={clsx(
                                'text-start text-xs col-span-full flex flex-wrap',
                                annotationClassName ??
                                    'text-danger-dark font-semibold'
                            )}
                        >
                            {annotation.map((str, i) => {
                                const capitalizedStr =
                                    capitalizeFirstLetter(str);
                                const divider =
                                    i === annotation.length - 1
                                        ? ''
                                        : ',\u00A0';

                                return (
                                    <li key={str + i} className="w-max">
                                        {capitalizedStr + divider}
                                    </li>
                                );
                            })}
                        </ul>
                    )}
                </button>
            )}
        </SwipeableRow>
    );
};

export default Dish;
