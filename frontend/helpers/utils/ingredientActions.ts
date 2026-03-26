export type IngredientAddAction = 'restore' | 'add-extra';

export const getIngredientAddAction = ({
    isBaseIngredient,
    isCurrentlyRemoved,
}: {
    isBaseIngredient: boolean;
    isCurrentlyRemoved: boolean;
}): IngredientAddAction => {
    if (isBaseIngredient && isCurrentlyRemoved) {
        return 'restore';
    }

    return 'add-extra';
};