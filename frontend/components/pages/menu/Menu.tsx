'use client';

import clsx from 'clsx';

import { useState } from 'react';
import { useSearchParams } from 'next/navigation';

import menuSvg from '@/public/images/svg/calendar.svg';
import menuSvgWhite from '@/public/images/svg/calendar-white.svg';

import MenuCategory from '@/components/shared/cards/MenuCategory';

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
import IgredientsMenu from './IgredientsMenu';
import SearchInput from '@/components/shared/Input/SearchInput';

interface MenuProps {
    variant?: 'card' | 'order';
    children?: React.ReactNode;
}

const Menu = ({ variant = 'order', children }: MenuProps) => {
    const { language } = useLanguage();
    const searchParams = useSearchParams();
    const menuItemId = searchParams.get(SEARCH_PARAMS_NAMES.MENU_ITEM_ID);
    const {
        displayedCategories,
        filteredMenuItems,
        totalItems,
        displayedTags,
    } = useFilterMenu();
    const [isModalOpen, setIsModalOpen] = useState(false);

    const {
        menuPage: { searchInputPlaceholder, allCategories },
    } = languagePacks[language];

    const openModalHandler = () => setIsModalOpen(true);

    return (
        <div className="bg-light-gray w-full rounded-3xl h-full flex flex-row ">
            {!menuItemId ? (
                <div
                    className={clsx(
                        'pt-10 flex flex-col',
                        menuStyles.variants[variant].container
                    )}
                >
                    <SearchInput placeholder={searchInputPlaceholder} />
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
                                size={(item?.size as 'sm') || 'lg'}
                                iconActive={menuSvgWhite}
                                type={
                                    (item?.type as
                                        | 'category'
                                        | 'subcategory') || 'category'
                                }
                            />
                        ))}
                    </div>
                    {displayedTags?.length ? (
                        <ul className="flex gap-3 px-5 py-2">
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
                                handleClick={openModalHandler}
                                variant={
                                    menuStyles.variants[variant].menuItemVariant
                                }
                                key={item.id}
                                {...item}
                            />
                        ))}
                    </ul>
                </div>
            ) : (
                <IgredientsMenu />
            )}

            {isModalOpen && (
                <Modal onClose={() => setIsModalOpen(false)}>
                    <MenuItemInformation
                        onClick={() => setIsModalOpen(false)}
                    />
                </Modal>
            )}

            {children}
        </div>
    );
};

export default Menu;
