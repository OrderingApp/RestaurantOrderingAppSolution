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
        addNewOrder: string;
        delivery: string;
        receipt: string;
        table: string;
        noOrders?: string;
        noTableChosen?: string;
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
        daySummary: string;
        settings: string;
    };
    reservationsPage: {
        reservationTitle: string;
        createReservation: string;
        reservationsList: string;
        searchPlaceholder: string;
        reservationCard: {
            totalGuests: string;
            phone: string;
            table: string;
        };
        errorMsg: string;
        messageNoReservations: string;
    };
    createReservationPage: {
        createReservation: string;
        editReservation: string;
        chooseReservation: string;
        toasts: {
            loading: {
                create: string;
                update: string;
                delete: string;
            };
            success: {
                create: string;
                update: string;
                delete: string;
            };
            error: {
                prefix: string;
                actionFailed: string;
                unexpected: string;
                missingDateTimeAndGuests: string;
            };
        };
        form: createReservationPageFormFields & {
            submit: string;
            edit: string;
            events: {
                saving: string;
                deleting: string;
            };
            errors: Omit<createReservationPageFormFields, 'name'> & {
                name: {
                    min: string;
                    max: string;
                };
            };
        };
        reservationsList: {
            table: string;
            time: string;
            guests: string;
            tableExpanded: {
                deleteBtn: string;
                messageNoReservations: string;
            };
        };
    };
    menuPage: {
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
            descriptions: {
                available: string;
                closed: string;
            };
            balanceName: string;
            receiptsCountName: string;
            reservationName: string;
            personsCountName: string;
            hourName: string;
            reservationInName: string;
        };
    };
    ordersPage: {
        orderPickup: string;
        orderDelivery: string;
        createOrder: string;
        editOrder: string;
        payment: string;
        orderCard: {
            pickup: string;
            delivery: string;
            price: string;
            phone: string;
            address: string;
        };
        asideTitle: string;
        noOrdersFoundFallback: string;
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
                    discount: string;
                };
            };
            timeBtns: {
                asap: string;
            };
            form: {
                fields: orderFormFields;
                errors: orderFormErrorFields;
            };
        };
        orderOptionsModal: {
            titleTakeway: string;
            titleDelivery: string;
            paymentDue: string;
            customerInformation: {
                time: string;
                phoneNumber: string;
                address: string;
                comment: string;
            };
            summary: {
                title: string;
                status: string;
                paid: string;
                unpaid: string;
            };
        };
    };
    createOrderPage: {
        asideTitle: string;
        asideButtons: {
            accept: string;
            close: string;
            discount: string;
        };
        confirmation?: string;
        error?: string;
        addOrder: string;
    };
    paymentModal: {
        title: string;
        inputLabel: string;
        paymentByCard: string;
        paymentByCash: string;
    };

    paymentDetails: {
        bill: string;
        productNumber: string;
        product: string;
        discount: string;
        quantity: string;
        productPrice: string;
        total: string;
    };

    discountModal: {
        disscountTitle: string;
    };
    alertDialog: {
        attentionTitle: string;
        attentionContent: string;
        closeBtn: string;
        confirmBtn: string;
    };
    delivertyMapModal: {
        deliveryTitle: string;
    };
    settingsPage: {
        settings: {
            printerSettings: string;
            editMenu: string;
            systemInfo: string;
            archive: string;
            users: string;
        };
    };

    pagination: {
        previousBtn: string;
        nextBtn: string;
    };
    daySummaryPage: {
        tabs: {
            allOrders: string;
            summary: string;
        };
        heading: string;
    };
    ordersTable: {
        actions: {
            label: string;
            copyId: {
                label: string;
                notification: string;
            };
            viewOrder: string;
            viewReceipt: string;
        };
        searchPlaceholder: string;
        noResults: string;
    };
    entities: {
        order: {
            descriptive: OrderEntity;
            type: {
                dinein: string;
                takeaway: string;
                delivery: string;
            };
            statuses: {
                ongoing: string;
                pendingPayment: string;
                cancelled: string;
                closed: string;
                paidAndReadyToPrepare: string;
            };
        };
        menuItem: {
            ingredient: string;
            allergens: string;
        };
    };
    generic: {
        action: string;
        options: {
            yes: string;
            no: string;
            previousPage: {
                descriptive: string;
            };
            nextPage: {
                descriptive: string;
            };
        };
        noResults: string;
        searchPlaceholder: string;
        errorMsg: string;
        close: string;
    };
}

export type TableCardDescriptions =
    languagePack['tablePage']['tableCard']['descriptions'];

export type TableDescriptionKey = keyof TableCardDescriptions;

interface OrderEntity {
    id: string;
    number: string;
    price: string;
    status: string;
    type: string;
    discount: string;
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
    comment: string;
    time: string;
    phoneNumber: string;
    address: string;
}

interface orderFormErrorFields {
    comment: {
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
