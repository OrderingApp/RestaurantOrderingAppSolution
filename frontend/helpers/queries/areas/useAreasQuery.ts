import { useQuery } from '@tanstack/react-query';

import { fetchWithParams } from '@/helpers/utils/utils';
import { Areas } from '@/helpers/utils/queryKeys';

const useQueryAreas = (id: string) =>
    useQuery({
        queryKey: [Areas.All],
        queryFn: () => fetchWithParams('areas', id),
    });

export default useQueryAreas;
