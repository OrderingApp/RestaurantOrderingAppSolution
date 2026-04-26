import { z } from 'zod';
import languagePacks, { LanguageTypes } from '../constants/languagePacks';

const MIN_PIN_LENGTH = 4;

export const getLoginSchema = (
    lang: LanguageTypes,
    isUserAssignedToThisDevice: boolean
) => {
    const {
        loginPage: {
            form: {
                errors: { pin },
            },
        },
    } = languagePacks[lang];

    return z
        .object({
            login: z.string().optional(),
            password: z.string().optional(),
            pin: z.string().optional(),
        })
        .superRefine((data, ctx) => {
            if (isUserAssignedToThisDevice) {
                if (!data.pin || data.pin.trim().length < MIN_PIN_LENGTH) {
                    ctx.addIssue({
                        code: z.ZodIssueCode.custom,
                        path: ['pin'],
                        message: pin,
                    });
                }

                return;
            }
        });
};

export type LoginSchema = z.infer<ReturnType<typeof getLoginSchema>>;
