'use client';

import MenuItem from '@/components/shared/cards/MenuItem';
import SearchInput from '@/components/shared/Input/SearchInput';

import { useQueryMenuIngredients } from '@/helpers/queries/menu-items/useQueryMenuItems';
import clsx from 'clsx';

const IgredientsMenu = () => {
    const { data, isError, isLoading } = useQueryMenuIngredients();

    if (isLoading) return <div>Loading...</div>;
    if (isError) return <div>Error...</div>;

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

//TODO - fix this comopnent
