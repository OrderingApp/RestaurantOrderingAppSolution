'use client';
import { useLanguage } from '@/providers/LanguageProvider';

import ItemCard, { ItemCardProps } from './ItemCard';

import { Reservation } from '@/helpers/queries/reservations/useQueryReservations';
import languagePacks from '@/helpers/constants/languagePacks';

interface ReservationCardProps
    extends Omit<ItemCardProps, 'variant' | 'children' | 'title'>,
        Partial<Reservation> {}

export const ReservationCard = ({
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

    const checkNumberOfPeople = (amount: number) => {
        return amount === 1 ? people[0] : amount <= 4 ? people[1] : people[2];
    };

    const peopleName =
        language === 'en' ? people : checkNumberOfPeople(capacityNeeded!);

    return (
        <ItemCard
            title={title}
            subtitle={dateTime!.split('T')[1].split('.')[0].slice(0, -3)}
            variant="reservation"
            onClick={onClick}
            className={className}
        >
            <div>
                <p className="text-[11px] text-left font-bold">{name}</p>
                <p className="text-[11px] text-left font-bold">
                    {capacityNeeded} {peopleName}
                </p>
            </div>
        </ItemCard>
    );
};
