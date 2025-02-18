'use server';

import { cookies } from 'next/headers';

import { langKey } from '@/helpers/constants/keys';
import { type LanguageTypes } from '@/helpers/constants/languagePacks';

const readLangCookie = async () => {
    const cookieStore = await cookies();

    return cookieStore.get(langKey)!.value as LanguageTypes;
};

export default readLangCookie;
