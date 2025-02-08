import { useQuery } from "@tanstack/react-query";

import apiClient, { API } from "../../../utils/apiClient";
import { QueryKeys } from "../../../utils/queryKeys";

import type { Table } from "../../../types/table";


export const useTableQuery = () => 
    useQuery<Table[]>({
        queryKey: [QueryKeys.Tables],
        queryFn: () => apiClient<Table[]>(API.Tables)
    });