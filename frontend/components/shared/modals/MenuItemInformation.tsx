import Image from 'next/image';
import languagePacks from '@/helpers/constants/languagePacks';
import { useLanguage } from '@/providers/LanguageProvider';
import clsx from 'clsx';
import closeIcon from '@/public/images/svg/close.svg';
import {
    MenuItemType,
    useQueryMenuItem,
} from '@/helpers/queries/menu-items/useQueryMenuItems';
import LoadingSpinner from '../states/LoadingSpinner';

// TODO: AFTER BE APPLIES ALLERGENS TO INGREDIENTS, REMOVE THE MOCK FN

// uncomment data ing spreads and more allergens if u wanna test scrollbar and modal height/width behaviour
const applyMockAllergensToIngredientsData = (data: MenuItemType) =>
    [
        ...data.ingredients,
        // ...data.ingredients,
        // ...data.ingredients,
        // ...data.ingredients,
        // ...data.ingredients,
    ].map(({ name, id }) => ({
        id,
        name,
        allergens: [
            'laktoza',
            'gluten',
            'gorczyca',
            'jajka',
            'sezam',
            // 'soja',
            // 'ser',
            // 'laktoza',
            // 'gluten',
            // 'gorczyca',
            // 'jajka',
            // 'sezam',
            // 'soja',
            // 'ser',
            // 'laktoza',
            // 'gluten',
            // 'gorczyca',
            // 'jajka',
            // 'sezam',
            // 'soja',
            // 'ser',
        ],
    }));

interface MenuItemInformationProps {
    id: string;
    onClose: () => void;
}

const MenuItemInformation = ({ id, onClose }: MenuItemInformationProps) => {
    const { data, isLoading, isError, error } = useQueryMenuItem(id);
    const { language } = useLanguage();
    const {
        entities: {
            menuItem: { ingredient, allergens },
        },
        generic: { close, errorMsg },
    } = languagePacks[language];

    return (
        <section className="relative min-w-[445px] max-w-[992px] max-h-[736px] bg-white mt-[0.4rem] rounded-2xl shadow-xl p-4 pr-10 flex flex-col z-50">
            <button
                onClick={onClose}
                className="absolute top-3 right-2 z-10 group"
            >
                <Image
                    src={closeIcon}
                    alt={close}
                    className="transition-transform group-focus:scale-90 group-hover:scale-90"
                />
            </button>

            {isLoading && (
                <div className="flex items-center justify-center h-full">
                    <LoadingSpinner />
                </div>
            )}

            {isError && (
                <p className="text-danger font-semibold">
                    {error?.message ?? errorMsg}
                </p>
            )}

            {data && (
                <div className="overflow-auto scrollbar">
                    <table className="table-auto">
                        <thead className="sticky top-0 bg-white">
                            <tr className="text-center font-semibold">
                                <th className="p-1">{ingredient}</th>
                                <th className="p-1">{allergens}</th>
                            </tr>
                        </thead>

                        <tbody className="shadow-inner-lg text-sm whitespace-nowrap">
                            {applyMockAllergensToIngredientsData(data).map(
                                (ing, id) => (
                                    <tr
                                        key={id}
                                        className={clsx(
                                            id % 2 === 0
                                                ? 'bg-gray'
                                                : 'bg-lighter-gray'
                                        )}
                                    >
                                        <td className="p-3 font-semibold border-r border-black">
                                            {ing.name}
                                        </td>
                                        <td className="p-3 w-full">
                                            {ing.allergens.join(', ')}
                                        </td>
                                    </tr>
                                )
                            )}
                        </tbody>
                    </table>
                </div>
            )}
        </section>
    );
};

export default MenuItemInformation;
