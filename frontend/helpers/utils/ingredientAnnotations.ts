import type { OrderItem } from '@/helpers/interfaces/orders';

export interface IngredientLike {
    ingredientName?: string;
    quantity?: number;
    name?: string;
}

/**
 * Generate ingredient annotations for an order item.
 * Shows extra ingredients with + prefix and removed ingredients with - prefix.
 * @param item - The order item with extraIngredients and removedIngredients
 * @returns Array of annotation strings, or undefined if no ingredients to show
 */
export const getIngredientAnnotations = (
    item: OrderItem
): string[] | undefined => {
    const annotations = [
        ...(item.extraIngredients?.map(
            (extra) =>
                `+ ${extra.ingredientName}${extra.quantity > 1 ? ` x${extra.quantity}` : ''}`
        ) ?? []),
        ...(item.removedIngredients?.map(
            (removed) =>
                `- ${removed.ingredientName}${removed.quantity > 1 ? ` x${removed.quantity}` : ''}`
        ) ?? []),
    ];

    return annotations.length ? annotations : undefined;
};
