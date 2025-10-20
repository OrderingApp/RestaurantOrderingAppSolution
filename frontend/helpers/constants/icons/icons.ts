import userSvg from '@/public/images/svg/user.svg';
import usersSvg from '@/public/images/svg/users.svg';
import calendarSvg from '@/public/images/svg/calendar.svg';
import timeSvg from '@/public/images/svg/time.svg';
import phoneSvg from '@/public/images/svg/phone.svg';
import markerSvg from '@/public/images/svg/marker.svg';
import closeSvg from '@/public/images/svg/close.svg';
import closeWhiteSvg from '@/public/images/svg/close-white.svg';
import listSvg from '@/public/images/svg/list.svg';
import listWhiteSvg from '@/public/images/svg/list-white.svg';
import menuOpenSvg from '@/public/images/svg/menu_open.svg';
import menuOpenWhiteSvg from '@/public/images/svg/menu_open-white.svg';

export const ICONS = {
    USER: userSvg,
    USERS: usersSvg,
    CALENDAR: calendarSvg,
    TIME: timeSvg,
    PHONE: phoneSvg,
    MARKER: markerSvg,
    CLOSE: closeSvg,
    CLOSE_WHITE: closeWhiteSvg,
    LIST: listSvg,
    LIST_WHITE: listWhiteSvg,
    MENU_OPEN: menuOpenSvg,
    MENU_OPEN_WHITE: menuOpenWhiteSvg,
} as const;
