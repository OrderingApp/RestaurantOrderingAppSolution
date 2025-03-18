'use client';

import { useMemo } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import Image from 'next/image';

import menuSvg from '@/public/images/svg/calendar.svg';
import menuSvgWhite from '@/public/images/svg/calendar-white.svg';
import searchSvg from '@/public/images/svg/search.svg';

import MenuCategory from '@/components/shared/cards/MenuCategory';
import Input from '@/components/shared/Input/Input';
import MenuItem from '@/components/shared/cards/MenuItem';
import useQueryMenuItems, {
    MenuItemType,
    DisplayedCategoriesType,
} from '@/helpers/queries/menu-items/useQueryMenuItems';
import {
    MENU_CATEGORY_NAMES,
    SEARCH_PARAMS_NAMES,
} from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import useLanguage from '@/helpers/hooks/useLanguage';

const MenuPage = () => {
    const { data } = useQueryMenuItems();
    const searchParams = useSearchParams();
    const router = useRouter();
    const { language } = useLanguage();
    const categoryId = searchParams.get(SEARCH_PARAMS_NAMES.CATEGORY);
    const subcategoryId = searchParams.get(SEARCH_PARAMS_NAMES.SUBCATEGORY);
    const name = searchParams.get(SEARCH_PARAMS_NAMES.NAME);

    const {
        menuPage: { searchInputPlaceholder },
    } = languagePacks[language];

    const { displayedCategories, filteredMenuItems } = useMemo(() => {
        if (!data) return { displayedCategories: [], filteredMenuItems: [] };
        const newOrder = [7, 0, 1, 4, 2, 6, 8, 3, 5];
        const reorderedData = newOrder.map((i) => data[i]);
        let displayedCategories: DisplayedCategoriesType[] = reorderedData;
        let filteredMenuItems: MenuItemType[] = [];

        if (categoryId) {
            const selectedCategory = data.find(
                (item) => item.id.toLowerCase() === categoryId.toLowerCase()
            );

            if (!selectedCategory)
                return { displayedCategories, filteredMenuItems };

            if (selectedCategory.subCategories.length > 0) {
                displayedCategories = [
                    selectedCategory,
                    ...selectedCategory.subCategories.map((sub) => ({
                        ...sub,
                        size: 'sm',
                        type: 'subcategory',
                        menuItems: selectedCategory.menuItems,
                    })),
                ];
            }

            if (subcategoryId) {
                const selectedSubcategory = selectedCategory.subCategories.find(
                    (sub) =>
                        sub.id.toLowerCase() === subcategoryId.toLowerCase()
                );

                filteredMenuItems = selectedSubcategory
                    ? selectedCategory.menuItems.filter(
                          (item) => item.subCategoryId === subcategoryId
                      )
                    : [];
            }

            if (!subcategoryId) filteredMenuItems = selectedCategory.menuItems;
        }

        if (!categoryId) {
            const sortByNumber = (a: MenuItemType, b: MenuItemType) => {
                const extractNumber = (str: string) =>
                    parseInt(str.match(/\d+/g)?.pop() || '0', 10);
                return extractNumber(a.name) - extractNumber(b.name);
            };

            filteredMenuItems = reorderedData.flatMap((category) =>
                ['pizza', 'calzone', 'piadina', 'panuozzo'].includes(
                    category.name.toLowerCase()
                )
                    ? category.menuItems.sort(sortByNumber)
                    : category.menuItems
            );
        }

        if (name) {
            filteredMenuItems = filteredMenuItems.filter((item) =>
                item.name.toLowerCase().includes(name.toLowerCase())
            );
        }

        return { displayedCategories, filteredMenuItems };
    }, [data, categoryId, subcategoryId, name]);

    const changeInputValue = (e: React.ChangeEvent<HTMLInputElement>) => {
        const newParams = new URLSearchParams(searchParams.toString());
        newParams.set('name', e.target.value);
        router.push(`?${newParams.toString()}`);
    };

    return (
        <div className="max-h-[48rem] flex flex-col py-2 pl-4 pr-2 rounded-3xl overflow-hidden">
            <div className="min-h-[728px] bg-[#F6F6F6] w-full rounded-3xl p-10 flex flex-col">
                <Input
                    type="search"
                    placeholder={searchInputPlaceholder}
                    inputClassName="w-[70%] [&::placeholder]:text-black bg-white shadow-[0px_4px_4px_0px_#00000040] pl-8"
                    icon={<Image src={searchSvg} alt="searchIcon" />}
                    iconClassName="left-3 top-[11px] w-4"
                    onChange={changeInputValue}
                />
                <div className="p-5 flex items-center gap-3 overflow-x-auto [&::-webkit-scrollbar]:hidden">
                    <MenuCategory
                        id={MENU_CATEGORY_NAMES.ALL}
                        icon={menuSvg}
                        iconActive={menuSvgWhite}
                        name="Wszystko"
                        items={data || []}
                    />
                    {displayedCategories?.map((item) => (
                        <MenuCategory
                            {...item}
                            key={item.id}
                            items={item.menuItems!}
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

                <ul className="flex gap-4 py-5 flex-wrap overflow-y-auto max-h-[500px] pl-5 [&&::-webkit-scrollbar]:hidden">
                    {filteredMenuItems.map((item) => (
                        <MenuItem key={item.id} {...item} />
                    ))}
                </ul>
            </div>
        </div>
    );
};

export default MenuPage;
