import type { StaticImport } from 'next/dist/shared/lib/get-img-props';

import { languagePack } from './languagePacks';

import tablesSvg from '@/public/images/svg/tables.svg';
import ordersSvg from '@/public/images/svg/orders.svg';
import menuSvg from '@/public/images/svg/menu.svg';
import calendarSvg from '@/public/images/svg/calendar-white.svg';
import daySummarySvg from '@/public/images/svg/end-of-day.svg';
import settingsSvg from '@/public/images/svg/settings.svg';

export const COMPANY_NAME = 'piccolo';
export const RESTAURANT_OPENING_HOUR = '14:00';
export const RESTAURANT_CLOSING_HOUR = '24:00';
export const MIN_ITEM_SELECT = 1;
export const MAX_ITEM_SELECT = 99;

export const MENU_BAR_ICONS: {
    [K in keyof languagePack['menuBar']]: StaticImport;
} = {
    tables: tablesSvg,
    orders: ordersSvg,
    menu: menuSvg,
    reservations: calendarSvg,
    daySummary: daySummarySvg,
    settings: settingsSvg,
};

export const CURRENCIES = {
    pln: 'zł',
    eur: '€',
} as const;

export const COMPANYS_CURRENCY: keyof typeof CURRENCIES = 'pln';

export const COMPANY_INITIAL_GEO_COORDS = {
    lat: 50.05598658820353,
    lng: 21.61245102578422,
};

export const BACKEND_URL = 'http://localhost:5000/api';
export const BACKEND_URL_PAYMENT = 'http://localhost:5000';

export enum BACKEND_PATHS {
    Areas = 'areas',
    Reservations = 'reservations',
    MenuICategories = 'menu-categories',
    MenuItems = 'menu-items',
    Tags = 'tags',
    Orders = 'orders',
    Ingredients = 'ingredients',
    IngredientCategories = 'ingredient-categories',
}

export enum SEARCH_PARAMS_NAMES {
    CATEGORY = 'categoryId',
    SUBCATEGORY = 'subcategoryId',
    RESERVATION = 'reservationId',
    FILTER_BY = 'filterBy',
    NAME = 'name',
    TAG = 'tag',
    PAGE = 'page',
    ORDER_MENU_PAGE = 'orderMenuPage',
    ORDER_TYPE = 'orderType',
    ORDER_STATUS = 'orderStatus',
    PAYMENT_STATUS = 'paymentStatus',
    ORDER_ID = 'orderId',
    MENU_ITEM_ID = 'menuItemId',
    MODAL = 'modal',
    PAYMENT = 'payment',
    CLOSE_ORDER = 'closeOrder',
    OPTIONS = 'options',
    USER_DATA = 'userData',
}

export enum MENU_CATEGORY_NAMES {
    ALL = 'all',
}

// TODO: ask whats going on with getStatuses in useFilterOrders and the paid&ready to prepare status
export enum ORDER_STATUSES {
    ONGOING = 'Ongoing',
    CLOSED = 'Closed',
    CANCELLED = 'Cancelled',
}

export enum PAYMENT_STATUSES {
    UNPAID = 'Unpaid',
    PARTIALPAID = 'PartiallyPaid',
    PAID = 'Paid',
}

export enum ORDER_TYPES {
    DINEIN = 'Dinein',
    TAKEAWAY = 'Takeaway',
    DELIVERY = 'Delivery',
}

// TODO: would be best to use lang pack + enums for these
export const ordersTypes = {
    pl: [
        {
            id: 'Takeaway',
            name: 'Odbiór',
        },
        {
            id: 'Delivery',
            name: 'Dowóz',
        },
    ],
    en: [
        {
            id: 'Takeaway',
            name: 'Pickup',
        },
        {
            id: 'Delivery',
            name: 'Delivery',
        },
    ],
};

export const DISCOUNTS = [
    {
        id: '5',
        name: '5%',
    },
    {
        id: '10',
        name: '10%',
    },
    {
        id: '20',
        name: '20%',
    },
    {
        id: '50',
        name: '50%',
    },
] as const;
