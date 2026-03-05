import { useQuery } from '@tanstack/react-query';

import { fetchWithParams } from '@/helpers/utils/utils';
import { BACKEND_PATHS } from '@/helpers/constants/constants';
import { Areas } from '@/helpers/utils/queryKeys';

// Change tableName to tableId or remove it

export interface AreaReservation {
    id: string;
    phoneNumber: string;
    name: string;
    scheduledFor: string;
    capacityNeeded: number;
    tableName: string | null;
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

interface UseQueryAreasParams {
    id?: string;
    date?: string;
}

const useQueryAreas = ({ id, date }: UseQueryAreasParams = {}) =>
    useQuery({
        queryKey: [Areas.All, { id, date }],
        queryFn: async () => {
            let paramString = id ? id : '';
            if (date) {
                paramString += `?date=${date}`;
            }
            const response = await fetchWithParams(
                BACKEND_PATHS.Areas,
                paramString || undefined
            );
            return Array.isArray(response)
                ? (response as Area[])
                : ([response] as Area[]);
        },
    });

export default useQueryAreas;
