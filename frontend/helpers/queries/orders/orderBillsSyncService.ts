'use client';

import { BACKEND_URL, ORDER_TYPES } from '@/helpers/constants/constants';

export interface BillItemForSync {
    id: string;
    quantity: number;
    discount?: number;
    specialInstructions?: string;
    extraIngredients?: Array<{
        ingredientId: string;
        quantity: number;
    }>;
    removedIngredientIds?: string[];
}

export interface OrderItemUpdateForSync {
    id: string;
    discount?: number;
    specialInstructions?: string;
    extraIngredients?: Array<{
        ingredientId: string;
        quantity: number;
    }>;
    removedIngredientIds?: string[];
}

export interface OrderItemStatusUpdateForSync {
    id: string;
    status: 'Pending' | 'Served' | 'Cancelled';
}

export interface BillForSync {
    id: string;
    isNew: boolean;
    pendingItems: BillItemForSync[];
    updatedItems?: OrderItemUpdateForSync[];
    updatedItemStatuses?: OrderItemStatusUpdateForSync[];
}

export interface ConfirmOrderBillsInput {
    orderType: ORDER_TYPES;
    bills: BillForSync[];
    tableId?: string;
    deliveryPrice?: number;
    discount?: number;
    customerInformation?: {
        phoneNumber: string;
        orderCompletionType: 'Immediate' | 'Scheduled';
        preferredPaymentMethod: 'Card' | 'Cash' | 'Online';
        additionalInstructions?: string;
        expectedOrderCompletion?: string;
        address?: string;
    };
}

const toOrderItemsPayload = (items: BillItemForSync[]) =>
    items.flatMap((item) =>
        Array.from({ length: item.quantity }, () => ({
            specialInstructions: item.specialInstructions || '',
            discount: item.discount || 0,
            menuItemId: item.id,
            extraIngredients: item.extraIngredients || [],
            removedIngredientIds: item.removedIngredientIds || [],
        }))
    );

const getExpectedCompletionDateTime = () => {
    const now = new Date();
    const dateStr = now.toISOString().split('T')[0];
    const timeStr = now.toTimeString().split(':').slice(0, 2).join(':');

    return `${dateStr}T${timeStr}:00`;
};

const parseError = async (res: Response, fallbackMessage: string) => {
    const text = await res.text().catch(() => '');
    throw new Error(text || fallbackMessage);
};

const assertCreatePayloadRequirements = ({
    orderType,
    tableId,
    customerInformation,
}: Pick<
    ConfirmOrderBillsInput,
    'orderType' | 'tableId' | 'customerInformation'
>) => {
    if (orderType === ORDER_TYPES.DINEIN && !tableId) {
        throw new Error('tableId is required for dine-in orders');
    }

    if (
        (orderType === ORDER_TYPES.TAKEAWAY ||
            orderType === ORDER_TYPES.DELIVERY) &&
        !customerInformation
    ) {
        throw new Error(
            'customerInformation is required for takeaway and delivery orders'
        );
    }
};

const createOrder = async ({
    orderType,
    tableId,
    deliveryPrice,
    discount,
    customerInformation,
    orderItems,
}: {
    orderType: ORDER_TYPES;
    tableId?: string;
    deliveryPrice?: number;
    discount?: number;
    customerInformation?: ConfirmOrderBillsInput['customerInformation'];
    orderItems: ReturnType<typeof toOrderItemsPayload>;
}) => {
    assertCreatePayloadRequirements({
        orderType,
        tableId,
        customerInformation,
    });

    const expectedOrderCompletion =
        customerInformation?.expectedOrderCompletion ||
        getExpectedCompletionDateTime();
    const orderTypePath = orderType.toLowerCase();

    const payloadByType: Record<ORDER_TYPES, Record<string, unknown>> = {
        [ORDER_TYPES.DINEIN]: {
            tableId,
            discount: discount || 0,
            orderItems,
        },
        [ORDER_TYPES.TAKEAWAY]: {
            discount: discount || 0,
            customerInformation: {
                phoneNumber: customerInformation?.phoneNumber,
                orderCompletionType: customerInformation?.orderCompletionType,
                preferredPaymentMethod:
                    customerInformation?.preferredPaymentMethod,
                additionalInstructions:
                    customerInformation?.additionalInstructions || '',
                expectedOrderCompletion,
            },
            orderItems,
        },
        [ORDER_TYPES.DELIVERY]: {
            discount: discount || 0,
            deliveryPrice: deliveryPrice || 0,
            customerInformation: {
                phoneNumber: customerInformation?.phoneNumber,
                orderCompletionType: customerInformation?.orderCompletionType,
                preferredPaymentMethod:
                    customerInformation?.preferredPaymentMethod,
                additionalInstructions:
                    customerInformation?.additionalInstructions || '',
                expectedOrderCompletion,
                address: customerInformation?.address || '',
            },
            orderItems,
        },
    };

    const createRes = await fetch(`${BACKEND_URL}/orders/${orderTypePath}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payloadByType[orderType]),
    });

    if (!createRes.ok) {
        await parseError(
            createRes,
            `Failed to create ${orderType.toLowerCase()} order`
        );
    }
};

const addItemsToOrder = async (
    orderId: string,
    orderItems: BillItemForSync[]
) => {
    const addItemsRes = await fetch(
        `${BACKEND_URL}/orders/${orderId}/order-items`,
        {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(toOrderItemsPayload(orderItems)),
        }
    );

    if (!addItemsRes.ok) {
        await parseError(addItemsRes, 'Failed to add items to order');
    }
};

const updateOrderItem = async (
    orderId: string,
    update: OrderItemUpdateForSync
) => {
    const updateRes = await fetch(
        `${BACKEND_URL}/orders/${orderId}/order-items/${update.id}`,
        {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                specialInstructions: update.specialInstructions,
                discount: update.discount,
                extraIngredients: update.extraIngredients || [],
                removedIngredientIds: update.removedIngredientIds || [],
            }),
        }
    );

    if (!updateRes.ok) {
        await parseError(updateRes, `Failed to update order item ${update.id}`);
    }
};

const updateOrderItemStatus = async (
    orderId: string,
    update: OrderItemStatusUpdateForSync
) => {
    const updateStatusRes = await fetch(
        `${BACKEND_URL}/orders/${orderId}/order-items/${update.id}/status`,
        {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(update.status),
        }
    );

    if (!updateStatusRes.ok) {
        await parseError(
            updateStatusRes,
            `Failed to update status for order item ${update.id}`
        );
    }
};

export const syncOrderBills = async ({
    bills,
    orderType,
    tableId,
    deliveryPrice,
    discount,
    customerInformation,
}: ConfirmOrderBillsInput) => {
    for (const bill of bills) {
        const hasPendingItems = bill.pendingItems.length > 0;

        if (bill.isNew) {
            if (!hasPendingItems) continue;

            await createOrder({
                orderType,
                tableId,
                deliveryPrice,
                discount,
                customerInformation,
                orderItems: toOrderItemsPayload(bill.pendingItems),
            });

            continue;
        }

        if (hasPendingItems) {
            await addItemsToOrder(bill.id, bill.pendingItems);
        }

        if (bill.updatedItems && bill.updatedItems.length > 0) {
            for (const update of bill.updatedItems) {
                await updateOrderItem(bill.id, update);
            }
        }

        if (bill.updatedItemStatuses && bill.updatedItemStatuses.length > 0) {
            for (const update of bill.updatedItemStatuses) {
                await updateOrderItemStatus(bill.id, update);
            }
        }
    }
};
