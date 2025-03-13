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
