import { describe, expect, it } from 'vitest';

import { Order, OrderItem } from '@/helpers/interfaces/orders';
import {
    aggregateOrderItems,
    createOrderItemAggregationKey,
} from '@/helpers/utils/orderTransforms';

const extra = (
    ingredientId: string,
    ingredientName: string,
    quantity = 1,
    price = 0
) => ({
    ingredientId,
    ingredientName,
    quantity,
    price,
});

const removed = (id: string, name: string) => ({ id, name });

const createOrderItem = (
    overrides: Partial<OrderItem> = {},
    menuItemId = 'menu-item-1'
): OrderItem => ({
    id: crypto.randomUUID(),
    price: 25,
    discount: 0,
    specialInstructions: null,
    extraIngredients: [],
    removedIngredients: [],
    menuItem: {
        id: menuItemId,
        name: 'Margherita',
        description: null,
        price: 25,
        baseIngredients: [],
    },
    ...overrides,
});

const createOrder = (orderItems: OrderItem[]): Order => ({
    id: 'order-1',
    createdAt: '2026-03-04T07:20:59.416Z',
    totalAmount: 50,
    discount: 0,
    orderStatus: 'Ongoing',
    orderType: 'dinein',
    tableId: 'table-1',
    customerInformation: {
        phoneNumber: '123456789',
        additionalInstructions: null,
        orderCompletionType: 'Immediate',
        preferredPaymentMethod: 'Cash',
        expectedOrderCompletion: '2026-03-04T07:20:59.416Z',
        address: null,
    },
    orderItems,
});

describe('aggregateOrderItems', () => {
    it('aggregates items when menu item and all customizations match', () => {
        const first = createOrderItem({
            specialInstructions: 'well done',
            extraIngredients: [
                {
                    ingredientId: 'ing-2',
                    ingredientName: 'Olives',
                    quantity: 1,
                    price: 2,
                },
                {
                    ingredientId: 'ing-1',
                    ingredientName: 'Cheese',
                    quantity: 1,
                    price: 3,
                },
            ],
            removedIngredients: [{ id: 'base-1', name: 'Onion' }],
        });

        const second = createOrderItem({
            specialInstructions: 'well done',
            extraIngredients: [
                {
                    ingredientId: 'ing-1',
                    ingredientName: 'Cheese',
                    quantity: 1,
                    price: 3,
                },
                {
                    ingredientId: 'ing-2',
                    ingredientName: 'Olives',
                    quantity: 1,
                    price: 2,
                },
            ],
            removedIngredients: [{ id: 'base-1', name: 'Onion' }],
        });

        const [order] = aggregateOrderItems([createOrder([first, second])]);

        expect(order.orderItems).toHaveLength(1);
        expect(order.orderItems[0].quantity).toBe(2);
    });

    it('does not aggregate when specialInstructions differ', () => {
        const first = createOrderItem({ specialInstructions: 'no sauce' });
        const second = createOrderItem({ specialInstructions: 'extra sauce' });

        const [order] = aggregateOrderItems([createOrder([first, second])]);

        expect(order.orderItems).toHaveLength(2);
        expect(order.orderItems.map((item) => item.quantity)).toEqual([1, 1]);
    });

    it('does not aggregate when removedIngredients differ', () => {
        const first = createOrderItem({
            removedIngredients: [removed('base-1', 'Onion')],
        });
        const second = createOrderItem({
            removedIngredients: [removed('base-2', 'Garlic')],
        });

        const [order] = aggregateOrderItems([createOrder([first, second])]);

        expect(order.orderItems).toHaveLength(2);
        expect(order.orderItems.every((item) => item.quantity === 1)).toBe(
            true
        );
    });

    it('does not aggregate when menuItem.id differs', () => {
        const first = createOrderItem({}, 'menu-item-1');
        const second = createOrderItem({}, 'menu-item-2');

        const [order] = aggregateOrderItems([createOrder([first, second])]);

        expect(order.orderItems).toHaveLength(2);
        expect(order.orderItems.every((item) => item.quantity === 1)).toBe(
            true
        );
    });

    it('aggregates independently per order', () => {
        const firstOrder = createOrder([createOrderItem(), createOrderItem()]);
        const secondOrder = {
            ...createOrder([createOrderItem(), createOrderItem()]),
            id: 'order-2',
        };

        const aggregated = aggregateOrderItems([firstOrder, secondOrder]);

        expect(aggregated).toHaveLength(2);
        expect(aggregated[0].orderItems).toHaveLength(1);
        expect(aggregated[0].orderItems[0].quantity).toBe(2);
        expect(aggregated[1].orderItems).toHaveLength(1);
        expect(aggregated[1].orderItems[0].quantity).toBe(2);
    });

    it('treats undefined and empty ingredient arrays as equal', () => {
        const first = createOrderItem({ extraIngredients: undefined });
        const second = createOrderItem({ extraIngredients: [] });

        const [order] = aggregateOrderItems([createOrder([first, second])]);

        expect(order.orderItems).toHaveLength(1);
        expect(order.orderItems[0].quantity).toBe(2);
    });

    it('treats null and undefined specialInstructions as equal', () => {
        const first = createOrderItem({ specialInstructions: null });
        const second = createOrderItem({ specialInstructions: undefined });

        const [order] = aggregateOrderItems([createOrder([first, second])]);

        expect(order.orderItems).toHaveLength(1);
        expect(order.orderItems[0].quantity).toBe(2);
    });
});

describe('createOrderItemAggregationKey', () => {
    it('creates the same key for same ingredients in different order', () => {
        const first = createOrderItem({
            specialInstructions: 'well done',
            extraIngredients: [
                extra('ing-2', 'Olives', 1, 2),
                extra('ing-1', 'Cheese', 1, 3),
            ],
            removedIngredients: [removed('base-2', 'Garlic')],
        });

        const second = createOrderItem({
            specialInstructions: 'well done',
            extraIngredients: [
                extra('ing-1', 'Cheese', 1, 3),
                extra('ing-2', 'Olives', 1, 2),
            ],
            removedIngredients: [removed('base-2', 'Garlic')],
        });

        expect(createOrderItemAggregationKey(first)).toBe(
            createOrderItemAggregationKey(second)
        );
    });

    it('creates different keys when specialInstructions differ', () => {
        const first = createOrderItem({ specialInstructions: 'no onion' });
        const second = createOrderItem({ specialInstructions: 'extra onion' });

        expect(createOrderItemAggregationKey(first)).not.toBe(
            createOrderItemAggregationKey(second)
        );
    });
});
