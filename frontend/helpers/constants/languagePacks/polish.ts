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
        orderPickup: 'Odbiór',
        orderDelivery: 'Dowóz',
        createOrder: 'Utwórz',
        editOrder: 'Edytuj',
        payment: 'Zamknij zamówienie',
        orderCard: {
            pickup: 'Odbiór',
            delivery: 'Dostawa',
        },
        orderCustomerInformationForm: {
            title: 'Dane',
            buttons: {
                takeway: 'Odbiór',
                delivery: 'Dostawa',
            },
            aside: {
                title: 'Do zapłaty',
                buttons: {
                    accept: 'Zatwierdź',
                    cancel: 'Zamknij bez zmian',
                },
            },
            form: {
                fields: {
                    name: 'Dane',
                    time: 'Godzina',
                    phoneNumber: 'Numer telefonu',
                    address: 'Adres',
                },
                errors: {
                    name: {
                        min: 'Proszę podać co najmniej 3 znaki',
                        max: 'Proszę podać maksymalnie 30 znaków',
                    },
                    time: 'Proszę podać poprawną godzinę',
                    phoneNumber:
                        'Proszę podać poprawny 9-cyfrowy numer telefonu',
                    address: {
                        min: 'Proszę podać adres o długości conajmniej 5 znaków',
                        max: 'Proszę podać adres o długości maksymalnie 100 znaków',
                    },
                },
            },
        },

        asideTitle: 'Zamówienie',
    },
    paymentModal: {
        title: 'Wybierz Formę Płatnośći',
        inputLabel: 'Wpisz wartość',
        paymentByCard: 'Płatność Kartą/Blikiem',
        paymentByCash: 'Płatność Gotówką',
    },
    paymentDetails: {
        bill: 'Rachunek',
        product: 'Produkt',
        quantity: 'Ilość',
        productPrice: 'Cena za produkt',
        total: 'Razem',
    },
    discountModal: {
        disscountTitle: 'Dodaj zniżkę',
    },
};

export default plPack;
