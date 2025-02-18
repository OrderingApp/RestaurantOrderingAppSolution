import z from 'zod';

import languagePacks, { type LanguageTypes } from '../constants/languagePacks';

export const getReservationSchema = (lang: LanguageTypes) => {
    const {
        createReservationPage: {
            form: {
                errors: {
                    personalData: {
                        min: minPersonalData,
                        max: maxPersonalData,
                    },
                    noOfPeople,
                    date,
                    time,
                    phone,
                },
            },
        },
    } = languagePacks[lang];

    return z.object({
        personalData: z
            .string()
            .min(3, minPersonalData)
            .max(30, maxPersonalData),
        noOfPeople: z
            .string()
            .refine(
                (val) =>
                    !Number.isNaN(parseInt(val, 10)) &&
                    +val > 0 &&
                    Number.isInteger(+val),
                noOfPeople
            ),
        date: z.string().date(date),
        time: z.string().nonempty(time),
        phone: z.string().refine((val) => /^\d{9}$/.test(val), {
            message: phone,
        }),
    });
};

export type ReservationSchema = z.infer<
    ReturnType<typeof getReservationSchema>
>;
