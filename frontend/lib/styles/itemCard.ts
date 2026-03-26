export enum variantTableMap {
    ACTIVE = 'table-active',
    IN_PROGGRESS = 'table-in-progress',
    PAYMENT = 'table-payment',
}

export const itemCardStyles = {
    variants: {
        reservation: {
            container: 'border-[#CD5700] border-2',
            appearance: 'bg-[#CD5700] text-white',
        },
        reservationOccupied: {
            container: 'border-[#2C5364] border-2',
            appearance: 'bg-[#2C5364] text-white',
        },
        [variantTableMap.ACTIVE]: {
            container: 'border-[#008080] border-2',
            appearance: 'bg-[#008080] text-white',
        },
        [variantTableMap.IN_PROGGRESS]: {
            container: 'border-[#CD5700] border-2',
            appearance: 'bg-[#CD5700] text-black',
        },
        [variantTableMap.PAYMENT]: {
            container: 'border-[#C70039] border-2',
            appearance: 'bg-[#C70039] text-white',
        },
        orderActive: {
            container: 'border-[#CD5700] border-2',
            appearance: 'bg-[#CD5700] text-white',
        },
        orderPayment: {
            container: 'border-[#C70039] border-2',
            appearance: 'bg-[#C70039] text-white',
        },
        orderCompleted: {
            container: 'border-[#008080] border-2',
            appearance: 'bg-[#008080] text-white',
        },
        orderClosed: {
            container: 'border-[#2C5364] border-2',
            appearance: 'bg-[#2C5364] text-white',
        },
    },
};
