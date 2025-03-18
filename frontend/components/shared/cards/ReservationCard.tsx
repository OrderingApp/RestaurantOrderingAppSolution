'use client';
import { useLanguage } from '@/providers/LanguageProvider';

import ItemCard, { ItemCardProps } from './ItemCard';

import { Reservation } from '@/helpers/queries/reservations/useQueryReservations';
import languagePacks from '@/helpers/constants/languagePacks';
import { getPluralForm } from '@/helpers/utils/utils';
import { formatDate } from '@/helpers/utils/dates';

interface ReservationCardProps
    extends Omit<ItemCardProps, 'variant' | 'children' | 'title'>,
        Partial<Reservation> {
    capacityNeeded: number;
    dateTime: string;
}

const ReservationCard = ({
    dateTime,
    name,
    capacityNeeded,
    onClick,
    className,
}: ReservationCardProps) => {
    const { language } = useLanguage();

    const {
        reservationsPage: {
            reservationCard: { title, people },
        },
    } = languagePacks[language];

    const peopleName = getPluralForm(capacityNeeded, people, language);

    return (
        <ItemCard
            title={title}
            subtitle={formatDate(new Date(dateTime), language).time}
            variant="reservation"
            onClick={onClick}
            className={className}
        >
            <p className="text-[11px] text-left font-bold">{name}</p>
            <p className="text-[11px] text-left font-bold">
                {capacityNeeded} {peopleName}
            </p>
        </ItemCard>
    );
};

export default ReservationCard;
