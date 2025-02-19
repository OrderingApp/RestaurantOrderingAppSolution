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
    metadata: {
        title: string;
        description: string;
    };
    loginPage: {
        appName: string;
        form: loginPageFormFields & {
            submit: string;
            errors: loginPageFormFields;
        };
    };
    menuBar: {
        tables: string;
        orders: string;
        menu: string;
        reservations: string;
        endOfDay: string;
        settings: string;
    };
    reservationsPage: {
        reservationTitle: string;
        createOrEditReservation: string;
        listOfReservations: string;
    };
    createReservationPage: {
        createReservation: string;
        chooseReservation: string;
        form: createReservationPageFormFields & {
            submit: string;
            errors: Omit<createReservationPageFormFields, 'personalData'> & {
                personalData: {
                    min: string;
                    max: string;
                };
            };
        };
    };
}

interface loginPageFormFields {
    login: string;
    password: string;
}
interface createReservationPageFormFields {
    personalData: string;
    noOfPeople: string;
    date: string;
    time: string;
    phone: string;
}

export default languagePacks;
