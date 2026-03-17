import Image from 'next/image';
import { AnimatePresence, motion } from 'motion/react';
import clsx from 'clsx';
import type { ReactNode } from 'react';

import { useLanguage } from '@/providers/LanguageProvider';
import { formatPriceStr } from '@/helpers/utils/prices';

import languagePacks from '@/helpers/constants/languagePacks';
import PendingIcon from '@/public/images/svg/table-status-pending.svg';
import ServedIcon from '@/public/images/svg/table-status-served.svg';

import ItemsList, {
    type ItemsListProps,
} from '@/components/shared//lists/Items/Items';
import Button, { type ButtonProps } from '@/components/shared/button/Button';
import LoadingSpinner from '@/components/shared/states/LoadingSpinner';
import type { Currency } from '@/helpers/type/types';
import circlePlusSvg from '@/public/images/svg/circle-plus.svg';

interface ButtonHeader {
    price: number;
    currency: Currency;
    button?: Omit<ButtonProps, 'children'> &
        Partial<Pick<ButtonProps, 'children'>>;
    served?: never;
}

interface RegularHeader {
    served?: boolean;
    price?: never;
    currency?: never;
    button?: never;
}

interface EmptyAside {
    title?: never;
    items?: never;
}

interface FilledAside {
    title: string;
    items?: ItemsListProps['items'];
}

export type DetailsAsideProps = {
    buttons?: Omit<ButtonProps, 'className'>[];
    onSelectItem?: (id: string) => void;
    selectedItemId?: string | null;
    children?: ReactNode;
} & (FilledAside | EmptyAside) &
    (RegularHeader | ButtonHeader) & { className?: string } & {
        isDelivery?: boolean;
        deliveryPrice?: number;
        onAddNewOrder?: () => void;
        isItemsLoading?: boolean;
    };

const DetailsAside = ({
    title,
    items,
    buttons,
    price,
    currency,
    button,
    served,
    className,
    isDelivery,
    deliveryPrice,
    onAddNewOrder,
    onSelectItem,
    selectedItemId,
    isItemsLoading,
    children,
}: DetailsAsideProps) => {
    const { language } = useLanguage();
    const { detailsAside } = languagePacks[language];

    const showPriceHeader =
        typeof price === 'number' && !!currency && typeof title === 'string';

    return (
        <aside
            className={clsx(
                'flex flex-col gap-1.5 ml-auto py-6 h-full w-56 shadow-sm-left',
                className
            )}
        >
            {title &&
                (showPriceHeader ? (
                    <div className="flex items-center justify-center gap-x-10 flex-wrap px-4 gap-1">
                        <h2 className="text-xl/8 capitalize font-bold text-center">
                            {title}
                        </h2>

                        <p className="text-primary font-bold">
                            {formatPriceStr({ currency, price })}
                        </p>

                        {button && (
                            <Button variant="tertiary" size="xs" {...button}>
                                {button.children ?? detailsAside.info}
                            </Button>
                        )}
                    </div>
                ) : (
                    <>
                        <div className="mb-[18px] px-12 relative">
                            <h2 className="text-center text-xl/8 font-semibold capitalize leading-10">
                                {title}
                            </h2>
                            <AnimatePresence mode="wait" initial={false}>
                                <motion.div
                                    key={served ? 'served' : 'pending'}
                                    initial={{ opacity: 0, scale: 0.75 }}
                                    animate={{ opacity: 1, scale: 1 }}
                                    exit={{ opacity: 0, scale: 0.75 }}
                                    transition={{ duration: 0.5 }}
                                    className="absolute top-0 right-3 w-10 h-10"
                                >
                                    <Image
                                        src={served ? ServedIcon : PendingIcon}
                                        alt={
                                            served
                                                ? 'Table served'
                                                : 'Table pending'
                                        }
                                        fill
                                        sizes="40px"
                                    />
                                </motion.div>
                            </AnimatePresence>
                        </div>
                        <div className="h-0.5 w-full bg-dark-gray"></div>
                    </>
                ))}

            {isItemsLoading && (
                <div className="py-6 flex justify-center">
                    <LoadingSpinner />
                </div>
            )}

            {!isItemsLoading && children}

            {!isItemsLoading && !children && items && (
                <ItemsList
                    items={items}
                    onSelectItem={onSelectItem}
                    selectedItemId={selectedItemId}
                />
            )}

            {!isItemsLoading && !children && (!items || items.length === 0) && (
                <div className="px-4 py-2 text-sm text-black text-center">
                    {title
                        ? (detailsAside.noOrders ?? 'No orders')
                        : (detailsAside.noTableChosen ?? 'No table chosen')}
                </div>
            )}

            {title && onAddNewOrder && (
                <div className="px-2">
                    <button
                        className={clsx(
                            'flex items-center justify-center p-2 w-full border border-dashed border-black rounded-md transition-transform',
                            isItemsLoading
                                ? 'opacity-50 cursor-not-allowed'
                                : 'hocus:scale-95 hocus:translate-y-0.5'
                        )}
                        onClick={isItemsLoading ? undefined : onAddNewOrder}
                        disabled={isItemsLoading}
                    >
                        <Image
                            src={circlePlusSvg}
                            alt={detailsAside.addNewOrder}
                        />
                    </button>
                </div>
            )}

            {isDelivery && title && (
                <div>
                    <div className="h-0.5 w-full bg-[#707070] my-2"></div>
                    <div className="flex justify-between px-4">
                        <p className="font-medium">{detailsAside.delivery}</p>
                        <p className="font-bold">{deliveryPrice}</p>
                    </div>
                </div>
            )}
            {title && buttons && (
                <menu className="mt-auto pt-[18px] flex flex-col gap-3 px-5">
                    {buttons.map(({ children, ...btn }) => (
                        <li key={children!.toString()}>
                            <Button className="w-full capitalize" {...btn}>
                                {children}
                            </Button>
                        </li>
                    ))}
                </menu>
            )}
        </aside>
    );
};
// rather than having 'otworz rachunek' just create a + icon if table is empty, just like when u can add more receipts if u have at least 1
export default DetailsAside;
