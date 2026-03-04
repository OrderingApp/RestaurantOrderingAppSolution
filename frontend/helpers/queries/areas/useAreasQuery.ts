import { useQuery } from '@tanstack/react-query';

import { BACKEND_PATHS } from '@/helpers/constants/constants';
import { Areas } from '@/helpers/utils/queryKeys';
import { fetchWithParams } from '@/helpers/utils/utils';

export interface AreaReservation {
    id: string;
    phoneNumber: string;
    name: string;
    scheduledFor: string;
    capacityNeeded: number;
}

export interface AreaTable {
    id: string;
    name: string;
    capacity: number;
    isPrepared: boolean;
    reservations: AreaReservation[];
    status: string;
}

export interface Area {
    id: string;
    name: string;
    isUsed: boolean;
    isDeleted: boolean;
    tables: AreaTable[];
}

const useQueryAreas = (id?: string) =>
    useQuery({
        queryKey: id ? [Areas.All, Areas.BY_ID, id] : [Areas.All],
        queryFn: () =>
            fetchWithParams(BACKEND_PATHS.Areas, id).then((response) =>
                Array.isArray(response)
                    ? (response as Area[])
                    : [response as Area]
            ),
    });

export default useQueryAreas;
