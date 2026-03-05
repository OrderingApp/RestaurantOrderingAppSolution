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
    tableName: string | null;
}

const ReservationCard = ({
    scheduledFor,
    name,
    capacityNeeded,
    phoneNumber,
    tableName,
    onClick,
    className,
}: ReservationCardProps) => {
    const { language } = useLanguage();

    const {
        reservationsPage: {
            reservationCard: { totalGuests, phone, table },
        },
    } = languagePacks[language];

    const formatedPhoneNumber = formatPhoneNumber(phoneNumber);

    return (
        <ItemCard
            title={name}
            subtitle={formatDate(new Date(scheduledFor), language).time}
            variant={tableName ? 'reservationOccupied' : 'reservation'}
            onClick={onClick}
            className={className}
        >
            <p className="text-xs text-center">
                {totalGuests}:
                <span className="font-bold">{capacityNeeded}</span>
            </p>

            <p className="text-xs text-center">
                {phone}:<span className="font-bold">{formatedPhoneNumber}</span>
            </p>
            {tableName && (
                <p className="text-xs text-center">
                    {table}: <span className="font-bold">{tableName}</span>
                </p>
            )}
        </ItemCard>
    );
};

export default ReservationCard;
