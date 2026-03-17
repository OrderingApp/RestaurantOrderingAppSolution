'use client';

import MenuItem from '@/components/shared/cards/MenuItem';
import MenuCategory from '@/components/shared/cards/MenuCategory';
import SearchInput from '@/components/shared/Input/SearchInput';
import { PaginationWithLinks } from '@/components/ui/pagination-with-links';
import {
    MENU_CATEGORY_NAMES,
    SEARCH_PARAMS_NAMES,
} from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import useFilterIngredients from '@/helpers/hooks/useFilterIngredients';
import useLanguage from '@/helpers/hooks/useLanguage';
import { useQueryMenuIngredientCategory } from '@/helpers/queries/menu-items/useQueryMenuItems';
import { menuStyles } from '@/lib/styles/menu';

import menuSvg from '@/public/images/svg/calendar.svg';
import menuSvgWhite from '@/public/images/svg/calendar-white.svg';

import clsx from 'clsx';
import { useSearchParams } from 'next/navigation';
import type { AddItemHandler } from '@/components/shared/cards/MenuItem';

const IgredientsMenu = ({
    onAddIngredient,
}: {
    onAddIngredient?: AddItemHandler;
}) => {
    const searchParams = useSearchParams();
    const page = searchParams.get(SEARCH_PARAMS_NAMES.PAGE);
    const { filteredIngredients, totalItems, itemsPerPage } =
        useFilterIngredients();
    const { data: ingredientCategories } = useQueryMenuIngredientCategory();
    const { language } = useLanguage();

    const {
        menuPage: { allCategories },
    } = languagePacks[language];

    const allIngredientsCount = (() => {
        if (!ingredientCategories?.length) return 0;
        const unique = new Map(
            ingredientCategories.flatMap((cat) =>
                cat.ingredients.map((it) => [it.id, it] as const)
            )
        );
        return unique.size;
    })();

    return (
        <div className="relative pt-10 flex flex-col w-[776px]">
            <SearchInput className="w-[90%]" />
            <div className="p-5 flex items-center gap-3 overflow-x-auto [&::-webkit-scrollbar]:hidden">
                <MenuCategory
                    id={MENU_CATEGORY_NAMES.ALL}
                    icon={menuSvg}
                    iconActive={menuSvgWhite}
                    name={allCategories}
                    totalItems={allIngredientsCount}
                />
                {ingredientCategories?.map((cat) => (
                    <MenuCategory
                        key={cat.id}
                        id={cat.id}
                        icon={menuSvg}
                        iconActive={menuSvgWhite}
                        name={cat.name}
                        totalItems={cat.ingredients.length}
                    />
                ))}
            </div>
            <ul
                className={clsx(
                    'grid grid-cols-3 py-5 gap-6 overflow-y-auto max-h-[500px] [&&::-webkit-scrollbar]:hidden mr-4',
                    menuStyles.variants.card.list
                )}
            >
                {filteredIngredients?.map((item) => (
                    <MenuItem
                        key={item.id}
                        id={item.id}
                        name={item.name}
                        price={item.price}
                        onAddItem={onAddIngredient}
                        onOpenMenuItemInformation={() => {}}
                    />
                ))}
            </ul>
            <div className="absolute bottom-3 inset-x-0 flex justify-center">
                <PaginationWithLinks
                    page={page ? parseInt(page, 10) : 1}
                    pageSize={itemsPerPage || 9}
                    totalCount={totalItems || 0}
                    navigationMode="router"
                />
            </div>
        </div>
    );
};

export default IgredientsMenu;
