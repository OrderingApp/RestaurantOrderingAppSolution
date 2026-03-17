import { useMemo } from 'react';
import { useSearchParams } from 'next/navigation';
import { SEARCH_PARAMS_NAMES } from '../constants/constants';
import { useQueryMenuIngredientCategory } from '../queries/menu-items/useQueryMenuItems';

const useFilterIngredients = () => {
    const { data } = useQueryMenuIngredientCategory();
    const searchParams = useSearchParams();
    const categoryId = searchParams.get(SEARCH_PARAMS_NAMES.CATEGORY);
    const page = searchParams.get(SEARCH_PARAMS_NAMES.PAGE);
    const name = searchParams.get(SEARCH_PARAMS_NAMES.NAME);

    const itemsPerPage = 9;

    const { filteredIngredients, totalItems } = useMemo(() => {
        if (!data?.length) {
            return { filteredIngredients: [], totalItems: 0 };
        }

        const normalizedCategoryId = categoryId?.toLowerCase();
        const normalizedName = name?.toLowerCase().trim();

        const ingredientSource = normalizedCategoryId
            ? (data.find((cat) => cat.id.toLowerCase() === normalizedCategoryId)
                  ?.ingredients ?? [])
            : data.flatMap((cat) => cat.ingredients);

        // Avoid duplicates if the same ingredient appears in multiple categories
        const uniqueById = new Map(ingredientSource.map((it) => [it.id, it]));
        let result = Array.from(uniqueById.values());

        if (normalizedName) {
            result = result.filter((item) =>
                item.name.toLowerCase().includes(normalizedName)
            );
        }

        const totalItems = result.length;

        const pageNumber = page ? parseInt(page, 10) : 1;
        const safePageNumber =
            Number.isFinite(pageNumber) && pageNumber > 0 ? pageNumber : 1;
        const startIndex = (safePageNumber - 1) * itemsPerPage;
        const endIndex = startIndex + itemsPerPage;
        result = result.slice(startIndex, endIndex);

        return { filteredIngredients: result, totalItems };
    }, [data, categoryId, name, page]);

    return { filteredIngredients, totalItems, itemsPerPage };
};

export default useFilterIngredients;
