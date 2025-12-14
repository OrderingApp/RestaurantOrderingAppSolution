import { useQuery } from '@tanstack/react-query';

import { fetchWithParams } from '@/helpers/utils/utils';
import { MenuItems } from '@/helpers/utils/queryKeys';

type NamedEntity = { id: string; name: string };

export type TagType = NamedEntity;

interface SubCategory extends NamedEntity {
    totalItems: number;
}

interface Ingredient extends NamedEntity {
    id: string;
    tags: string[];
}

export interface IngredientWithTags extends Ingredient {
    name: string;
    price: number;
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
            fetchWithParams(MenuItems.All, '').then(
                (response) => response as MenuCategoryType[]
            ),
        staleTime: 14400000,
    });

export const useQueryMenuItems = () =>
    useQuery({
        queryKey: [MenuItems.ITEMS],
        queryFn: () =>
            fetchWithParams(MenuItems.ITEMS, '').then(
                (response) => response as MenuItemType[]
            ),
        staleTime: 14400000,
    });

export const useQueryMenuItem = (id: string) =>
    useQuery({
        queryKey: [MenuItems.BY_ID],
        queryFn: () =>
            fetchWithParams(MenuItems.ITEMS, id).then(
                (response) => response as MenuItemType
            ),
        staleTime: 14400000,
    });

export const useQueryMenuTags = () =>
    useQuery({
        queryKey: [MenuItems.TAGS],
        queryFn: () =>
            fetchWithParams(MenuItems.TAGS, '').then(
                (response) => response as TagType[]
            ),
        staleTime: 14400000,
    });

export const useQueryMenuIngredients = () =>
    useQuery({
        queryKey: [MenuItems.INGREDIENTS],
        queryFn: () =>
            fetchWithParams(MenuItems.INGREDIENTS, '').then(
                (response) => response as IngredientWithTags[]
            ),
        staleTime: 14400000,
    });
