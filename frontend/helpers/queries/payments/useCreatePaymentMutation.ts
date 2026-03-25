import { useMutation, useQueryClient } from '@tanstack/react-query';

import {
    BACKEND_URL,
    BACKEND_URL_PAYMENT,
} from '@/helpers/constants/constants';
import { OrdersItems } from '@/helpers/utils/queryKeys';

export type PaymentMethod = 'Card' | 'Cash';

interface CreatePaymentInput {
    orderId: string;
    amount: number;
    paymentMethod: PaymentMethod;
}

interface UseCreatePaymentMutationOptions {
    onSuccess?: () => void | Promise<void>;
    onError?: (error: unknown) => void;
}

const useCreatePaymentMutation = (
    options?: UseCreatePaymentMutationOptions
) => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async ({
            orderId,
            amount,
            paymentMethod,
        }: CreatePaymentInput) => {
            if (!orderId) throw new Error('Missing order id');

            const response = await fetch(
                `${BACKEND_URL_PAYMENT}/orders/${orderId}/payments`,
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ amount, paymentMethod }),
                }
            );

            if (!response.ok) {
                let message = 'Failed to create payment.';
                const contentType = response.headers.get('content-type') || '';

                if (contentType.includes('application/json')) {
                    const body = await response.json();
                    message = body?.errorMessage || message;
                } else {
                    const text = await response.text();
                    if (text) message = text;
                }

                throw new Error(message);
            }

            return response.json();
        },
        onSuccess: async (_, variables) => {
            await queryClient.invalidateQueries({
                queryKey: [OrdersItems.BY_ID, variables.orderId],
            });
            await queryClient.invalidateQueries({
                queryKey: [OrdersItems.BY_TYPE],
            });

            await options?.onSuccess?.();
        },
        onError: (error) => {
            options?.onError?.(error);
        },
    });
};

export default useCreatePaymentMutation;
