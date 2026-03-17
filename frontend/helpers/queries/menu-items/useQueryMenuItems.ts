import { useQuery } from '@tanstack/react-query';

import { fetchWithParams } from '@/helpers/utils/utils';
import { MenuItems } from '@/helpers/utils/queryKeys';

type NamedEntity = { id: string; name: string };

export type TagType = NamedEntity;

interface SubCategory extends NamedEntity {
    totalItems: number;
}

export interface Allergen {
    id: string;
    name: string;
    euNumber: number;
}

interface Ingredient extends NamedEntity {
    tagIds: string[];
}

interface IngredientWithTags extends Ingredient {
    price: number;
    tags: TagType[];
    allergens: Allergen[];
}

export interface MenuItemType {
    id: string;
    description: string;
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

export interface IngredientCategoryType {
    id: string;
    name: string;
    ingredients: IngredientWithTags[];
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
        queryKey: [MenuItems.BY_ID, id],
        queryFn: () =>
            fetchWithParams(MenuItems.ITEMS, id).then(
                (response) => response as MenuItemType
            ),
        enabled: !!id,
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

export const useQueryMenuIngredientCategory = () =>
    useQuery({
        queryKey: [MenuItems.INGREDIENT_CATEGORIES],
        queryFn: () =>
            fetchWithParams(MenuItems.INGREDIENT_CATEGORIES, '').then(
                (response) => response as IngredientCategoryType[]
            ),
        staleTime: 14400000,
    });
