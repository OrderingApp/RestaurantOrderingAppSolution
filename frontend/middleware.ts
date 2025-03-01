import { NextResponse, type NextRequest } from 'next/server';

import {
    defaultLanguage,
    supportedLanguages,
    type LanguageTypes,
} from '@/helpers/constants/languagePacks';
import { langKey } from './helpers/constants/keys';

export const middleware = (req: NextRequest) => {
    const res = NextResponse.next();
    let lang: LanguageTypes = req.cookies.get(langKey)?.value as LanguageTypes;

    if (!supportedLanguages.includes(lang)) {
        lang = defaultLanguage;

        req.cookies.set(langKey, lang);
        res.cookies.set(langKey, lang);
    }

    return res;
};
