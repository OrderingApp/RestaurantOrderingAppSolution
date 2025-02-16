import z from 'zod';

export const reservationSchema = z.object({
    name: z
        .string()
        .min(3, { message: 'Name must have min 3 characters' })
        .max(30, { message: 'Name must have max 30 characters' })
        .nonempty(),
    numberOfPeople: z
        .string()
        .refine((val) => !Number.isNaN(parseInt(val, 10)), {
            message: 'Plese enter a correct number of people',
        }),
    date: z.string({ message: 'Please enter a correct date' }).date(),
    time: z.string().nonempty({ message: 'Plese enter a correct time' }),
    phone: z.string().refine((val) => /^\d{9}$/.test(val), {
        message: 'Please enter a valid 9-digit phone number',
    }),
});

export type User = z.infer<typeof reservationSchema>;

// Change language
