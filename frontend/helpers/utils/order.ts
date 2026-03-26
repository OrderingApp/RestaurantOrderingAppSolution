import { ORDER_STATUSES, ORDER_TYPES } from '../constants/constants';
import languagePacks, { LanguageTypes } from '../constants/languagePacks';

export const getOrderStatus = (
    status: ORDER_STATUSES,
    language: LanguageTypes
) => {
    const {
        entities: {
            order: {
                statuses: {
                    ongoing,
                    pendingPayment,
                    cancelled,
                    completed,
                    closed,
                },
            },
        },
    } = languagePacks[language];

    switch (status.toLowerCase()) {
        case ORDER_STATUSES.ONGOING.toLowerCase():
            return ongoing;

        case ORDER_STATUSES.COMPLETED.toLowerCase():
            return completed;

        case ORDER_STATUSES.PENDING_PAYMENT.toLowerCase():
            return pendingPayment;
        case ORDER_STATUSES.CLOSED.toLowerCase():
            return closed;

        case ORDER_STATUSES.CANCELLED.toLowerCase():
            return cancelled;

        default:
            return 'Unknown';
    }
};

export const getOrderType = (type: ORDER_TYPES, language: LanguageTypes) => {
    const {
        entities: {
            order: {
                type: { dinein, takeaway, delivery },
            },
        },
    } = languagePacks[language];

    switch (type.toLowerCase()) {
        case ORDER_TYPES.DINEIN.toLowerCase():
            return dinein;

        case ORDER_TYPES.TAKEAWAY.toLowerCase():
            return takeaway;

        case ORDER_TYPES.DELIVERY.toLowerCase():
            return delivery;

        default:
            return 'Unknown';
    }
};
