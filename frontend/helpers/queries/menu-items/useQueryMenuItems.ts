import { useQuery } from '@tanstack/react-query';

import { fetchWithToken } from '@/helpers/utils/utils';
import { MenuItems } from '@/helpers/utils/queryKeys';

type NamedEntity = { id: string; name: string };

type Ingredient = NamedEntity;
type SubCategory = NamedEntity;

export interface MenuItemType {
    menuItems: any;
    id: string;
    desription: string;
    ingredients: Ingredient[];
    name: string;
    price: number;
    subCategoryId: string | null;
}

export interface MenuCategoryType {
    id: string;
    menuItems: MenuItemType[];
    name: string;
    subCategories: SubCategory[];
}

export interface DisplayedCategoriesType extends SubCategory {
    size?: string;
    type?: string;
    menuItems?: MenuItemType[];
}

const useQueryMenuItems = () =>
    useQuery({
        queryKey: [MenuItems.All],
        queryFn: () =>
            fetchWithToken('menu-categories', 'hierarchy').then(
                (response) => response as MenuCategoryType[]
            ),
        staleTime: 14400000,
    });

export default useQueryMenuItems;
