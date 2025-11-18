import { ReadonlyURLSearchParams } from 'next/navigation';
import { BACKEND_PATHS, BACKEND_URL } from '../constants/constants';
import { AppRouterInstance } from 'next/dist/shared/lib/app-router-context.shared-runtime';
import { toast } from 'sonner';

/**
 * Converts a camelCase string to kebab-case.
 * @param str - The camelCase string to convert.
 * @returns The kebab-case version of the string.
 */
export const camelToKebab = (str: string): string => {
    return str.replace(/([a-z])([A-Z])/g, '$1-$2').toLowerCase();
};

/**
 * Capitalizes the first letter of a string.
 * @param str - The string to capitalize.
 * @returns The string with the first letter capitalized.
 */
export const capitalizeFirstLetter = (str: string): string => {
    const match = str.match(/[a-zA-Z]/);

    if (!match) return str;

    const index = match.index!;

    return str.slice(0, index) + match[0].toUpperCase() + str.slice(index + 1);
};

export const fetchWithParams = (path: `${BACKEND_PATHS}`, params: string) =>
    fetch(`${BACKEND_URL}/${path}/${params}`).then((res) => res.json());

export const getPluralForm = (
    amount: number,
    titles: string[],
    lang: string
) => {
    if (amount === 1) return titles[1];

    if (
        lang === 'pl' &&
        [2, 3, 4].includes(amount % 10) &&
        ![12, 13, 14].includes(amount % 100)
    )
        return titles[2];

    return titles[0];
};

export const toggleQueryParam = (
    paramName: string,
    value: string | undefined = undefined,
    searchParams: ReadonlyURLSearchParams,
    router: AppRouterInstance,
    pathname: string
) => {
    const params = new URLSearchParams(searchParams.toString());

    if (value === undefined || params.get(paramName) === value) {
        params.delete(paramName);
    } else {
        params.set(paramName, value);
    }

    router.push(`${pathname}?${params.toString()}`);
};

export const formatPhoneNumber = (phoneNumber: string) => {
    return phoneNumber.replace(/(\d{3})(\d{3})(\d{3})/, '$1 $2 $3');
};

export const calculateDeliveryPrice = (
    distanceInKm: number,
    perKmRate: number
) => {
    if (distanceInKm <= 3) {
        return 0;
    }
    const extraDistance = distanceInKm - 3;
    return Math.ceil(extraDistance) * perKmRate;
};

interface CopyToClipboardParams {
    text: string;
    notification?: string;
}

export const copyToClipboard = ({
    text,
    notification = 'Copied to clipboard!',
}: CopyToClipboardParams) => {
    navigator.clipboard.writeText(text);
    toast.success(notification);
};

export const getPaginationRange = (currentPage: number, totalPages: number) => {
    const pages: {
        label: string;
        index: number;
        disabled: boolean;
        isActive: boolean;
    }[] = [];
    const maxButtons = 5;
    const ellipsis = {
        label: '...',
        index: -1,
        disabled: true,
        isActive: false,
    };

    pages.push({
        label: '1',
        index: 0,
        disabled: false,
        isActive: currentPage === 0,
    });

    if (totalPages <= maxButtons) {
        for (let i = 1; i < totalPages; i++) {
            pages.push({
                label: String(i + 1),
                index: i,
                disabled: false,
                isActive: i === currentPage,
            });
        }

        return pages;
    }

    if (currentPage > 2) pages.push(ellipsis);

    const start = Math.max(1, currentPage - 1);
    const end = Math.min(totalPages - 2, currentPage + 1);

    for (let i = start; i <= end; i++) {
        pages.push({
            label: String(i + 1),
            index: i,
            disabled: false,
            isActive: i === currentPage,
        });
    }

    if (currentPage < totalPages - 3) {
        pages.push(ellipsis);
    }

    pages.push({
        label: String(totalPages),
        index: totalPages - 1,
        disabled: false,
        isActive: currentPage === totalPages - 1,
    });

    return pages;
};
