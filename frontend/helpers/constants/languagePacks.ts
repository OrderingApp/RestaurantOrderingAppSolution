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
        upsertReservation: string;
        listOfReservations: string;
        reservationCard: {
            title: string;
            people: string[];
        };
    };
    createReservationPage: {
        createReservation: string;
        editReservation: string;
        chooseReservation: string;
        form: createReservationPageFormFields & {
            submit: string;
            edit: string;
            errors: Omit<createReservationPageFormFields, 'name'> & {
                name: {
                    min: string;
                    max: string;
                };
            };
        };
    };
    tablePage: {
        tableCard: {
            title: {
                payment: string;
                normal: string;
            };
            balanceName: string;
        };
    };
}

interface loginPageFormFields {
    login: string;
    password: string;
}
interface createReservationPageFormFields {
    name: string;
    capacityNeeded: string;
    date: string;
    time: string;
    phoneNumber: string;
}

export default languagePacks;
