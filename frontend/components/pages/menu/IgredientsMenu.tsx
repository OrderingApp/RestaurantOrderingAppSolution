'use client';

import MenuItem from '@/components/shared/cards/MenuItem';
import SearchInput from '@/components/shared/Input/SearchInput';
import { SEARCH_PARAMS_NAMES } from '@/helpers/constants/constants';
import {
    useQueryMenuIngredients,
    useQueryMenuItem,
} from '@/helpers/queries/menu-items/useQueryMenuItems';
import clsx from 'clsx';
import { useSearchParams } from 'next/navigation';

const IgredientsMenu = () => {
    const { data, isError, isLoading } = useQueryMenuIngredients();
    const searchParams = useSearchParams();
    const menuItemId = searchParams.get(SEARCH_PARAMS_NAMES.MENU_ITEM_ID);
    const { data: menuItem } = useQueryMenuItem(menuItemId || '');

    if (isLoading) return <div>Loading...</div>;
    if (isError) return <div>Error...</div>;

    console.log(menuItem);

    // const menuItemIngredients = menuItem?.ingredients || [];
    // if (menuItemIngredients.length > 0) {
    //     data?.filter((ingredient) => {
    //         if (menuItemIngredients.includes(ingredient.id)) {
    //             ingredient.defaultValue = 1;
    //         }
    //     });
    // }

    return (
        <div className="py-10 pt-10 flex flex-col">
            <SearchInput placeholder="Wyszukaj" />
            <ul
                className={clsx(
                    'grid py-5 flex-wrap overflow-y-auto max-h-[500px] [&&::-webkit-scrollbar]:hidden gap-y-4 pl-5 w-[776px]'
                )}
                style={{
                    gridTemplateColumns: `repeat(3, minmax(0, 1fr))`,
                }}
            >
                {data?.map((item) => (
                    <MenuItem
                        variant="order"
                        handleClick={() => {}}
                        key={item.id}
                        {...item}
                    />
                ))}
            </ul>
        </div>
    );
};

export default IgredientsMenu;
