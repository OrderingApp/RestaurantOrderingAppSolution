import plPack from '@/helpers/constants/languagePacks/polish';
import enPack from './languagePacks/english';

export const defaultLanguage = 'pl';

const languagePacks = {
    [defaultLanguage]: plPack,
    en: enPack,
};

export type LanguageTypes = keyof typeof languagePacks;

export const supportedLanguages: LanguageTypes[] = Object.keys(
    languagePacks
) as LanguageTypes[];

export interface languagePack {
    loginPage: {
        appName: string;
        login: string;
        password: string;
        enter: string;
    };
    menuBar: {
        tables: string;
        orders: string;
        menu: string;
        reservations: string;
        endDay: string;
        settings: string;
    };
}

export default languagePacks;
