'use client';

import MenuItem from '@/components/shared/cards/MenuItem';
import SearchInput from '@/components/shared/Input/SearchInput';
import { PaginationWithLinks } from '@/components/ui/pagination-with-links';
import { SEARCH_PARAMS_NAMES } from '@/helpers/constants/constants';
import useFilterIngredients from '@/helpers/hooks/useFilterIngredients';

import clsx from 'clsx';
import { useSearchParams } from 'next/navigation';

const IgredientsMenu = () => {
    const searchParams = useSearchParams();
    const page = searchParams.get(SEARCH_PARAMS_NAMES.PAGE);
    const { filteredIngredients, totalItems, itemsPerPage } =
        useFilterIngredients();

    return (
        <div className="py-10 pt-10 flex flex-col">
            <SearchInput className="w-[70%]" placeholder="Wyszukaj" />
            <ul
                className={clsx(
                    'grid py-5 flex-wrap gap-y-8 pl-5 w-[776px] mt-8 '
                )}
                style={{
                    gridTemplateColumns: `repeat(4, minmax(0, 1fr))`,
                }}
            >
                {filteredIngredients?.map((item) => (
                    <MenuItem
                        variant="order"
                        handleClick={() => {}}
                        key={item.id}
                        {...item}
                    />
                ))}
            </ul>
            <div
                className={`absolute bottom-10 left-[calc(50%-112px)] -translate-x-1/2`}
            >
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
