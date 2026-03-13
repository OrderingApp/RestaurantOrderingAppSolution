export type OrderKind = 'dinein' | 'Takeaway' | 'Delivery';

export type OrderType = 'dinein' | 'Takeaway' | 'Delivery';

export type OrderCompletionType = 'Immediate' | 'Scheduled';

export type PaymentMethod = 'Card' | 'Cash' | 'Online';

export type OrderStatus =
    | 'Ongoing'
    | 'PendingPayment'
    | 'Cancelled'
    | 'Closed'
    | 'PaidAndReadyToPrepare';

export interface ExtraIngredient {
    ingredientId: string;
    ingredientName: string;
    quantity: number;
    price: number;
}

export interface RemovedIngredient {
    ingredientId: string;
    ingredientName: string;
    quantity: number;
    price: number;
}

interface Ingredient {
    ingredientId: string;
    ingredientName: string;
    quantity: number;
    price: number;
}

interface BaseIngredient {
    id: string;
    name: string;
}

interface MenuItem {
    id: string;
    name: string;
    description: string | null;
    price: number;
    baseIngredients: BaseIngredient[];
}

export interface OrderItem {
    id: string;
    price: number;
    discount: number;
    menuItem: MenuItem;
    removedIngredients?: BaseIngredient[];
    specialInstructions?: string | null;
    extraIngredients?: Ingredient[];
}

interface CustomerInformation {
    phoneNumber: string;
    additionalInstructions: string | null;
    address?: string | null;
    orderCompletionType: OrderCompletionType;
    preferredPaymentMethod: PaymentMethod;
    expectedOrderCompletion: string;
}

export interface Order {
    id: string;
    createdAt: string;
    totalAmount: number;
    discount: number;
    orderStatus: OrderStatus;
    orderType: OrderType;
    tableId: string | null;
    customerInformation: CustomerInformation;
    orderItems: OrderItem[];
}

export interface NotDineInOrder {
    id: string;
    createdAt: string;
    expectedOrderCompletion: string;
    totalAmount: number;
    orderStatus: OrderStatus;
    orderType: OrderType;
    phoneNumber: string;
    address: string | null;
}

export interface OrderDto {
    createdAt: string;
    discount: number;
    deliveryPrice: number;
    customerInformation: {
        phoneNumber: string;
        orderCompletionType: OrderCompletionType;
        preferredPaymentMethod: PaymentMethod;
        additionalInstructions?: string;
        expectedOrderCompletion: string;
        address?: string;
    };
    orderItems: {
        menuItemId: string;
    }[];
}
