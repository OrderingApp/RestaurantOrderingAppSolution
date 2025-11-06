import { useSearchParams } from 'next/navigation';
import { SEARCH_PARAMS_NAMES } from '../constants/constants';
import { useQueryMenuIngredients } from '../queries/menu-items/useQueryMenuItems';

const useFilterIngredients = () => {
    const { data } = useQueryMenuIngredients();
    const searchParams = useSearchParams();
    const page = searchParams.get(SEARCH_PARAMS_NAMES.PAGE);
    const name = searchParams.get(SEARCH_PARAMS_NAMES.NAME);
    
    let filteredIngredients = data || [];
    const totalItems = filteredIngredients.length;
    const itemsPerPage = 8;

    if (name) {
        filteredIngredients = filteredIngredients.filter((item) =>
            item.name.toLowerCase().includes(name.toLowerCase())
        );
    }

    if (page) {
        const pageNumber = parseInt(page, 10);
        const startIndex = (pageNumber - 1) * itemsPerPage;
        const endIndex = startIndex + itemsPerPage;
        filteredIngredients = filteredIngredients.slice(startIndex, endIndex);
    } else {
        filteredIngredients = filteredIngredients.slice(0, itemsPerPage);
    }

    return {
        filteredIngredients,
        totalItems,
        itemsPerPage,
    };
};

export default useFilterIngredients;
