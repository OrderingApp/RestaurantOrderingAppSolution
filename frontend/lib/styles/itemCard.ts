export interface VariantTableMap {
    active: 'table-active';
    inProgress: 'table-in-progress';
    payment: 'table-payment';
}

export const variantTableMap: VariantTableMap = {
    active: 'table-active',
    inProgress: 'table-in-progress',
    payment: 'table-payment',
};

export const itemCardStyles = {
    variants: {
        reservation: 'bg-[#2C5364] text-white',
        [variantTableMap.active]: 'bg-[#008080] text-white',
        [variantTableMap.inProgress]: 'bg-[#CD5700] text-black',
        [variantTableMap.payment]: 'bg-[#C70039] text-white',
        orderActive: 'bg-[#CD5700] text-black',
        orderCompleted: 'bg-[#008080] text-white',
    },
};
