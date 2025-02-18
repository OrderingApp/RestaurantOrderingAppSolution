import { z } from 'zod';
import languagePacks, { LanguageTypes } from '../constants/languagePacks';

export const getLoginSchema = (lang: LanguageTypes) => {
    const {
        loginPage: {
            form: {
                errors: { login, password },
            },
        },
    } = languagePacks[lang];

    return z.object({
        login: z.string().min(3, login),
        password: z.string().min(8, password),
    });
};

export type LoginSchema = z.infer<ReturnType<typeof getLoginSchema>>;
