import { useQuery } from '@tanstack/react-query';

import { fetchWithParams } from '@/helpers/utils/utils';
import { Reservations } from '@/helpers/utils/queryKeys';

export interface Reservation {
    id: string;
    phoneNumber: string;
    name: string;
    scheduledFor: string;
    capacityNeeded: number;
    isAssigned: number;
    tableId: string | null;
}

const useQueryReservations = (id: string) =>
    useQuery({
        queryKey: [Reservations.BY_DATE, id],
        queryFn: () =>
            fetchWithParams('reservations', `?date=${id}`).then(
                (response) => response as Reservation[]
            ),
    });

export const useQueryReservationsById = (id: string) =>
    useQuery({
        queryKey: [Reservations.BY_ID, id],
        queryFn: () =>
            fetchWithParams('reservations', id).then(
                (response) => response as Reservation
            ),
        enabled: !!id,
    });

export default useQueryReservations;
