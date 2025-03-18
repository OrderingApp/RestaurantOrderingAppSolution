enum AREA_KEYS {
    All = 'all-areas',
}

enum RESERVATIONS_KEYS {
    BY_DATE = 'by-date',
    BY_ID = 'by-id',
}
enum MENU_ITEMS {
    All = 'menu-categories',
}

const QUERY_KEYS = {
    Areas: { ...AREA_KEYS },
    Reservations: { ...RESERVATIONS_KEYS },
    MenuItems: { ...MENU_ITEMS },
} as const;

export const { Areas, Reservations, MenuItems } = QUERY_KEYS;
