import { describe, expect, it } from 'vitest';

import { getIngredientAddAction } from '@/helpers/utils/ingredientActions';

describe('getIngredientAddAction', () => {
    it('returns restore for a removed base ingredient', () => {
        expect(
            getIngredientAddAction({
                isBaseIngredient: true,
                isCurrentlyRemoved: true,
            })
        ).toBe('restore');
    });

    it('returns add-extra for base ingredient when it is not removed', () => {
        expect(
            getIngredientAddAction({
                isBaseIngredient: true,
                isCurrentlyRemoved: false,
            })
        ).toBe('add-extra');
    });

    it('returns add-extra for non-base ingredient even if marked removed', () => {
        expect(
            getIngredientAddAction({
                isBaseIngredient: false,
                isCurrentlyRemoved: true,
            })
        ).toBe('add-extra');
    });
});
