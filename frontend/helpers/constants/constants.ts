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
export const RESTAURANT_ClOSING_HOUR = '24:00';

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
