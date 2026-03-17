'use client';

import { usePathname, useRouter, useSearchParams } from 'next/navigation';

import Image from 'next/image';
import clsx from 'clsx';

import {
    MENU_CATEGORY_NAMES,
    SEARCH_PARAMS_NAMES,
} from '@/helpers/constants/constants';
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
        (id === MENU_CATEGORY_NAMES.ALL && !categoryId) ||
        (type === 'category' && categoryId === id) ||
        (type === 'subcategory' && subcategoryId === id);

    const toggleSelectedCategory = () => {
        const newParams = new URLSearchParams(searchParams.toString());

        if (type === 'category') {
            if (id === MENU_CATEGORY_NAMES.ALL) {
                newParams.delete(SEARCH_PARAMS_NAMES.CATEGORY);
                newParams.delete(SEARCH_PARAMS_NAMES.SUBCATEGORY);
            } else if (categoryId === id) {
                newParams.delete(SEARCH_PARAMS_NAMES.CATEGORY);
                newParams.delete(SEARCH_PARAMS_NAMES.SUBCATEGORY);
            } else {
                newParams.set(SEARCH_PARAMS_NAMES.CATEGORY, id);
                newParams.delete(SEARCH_PARAMS_NAMES.SUBCATEGORY);
            }

            // Reset pagination when filter changes
            newParams.delete(SEARCH_PARAMS_NAMES.PAGE);
            newParams.delete(SEARCH_PARAMS_NAMES.ORDER_MENU_PAGE);
        } else if (type === 'subcategory') {
            if (subcategoryId === id) {
                newParams.delete(SEARCH_PARAMS_NAMES.SUBCATEGORY);
            } else {
                newParams.set(SEARCH_PARAMS_NAMES.SUBCATEGORY, id);
            }

            // Reset pagination when filter changes
            newParams.delete(SEARCH_PARAMS_NAMES.PAGE);
            newParams.delete(SEARCH_PARAMS_NAMES.ORDER_MENU_PAGE);
        }

        const queryString = newParams.toString();
        router.push(queryString ? `${pathname}?${queryString}` : pathname);
    };

    const amountItemsName = getPluralForm(totalItems, itemsTitle, language);

    return (
        <button
            onClick={toggleSelectedCategory}
            className={clsx(
                'shadow-[0px_4px_4px_0px_#00000040] flex flex-col justify-between flex-shrink-0 rounded-lg p-3 px-2 transition-all ',
                isActive ? 'bg-primary' : 'bg-white',
                size === 'lg'
                    ? 'w-[7.5rem] h-[6.2rem]'
                    : 'w-[6.2rem] h-[5.3rem]'
            )}
        >
            <Image
                src={isActive ? iconActive : icon}
                alt={name}
                className="group-hover:brightness-0 group-hover:invert group-focus:brightness-0 group-focus:invert transition-all"
            />
            <div>
                <h3
                    style={{ lineHeight: '12px' }}
                    className={clsx(
                        'text-left',
                        isActive
                            ? 'text-white transition-colors'
                            : 'text-black group-hover:text-white',
                        size === 'lg' ? 'text-[15px]' : 'text-[13px]'
                    )}
                >
                    {name}
                </h3>
                <p
                    className={clsx(
                        'text-[10px] text-left transition-colors',
                        isActive
                            ? 'text-white'
                            : 'text-black group-hover:text-white'
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
