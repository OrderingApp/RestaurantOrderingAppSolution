import { BACKEND_PATHS, BACKEND_URL } from '../constants/constants';

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
export const capitalizeFirstLetter = (str: string) =>
    str?.charAt(0).toUpperCase() + str?.slice(1);

export const fetchWithToken = (path: `${BACKEND_PATHS}`, params: string) =>
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

//TODO: make getPluralForm more readable (titles, pl if check) AND probably set 'lang' arg to typeof languages AND maybe use switch case here, but probably not needed now
