import { useState } from 'react';
import clsx from 'clsx';

import Button, { type ButtonProps } from '../../Button/Button';
import ItemsList from './Items';

import { capitalizeFirstLetter } from '@/helpers/utils/utils';
import { formatPriceStr } from '@/helpers/utils/prices';
import { CURRENCIES } from '@/helpers/constants/constants';

export interface BaseItemProps {
    name: string;
    price: number;
    currency: keyof typeof CURRENCIES;
    onClick?: () => void;
    quantity?: number;
    annotation?: string;
    button?: Omit<ButtonProps, 'className' | 'children'> & { name?: string };
    nestedItems?: BaseItemProps[];
}

export type ButtonItemProps = Required<
    Omit<BaseItemProps, 'quantity' | 'annotation' | 'nestedItems'>
>;

export type ItemProps = BaseItemProps | ButtonItemProps;

const Item = ({ name, price, currency, button, ...props }: ItemProps) => {
    const castedProps = props as BaseItemProps;
    const [quantity, annotation, nestedItems] = [
        castedProps?.quantity,
        castedProps?.annotation,
        castedProps?.nestedItems,
    ];

    const priceStr = formatPriceStr({ currency, price, quantity });

    return button ? (
        <div className="flex flex-col items-center gap-1">
            <h2 className="text-xl/8 capitalize font-bold">{name}</h2>

            <p className="text-primary font-bold">{priceStr}</p>
            <Button className="px-14" {...button}>
                {button.name ?? 'Informacje'}
            </Button>
        </div>
    ) : (
        <ListItem {...{ name, annotation, nestedItems, priceStr, quantity }} />
    );
};

const ListItem = ({
    name,
    annotation,
    nestedItems,
    priceStr,
    quantity,
    onClick,
}: Pick<
    BaseItemProps,
    'name' | 'onClick' | 'quantity' | 'annotation' | 'nestedItems'
> & {
    priceStr: ReturnType<typeof formatPriceStr>;
}) => {
    const [isExpanded, setIsExpanded] = useState(false);

    const toggleExpand = () => setIsExpanded((prev) => !prev);

    return (
        <button
            onClick={nestedItems ? toggleExpand : onClick}
            className="grid grid-cols-[1fr_auto] items-center gap-x-4 py-2 px-2 min-h-12 rounded-md shadow-[0px_0px_5px_0px_rgba(0,_0,_0,_0.22)]"
        >
            <dt className="font-bold leading-none capitalize">
                {name}
                <span className="text-[12px] font-normal">x{quantity}</span>
            </dt>
            <dd
                className={clsx(
                    'text-xs text-primary',
                    annotation && 'row-span-2'
                )}
            >
                {priceStr}
            </dd>

            {annotation && (
                <dd className="text-xs text-danger-dark">
                    {capitalizeFirstLetter(annotation)}
                </dd>
            )}

            {nestedItems && isExpanded && <ItemsList items={nestedItems} />}
        </button>
    );
};

//TODO: make button proper, needs to adjust variants/sizes in the component itself
//TODO: 'Informacje' -> get from lang pack

export default Item;
