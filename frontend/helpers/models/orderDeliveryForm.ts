import z from 'zod';

import languagePacks, { type LanguageTypes } from '../constants/languagePacks';

export const getOrderDeliverySchema = (lang: LanguageTypes) => {
    const {
        ordersPage: {
            orderCustomerInformationForm: {
                form: {
                    errors: {
                        comment: { min: minName, max: maxMame },
                        time,
                        phoneNumber,
                        address: { min: minAdress, max: maxAdress },
                    },
                },
            },
        },
    } = languagePacks[lang];

    return z.object({
        time: z.string().nonempty(time),
        phoneNumber: z.string().refine((val) => /^\d{9}$/.test(val), {
            message: phoneNumber,
        }),
        address: z.string().min(5, minAdress).max(100, maxAdress).optional(),
        comment: z.string().max(100, maxMame).optional(),
    });
};

export type ReservationSchema = z.infer<
    ReturnType<typeof getOrderDeliverySchema>
>;
