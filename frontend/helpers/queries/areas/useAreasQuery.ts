import { useQuery } from '@tanstack/react-query';

import { fetchWithToken } from '@/helpers/utils/utils';
import { Areas } from '@/helpers/utils/query-keys';

const useQueryAreas = (id: string) =>
    useQuery({
        queryKey: [Areas.All],
        queryFn: () => fetchWithToken('areas', id),
    });

export default useQueryAreas;
