'use client';

import { usePathname, useRouter, useSearchParams } from 'next/navigation';

import Image from 'next/image';
import clsx from 'clsx';

import { SEARCH_PARAMS_NAMES } from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import { getPluralForm } from '@/helpers/utils/utils';

import useLanguage from '@/helpers/hooks/useLanguage';

export interface MenuCategoryProps {
    id: string;
    icon: string;
    iconActive: string;
    name: string;
    totalItems: number;
    type?: 'category' | 'subcategory';
    size?: 'lg' | 'sm';
}

export type MenuCategoryType = NonNullable<MenuCategoryProps['type']>;
export type MenuCategorySize = NonNullable<MenuCategoryProps['size']>;

const MenuCategory = ({
    id,
    icon,
    iconActive,
    name,
    size = 'lg',
    totalItems,
    type = 'category',
}: MenuCategoryProps) => {
    const searchParams = useSearchParams();
    const router = useRouter();
    const pathname = usePathname();
    const { language } = useLanguage();
    const categoryId = searchParams.get(SEARCH_PARAMS_NAMES.CATEGORY);
    const subcategoryId = searchParams.get(SEARCH_PARAMS_NAMES.SUBCATEGORY);

    const {
        menuPage: {
            menuCategoryCard: { itemsTitle },
        },
    } = languagePacks[language];

    const isActive =
        (id === 'all' && !categoryId) ||
        (type === 'category' && categoryId === id) ||
        (type === 'subcategory' && subcategoryId === id);

    const toggleSelectedCategory = () => {
        const newParams = new URLSearchParams(searchParams.toString());

        if (type === 'category') {
            if (categoryId === id) {
                newParams.delete('categoryId');
            } else {
                newParams.set('categoryId', id);
            }
        } else if (type === 'subcategory') {
            if (subcategoryId === id) {
                newParams.delete('subcategoryId');
            } else {
                newParams.set('subcategoryId', id);
            }
        }

        const modalParam = newParams.get('modal')
            ? `modal=${newParams.get('modal')}`
            : '';

        router.push(
            id === 'all'
                ? `${pathname}?${modalParam}`
                : `${pathname}?${newParams.toString()}`
        );
    };

    const amountItemsName = getPluralForm(totalItems, itemsTitle, language);

    return (
        <button
            onClick={toggleSelectedCategory}
            className={clsx(
                'shadow-[0px_4px_4px_0px_#00000040] flex flex-col justify-between flex-shrink-0 rounded-lg p-3 px-2',
                isActive ? 'bg-primary' : 'bg-white',
                size === 'lg'
                    ? 'w-[7.5rem] h-[6.2rem]'
                    : 'w-[6.2rem] h-[5.3rem]'
            )}
        >
            <Image src={isActive ? iconActive : icon} alt={name} />
            <div>
                <h3
                    style={{ lineHeight: '12px' }}
                    className={clsx(
                        'text-left',
                        isActive ? 'text-white' : 'text-black',
                        size === 'lg' ? 'text-[15px]' : 'text-[13px]'
                    )}
                >
                    {name}
                </h3>
                <p
                    className={clsx(
                        'text-[10px] text-left',
                        isActive ? 'text-white' : 'text-black'
                    )}
                >
                    {totalItems} {amountItemsName}
                </p>
            </div>
        </button>
    );
};

export default MenuCategory;

//TODO BUTTONS CHANGE TRY USE TRIARY OPERATOR
