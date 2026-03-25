'use client';

import { CURRENCIES } from '@/helpers/constants/constants';

import { createContext, useState, ReactNode, useCallback, use } from 'react';

interface OrderContextType {
    id: string;
    name: string;
    price: number;
    quantity: number;
    discount: number;
    currency: keyof typeof CURRENCIES;
    deliveryPrice?: number;
}

export interface ExtraIngredientState {
    ingredientId: string;
    ingredientName: string;
    price: number;
    quantity: number;
}

interface OrdersContextType {
    orders: OrderContextType[];
    deliveryPrice?: number;
    extraIngredientsByOrderItemId: Record<string, ExtraIngredientState[]>;
    removedIngredientIdsByOrderItemId: Record<string, string[]>;
    getExtraIngredients: (orderItemId: string) => ExtraIngredientState[];
    getExtraIngredientsTotalPrice: (orderItemId: string) => number;
    getExtraIngredientsPayloadByOrderItemId: () => Record<
        string,
        { ingredientId: string; quantity: number }[]
    >;
    getRemovedIngredientIds: (orderItemId: string) => string[];
    getRemovedIngredientIdsPayloadByOrderItemId: () => Record<string, string[]>;
    addOrder: (order: OrderContextType) => void;
    addExtraIngredient: (params: {
        orderItemId: string;
        ingredientId: string;
        ingredientName: string;
        price: number;
        quantity?: number;
    }) => void;
    decrementExtraIngredient: (params: {
        orderItemId: string;
        ingredientId: string;
        quantity?: number;
    }) => void;
    removeIngredientFromOrderItem: (params: {
        orderItemId: string;
        ingredientId: string;
    }) => void;
    restoreIngredientForOrderItem: (params: {
        orderItemId: string;
        ingredientId: string;
    }) => void;
    clearExtraIngredients: (orderItemId?: string) => void;
    clearRemovedIngredients: (orderItemId?: string) => void;
    updateDeliveryPrice: (price: number) => void;
    clearOrders: () => void;
}

export const OrdersContext = createContext<OrdersContextType | undefined>(
    undefined
);

const OrdersProvider = ({ children }: { children: ReactNode }) => {
    const [orders, setOrders] = useState<OrderContextType[]>([]);

    const [deliveryPrice, setDeliveryPrice] = useState(0);

    const [extraIngredientsByOrderItemId, setExtraIngredientsByOrderItemId] =
        useState<Record<string, ExtraIngredientState[]>>({});

    const [
        removedIngredientIdsByOrderItemId,
        setRemovedIngredientIdsByOrderItemId,
    ] = useState<Record<string, string[]>>({});

    const addOrderHandler = useCallback((order: OrderContextType) => {
        setOrders((prevOrders) => {
            const existingOrderIndex = prevOrders.findIndex(
                (item) => item.id === order.id
            );

            if (existingOrderIndex !== -1) {
                return prevOrders.map((item, index) =>
                    index === existingOrderIndex
                        ? { ...item, quantity: item.quantity + order.quantity }
                        : item
                );
            }

            return [...prevOrders, order];
        });
    }, []);

    const updateDeliveryPrice = (price: number) => setDeliveryPrice(price);

    const addExtraIngredient = useCallback(
        ({
            orderItemId,
            ingredientId,
            ingredientName,
            price,
            quantity = 1,
        }: {
            orderItemId: string;
            ingredientId: string;
            ingredientName: string;
            price: number;
            quantity?: number;
        }) => {
            if (!orderItemId || !ingredientId) return;

            setExtraIngredientsByOrderItemId((prev) => {
                const existing = prev[orderItemId] ?? [];

                const index = existing.findIndex(
                    (x) => x.ingredientId === ingredientId
                );

                // Backend caps ingredient quantity at 2

                const safeQty = Math.max(1, quantity);

                if (index === -1) {
                    return {
                        ...prev,

                        [orderItemId]: [
                            ...existing,
                            {
                                ingredientId,
                                ingredientName,
                                price,
                                quantity: Math.min(2, safeQty),
                            },
                        ],
                    };
                }

                const next = existing.map((x, idx) =>
                    idx === index
                        ? {
                              ...x,
                              quantity: Math.min(2, x.quantity + safeQty),
                          }
                        : x
                );

                return { ...prev, [orderItemId]: next };
            });
        },

        []
    );

    const decrementExtraIngredient = useCallback(
        ({
            orderItemId,
            ingredientId,
            quantity = 1,
        }: {
            orderItemId: string;
            ingredientId: string;
            quantity?: number;
        }) => {
            if (!orderItemId || !ingredientId) return;

            setExtraIngredientsByOrderItemId((prev) => {
                const existing = prev[orderItemId] ?? [];

                const index = existing.findIndex(
                    (x) => x.ingredientId === ingredientId
                );

                if (index === -1) return prev;

                const current = existing[index];

                const nextQty = current.quantity - quantity;

                if (nextQty <= 0) {
                    const next = existing.filter(
                        (x) => x.ingredientId !== ingredientId
                    );

                    if (next.length === 0) {
                        const cleared = { ...prev };

                        delete cleared[orderItemId];

                        return cleared;
                    }

                    return { ...prev, [orderItemId]: next };
                }

                const next = existing.map((x, idx) =>
                    idx === index ? { ...x, quantity: nextQty } : x
                );

                return { ...prev, [orderItemId]: next };
            });
        },

        []
    );

    const removeIngredientFromOrderItem = useCallback(
        ({
            orderItemId,
            ingredientId,
        }: {
            orderItemId: string;
            ingredientId: string;
        }) => {
            if (!orderItemId || !ingredientId) return;

            setRemovedIngredientIdsByOrderItemId((prev) => {
                const existing = prev[orderItemId] ?? [];

                if (existing.includes(ingredientId)) return prev;

                return { ...prev, [orderItemId]: [...existing, ingredientId] };
            });
        },

        []
    );

    const restoreIngredientForOrderItem = useCallback(
        ({
            orderItemId,
            ingredientId,
        }: {
            orderItemId: string;

            ingredientId: string;
        }) => {
            if (!orderItemId || !ingredientId) return;

            setRemovedIngredientIdsByOrderItemId((prev) => {
                const existing = prev[orderItemId] ?? [];

                if (!existing.includes(ingredientId)) return prev;

                const next = existing.filter((x) => x !== ingredientId);

                if (next.length === 0) {
                    const cleared = { ...prev };

                    delete cleared[orderItemId];

                    return cleared;
                }

                return { ...prev, [orderItemId]: next };
            });
        },

        []
    );

    const clearExtraIngredients = useCallback((orderItemId?: string) => {
        if (!orderItemId) {
            setExtraIngredientsByOrderItemId({});

            return;
        }

        setExtraIngredientsByOrderItemId((prev) => {
            if (!prev[orderItemId]) return prev;

            const next = { ...prev };

            delete next[orderItemId];

            return next;
        });
    }, []);

    const clearRemovedIngredients = useCallback((orderItemId?: string) => {
        if (!orderItemId) {
            setRemovedIngredientIdsByOrderItemId({});

            return;
        }

        setRemovedIngredientIdsByOrderItemId((prev) => {
            if (!prev[orderItemId]) return prev;

            const next = { ...prev };

            delete next[orderItemId];

            return next;
        });
    }, []);

    const clearOrders = useCallback(() => {
        setOrders([]);

        setDeliveryPrice(0);

        setExtraIngredientsByOrderItemId({});

        setRemovedIngredientIdsByOrderItemId({});
    }, []);

    const getExtraIngredients = useCallback(
        (orderItemId: string) =>
            extraIngredientsByOrderItemId[orderItemId] ?? [],

        [extraIngredientsByOrderItemId]
    );

    const getExtraIngredientsTotalPrice = useCallback(
        (orderItemId: string) =>
            (extraIngredientsByOrderItemId[orderItemId] ?? []).reduce(
                (acc, x) => acc + (Number(x.price) || 0) * x.quantity,

                0
            ),

        [extraIngredientsByOrderItemId]
    );

    const getExtraIngredientsPayloadByOrderItemId = useCallback(() => {
        return Object.fromEntries(
            Object.entries(extraIngredientsByOrderItemId).map(
                ([orderItemId, extras]) => [
                    orderItemId,

                    extras.map((x) => ({
                        ingredientId: x.ingredientId,

                        quantity: x.quantity,
                    })),
                ]
            )
        );
    }, [extraIngredientsByOrderItemId]);

    const getRemovedIngredientIds = useCallback(
        (orderItemId: string) =>
            removedIngredientIdsByOrderItemId[orderItemId] ?? [],

        [removedIngredientIdsByOrderItemId]
    );

    const getRemovedIngredientIdsPayloadByOrderItemId = useCallback(() => {
        return removedIngredientIdsByOrderItemId;
    }, [removedIngredientIdsByOrderItemId]);

    return (
        <OrdersContext.Provider
            value={{
                orders,
                addOrder: addOrderHandler,
                extraIngredientsByOrderItemId,
                removedIngredientIdsByOrderItemId,
                getExtraIngredients,
                getExtraIngredientsTotalPrice,
                getExtraIngredientsPayloadByOrderItemId,
                getRemovedIngredientIds,
                getRemovedIngredientIdsPayloadByOrderItemId,
                addExtraIngredient,
                decrementExtraIngredient,
                removeIngredientFromOrderItem,
                restoreIngredientForOrderItem,
                clearExtraIngredients,
                clearRemovedIngredients,
                updateDeliveryPrice,
                deliveryPrice,
                clearOrders,
            }}
        >
            {children}
        </OrdersContext.Provider>
    );
};

export default OrdersProvider;

export const useOrdersContext = () => {
    const ctx = use(OrdersContext);

    if (!ctx)
        throw new Error(
            'useOrdersContext must be used within an OrdersProvider'
        );

    return ctx;
};
