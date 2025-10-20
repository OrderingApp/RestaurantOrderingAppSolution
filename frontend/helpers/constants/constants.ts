import type { StaticImport } from 'next/dist/shared/lib/get-img-props';

import { languagePack } from './languagePacks';

import tablesSvg from '@/public/images/svg/tables.svg';
import ordersSvg from '@/public/images/svg/orders.svg';
import menuSvg from '@/public/images/svg/menu.svg';
import calendarSvg from '@/public/images/svg/calendar-white.svg';
import endOfDaySvg from '@/public/images/svg/end-of-day.svg';
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
    endOfDay: endOfDaySvg,
    settings: settingsSvg,
};

export const CURRENCIES = {
    pln: 'zł',
    eur: '€',
};

export const BACKEND_URL = 'http://localhost:5000/api';

export enum BACKEND_PATHS {
    Areas = 'areas',
    Reservations = 'reservations',
    MenuICategories = 'menu-categories',
    MenuItems = 'menu-items',
    Tags = 'tags',
    Orders = 'orders',
    Ingredients = 'ingredients',
}

export enum SEARCH_PARAMS_NAMES {
    CATEGORY = 'categoryId',
    SUBCATEGORY = 'subcategoryId',
    NAME = 'name',
    TAG = 'tag',
    ORDER_TYPE = 'orderType',
    ORDER_STATUS = 'orderStatus',
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

export enum FILTER_STATUS {
    ONGOING = 'Ongoing',
    CLOSED = 'Closed',
    PAIDANDREADYTOPREPARE = 'PaidAndReadyToPrepare',
}

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
];
