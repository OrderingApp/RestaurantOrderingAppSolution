import z from 'zod';

import languagePacks, { type LanguageTypes } from '../constants/languagePacks';

export const getOrderTakewaySchema = (lang: LanguageTypes) => {
    const {
        ordersPage: {
            orderCustomerInformationForm: {
                form: {
                    errors: {
                        comment: { max: maxMame },
                        time,
                        phoneNumber,
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
        comment: z.string().max(100, maxMame).optional(),
    });
};

export type ReservationSchema = z.infer<
    ReturnType<typeof getOrderTakewaySchema>
>;
