/**
 * Converts a camelCase string to kebab-case.
 * @param str - The camelCase string to convert.
 * @returns The kebab-case version of the string.
 */
export const camelToKebab = (str: string): string => {
    return str.replace(/([a-z])([A-Z])/g, '$1-$2').toLowerCase();
};
