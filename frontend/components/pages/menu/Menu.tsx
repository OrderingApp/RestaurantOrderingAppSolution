'use client';

import clsx from 'clsx';

import { useState } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';

import Image from 'next/image';

import menuSvg from '@/public/images/svg/calendar.svg';
import menuSvgWhite from '@/public/images/svg/calendar-white.svg';
import searchSvg from '@/public/images/svg/search.svg';

import MenuCategory from '@/components/shared/cards/MenuCategory';
import Input from '@/components/shared/Input/Input';
import MenuItem from '@/components/shared/cards/MenuItem';
import Modal from '@/components/shared/modals/Modal';
import MenuTag from '@/components/shared/cards/MenuTag';

import { MENU_CATEGORY_NAMES } from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import { menuStyles } from '@/lib/styles/menu';

import useFilterMenu from '@/helpers/hooks/useFilterMenu';
import useLanguage from '@/helpers/hooks/useLanguage';
import MenuItemInformation from '@/components/shared/modals/MenuItemInformation';

interface MenuProps {
    variant?: 'card' | 'order';
    children?: React.ReactNode;
}

const Menu = ({ variant = 'order', children }: MenuProps) => {
    const router = useRouter();
    const searchParams = useSearchParams();
    const { language } = useLanguage();
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

    const changeInputValue = (e: React.ChangeEvent<HTMLInputElement>) => {
        const newParams = new URLSearchParams(searchParams.toString());
        newParams.set('name', e.target.value);
        router.push(`?${newParams.toString()}`);
    };

    const openModalHandler = () => setIsModalOpen(true);

    return (
        <div className="bg-[#F6F6F6] w-full rounded-3xl h-full flex flex-row ">
            <div
                className={clsx(
                    'pt-10 flex flex-col',
                    menuStyles.variants[variant].container
                )}
            >
                <Input
                    type="search"
                    placeholder={searchInputPlaceholder}
                    inputClassName="w-[70%] [&::placeholder]:text-black bg-white shadow-[0px_4px_4px_0px_#00000040] pl-8 ml-4"
                    icon={<Image src={searchSvg} alt="searchIcon" />}
                    iconClassName="left-7 top-[11px] w-4"
                    onChange={changeInputValue}
                />
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
                                (item?.type as 'category' | 'subcategory') ||
                                'category'
                            }
                        />
                    ))}
                </div>
                {displayedTags?.length! > 0 && (
                    <ul className="flex gap-3 px-5 py-2">
                        {displayedTags?.map((tag) => (
                            <MenuTag {...tag} key={tag.id} />
                        ))}
                    </ul>
                )}

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
