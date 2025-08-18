interface CustomerInformation {
    id: string;
    phoneNumber: string;
    additionalInstructions: string;
    address: string;
    orderCompletionType: 'Immediate' | 'Scheduled';
    preferredPaymentMethod: 'Card' | 'Cash' | 'Digital';
    expectedOrderCompletion: string;
}

interface Ingredient {
    ingredientId: string;
    ingredientName: string;
    quantity: number;
    price: number;
}

interface MenuItem {
    id: string;
    name: string;
    description: string;
    price: number;
}

interface OrderItem {
    id: string;
    price: number;
    specialInstructions: string;
    discount: number;
    extraIngredients: Ingredient[];
    removedIngredients: Ingredient[];
    menuItem: MenuItem;
}

export interface Order {
    id: string;
    dateTime: string;
    totalAmount: number;
    discount: number;
    orderStatus: string;
    orderType: string;
    tableId: string;
    customerInformation: CustomerInformation;
    orderItems: OrderItem[];
}
