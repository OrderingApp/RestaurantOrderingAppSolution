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

type ConfirmDeliveryBillsInput = Omit<
    ConfirmOrderBillsMutationInput,
    'orderType'
>;

type UseConfirmDeliveryBillsMutationOptions = Omit<
    UseConfirmOrderBillsMutationOptions,
    'orderType'
>;

const useConfirmDeliveryBillsMutation = (
    options?: UseConfirmDeliveryBillsMutationOptions
) => {
    const mutation = useConfirmOrderBillsMutation({
        ...options,
        orderType: ORDER_TYPES.DELIVERY,
    });

    return {
        ...mutation,
        mutate: (
            variables: ConfirmDeliveryBillsInput,
            mutateOptions?: Parameters<typeof mutation.mutate>[1]
        ) => mutation.mutate(variables, mutateOptions),
        mutateAsync: (
            variables: ConfirmDeliveryBillsInput,
            mutateOptions?: Parameters<typeof mutation.mutateAsync>[1]
        ) => mutation.mutateAsync(variables, mutateOptions),
    };
};

export default useConfirmDeliveryBillsMutation;
