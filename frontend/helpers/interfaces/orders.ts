export type OrderKind = 'dinein' | 'Takeaway' | 'Delivery';

export type OrderType = 'dinein' | 'Takeaway' | 'Delivery';

export type OrderCompletionType = 'Immediate' | 'Scheduled';

export type OrderStatus = 'Ongoing' | 'Cancelled' | 'Closed';

export type PaymentStatus = 'Unpaid' | 'PartiallyPaid' | 'Paid';

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
    paymentStatus?: PaymentStatus;
    orderType: OrderType;
    phoneNumber: string;
    address: string | null;
}

export interface OrderDto {
    createdAt: string;
    deliveryPrice: number;
    customerInformation: {
        phoneNumber: string;
        orderCompletionType: OrderCompletionType;
        additionalInstructions?: string;
        expectedOrderCompletion: string;
        address?: string;
    };
    orderItems: {
        menuItemId: string;
    }[];
}
