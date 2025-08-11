enum AREA_KEYS {
    All = 'all-areas',
}

enum RESERVATIONS_KEYS {
    BY_DATE = 'by-date',
    BY_ID = 'by-id',
}
enum MENU_ITEMS {
    All = 'menu-categories',
    ITEMS = 'menu-items',
    TAGS = 'tags',
}

enum ORDERS_KEYS {
    BY_TYPE = 'orderType',
    BY_ID = 'orderId',
}

const QUERY_KEYS = {
    Areas: { ...AREA_KEYS },
    Reservations: { ...RESERVATIONS_KEYS },
    MenuItems: { ...MENU_ITEMS },
    OrdersItems: { ...ORDERS_KEYS },
} as const;

export const { Areas, Reservations, MenuItems, OrdersItems } = QUERY_KEYS;
