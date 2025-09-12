import z from 'zod';

import languagePacks, { type LanguageTypes } from '../constants/languagePacks';

export const getOrderTakewaySchema = (lang: LanguageTypes) => {
    const {
        ordersPage: {
            orderCustomerInformationForm: {
                form: {
                    errors: {
                        name: { min: minName, max: maxMame },
                        time,
                        phoneNumber,
                    },
                },
            },
        },
    } = languagePacks[lang];

    return z.object({
        name: z.string().min(3, minName).max(30, maxMame),
        time: z.string().nonempty(time),
        phoneNumber: z.string().refine((val) => /^\d{9}$/.test(val), {
            message: phoneNumber,
        }),
    });
};

export type ReservationSchema = z.infer<
    ReturnType<typeof getOrderTakewaySchema>
>;
