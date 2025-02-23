import clsx from 'clsx';

import { CURRENCIES } from '@/helpers/constants/constants';
import { capitalizeFirstLetter } from '@/helpers/utils/utils';
import Button, { type ButtonProps } from '@/components/shared/Button/Button';

export interface BaseItemProps {
    name: string;
    price: number;
    currency: keyof typeof CURRENCIES;
    quantity?: number;
    annotation?: string;
    button?: Omit<ButtonProps, 'children' | 'className'>;
}

export type ButtonItemProps = Required<
    Omit<BaseItemProps, 'quantity' | 'annotation'>
>;

export type ItemProps = BaseItemProps | ButtonItemProps;

const ItemsList = ({ items }: { items: ItemProps[] }) => (
    <dl className="grid auto-rows-fr gap-2.5 px-2 mt-5">
        {items.map((props, i) => (
            <Item key={`${props.name}-${props.price}-${i}`} {...props} />
        ))}
    </dl>
);

export const Item = ({
    name,
    price,
    currency,
    button,
    ...props
}: ItemProps) => {
    const castedProps = props as BaseItemProps;
    const [quantity, annotation] = [
        castedProps?.quantity,
        castedProps?.annotation,
    ];

    const curr = CURRENCIES[currency];
    const formattedPrice = (price * (quantity ?? 1)).toFixed(2);
    const priceStr =
        currency === 'pln'
            ? `${formattedPrice}${curr}`
            : `${curr}${formattedPrice}`;

    return button ? (
        <div className="flex flex-col items-center gap-1">
            <h2 className="text-xl/8 capitalize font-bold">{name}</h2>
            <p className="text-primary font-bold">{priceStr}</p>
            <Button className="w-full">Informacje</Button>
        </div>
    ) : (
        <div className="grid grid-cols-[1fr_auto] items-center gap-x-4 py-2 px-2 min-h-12 rounded-md shadow-[0px_0px_5px_0px_rgba(0,_0,_0,_0.22)]">
            <dt className="font-bold leading-none capitalize">{name}</dt>

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
        </div>
    );
};
//TODO: make button proper, needs to adjust variants/sizes in the component itself
export default ItemsList;
