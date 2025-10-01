export enum variantTableMap {
    ACTIVE = 'table-active',
    IN_PROGGRESS = 'table-in-progress',
    PAYMENT = 'table-payment',
}

export const itemCardStyles = {
    variants: {
        reservation: 'bg-[#2C5364] text-white',
        [variantTableMap.ACTIVE]: 'bg-[#008080] text-white',
        [variantTableMap.IN_PROGGRESS]: 'bg-[#CD5700] text-black',
        [variantTableMap.PAYMENT]: 'bg-[#C70039] text-white',
        orderActive: 'bg-[#CD5700] text-white',
        orderCompleted: 'bg-[#008080] text-white',
    },
};
