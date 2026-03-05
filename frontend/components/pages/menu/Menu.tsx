'use client';

import clsx from 'clsx';

import { useState } from 'react';
import { useSearchParams } from 'next/navigation';

import menuSvg from '@/public/images/svg/calendar.svg';
import menuSvgWhite from '@/public/images/svg/calendar-white.svg';

import MenuCategory, {
    MenuCategorySize,
    MenuCategoryType,
} from '@/components/shared/cards/MenuCategory';

import MenuItem from '@/components/shared/cards/MenuItem';
import Modal from '@/components/shared/modals/Modal';
import MenuTag from '@/components/shared/cards/MenuTag';

import {
    MENU_CATEGORY_NAMES,
    SEARCH_PARAMS_NAMES,
} from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import { menuStyles } from '@/lib/styles/menu';

import useFilterMenu from '@/helpers/hooks/useFilterMenu';
import useLanguage from '@/helpers/hooks/useLanguage';

import MenuItemInformation from '@/components/shared/modals/MenuItemInformation';
import IngredientsMenu from './IngredientsMenu';
import SearchInput from '@/components/shared/Input/SearchInput';

import { PaginationWithLinks } from '@/components/ui/pagination-with-links';
import type { AddItemHandler } from '@/components/shared/cards/MenuItem';

interface MenuProps {
    variant?: 'card' | 'order';
    children?: React.ReactNode;
    onAddItem?: AddItemHandler;
}

const Menu = ({ variant = 'order', children, onAddItem }: MenuProps) => {
    const searchParams = useSearchParams();
    const menuItemId = searchParams.get(SEARCH_PARAMS_NAMES.MENU_ITEM_ID);
    const pageSearchParam =
        variant === 'order'
            ? SEARCH_PARAMS_NAMES.ORDER_MENU_PAGE
            : SEARCH_PARAMS_NAMES.PAGE;
    const page = searchParams.get(pageSearchParam);
    const {
        displayedCategories,
        filteredMenuItems,
        totalItems,
        displayedTags,
        itemsPerPage,
    } = useFilterMenu(variant);
    const [menuItemInformationId, setMenuItemInformationId] = useState('');

    const { language } = useLanguage();

    const {
        menuPage: { allCategories },
    } = languagePacks[language];

    // TODO: fix totalitems on 'all' category, as it shows the count of subcategory, or even sub-subcategory when selected. it shoudl always show count of all possible items rather than that
    return (
        <div className="bg-light-gray w-full rounded-3xl h-full flex flex-row ">
            {menuItemId && <IngredientsMenu />}

            {!menuItemId && (
                <div
                    className={clsx(
                        'pt-10 flex flex-col',
                        menuStyles.variants[variant].container
                    )}
                >
                    <SearchInput className="w-[90%]" />
                    <div className="p-5 flex items-center gap-3 overflow-x-auto [&::-webkit-scrollbar]:hidden">
                        <MenuCategory
                            id={MENU_CATEGORY_NAMES.ALL}
                            icon={menuSvg}
                            iconActive={menuSvgWhite}
                            name={allCategories}
                            totalItems={totalItems!}
                        />
                        {displayedCategories?.map((item) => (
                            <MenuCategory
                                {...item}
                                key={item.id}
                                icon={menuSvg}
                                size={item?.size as MenuCategorySize}
                                iconActive={menuSvgWhite}
                                type={item?.type as MenuCategoryType}
                            />
                        ))}
                    </div>
                    {displayedTags?.length ? (
                        <ul className="flex gap-3 px-5 ">
                            {displayedTags?.map((tag) => (
                                <MenuTag {...tag} key={tag.id} />
                            ))}
                        </ul>
                    ) : null}

                    <ul
                        className={clsx(
                            'grid py-5 flex-wrap overflow-y-auto max-h-[500px] [&&::-webkit-scrollbar]:hidden',
                            menuStyles.variants[variant].list
                        )}
                        style={{
                            gridTemplateColumns: `repeat(${menuStyles.variants[variant].listCols}, minmax(0, 1fr))`,
                        }}
                    >
                        {filteredMenuItems?.map((item) => (
                            <MenuItem
                                onOpenMenuItemInformation={(id: string) =>
                                    setMenuItemInformationId(id)
                                }
                                variant={
                                    menuStyles.variants[variant].menuItemVariant
                                }
                                key={item.id}
                                onAddItem={onAddItem}
                                {...item}
                            />
                        ))}
                    </ul>
                    <div
                        className={`absolute bottom-3  ${variant === 'order' ? 'left-[calc(50%-112px)] ' : 'left-1/2'} -translate-x-1/2`}
                    >
                        <PaginationWithLinks
                            page={page ? parseInt(page, 10) : 1}
                            pageSize={itemsPerPage || 9}
                            totalCount={totalItems || 0}
                            pageSearchParam={pageSearchParam}
                            navigationMode="router"
                        />
                    </div>
                </div>
            )}

            <Modal
                isOpen={!!menuItemInformationId}
                onClose={() => setMenuItemInformationId('')}
            >
                <MenuItemInformation
                    id={menuItemInformationId}
                    onClose={() => setMenuItemInformationId('')}
                />
            </Modal>

            {children}
        </div>
    );
};

export default Menu;
