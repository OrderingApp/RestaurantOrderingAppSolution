export const VIEWS = {
    INFO: 'info',
    SUMMARY: 'summary',
} as const;

export type View = (typeof VIEWS)[keyof typeof VIEWS];

export type InformationInput = {
    label: string;
    value?: string | null;
    icon?: string;
    alt?: string;
};
