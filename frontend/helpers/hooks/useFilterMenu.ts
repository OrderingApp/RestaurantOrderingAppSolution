import { useMemo } from 'react';
import {
    DisplayedCategoriesType,
    MenuItemType,
    TagType,
    useQueryMenuCategory,
    useQueryMenuItems,
    useQueryMenuTags,
} from '../queries/menu-items/useQueryMenuItems';
import { useSearchParams } from 'next/navigation';
import { SEARCH_PARAMS_NAMES } from '../constants/constants';

const useFilterMenu = () => {
    const { data: menuCategories } = useQueryMenuCategory();
    const { data: menuItems } = useQueryMenuItems();
    const { data: menuItemsTags } = useQueryMenuTags();
    const searchParams = useSearchParams();
    const categoryId = searchParams.get(SEARCH_PARAMS_NAMES.CATEGORY);
    const subcategoryId = searchParams.get(SEARCH_PARAMS_NAMES.SUBCATEGORY);
    const name = searchParams.get(SEARCH_PARAMS_NAMES.NAME);
    const tag = searchParams.getAll(SEARCH_PARAMS_NAMES.TAG);
    const {
        displayedCategories,
        filteredMenuItems,
        totalItems,
        displayedTags,
    } = useMemo(() => {
        if (!menuCategories || !menuItems || !menuItemsTags)
            return {
                displayedCategories: [],
                filteredMenuItems: [],
                totalItems: 0,
                displayedTags: [],
            };

        let displayedCategories: DisplayedCategoriesType[] = menuCategories;
        let filteredMenuItems: MenuItemType[] = [];
        let displayedTags: TagType[] = [];
        let totalItems = menuCategories.reduce(
            (sum, category) => sum + category.totalItems,
            0
        );

        if (categoryId) {
            const selectedCategory = menuCategories.find(
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
                    })),
                ];
            }

            if (subcategoryId) {
                filteredMenuItems = menuItems.filter(
                    (item) => item.subCategoryId === subcategoryId
                );
            }

            if (!subcategoryId) {
                filteredMenuItems = menuItems.filter(
                    (item) => item.menuCategoryId === categoryId
                );
            }

            displayedTags = menuItemsTags.filter((tag) =>
                filteredMenuItems.some((item) =>
                    item.ingredients.some((ingredient) =>
                        ingredient.tags.some((id) => id === tag.id)
                    )
                )
            );

            if (tag.length > 0) {
                filteredMenuItems = filteredMenuItems.filter((item) =>
                    item.ingredients.some((ingredient) =>
                        ingredient.tags.some((id) => tag.includes(id))
                    )
                );
            }
        }

        if (!categoryId) {
            filteredMenuItems = menuItems;
        }

        if (name) {
            filteredMenuItems = filteredMenuItems.filter((item) =>
                item.name.toLowerCase().includes(name.toLowerCase())
            );
        }

        return {
            displayedCategories,
            filteredMenuItems,
            totalItems,
            displayedTags,
        };
    }, [menuCategories, categoryId, subcategoryId, name, menuItems, tag]);

    return {
        displayedCategories,
        filteredMenuItems,
        totalItems,
        displayedTags,
    };
};

export default useFilterMenu;
