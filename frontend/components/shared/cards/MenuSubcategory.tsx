'use client';

import { usePathname, useSearchParams } from 'next/navigation';
import { useRouter } from 'next/navigation';

interface MenuSubcategoryProps {
    id: string;
    name: string;
}

const MenuSubcategory = ({ name, id }: MenuSubcategoryProps) => {
    const searchParams = useSearchParams();
    const pathname = usePathname();
    const router = useRouter();

    const existingIngredients = searchParams.getAll('ingredientsIds');
    const isSelected = existingIngredients.includes(id);

    const toggleSelectedCategory = () => {
        const params = new URLSearchParams(searchParams.toString());

        if (isSelected) {
            const updatedIngredients = existingIngredients.filter(
                (item) => item !== id
            );
            params.delete('ingredientsIds');
            updatedIngredients.forEach((ingredient) =>
                params.append('ingredientsIds', ingredient)
            );
        } else {
            params.append('ingredientsIds', id);
        }
        router.push(`${pathname}?${params.toString()}`, { scroll: false });
    };

    return (
        <button
            className={`w-28 h-9 shadow-[0px_4px_4px_0px_#00000040] rounded-lg ${
                isSelected ? 'bg-blue-500 text-white' : 'bg-gray-200'
            }`}
            onClick={toggleSelectedCategory}
        >
            {name}
        </button>
    );
};

export default MenuSubcategory;
