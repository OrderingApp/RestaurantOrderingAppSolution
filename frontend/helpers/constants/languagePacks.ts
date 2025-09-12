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
    detailsAside: {
        info: string;
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
        reservationsList: string;
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
    menuPage: {
        searchInputPlaceholder: string;
        menuCategoryCard: {
            itemsTitle: string[];
        };
        allCategories: string;
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
    ordersPage: {
        ordersActiveTitle: string;
        ordersClosedTitle: string;
        orderPickup: string;
        orderDelivery: string;
        createOrder: string;
        editOrder: string;
        payment: string;
        orderCard: {
            pickup: string;
            delivery: string;
        };
        asideTitle: string;
        orderCustomerInformationForm: {
            title: string;
            buttons: {
                takeway: string;
                delivery: string;
            };
            aside: {
                title: string;
                buttons: {
                    accept: string;
                    cancel: string;
                };
            };
            form: {
                fields: orderFormFields;
                errors: orderFormErrorFields;
            };
        };
    };
    paymentModal: {
        title: string;
        inputLabel: string;
        paymentByCard: string;
        paymentByCash: string;
    };
    paymentDetails: {
        bill: string;
        product: string;
        quantity: string;
        productPrice: string;
        total: string;
    };

    discountModal: {
        disscountTitle: string;
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

interface orderFormFields {
    name: string;
    time: string;
    phoneNumber: string;
    address: string;
}

interface orderFormErrorFields {
    name: {
        min: string;
        max: string;
    };
    time: string;
    phoneNumber: string;
    address: {
        min: string;
        max: string;
    };
}

export default languagePacks;
