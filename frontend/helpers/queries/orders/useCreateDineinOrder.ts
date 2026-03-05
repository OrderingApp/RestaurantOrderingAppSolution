'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { BACKEND_URL } from '@/helpers/constants/constants';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import languagePacks from '@/helpers/constants/languagePacks';
import { useLanguage } from '@/providers/LanguageProvider';
import { toast } from 'sonner';

const useCreateDineinOrder = (options?: {
    onSuccess?: (data: unknown) => void;
    onError?: (err: unknown) => void;
}) => {
    const queryClient = useQueryClient();
    const { language } = useLanguage();

    return useMutation({
        mutationFn: async (data: Record<string, unknown>) => {
            const res = await fetch(`${BACKEND_URL}/orders/dinein`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data),
            });

            if (!res.ok) {
                const text = await res.text().catch(() => '');
                throw new Error(text || 'Failed to create order');
            }

            return res.json();
        },
        onSuccess: (data, variables, context) => {
            queryClient.invalidateQueries({
                queryKey: [OrdersItems.BY_TYPE, 'dinein'],
            });

            toast.success(
                languagePacks[language].createOrderPage.confirmation ||
                    'Added to bill'
            );

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

export default useCreateDineinOrder;
