import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Reservation } from './useQueryReservations';
import { BACKEND_URL } from '@/helpers/constants/constants';
import { Reservations } from '@/helpers/utils/queryKeys';

export interface ReservationDto {
    phoneNumber: string;
    name: string;
    dateTime: string;
    capacityNeeded: string;
}

const useReservationMutationCreate = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async ({ data }: { data: ReservationDto }) => {
            const response = await fetch(`${BACKEND_URL}/reservations`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data),
            });

            if (!response.ok) {
                throw new Error('Failed to create reservation');
            }

            return response.json();
        },

        onMutate: async (newReservation: { data: ReservationDto }) => {
            await queryClient.cancelQueries({
                queryKey: [Reservations.BY_DATE],
            });

            const previousReservations = queryClient.getQueryData<
                Reservation[]
            >([Reservations.BY_DATE]);

            queryClient.setQueryData(
                [Reservations.BY_DATE],
                (old: Reservation[] = []) => [
                    ...old,
                    { id: Date.now(), ...newReservation.data },
                ]
            );

            return { previousReservations };
        },

        onError: (err, newReservation, context) => {
            if (context?.previousReservations) {
                queryClient.setQueryData(
                    [Reservations.BY_DATE],
                    context.previousReservations
                );
            }
        },

        onSettled: () => {
            queryClient.invalidateQueries({ queryKey: [Reservations.BY_DATE] });
        },
    });
};

export const useReservationMutationUpdate = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async ({
            id,
            data,
        }: {
            id: string;
            data: ReservationDto;
        }) => {
            const response = await fetch(`${BACKEND_URL}/reservations/${id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data),
            });

            if (!response.ok) {
                throw new Error('Failed to update reservation');
            }

            return response.json();
        },

        onMutate: async ({
            id,
            data,
        }: {
            id: string;
            data: ReservationDto;
        }) => {
            await queryClient.cancelQueries({
                queryKey: [Reservations.BY_DATE],
            });

            const previousReservations = queryClient.getQueryData<
                Reservation[]
            >([Reservations.BY_DATE]);

            queryClient.setQueryData(
                [Reservations.BY_DATE],
                (old: Reservation[] = []) =>
                    old.map((reservation) =>
                        reservation.id === id
                            ? { ...reservation, ...data }
                            : reservation
                    )
            );

            return { previousReservations };
        },

        onError: (err, newReservation, context) => {
            if (context?.previousReservations) {
                queryClient.setQueryData(
                    [Reservations.BY_DATE],
                    context.previousReservations
                );
            }
        },

        onSettled: () => {
            queryClient.invalidateQueries({ queryKey: [Reservations.BY_DATE] });
        },
    });
};
export const useReservationMutationDelete = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async ({ id }: { id: string }) => {
            const response = await fetch(`${BACKEND_URL}/reservations/${id}`, {
                method: 'DELETE',
                headers: { 'Content-Type': 'application/json' },
            });

            if (!response.ok) {
                throw new Error('Failed to update reservation');
            }

            return response.json();
        },

        onMutate: async ({ id }: { id: string }) => {
            await queryClient.cancelQueries({
                queryKey: [Reservations.BY_DATE],
            });

            const previousReservations = queryClient.getQueryData<
                Reservation[]
            >([Reservations.BY_DATE]);

            queryClient.setQueryData(
                [Reservations.BY_DATE],
                (old: Reservation[] = []) =>
                    old.filter((reservation) => reservation.id !== id)
            );

            return { previousReservations };
        },

        onError: (err, newReservation, context) => {
            if (context?.previousReservations) {
                queryClient.setQueryData(
                    [Reservations.BY_DATE],
                    context.previousReservations
                );
            }
        },

        onSettled: () => {
            queryClient.invalidateQueries({ queryKey: [Reservations.BY_DATE] });
        },
    });
};

export default useReservationMutationCreate;
