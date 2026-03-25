'use client';

import { ORDER_TYPES } from '@/helpers/constants/constants';
import useConfirmOrderBillsMutation, {
    UseConfirmOrderBillsMutationOptions,
    ConfirmOrderBillsMutationInput,
    BillForSync as BaseBillForSync,
    OrderItemUpdateForSync,
    OrderItemStatusUpdateForSync,
} from './useConfirmOrderBillsMutation';

type DineinBillItemForSync = {
    id: string;
    menuItemId: string;
    quantity: number;
    discount?: number;
};

type DineinBillForSync = Omit<BaseBillForSync, 'pendingItems'> & {
    pendingItems: DineinBillItemForSync[];
};

interface ConfirmDineinBillsInput
    extends Omit<ConfirmOrderBillsMutationInput, 'orderType' | 'bills'> {
    bills: DineinBillForSync[];
    extraIngredientsByOrderItemId?: Record<
        string,
        { ingredientId: string; quantity: number }[]
    >;

    removedIngredientIdsByOrderItemId?: Record<string, string[]>;
}

type UseConfirmDineinBillsMutationOptions = Omit<
    UseConfirmOrderBillsMutationOptions,
    'orderType'
>;

export type {
    DineinBillItemForSync as BillItemForSync,
    DineinBillForSync as BillForSync,
    OrderItemUpdateForSync,
    OrderItemStatusUpdateForSync,
};

const mapBillsToSyncInput = (
    bills: DineinBillForSync[],
    extraIngredientsByOrderItemId?: Record<
        string,
        { ingredientId: string; quantity: number }[]
    >,
    removedIngredientIdsByOrderItemId?: Record<string, string[]>
) =>
    bills.map((bill) => ({
        ...bill,
        pendingItems: bill.pendingItems.map((item) => ({
            id: item.menuItemId,
            quantity: item.quantity,
            discount: item.discount,
            specialInstructions: '',
            extraIngredients:
                extraIngredientsByOrderItemId?.[item.id]?.map((x) => ({
                    ingredientId: x.ingredientId,
                    quantity: x.quantity,
                })) ?? [],
            removedIngredientIds:
                removedIngredientIdsByOrderItemId?.[item.id] ?? [],
        })),
    }));

const useConfirmDineinBillsMutation = (
    options?: UseConfirmDineinBillsMutationOptions
) => {
    const mutation = useConfirmOrderBillsMutation({
        ...options,
        orderType: ORDER_TYPES.DINEIN,
    });

    return {
        ...mutation,
        mutate: (
            variables: ConfirmDineinBillsInput,
            mutateOptions?: Parameters<typeof mutation.mutate>[1]
        ) => {
            const {
                bills,
                extraIngredientsByOrderItemId,
                removedIngredientIdsByOrderItemId,
                ...rest
            } = variables;

            return mutation.mutate(
                {
                    ...rest,
                    bills: mapBillsToSyncInput(
                        bills,
                        extraIngredientsByOrderItemId,
                        removedIngredientIdsByOrderItemId
                    ),
                },
                mutateOptions
            );
        },
        mutateAsync: (
            variables: ConfirmDineinBillsInput,
            mutateOptions?: Parameters<typeof mutation.mutateAsync>[1]
        ) => {
            const {
                bills,
                extraIngredientsByOrderItemId,
                removedIngredientIdsByOrderItemId,
                ...rest
            } = variables;

            return mutation.mutateAsync(
                {
                    ...rest,
                    bills: mapBillsToSyncInput(
                        bills,
                        extraIngredientsByOrderItemId,
                        removedIngredientIdsByOrderItemId
                    ),
                },
                mutateOptions
            );
        },
    };
};

export default useConfirmDineinBillsMutation;
