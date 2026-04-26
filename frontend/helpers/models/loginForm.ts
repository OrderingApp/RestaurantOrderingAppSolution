import { z } from 'zod';
import languagePacks, { LanguageTypes } from '../constants/languagePacks';

const MIN_LOGIN_LENGTH = 3;
const MIN_PASSWORD_LENGTH = 8;
const MIN_PIN_LENGTH = 4;

export const getLoginSchema = (
    lang: LanguageTypes,
    isUserAssignedToThisDevice: boolean
) => {
    const {
        loginPage: {
            form: {
                errors: { login, password, pin },
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

            if (!data.login || data.login.trim().length < MIN_LOGIN_LENGTH) {
                ctx.addIssue({
                    code: z.ZodIssueCode.custom,
                    path: ['login'],
                    message: login,
                });
            }

            if (!data.password || data.password.length < MIN_PASSWORD_LENGTH) {
                ctx.addIssue({
                    code: z.ZodIssueCode.custom,
                    path: ['password'],
                    message: password,
                });
            }
        });
};

export type LoginSchema = z.infer<ReturnType<typeof getLoginSchema>>;
