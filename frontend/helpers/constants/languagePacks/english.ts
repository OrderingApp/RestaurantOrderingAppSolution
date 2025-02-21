import { COMPANY_NAME } from '../constants';
import { type languagePack } from '@/helpers/constants/languagePacks';

const enPack: languagePack = {
    metadata: {
        title: `${COMPANY_NAME} | Ordering App`,
        description: `Ordering application created for ${COMPANY_NAME}.`,
    },
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
        listOfReservations: 'List of reservations',
    },
    createReservationPage: {
        createReservation: 'Create reservation',
        chooseReservation: 'Choose reservation',
        form: {
            personalData: 'Personal data',
            noOfPeople: 'Number of people',
            date: 'Date',
            time: 'Time',
            phone: 'Phone number',
            submit: 'Book a table',
            errors: {
                personalData: {
                    min: 'Please enter at least 3 characters',
                    max: 'Please enter max 30 characters',
                },
                noOfPeople: 'Plese enter a correct number of people',
                date: 'Please enter a correct date',
                time: 'Plese enter a correct time',
                phone: 'Please enter a valid 9-digit phone number',
            },
        },
    },
};

export default enPack;
