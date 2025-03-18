import z from 'zod';

import languagePacks, { type LanguageTypes } from '../constants/languagePacks';

export const getReservationSchema = (lang: LanguageTypes) => {
    const {
        createReservationPage: {
            form: {
                errors: {
                    name: { min: minName, max: maxMame },
                    capacityNeeded,
                    date,
                    time,
                    phoneNumber,
                },
            },
        },
    } = languagePacks[lang];

    return z.object({
        name: z.string().min(3, minName).max(30, maxMame),
        capacityNeeded: z
            .string()
            .refine(
                (val) =>
                    !Number.isNaN(parseInt(val, 10)) &&
                    +val > 0 &&
                    Number.isInteger(+val),
                capacityNeeded
            ),
        date: z.string().date(date),
        time: z.string().nonempty(time),
        phoneNumber: z.string().refine((val) => /^\d{9}$/.test(val), {
            message: phoneNumber,
        }),
    });
};

export type ReservationSchema = z.infer<
    ReturnType<typeof getReservationSchema>
>;
