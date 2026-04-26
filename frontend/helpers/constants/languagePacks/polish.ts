import { COMPANY_NAME } from '../constants';
import { type languagePack } from '@/helpers/constants/languagePacks';

const plPack: languagePack = {
    metadata: {
        title: `${COMPANY_NAME} | Aplikacja do zamówień`,
        description: `Aplikacja do zamówień stworzona dla ${COMPANY_NAME}.`,
    },
    detailsAside: {
        info: 'Informacje',
        addNewOrder: 'Dodaj nowy rachunek',
        delivery: 'Dostawa',
        receipt: 'rachunek nr',
        table: 'Stolik',
        noOrders: 'Brak zamówień',
        noTableChosen: 'Proszę wybrać stolik',
    },
    loginPage: {
        heading: {
            defaultHeading: 'Logowanie do systemu',
            userHasBeenAssignedHeading: 'Logowanie',
        },
        form: {
            login: 'login',
            submit: 'zaloguj się',
            password: 'hasło',
            pin: 'wpisz PIN',
            pinLoginFallbackCta:
                'Nie pamiętasz PIN-u lub to nie Ty dzisiaj się logowałeś? || Przejdź do głównej strony logowania',
            keycloakHint:
                'Kontynuuj przez Keycloak, aby się zalogować. Po poprawnym logowaniu przez Keycloak to urządzenie przejdzie w tryb PIN na 12 godzin.',
            keycloakSignInCta: 'Zaloguj przez Keycloak',
            pinSubmitting: 'Sprawdzanie PIN-u...',
            pinError: 'Nieprawidłowy PIN. Spróbuj ponownie.',
            sessionChecking: 'Sprawdzanie statusu logowania...',
            sessionCheckFailed:
                'Nie udało się zweryfikować sesji logowania. Spróbuj ponownie później.',
            sessionRetryCta: 'Ponów sprawdzenie sesji',
            missingEnvWarningPrefix: 'Brak wymaganych zmiennych env:',
            errors: {
                login: 'Login musi mieć co najmniej 3 znaki',
                password: 'Hasło musi mieć co najmniej 8 znaków',
                pin: 'PIN musi mieć co najmniej 4 cyfry',
            },
        },
    },
    menuBar: {
        main: 'główny',
        tables: 'sala',
        orders: 'zamówienia',
        menu: 'karta',
        reservations: 'rezerwacje',
        daySummary: 'podsumowanie dnia',
        settings: 'ustawienia',
    },
    mainPage: {
        reservations: 'Rezerwacje',
        orders: 'Zamówienia',
        deliveries: 'Dowozy',
        pickups: 'Odbiory',
        billCount: 'Ilość Rachunków',
        asap: 'Jak najszybciej',
    },
    reservationsPage: {
        reservationTitle: 'Sprawdź Rezerwacje',
        createReservation: 'Utwórz Rezerwacje',
        reservationsList: 'Lista rezerwacji',
        searchPlaceholder: 'Wyszukaj',
        reservationCard: {
            totalGuests: 'Liczba osób',
            phone: 'Nr telefonu',
            table: 'Stolik',
        },
        errorMsg:
            'Wystąpił błąd podczas pobierania rezerwacji. Proszę spróbować ponownie później.',
        messageNoReservations: 'Brak rezerwacji na ten dzień',
    },
    createReservationPage: {
        createReservation: 'Stwórz rezerwację',
        editReservation: 'Edytuj rezerwację',
        chooseReservation: 'Wybierz rezerwację',
        toasts: {
            loading: {
                create: 'Tworzenie rezerwacji...',
                update: 'Aktualizacja rezerwacji...',
                delete: 'Usuwanie rezerwacji...',
            },
            success: {
                create: 'Rezerwacja utworzona!',
                update: 'Rezerwacja zaktualizowana!',
                delete: 'Rezerwacja usunięta!',
            },
            error: {
                prefix: 'Błąd',
                actionFailed: 'Nie udało się wykonać akcji.',
                unexpected: 'Wystąpił nieoczekiwany błąd.',
                missingDateTimeAndGuests:
                    'Proszę uzupełnić datę, godzinę i liczbę osób',
            },
        },
        form: {
            name: 'Dane osobowe',
            capacityNeeded: 'Liczba osób',
            date: 'Data',
            time: 'Godzina',
            phoneNumber: 'Numer telefonu',
            submit: 'Zarezerwuj stolik',
            edit: 'Zatwierdź zmiany',
            events: {
                saving: 'Zapisywanie...',
                deleting: 'Usuwanie...',
            },
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
        reservationsList: {
            table: 'Stolik',
            time: 'Godziina',
            guests: 'Goście',
            tableExpanded: {
                deleteBtn: 'Usuń',
                messageNoReservations: 'Brak rezerwacji',
            },
        },
    },
    menuPage: {
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
            descriptions: {
                available: 'Dostępny',
                closed: 'Nieczynny',
            },
            balanceName: 'Saldo',
            receiptsCountName: 'Liczba rachunków',
            reservationName: 'Rezerwacja',
            personsCountName: 'Liczba osób',
            hourName: 'Godzina',
            reservationInName: 'Rezerwacja za',
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
            price: 'Cena',
            phone: 'Nr telefonu',
            address: 'Adres',
        },
        orderCustomerInformationForm: {
            title: 'Dane',
            buttons: {
                takeway: 'Odbiór',
                delivery: 'Dostawa',
            },
            toasts: {
                updateSuccess: 'Dane klienta zostały zaktualizowane.',
                updateError: 'Nie udało się zaktualizować danych klienta.',
            },
            aside: {
                title: 'Do zapłaty',
                buttons: {
                    accept: 'Zatwierdź',
                    cancel: 'Zamknij bez zmian',
                    discount: 'Dodaj zniżkę',
                },
            },
            timeBtns: {
                asap: 'Jak najszybciej',
            },
            form: {
                fields: {
                    comment: 'Dodaj Komentarz',
                    time: 'Godzina',
                    phoneNumber: 'Numer telefonu',
                    address: 'Adres',
                },
                errors: {
                    comment: {
                        max: 'Proszę podać maksymalnie 100 znaków',
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
        orderOptionsModal: {
            titleDelivery: 'Dowóz',
            titleTakeway: 'Odbiór',
            paymentDue: 'Cena',
            customerInformation: {
                time: 'Godzina',
                phoneNumber: 'Numer telefonu',
                address: 'Adres',
                comment: 'Komentarz',
            },
            summary: {
                title: 'Podsumowanie',
                status: 'Status',
                paid: 'Opłacone',
                unpaid: 'Nieopłacone',
                paidAmount: 'Opłacono',
                remainingAmount: 'Pozostało',
            },
            toasts: {
                deleteSuccess: 'Zamówienie zostało usunięte.',
                deleteError: 'Nie udało się usunąć zamówienia.',
                closeSuccess: 'Zamówienie zostało zamknięte.',
                closeError: 'Nie udało się zamknąć zamówienia.',
            },
        },
        asideTitle: 'Zamówienie',
        noOrdersFoundFallback: 'Nie znaleziono zamówień',
    },
    createOrderPage: {
        asideTitle: 'Zamówienie',
        asideButtons: {
            accept: 'Zatwierdź',
            close: 'Zamknij bez zmian',
            discount: 'Dodaj zniżkę',
        },
        extras: {
            title: 'Dodatki',
            emptyFallback: 'Brak dodatków',
        },
        confirmation: 'Dodano do rachunku',
        error: 'Nie udało się dodać do rachunku',
        addOrder: 'Dodaj',
    },
    paymentModal: {
        title: 'Wybierz Formę Płatności',
        inputLabel: 'Wpisz wartość',
        paymentByCard: 'Płatność Kartą/Blikiem',
        paymentByCash: 'Płatność Gotówką',
        customerAmountLabel: 'Klient',
        orderPriceLabel: 'Cena',
        changeDueLabel: 'Reszta',
        confirmCloseOrder: 'Zamknij Zamówienie',
        exitSummary: 'Wyjdź',
        alreadyPaidMsg: 'Zamówienie jest już w całości opłacone.',
        amountRangeError: 'Wpisz kwotę pomiędzy 0 a {max}.',
        toastPaymentCreated: 'Płatność zrealizowana',
        toastPaymentFailed: 'Nie udało się zrealizować płatności.',
    },
    paymentDetails: {
        bill: 'Rachunek',
        product: 'Produkt',
        quantity: 'Ilość',
        productPrice: 'Cena',
        discount: 'Zniżka',
        productNumber: 'Numer',
        total: 'Suma',
    },
    splitBillModal: {
        billTab: 'Rachunek nr',
        addBill: 'Dodaj rachunek',
        chooseBill: 'Wybierz Rachunek',
        closeNoChanges: 'Zamknij Bez Zmian',
        splitBill: 'Rozdziel Rachunek',
        toastSuccess: 'Rachunek został rozdzielony.',
        toastError: 'Nie udało się rozdzielić rachunku.',
        toastNoChanges: 'Przenieś pozycje do nowego rachunku aby rozdzielić.',
    },
    discountModal: {
        disscountTitle: 'Dodaj zniżkę',
    },
    alertDialog: {
        attentionTitle: 'Uwaga',
        attentionContent: 'Czy na pewno chcesz wykonać tę akcję?',
        closeBtn: 'Nie',
        confirmBtn: 'Tak',
    },
    deliveryMapModal: {
        deliveryTitle: 'Dostawa',
        inputEmptyError: 'Proszę wpisać adres dostawy',
        distanceExceedError:
            'Adres dostawy jest poza zasięgiem. Maksymalna odległość to 10 km.',
        mapLoadingError: 'Nie można załadować mapy. Spróbuj ponownie później.',
        noneAddressError: 'Nie znaleziono adresu dostawy.',
    },
    settingsPage: {
        settings: {
            printerSettings: 'Ustawienia Drukarki',
            editMenu: 'Edytuj Menu',
            systemInfo: 'Informacje o Systemie',
            archive: 'Archiwum',
            users: 'Użytkownicy',
        },
    },
    pagination: {
        previousBtn: 'Porzedni',
        nextBtn: 'Następny',
    },
    daySummaryPage: {
        tabs: {
            allOrders: 'Wszystkie Zamówienia',
            summary: 'Podsumowanie',
        },
        heading: 'Zamówienia',
        summary: {
            revenueTitle: 'Utarg',
            revenueByPayment: {
                card: 'Karta',
                cash: 'Gotówka',
                total: 'Suma',
            },
            productSales: 'Sprzedaż produktów',
            mostSoldProducts: 'Najczęściej sprzedawane produkty',
            mostSoldCountLabel: 'Ilość',
            totalOrdersLabel: 'Łączna liczba zamówień',
            ordersCountLabel: 'zamówień',
            orderTypeLabels: {
                dineIn: 'Na miejscu',
                takeawayDelivery: 'Wynos/Dowóz',
            },
            tableColumns: {
                name: 'Nazwa',
                qty: 'Ilość',
                price: 'Cena',
            },
        },
    },
    ordersTable: {
        actions: {
            label: 'Akcje Dodatkowe',
            copyId: {
                label: 'Kopiuj ID Zamówienia',
                notification: 'Skopiowanio ID zamówienia',
            },
            viewOrder: 'Wyświetl Zamówienie',
            viewReceipt: 'Wyświetl Paragon',
        },
        searchPlaceholder: 'Wyszukaj zamówienia...',
        noResults: 'Nie znaleziono zamówień',
    },
    entities: {
        order: {
            descriptive: {
                id: 'ID Zamówienia',
                number: 'Numer Zamówienia',
                price: 'Cena',
                status: 'Status',
                type: 'Typ Zamówienia',
                discount: 'Zniżka',
            },
            type: {
                dinein: 'Na miejscu',
                takeaway: 'Odbiór',
                delivery: 'Dowóz',
            },
            statuses: {
                ongoing: 'W trakcie',
                pendingPayment: 'Oczekuje na płatność',
                cancelled: 'Anulowane',
                closed: 'Zamknięte',
                completed: 'Opłacone',
            },
        },
        menuItem: {
            ingredient: 'Składnik',
            allergens: 'Alergeny',
        },
    },

    generic: {
        action: 'Akcja Dodatkowa',
        options: {
            yes: 'Tak',
            no: 'Nie',
            previousPage: { descriptive: 'Poprzednia strona' },
            nextPage: { descriptive: 'Następna strona' },
        },
        noResults: 'Brak wyników',
        searchPlaceholder: 'Wyszukaj...',
        errorMsg:
            'Wystąpił niespodziewany błąd. Spróbuj ponownie później lub skontaktuj się z administratorem.',
        close: 'Zamknij',
    },
};

export default plPack;
