import { useQuery } from '@tanstack/react-query';

import { fetchWithToken } from '@/helpers/utils/utils';
import { MenuItems } from '@/helpers/utils/queryKeys';

type NamedEntity = { id: string; name: string };

export type TagType = NamedEntity;

interface SubCategory extends NamedEntity {
    totalItems: number;
}

interface Ingredient extends NamedEntity {
    id:string
    tagIds: string[];
}

export interface MenuItemType {
    id: string;
    desription: string;
    ingredients: Ingredient[];
    name: string;
    price: number;
    sequenceNumber: number;
    menuCategoryId: string;
    subCategoryId: string | null;
}

export interface MenuCategoryType {
    size: 'sm';
    type: 'subcategory' | 'category';
    id: string;
    name: string;
    totalItems: number;
    subCategories: SubCategory[];
}

export interface DisplayedCategoriesType extends SubCategory {
    size?: string;
    type?: string;
    menuItems?: MenuItemType[];
}

export const useQueryMenuCategory = () =>
    useQuery({
        queryKey: [MenuItems.All],
        queryFn: () =>
            fetchWithToken('menu-categories', '').then(
                (response) => response as MenuCategoryType[]
            ),
        staleTime: 14400000,
    });

export const useQueryMenuItems = () =>
    useQuery({
        queryKey: [MenuItems.ITEMS],
        queryFn: () =>
            fetchWithToken('menu-items', '').then(
                (response) => response as MenuItemType[]
            ),
        staleTime: 14400000,
    });

export const useQueryMenuTags = () =>
    useQuery({
        queryKey: [MenuItems.TAGS],
        queryFn: () =>
            fetchWithToken('tags', '').then(
                (response) => response as TagType[]
            ),
        staleTime: 14400000,
    });
