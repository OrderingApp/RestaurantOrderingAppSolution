'use client';

import { useMutation } from '@tanstack/react-query';
import { toast } from 'sonner';

import { BACKEND_URL } from '@/helpers/constants/constants';
import languagePacks from '@/helpers/constants/languagePacks';
import { useLanguage } from '@/providers/LanguageProvider';

export interface UpdateCustomerInformationPayload {
    phoneNumber: string;
    additionalInstructions: string;
    address: string;
    orderCompletionType: 'Immediate' | 'Scheduled';
    expectedOrderCompletion: string;
}

const useUpdateCustomerInformationMutation = (options?: {
    onSuccess?: (data: unknown) => void;
    onError?: (err: unknown) => void;
}) => {
    const { language } = useLanguage();

    return useMutation({
        mutationFn: async ({
            customerInformationId,
            data,
        }: {
            customerInformationId: string;
            data: UpdateCustomerInformationPayload;
        }) => {
            const response = await fetch(
                `${BACKEND_URL}/customers/${customerInformationId}/information`,
                {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(data),
                }
            );

            if (!response.ok) {
                const text = await response.text().catch(() => '');
                throw new Error(
                    text || 'Failed to update customer information'
                );
            }

            return response.json();
        },
        onSuccess: (data) => {
            options?.onSuccess?.(data);
        },
        onError: (err) => {
            toast.error(
                languagePacks[language].ordersPage.orderCustomerInformationForm
                    .toasts.updateError ||
                    'Failed to update customer information.'
            );
            options?.onError?.(err as unknown);
        },
    });
};

export default useUpdateCustomerInformationMutation;
