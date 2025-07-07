export interface BasicStyles {
    variants: {
        primary: string;
        secondary: string;
        tertiary: string;
        success: string;
        danger: string;
    };
    sizes: {
        xs: string;
        sm: string;
        md: string;
        lg: string;
        xl: string;
    };
}

// eslint-disable-next-line
export type DistributiveOmitPartial<T, K extends keyof any> = T extends any
    ? Omit<T, K> & Partial<Pick<T, K & keyof T>>
    : never;
