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
    id?: string;
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
    paidAmount?: number;
    unpaidAmount?: number;
    remainingAmount?: number;
    discount: number;
    orderStatus: OrderStatus;
    paymentStatus?: PaymentStatus;
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
    discount: number;
    customerInformation: {
        phoneNumber: string;
        additionalInstructions: string;
        address: string;
        orderCompletionType: OrderCompletionType;
        expectedOrderCompletion: string;
    };
    orderItems: {
        specialInstructions: string;
        discount: number;
        menuItemId: string;
        extraIngredients: {
            ingredientId: string;
            quantity: number;
        }[];
        removedIngredientIds: string[];
    }[];
}
