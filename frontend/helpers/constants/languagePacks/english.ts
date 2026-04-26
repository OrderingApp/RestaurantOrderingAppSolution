import { COMPANY_NAME } from '../constants';
import { type languagePack } from '@/helpers/constants/languagePacks';

const enPack: languagePack = {
    metadata: {
        title: `${COMPANY_NAME} | Ordering App`,
        description: `Ordering application created for ${COMPANY_NAME}.`,
    },
    detailsAside: {
        info: 'Information',
        addNewOrder: 'Add new order',
        delivery: 'Delivery',
        receipt: 'receipt no',
        table: 'Table',
        noOrders: 'No orders',
        noTableChosen: 'Please choose a table',
    },
    loginPage: {
        heading: {
            defaultHeading: 'Login to system',
            userHasBeenAssignedHeading: 'User login',
        },
        form: {
            login: 'login',
            password: 'password',
            pin: 'enter PIN',
            submit: 'enter',
            pinLoginFallbackCta:
                "Don't remember your PIN or wasn't it you logging in today? || Go to the main login page",
            errors: {
                login: 'Please enter at least 3 characters',
                password: 'Please enter at least 8 characters',
                pin: 'Please enter at least 4 digits',
            },
        },
    },
    menuBar: {
        main: 'main',
        tables: 'hall',
        orders: 'orders',
        menu: 'menu',
        reservations: 'reservations',
        daySummary: 'day summary',
        settings: 'settings',
    },
    mainPage: {
        reservations: 'Reservations',
        orders: 'Orders',
        deliveries: 'Deliveries',
        pickups: 'Pickups',
        billCount: 'Number of Bills',
        asap: 'ASAP',
    },
    reservationsPage: {
        reservationTitle: 'Check Reservations',
        createReservation: 'Create Reservation',
        reservationsList: 'List of reservations',
        searchPlaceholder: 'Search',
        reservationCard: {
            totalGuests: 'Number of people',
            phone: 'Phone number',
            table: 'Table',
        },
        errorMsg:
            'An error occurred while fetching reservations. Please try again later.',
        messageNoReservations: 'No reservations for this date',
    },
    createReservationPage: {
        createReservation: 'Create reservation',
        editReservation: 'Edit reservation',
        chooseReservation: 'Choose reservation',
        toasts: {
            loading: {
                create: 'Creating reservation...',
                update: 'Updating reservation...',
                delete: 'Deleting reservation...',
            },
            success: {
                create: 'Reservation created!',
                update: 'Reservation updated!',
                delete: 'Reservation deleted!',
            },
            error: {
                prefix: 'Error',
                actionFailed: 'Failed to complete the action.',
                unexpected: 'An unexpected error occurred.',
                missingDateTimeAndGuests:
                    'Please provide date, time and number of guests.',
            },
        },
        form: {
            name: 'Personal data',
            capacityNeeded: 'Number of people',
            date: 'Date',
            time: 'Time',
            phoneNumber: 'Phone number',
            submit: 'Book a table',
            edit: 'Confirm changes',
            events: {
                saving: 'Saving...',
                deleting: 'Deleting...',
            },
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
        reservationsList: {
            table: 'Table',
            time: 'Time',
            guests: 'Guests',
            tableExpanded: {
                deleteBtn: 'Delete',
                messageNoReservations: 'No reservations for this table',
            },
        },
    },
    menuPage: {
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
            descriptions: {
                available: 'Available',
                closed: 'Unavailable',
            },
            balanceName: 'Balance',
            receiptsCountName: 'Number of bills',
            reservationName: 'Reservation',
            personsCountName: 'Number of people',
            hourName: 'Time',
            reservationInName: 'Reservation in',
        },
    },
    ordersPage: {
        orderPickup: 'Pickup',
        orderDelivery: 'Delivery',
        createOrder: 'Create',
        editOrder: 'Edit',
        payment: 'Close Order',
        orderCard: {
            pickup: 'Pickup',
            delivery: 'Delivery',
            price: 'Price',
            phone: 'Phone number',
            address: 'Address',
        },
        orderCustomerInformationForm: {
            title: 'Data',
            buttons: {
                takeway: 'Pickup',
                delivery: 'Delivery',
            },
            toasts: {
                updateSuccess: 'Customer information updated.',
                updateError: 'Failed to update customer information.',
            },
            aside: {
                title: 'Order',
                buttons: {
                    accept: 'Accept',
                    cancel: 'Close without changes',
                    discount: 'Add Discount',
                },
            },
            timeBtns: {
                asap: 'Right now',
            },
            form: {
                fields: {
                    comment: 'Comment',
                    time: 'Time',
                    phoneNumber: 'Phone Number',
                    address: 'Address',
                },
                errors: {
                    comment: {
                        max: 'Please enter max 100 characters',
                    },
                    time: 'Plese enter a correct time',
                    phoneNumber: 'Please enter a valid 9-digit phone number',
                    address: {
                        min: 'Address must be at least 5 characters long',
                        max: 'Address can be up to 100 characters long',
                    },
                },
            },
        },
        orderOptionsModal: {
            titleDelivery: 'Delivery',
            titleTakeway: 'Pickup',
            paymentDue: 'Price',
            customerInformation: {
                time: 'Time',
                phoneNumber: 'Phone Number',
                address: 'Adress',
                comment: 'Comment',
            },
            summary: {
                title: 'Summary',
                status: 'Status',
                paid: 'Paid',
                unpaid: 'Unpaid',
                paidAmount: 'Paid amount',
                remainingAmount: 'Remaining',
            },
            toasts: {
                deleteSuccess: 'Order deleted.',
                deleteError: 'Failed to delete order.',
                closeSuccess: 'Order closed.',
                closeError: 'Failed to close order.',
            },
        },
        asideTitle: 'Order',
        noOrdersFoundFallback: 'No orders found',
    },
    createOrderPage: {
        asideTitle: 'Order',
        asideButtons: {
            accept: 'Confirm',
            close: 'Close',
            discount: 'Add discount',
        },
        extras: {
            title: 'Extras',
            emptyFallback: 'No extras',
        },
        confirmation: 'Added to bill',
        error: 'Failed to add to bill',
        addOrder: 'Add',
    },
    paymentModal: {
        title: 'Choose Payment Method',
        inputLabel: 'Enter Amount',
        paymentByCard: 'Pay by Card/BLIK',
        paymentByCash: 'Pay by Cash',
        customerAmountLabel: 'Customer',
        orderPriceLabel: 'Price',
        changeDueLabel: 'Change',
        confirmCloseOrder: 'Close Order',
        exitSummary: 'Exit',
        alreadyPaidMsg: 'Order is already fully paid.',
        amountRangeError: 'Enter an amount between 0 and {max}.',
        toastPaymentCreated: 'Payment successful.',
        toastPaymentFailed: 'Failed to process payment.',
    },
    paymentDetails: {
        bill: 'Bill',
        product: 'Product',
        quantity: 'Quantity',
        productPrice: 'Price',
        discount: 'Discount',
        productNumber: 'Number',
        total: 'Total',
    },
    splitBillModal: {
        billTab: 'Bill no.',
        addBill: 'Add bill',
        chooseBill: 'Choose Bill',
        closeNoChanges: 'Close Without Changes',
        splitBill: 'Split Bill',
        toastSuccess: 'Bill has been split.',
        toastError: 'Failed to split the bill.',
        toastNoChanges: 'Move items to a new bill to split.',
    },
    discountModal: {
        disscountTitle: 'Add discount',
    },
    alertDialog: {
        attentionTitle: 'Attention',
        attentionContent: 'Are you sure you want to perform this action?',
        closeBtn: 'No',
        confirmBtn: 'Yes',
    },
    deliveryMapModal: {
        deliveryTitle: 'Delivery',
        inputEmptyError: 'Please enter a delivery address',
        distanceExceedError:
            'Delivery address is out of range. Maximum distance is 10 km.',
        mapLoadingError: 'Failed to load the map. Please try again later.',
        noneAddressError: 'No delivery address found.',
    },
    settingsPage: {
        settings: {
            printerSettings: 'Printer Settings',
            editMenu: 'Edit Menu',
            systemInfo: 'System Info',
            archive: 'Archive',
            users: 'Users',
        },
    },
    pagination: {
        previousBtn: 'Previous',
        nextBtn: 'Next',
    },
    daySummaryPage: {
        tabs: {
            allOrders: 'All Orders',
            summary: 'Summary',
        },
        heading: 'Orders',
        summary: {
            revenueTitle: 'Revenue',
            revenueByPayment: {
                card: 'Card',
                cash: 'Cash',
                total: 'Total',
            },
            productSales: 'Product Sales',
            mostSoldProducts: 'Most Sold Products',
            mostSoldCountLabel: 'Count',
            totalOrdersLabel: 'Total Orders',
            ordersCountLabel: 'orders',
            orderTypeLabels: {
                dineIn: 'Dine in',
                takeawayDelivery: 'Takeaway/Delivery',
            },
            tableColumns: {
                name: 'Name',
                qty: 'Qty',
                price: 'Price',
            },
        },
    },
    ordersTable: {
        actions: {
            label: 'Actions',
            copyId: {
                label: 'Copy Order ID',
                notification: 'Order ID Copied',
            },
            viewOrder: 'View Order',
            viewReceipt: 'View Receipt',
        },
        searchPlaceholder: 'Search orders...',
        noResults: 'No orders found',
    },
    entities: {
        order: {
            descriptive: {
                id: 'Order ID',
                number: 'Order Number',
                price: 'Price',
                status: 'Status',
                type: 'Order Type',
                discount: 'Discount',
            },
            type: {
                dinein: 'Dine in',
                takeaway: 'Takeaway',
                delivery: 'Delivery',
            },
            statuses: {
                ongoing: 'Ongoing',
                pendingPayment: 'Pending Payment',
                cancelled: 'Cancelled',
                closed: 'Closed',
                completed: 'Paid',
            },
        },
        menuItem: {
            ingredient: 'Ingredients',
            allergens: 'Allergens',
        },
    },

    generic: {
        searchPlaceholder: 'Search...',
        action: 'Additional Actions',
        options: {
            yes: 'Yes',
            no: 'No',
            previousPage: {
                descriptive: 'Previous page',
            },
            nextPage: {
                descriptive: 'Next page',
            },
        },
        noResults: 'No results',
        errorMsg:
            'An unexpected error occured. Try again later or contact the administrator.',
        close: 'Close',
    },
};

export default enPack;
