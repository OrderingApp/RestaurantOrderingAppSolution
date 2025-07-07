import clsx from 'clsx';
import { ItemProps } from './Item';
import { capitalizeFirstLetter } from '@/helpers/utils/utils';

type Dish = Omit<
    ItemProps,
    'button' | 'nestedItems' | 'id' | 'price' | 'currency' | 'isIngredient'
> & { priceStr: string };

const Dish = ({
    onClick,
    className,
    name,
    priceStr,
    quantity,
    annotation,
    isServed,
    isLastItem,
}: Dish) => {
    return (
        <li className={className}>
            <button
                onClick={onClick}
                className={clsx(
                    'relative grid grid-cols-[1fr_auto_auto] items-center gap-x-4 gap-y-0.5 w-full text-sm font-semibold py-1 pl-3 pr-2'
                )}
            >
                <span
                    className={clsx(
                        'absolute top-0 left-0 w-1 h-full',
                        isServed ? 'bg-served' : 'bg-danger',
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
                    <ul className="text-start text-xs text-danger-dark font-semibold col-span-full flex flex-wrap">
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
                )}
            </button>
        </li>
    );
};

export default Dish;
