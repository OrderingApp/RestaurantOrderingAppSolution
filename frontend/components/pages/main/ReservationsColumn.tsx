import { formatDate } from '@/helpers/utils/dates';
import { formatPhoneNumber } from '@/helpers/utils/utils';
import { Reservation } from '@/helpers/queries/reservations/useQueryReservations';

import Column from './Column';
import { DashboardCard, Badge } from './DashboardCard';

const ReservationsColumn = ({
    reservations,
    title,
    language,
    onCardClick,
}: {
    reservations: Reservation[];
    title: string;
    language: string;
    onCardClick: (id: string) => void;
}) => {
    const now = Date.now();

    return (
        <Column title={title} count={reservations.length}>
            {reservations.map((r) => {
                const isPast = new Date(r.scheduledFor).getTime() <= now;
                const borderColor = isPast ? 'bg-[#b20000]' : 'bg-[#2C5364]';
                const badgeColor = isPast ? 'bg-[#b20000]' : 'bg-[#2C5364]';
                const time = formatDate(new Date(r.scheduledFor), language).time;

                return (
                    <DashboardCard
                        key={r.id}
                        borderColor={borderColor}
                        onClick={() => onCardClick(r.id)}
                    >
                        <span className="flex flex-col gap-0.5 text-xs leading-tight">
                            <span className="font-bold text-sm">{r.name}</span>
                            {r.capacityNeeded && (
                                <span>{r.capacityNeeded} os.</span>
                            )}
                            {r.tableName && <span>{r.tableName}</span>}
                            <span>{formatPhoneNumber(r.phoneNumber)}</span>
                        </span>
                        <Badge label={time} className={badgeColor} />
                    </DashboardCard>
                );
            })}
        </Column>
    );
};

export default ReservationsColumn;
