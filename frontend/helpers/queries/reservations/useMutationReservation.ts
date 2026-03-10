import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Reservation } from './useQueryReservations';
import { BACKEND_URL } from '@/helpers/constants/constants';
import { Reservations } from '@/helpers/utils/queryKeys';
import { useRouter } from 'next/navigation';
import { toast } from 'sonner';
import useLanguage from '@/helpers/hooks/useLanguage';
import languagePacks from '@/helpers/constants/languagePacks';

export interface ReservationDto {
    phoneNumber: string;
    name: string;
    scheduledFor: string;
    capacityNeeded: string;
    tableId: string | null;
}

const useReservationMutation = (type: 'create' | 'update' | 'delete') => {
    const queryClient = useQueryClient();
    const router = useRouter();
    const { language } = useLanguage();

    const { createReservationPage } = languagePacks[language];
    const toastMessages = createReservationPage.toasts;

    const loadingMessages = {
        create: toastMessages.loading.create,
        update: toastMessages.loading.update,
        delete: toastMessages.loading.delete,
    };

    const successMessages = {
        create: toastMessages.success.create,
        update: toastMessages.success.update,
        delete: toastMessages.success.delete,
    };

    return useMutation({
        mutationFn: async ({
            id,
            data,
        }: {
            id?: string;
            data?: ReservationDto;
        }) => {
            let url = `${BACKEND_URL}/reservations`;
            let method = 'POST';

            if (type === 'update' && id) {
                url += `/${id}`;
                method = 'PUT';
            } else if (type === 'delete' && id) {
                url += `/${id}`;
                method = 'DELETE';
            }

            const response = await fetch(url, {
                method,
                headers: { 'Content-Type': 'application/json' },
                body: type !== 'delete' ? JSON.stringify(data) : undefined,
            });

            if (!response.ok) throw new Error(`Failed to ${type} reservation`);
            const text = await response.text();
            return text ? JSON.parse(text) : null;
        },

        onMutate: async ({
            id,
            data,
        }: {
            id?: string;
            data?: ReservationDto;
        }) => {
            const toastId = toast.loading(loadingMessages[type]);

            await queryClient.cancelQueries({
                queryKey: [Reservations.BY_DATE],
            });

            const previousReservations = queryClient.getQueryData<
                Reservation[]
            >([Reservations.BY_DATE]);

            queryClient.setQueryData(
                [Reservations.BY_DATE],
                (old: Reservation[] = []) => {
                    if (type === 'create' && data) {
                        return [...old, { id: Date.now(), ...data }];
                    } else if (type === 'update' && id && data) {
                        return old.map((reservation) =>
                            reservation.id === id
                                ? { ...reservation, ...data }
                                : reservation
                        );
                    } else if (type === 'delete' && id) {
                        return old.filter(
                            (reservation) => reservation.id !== id
                        );
                    }
                    return old;
                }
            );

            return { previousReservations, toastId };
        },

        onSuccess: (_, __, context) => {
            if (context?.toastId) {
                toast.success(successMessages[type], { id: context.toastId });
            }
        },

        onError: (err, _, context) => {
            if (context?.previousReservations) {
                queryClient.setQueryData(
                    [Reservations.BY_DATE],
                    context.previousReservations
                );
            }

            if (context?.toastId) {
                toast.error(
                    `${toastMessages.error.prefix}: ${err.message || toastMessages.error.actionFailed}`,
                    { id: context.toastId }
                );
            } else {
                toast.error(toastMessages.error.unexpected);
            }
        },

        onSettled: () => {
            queryClient.invalidateQueries({ queryKey: [Reservations.BY_DATE] });
            router.push('/reservations');
        },
    });
};

export default useReservationMutation;
