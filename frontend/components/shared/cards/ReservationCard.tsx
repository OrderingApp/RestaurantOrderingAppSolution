'use client';
import { useLanguage } from '@/providers/LanguageProvider';

import ItemCard, { ItemCardProps } from './ItemCard';

import { Reservation } from '@/helpers/queries/reservations/useQueryReservations';
import languagePacks from '@/helpers/constants/languagePacks';
import { formatPhoneNumber } from '@/helpers/utils/utils';
import { formatDate } from '@/helpers/utils/dates';

export interface ReservationCardProps
    extends Omit<ItemCardProps, 'variant' | 'children' | 'title'>,
        Reservation {
    capacityNeeded: number;
    phoneNumber: string;
    scheduledFor: string;
}

const ReservationCard = ({
    scheduledFor,
    name,
    capacityNeeded,
    phoneNumber,
    onClick,
    className,
}: ReservationCardProps) => {
    const { language } = useLanguage();

    const {
        reservationsPage: {
            reservationCard: { totalGuests, phone },
        },
    } = languagePacks[language];

    const formatedPhoneNumber = formatPhoneNumber(phoneNumber);

    return (
        <ItemCard
            title={name}
            subtitle={formatDate(new Date(scheduledFor), language).time}
            variant="reservation"
            onClick={onClick}
            className={className}
        >
            <p className="text-xs text-center mb-1">
                {totalGuests}:
                <span className="font-bold">{capacityNeeded}</span>
            </p>
            <p className="text-xs text-center">
                {phone}:<span className="font-bold">{formatedPhoneNumber}</span>
            </p>
        </ItemCard>
    );
};

export default ReservationCard;
