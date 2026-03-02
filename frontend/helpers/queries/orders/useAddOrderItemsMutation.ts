'use client';

import { useMutation } from '@tanstack/react-query';
import { BACKEND_URL } from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import { useLanguage } from '@/providers/LanguageProvider';
import { toast } from 'sonner';

interface OrderItemData {
    menuItemId: string;
    specialInstructions?: string;
    discount?: number;
    extraIngredients?: unknown[];
    removedIngredientIds?: unknown[];
}

const useAddOrderItemsMutation = (options?: {
    onSuccess?: (data: unknown) => void;
    onError?: (err: unknown) => void;
}) => {
    const { language } = useLanguage();

    return useMutation({
        mutationFn: async ({
            orderId,
            orderItems,
        }: {
            orderId: string;
            orderItems: OrderItemData[];
        }) => {
            const res = await fetch(
                `${BACKEND_URL}/orders/${orderId}/order-items`,
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(orderItems),
                }
            );

            if (!res.ok) {
                const text = await res.text().catch(() => '');
                throw new Error(text || 'Failed to add items to bill');
            }

            return res.json();
        },
        onSuccess: (data) => {
            // Query invalidation is handled in CreateOrder component
            // This prevents duplicate toasts and ensures proper timing
            options?.onSuccess?.(data as unknown);
        },
        onError: (err) => {
            console.error(err);
            toast.error(
                languagePacks[language].createOrderPage.error ||
                    'Failed to add to bill'
            );

            options?.onError?.(err);
        },
    });
};

export default useAddOrderItemsMutation;
