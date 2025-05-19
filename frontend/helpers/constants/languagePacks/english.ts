import { COMPANY_NAME } from '../constants';
import { type languagePack } from '@/helpers/constants/languagePacks';

const enPack: languagePack = {
    metadata: {
        title: `${COMPANY_NAME} | Ordering App`,
        description: `Ordering application created for ${COMPANY_NAME}.`,
    },
    detailsAside: { info: 'Information' },
    loginPage: {
        appName: 'order application',
        form: {
            login: 'login',
            password: 'password',
            submit: 'enter',
            errors: {
                login: 'Please enter at least 3 characters',
                password: 'Please enter at least 8 characters',
            },
        },
    },
    menuBar: {
        tables: 'tables',
        orders: 'orders',
        menu: 'menu',
        reservations: 'reservations',
        endOfDay: 'end of day',
        settings: 'settings',
    },
    reservationsPage: {
        reservationTitle: 'Check Reservations',
        upsertReservation: 'Create/Edit Reservation',
        reservationsList: 'List of reservations',
        reservationCard: {
            title: 'Reservation',
            people: ['person', 'persons'],
        },
    },
    createReservationPage: {
        createReservation: 'Create reservation',
        editReservation: 'Edit reservation',
        chooseReservation: 'Choose reservation',
        form: {
            name: 'Personal data',
            capacityNeeded: 'Number of people',
            date: 'Date',
            time: 'Time',
            phoneNumber: 'Phone number',
            submit: 'Book a table',
            edit: 'Confirm changes',
            errors: {
                name: {
                    min: 'Please enter at least 3 characters',
                    max: 'Please enter max 30 characters',
                },
                capacityNeeded: 'Plese enter a correct number of people',
                date: 'Please enter a correct date',
                time: 'Plese enter a correct time',
                phoneNumber: 'Please enter a valid 9-digit phone number',
            },
        },
    },
    menuPage: {
        searchInputPlaceholder: 'Search',
        menuCategoryCard: {
            itemsTitle: ['item', 'items'],
        },
        allCategories: 'All',
    },
    tablePage: {
        tableCard: {
            title: {
                payment: 'Table in payment process',
                normal: 'Active table',
            },
            balanceName: 'Balance',
        },
    },
    ordersPage: {
        ordersActiveTitle: 'Open',
        ordersClosedTitle: 'Closed',
        orderPickup: 'Pickup',
        orderDelivery: 'Delivery',
        createOrder: 'Create',
        editOrder: 'Edit',
        deleteOrder: 'Delete',
        payment: 'Payment',
        orderCard: {
            pickup: 'Pickup',
            delivery: 'Delivery',
        },
    },
};

export default enPack;
