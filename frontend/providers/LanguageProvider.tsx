'use client';

import { createContext, useContext, useState, type ReactNode } from 'react';

import { LanguageTypes } from '@/helpers/constants/languagePacks';
import { langKey } from '@/helpers/constants/keys';

type LanguageContextType = {
    language: LanguageTypes;
    setLanguage: (language: LanguageTypes) => void;
};

const LanguageContext = createContext<LanguageContextType | undefined>(
    undefined
);

export const useLanguage = () => {
    const ctx = useContext(LanguageContext);

    if (!ctx)
        throw new Error('useLanguage must be used within a LanguageProvider');

    return ctx;
};

const LanguageProvider = ({
    language,
    children,
}: {
    language: LanguageTypes;
    children: ReactNode;
}) => {
    const [lang, setLang] = useState<LanguageTypes>(language);

    const changeLang = (lang: LanguageTypes) => {
        document.cookie = `${langKey}=${lang}`;
        document.documentElement.lang = lang;

        setLang(lang);
    };

    return (
        <LanguageContext.Provider
            value={{ language: lang, setLanguage: changeLang }}
        >
            {children}
        </LanguageContext.Provider>
    );
};

export default LanguageProvider;
