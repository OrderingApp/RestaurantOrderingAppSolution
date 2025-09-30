import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Reservation } from './useQueryReservations';
import { BACKEND_URL } from '@/helpers/constants/constants';
import { Reservations } from '@/helpers/utils/queryKeys';
import { useRouter } from 'next/navigation';

export interface ReservationDto {
    phoneNumber: string;
    name: string;
    scheduledFor: string;
    capacityNeeded: string;
}

const useReservationMutation = (type: 'create' | 'update' | 'delete') => {
    const queryClient = useQueryClient();
    const router = useRouter();

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

            return response.json();
        },

        onMutate: async ({
            id,
            data,
        }: {
            id?: string;
            data?: ReservationDto;
        }) => {
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

            return { previousReservations };
        },

        onError: (err, _, context) => {
            if (context?.previousReservations) {
                queryClient.setQueryData(
                    [Reservations.BY_DATE],
                    context.previousReservations
                );
            }
        },

        onSettled: () => {
            queryClient.invalidateQueries({ queryKey: [Reservations.BY_DATE] });
            router.push('/reservations');
        },
    });
};

export default useReservationMutation;
