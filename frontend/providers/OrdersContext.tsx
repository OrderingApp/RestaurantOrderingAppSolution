'use client';
import { OrderProps } from '@/components/shared/cards/OrderCard';
import { createContext, useState, ReactNode, useCallback, use } from 'react';

interface OrderContextType extends OrderProps {
    quantity: number;
}

interface OrdersContextType {
    orders: OrderContextType[];
    addOrder: (order: OrderContextType) => void;
}

export const OrdersContext = createContext<OrdersContextType | undefined>(
    undefined
);

const OrdersProvider = ({ children }: { children: ReactNode }) => {
    const [orders, setOrders] = useState<OrderContextType[]>([]);

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

    return (
        <OrdersContext.Provider value={{ orders, addOrder: addOrderHandler }}>
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
