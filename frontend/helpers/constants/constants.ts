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
<<<<<<< HEAD
=======
    Reservations = 'reservations',
    MenuICategories = 'menu-categories',
    MenuItems = 'menu-items',
    Tags = 'tags',
    Orders = 'orders',
}

export enum SEARCH_PARAMS_NAMES {
    CATEGORY = 'categoryId',
    SUBCATEGORY = 'subcategoryId',
    NAME = 'name',
    TAG = 'tag',
    ORDER_TYPE = 'orderType',
    ORDER_ID = 'orderId',
    MODAL = 'modal',
}

export enum MENU_CATEGORY_NAMES {
    ALL = 'all',
>>>>>>> 80c954fc6beaaad83f96a0ce999d81e4463517c0
}

export enum FILTER_STATUS {
    ONGOING = 'Ongoing',
    CLOSED = 'Closed',
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
