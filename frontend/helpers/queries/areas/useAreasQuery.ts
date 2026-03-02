import { useQuery } from '@tanstack/react-query';

import { fetchWithParams } from '@/helpers/utils/utils';
import { Areas } from '@/helpers/utils/queryKeys';

const useQueryAreas = (date: string) =>
    useQuery({
        queryKey: [Areas.All, date],
        queryFn: () => fetchWithParams('areas', `?date=${date}`),
    });

export default useQueryAreas;
