import clsx from 'clsx';
import ReservationCard, {
    type ReservationCardProps,
} from '../cards/ReservationCard';

export interface BottomAsideProps {
    reservations: ReservationCardProps[];
    className?: string;
}

const BottomAside = ({ reservations, className }: BottomAsideProps) => {
    return (
        <aside
            className={clsx('p-4 w-full shadow-inner-sm bg-white', className)}
        >
            {reservations.length > 0 ? (
                <ul className="flex gap-4 ">
                    {reservations.map((r) => (
                        <ReservationCard key={r.id} {...r} />
                    ))}
                </ul>
            ) : null}
        </aside>
    );
};

export default BottomAside;
