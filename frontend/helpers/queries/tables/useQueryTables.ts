import { useQuery } from '@tanstack/react-query';
import { BACKEND_URL } from '@/helpers/constants/constants';

export interface Table {
    id: string;
    name: string;
    capacity: number;
    isPrepared: boolean;
    reservations: unknown[];
    status: string;
}

const useQueryTables = () =>
    useQuery({
        queryKey: ['tables'],
        queryFn: async () => {
            const res = await fetch(`${BACKEND_URL}/tables`);
            if (!res.ok) throw new Error('Failed to fetch tables');
            return res.json() as Promise<Table[]>;
        },
    });

export default useQueryTables;
