'use client';

import { ORDER_TYPES } from '@/helpers/constants/constants';
import useConfirmOrderBillsMutation, {
    UseConfirmOrderBillsMutationOptions,
    ConfirmOrderBillsMutationInput,
    BillForSync,
    BillItemForSync,
    OrderItemUpdateForSync,
    OrderItemStatusUpdateForSync,
} from './useConfirmOrderBillsMutation';

export type {
    BillItemForSync,
    BillForSync,
    OrderItemUpdateForSync,
    OrderItemStatusUpdateForSync,
};

type ConfirmTakeawayBillsInput = Omit<
    ConfirmOrderBillsMutationInput,
    'orderType'
>;

type UseConfirmTakeawayBillsMutationOptions = Omit<
    UseConfirmOrderBillsMutationOptions,
    'orderType'
>;

const useConfirmTakeawayBillsMutation = (
    options?: UseConfirmTakeawayBillsMutationOptions
) => {
    const mutation = useConfirmOrderBillsMutation({
        ...options,
        orderType: ORDER_TYPES.TAKEAWAY,
    });

    return {
        ...mutation,
        mutate: (
            variables: ConfirmTakeawayBillsInput,
            mutateOptions?: Parameters<typeof mutation.mutate>[1]
        ) => mutation.mutate(variables, mutateOptions),
        mutateAsync: (
            variables: ConfirmTakeawayBillsInput,
            mutateOptions?: Parameters<typeof mutation.mutateAsync>[1]
        ) => mutation.mutateAsync(variables, mutateOptions),
    };
};

export default useConfirmTakeawayBillsMutation;
