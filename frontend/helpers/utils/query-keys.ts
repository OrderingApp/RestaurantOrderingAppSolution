enum AREA_KEYS {
    All = 'all-areas',
}

const QUERY_KEYS = {
    Areas: { ...AREA_KEYS },
} as const;

export const { Areas } = QUERY_KEYS;
