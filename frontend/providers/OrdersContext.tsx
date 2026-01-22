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

interface OrdersContextType {
    orders: OrderContextType[];
    deliveryPrice?: number;
    addOrder: (order: OrderContextType) => void;
    updateDeliveryPrice: (price: number) => void;
    clearOrders: () => void;
}

export const OrdersContext = createContext<OrdersContextType | undefined>(
    undefined
);

const OrdersProvider = ({ children }: { children: ReactNode }) => {
    const [orders, setOrders] = useState<OrderContextType[]>([]);
    const [deliveryPrice, setDeliveryPrice] = useState(0);

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

    const clearOrders = useCallback(() => {
        setOrders([]);
        setDeliveryPrice(0);
    }, []);

    return (
        <OrdersContext.Provider
            value={{
                orders,
                addOrder: addOrderHandler,
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
        throw new Error('useLanguage must be used within a LanguageProvider');

    return ctx;
};
