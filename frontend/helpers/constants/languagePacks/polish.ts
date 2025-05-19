import { COMPANY_NAME } from '../constants';
import { type languagePack } from '@/helpers/constants/languagePacks';

const plPack: languagePack = {
    metadata: {
        title: `${COMPANY_NAME} | Aplikacja do zamówień`,
        description: `Aplikacja do zamówień stworzona dla ${COMPANY_NAME}.`,
    },
    detailsAside: { info: 'Informacje' },
    loginPage: {
        appName: 'aplikacja do zamówień',
        form: {
            login: 'login',
            submit: 'wejdź',
            password: 'hasło',
            errors: {
                login: 'Login musi mieć co najmniej 3 znaki',
                password: 'Hasło musi mieć co najmniej 8 znaków',
            },
        },
    },
    menuBar: {
        tables: 'stoliki',
        orders: 'zamówienia',
        menu: 'karta',
        reservations: 'rezerwacje',
        endOfDay: 'koniec dnia',
        settings: 'ustawienia',
    },
    reservationsPage: {
        reservationTitle: 'Sprawdź Rezerwacje',
        upsertReservation: 'Utwórz/Edytuj Rezerwacje',
        reservationsList: 'Lista rezerwacji',
        reservationCard: {
            title: 'Rezerwacja',
            people: ['osób', 'osoba', 'osoby', 'osób'],
        },
    },
    createReservationPage: {
        createReservation: 'Stwórz rezerwację',
        editReservation: 'Edytuj rezerwację',
        chooseReservation: 'Wybierz rezerwację',
        form: {
            name: 'Dane osobowe',
            capacityNeeded: 'Liczba osób',
            date: 'Data',
            time: 'Godzina',
            phoneNumber: 'Numer telefonu',
            submit: 'Zarezerwuj stolik',
            edit: 'Zatwierdź zmiany',
            errors: {
                name: {
                    min: 'Proszę podać co najmniej 3 znaki',
                    max: 'Proszę podać maksymalnie 30 znaków',
                },
                capacityNeeded: 'Proszę podać poprawną liczbę osób',
                date: 'Proszę podać poprawną datę',
                time: 'Proszę podać poprawną godzinę',
                phoneNumber: 'Proszę podać poprawny 9-cyfrowy numer telefonu',
            },
        },
    },
    menuPage: {
        searchInputPlaceholder: 'Wyszukaj',
        menuCategoryCard: {
            itemsTitle: ['pozycji', 'pozycja', 'pozycje', 'pozycji'],
        },
        allCategories: 'Wszystko',
    },
    tablePage: {
        tableCard: {
            title: {
                payment: 'Stolik w trakcie płatności',
                normal: 'Stolik aktywny',
            },
            balanceName: 'Saldo',
        },
    },
    ordersPage: {
        ordersActiveTitle: 'Otwarte',
        ordersClosedTitle: 'Zamknięte',
        orderPickup: 'Odbiór',
        orderDelivery: 'Dowóz',
        createOrder: 'Utwórz',
        editOrder: 'Edytuj',
        deleteOrder: 'Usuń',
        payment: 'Płatność',
        orderCard: {
            pickup: 'Odbiór',
            delivery: 'Dostawa',
        },
    },
};

export default plPack;
