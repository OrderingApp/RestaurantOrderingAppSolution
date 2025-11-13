import clsx from 'clsx';

import { type ButtonProps } from '@/components/shared/Button/Button';
import Dish from '@/components/shared/lists/Items/Dish';

import { capitalizeFirstLetter } from '@/helpers/utils/utils';
import { formatPriceStr } from '@/helpers/utils/prices';
import type { Currency } from '@/helpers/type/types';

interface NotNestedItem {
    nestedItems?: never;
    id?: never;
}

interface HasNestedItems {
    id: string;
    nestedItems: ItemProps[];
}

export type ItemProps = {
    name: string;
    price: number;
    currency: Currency; // eslint-disable-next-line @typescript-eslint/no-explicit-any
    onClick: (e: any) => unknown;
    isLastItem?: boolean;
    isSingleItem?: boolean;
    isServed?: boolean;
    className?: string;
    quantity?: number;
    annotation?: string[];
    button?: Omit<ButtonProps, 'className' | 'children'> & { name?: string };
} & (NotNestedItem | HasNestedItems);

const Item = ({
    name,
    price,
    currency,
    quantity,
    annotation,
    isSingleItem,
    className = '',
    ...props
}: ItemProps) => {
    const priceStr = formatPriceStr({ currency, price, quantity });

    const isList = (
        item: HasNestedItems | NotNestedItem
    ): item is HasNestedItems => !!item?.id;

    return isList(props) || isSingleItem ? (
        <button
            onClick={(e) => props.onClick(props.id || e)}
            className={`grid grid-cols-[1fr_auto_auto] items-center w-full gap-x-4 gap-y-0.5 py-2 px-2 shadow-[0px_0px_7px_0px_rgba(0,_0,_0,_0.4)] ${className}`}
        >
            <dt className="justify-self-start text-left text-balance font-bold leading-none">
                {capitalizeFirstLetter(name)}
            </dt>

            {quantity && <dd className="text-xs font-normal">x{quantity}</dd>}

            <dd
                className={clsx(
                    'text-primary',
                    quantity && 'text-xs font-semibold',
                    !quantity && 'col-span-2 font-bold'
                )}
            >
                {priceStr}
            </dd>

            {annotation && (
                <dd className="text-start text-xs text-danger-dark font-semibold col-span-full">
                    <ul className="flex flex-wrap">
                        {annotation.map((str, i) => {
                            const capitalizedStr = capitalizeFirstLetter(str);
                            const divider =
                                i === annotation.length - 1 ? '' : ',\u00A0';

                            return (
                                <li key={str + i} className="w-max">
                                    {capitalizedStr + divider}
                                </li>
                            );
                        })}
                    </ul>
                </dd>
            )}
        </button>
    ) : (
        <Dish
            {...{ name, quantity, annotation, priceStr, className, ...props }}
        />
    );
};

export default Item;
