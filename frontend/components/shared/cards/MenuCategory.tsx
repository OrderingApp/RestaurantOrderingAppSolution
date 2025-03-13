'use client';

import { useRouter, useSearchParams } from 'next/navigation';
import Image from 'next/image';
import {
    MenuCategoryType,
    MenuItemType,
} from '@/helpers/queries/menu-items/useQueryMenuItems';
import clsx from 'clsx';

interface MenuCategoryProps {
    id: string;
    icon: string;
    iconActive: string;
    size?: 'lg' | 'sm';
    name: string;
    items: MenuItemType[] | MenuCategoryType[];
    type?: 'category' | 'subcategory';
}

const MenuCategory = ({
    id,
    icon,
    iconActive,
    name,
    size = 'lg',
    items,
    type = 'category',
}: MenuCategoryProps) => {
    const searchParams = useSearchParams();
    const router = useRouter();
    const categoryId = searchParams.get('categoryId');
    const subcategoryId = searchParams.get('subcategoryId');

    const isMenuItem = (
        item: MenuItemType | MenuCategoryType
    ): item is MenuItemType => {
        return (item as MenuItemType).subCategoryId !== undefined;
    };

    const getAmount = () => {
        if (type === 'subcategory') {
            return (
                (items as Array<MenuItemType | MenuCategoryType>)
                    .filter(isMenuItem)
                    .filter((item) => item.subCategoryId === id).length || 0
            );
        }
        if (id === 'all') {
            return (
                (items as MenuCategoryType[])?.flatMap(
                    (category) => category.menuItems
                )?.length || 0
            );
        }
        return items?.length || 0;
    };

    const amount = getAmount();

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
            newParams.delete('subcategoryId');
        } else if (type === 'subcategory') {
            if (subcategoryId === id) {
                newParams.delete('subcategoryId');
            } else {
                newParams.set('subcategoryId', id);
            }
        }
        router.push(id === 'all' ? '/order' : `/order?${newParams.toString()}`);
    };

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
                    {amount} pozycji
                </p>
            </div>
        </button>
    );
};

export default MenuCategory;
