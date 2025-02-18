import { companyName } from '../constants';
import { type languagePack } from '@/helpers/constants/languagePacks';

const plPack: languagePack = {
    metadata: {
        title: `${companyName} | Aplikacja do zamówień`,
        description: `Aplikacja do zamówień stworzona dla ${companyName}.`,
    },
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
    createReservationPage: {
        createReservation: 'Stwórz rezerwację',
        chooseReservation: 'Wybierz rezerwację',
        form: {
            personalData: 'Dane osobowe',
            noOfPeople: 'Liczba osób',
            date: 'Data',
            time: 'Godzina',
            phone: 'Numer telefonu',
            submit: 'Zarezerwuj stolik',
            errors: {
                personalData: {
                    min: 'Proszę podać co najmniej 3 znaki',
                    max: 'Proszę podać maksymalnie 30 znaków',
                },
                noOfPeople: 'Proszę podać poprawną liczbę osób',
                date: 'Proszę podać poprawną datę',
                time: 'Proszę podać poprawną godzinę',
                phone: 'Proszę podać poprawny 9-cyfrowy numer telefonu',
            },
        },
    },
};

export default plPack;
