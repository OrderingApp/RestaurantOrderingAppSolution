import { Order, OrderItem } from '@/helpers/interfaces/orders';
import { ORDER_TYPES, PAYMENT_STATUSES } from '@/helpers/constants/constants';

export type AggregatedOrderItem = OrderItem & {
    quantity: number;
};

export type AggregatedOrder = Omit<Order, 'orderItems'> & {
    orderItems: AggregatedOrderItem[];
};

export type IngredientLike = {
    id?: string;
    ingredientId?: string;
    name?: string;
    ingredientName?: string;
    quantity?: number;
    price?: number;
};

export const sortOrdersByCreatedAt = (orders: Order[]): Order[] =>
    orders.toSorted((a, b) => {
        const createdAtDiff =
            new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime();

        if (createdAtDiff !== 0) return createdAtDiff;

        return a.id.localeCompare(b.id);
    });

export const getDineInOrders = (orders: Order[]): Order[] =>
    orders.filter(
        (order) =>
            order.orderType.toLowerCase() === ORDER_TYPES.DINEIN.toLowerCase()
    );

export const getAggregatedDineInOrdersForTable = (
    orders: Order[],
    tableId?: string | null
): AggregatedOrder[] => {
    if (!tableId) return [];

    return aggregateOrderItems(
        sortOrdersByCreatedAt(
            getDineInOrders(orders).filter((order) => order.tableId === tableId)
        )
    );
};

export const serializeIngredients = (ingredients?: IngredientLike[]): string =>
    JSON.stringify(
        (ingredients || [])
            .map((ingredient) => ({
                id: ingredient.ingredientId || ingredient.id || '',
                name: ingredient.ingredientName || ingredient.name || '',
                quantity: ingredient.quantity ?? 0,
                price: ingredient.price ?? 0,
            }))
            .sort((a, b) => {
                if (a.id !== b.id) return a.id.localeCompare(b.id);
                if (a.name !== b.name) return a.name.localeCompare(b.name);
                if (a.quantity !== b.quantity) return a.quantity - b.quantity;
                return a.price - b.price;
            })
    );

export const createOrderItemAggregationKey = (item: OrderItem): string => {
    const menuItemId = item.menuItem.id;
    const extraKey = serializeIngredients(item.extraIngredients);
    const removedKey = serializeIngredients(
        item.removedIngredients as IngredientLike[] | undefined
    );
    const specialInstructions = item.specialInstructions ?? '';

    return `${menuItemId}|${extraKey}|${removedKey}|${specialInstructions}`;
};

const compareAggregatedOrderItems = (
    a: AggregatedOrderItem,
    b: AggregatedOrderItem
): number => {
    // Show most frequent items first.
    if (a.quantity !== b.quantity) return b.quantity - a.quantity;

    const extrasDiff = serializeIngredients(a.extraIngredients).localeCompare(
        serializeIngredients(b.extraIngredients)
    );
    if (extrasDiff !== 0) return extrasDiff;

    const menuNameDiff = a.menuItem.name.localeCompare(b.menuItem.name);
    if (menuNameDiff !== 0) return menuNameDiff;

    return createOrderItemAggregationKey(a).localeCompare(
        createOrderItemAggregationKey(b)
    );
};

export const aggregateAndSortOrderItems = (
    orderItems: OrderItem[]
): AggregatedOrderItem[] => {
    const aggregated = new Map<string, AggregatedOrderItem>();

    orderItems.forEach((item) => {
        const key = createOrderItemAggregationKey(item);

        if (aggregated.has(key)) {
            const existing = aggregated.get(key)!;
            existing.quantity += 1;
        } else {
            aggregated.set(key, { ...item, quantity: 1 });
        }
    });

    return Array.from(aggregated.values()).toSorted(
        compareAggregatedOrderItems
    );
};

export const aggregateOrderItems = (orders: Order[]): AggregatedOrder[] =>
    orders.map((order) => {
        return {
            ...order,
            orderItems: aggregateAndSortOrderItems(order.orderItems),
        };
    });

export const aggregatePaymentStatus = (orders: Order[]) => {
    const statuses = orders.map((o) => o.paymentStatus);
    if (statuses.some((s) => s === PAYMENT_STATUSES.UNPAID || !s))
        return PAYMENT_STATUSES.UNPAID;
    if (statuses.some((s) => s === PAYMENT_STATUSES.PARTIALPAID))
        return PAYMENT_STATUSES.PARTIALPAID;
    return PAYMENT_STATUSES.PAID;
};
