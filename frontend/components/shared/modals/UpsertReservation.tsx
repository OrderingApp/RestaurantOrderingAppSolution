import Image from 'next/image';
import ReservationForm from '@/components/reservations/ReservationForm';
import { ICONS } from '@/helpers/constants/icons/icons';

const UpsertReservation = ({ onClose }: { onClose: () => void }) => {
    const MODAL_WIDTH = '445px';
    return (
        <div
            className="bg-reservation-gradient rounded-2xl relative h-[500px] "
            style={{ width: MODAL_WIDTH }}
        >
            <button onClick={onClose} className="absolute top-5 right-4 z-10">
                <Image className="w-6 h-6" src={ICONS.CLOSE} alt="close" />
            </button>
            <div className="bg-white mt-[0.4rem] rounded-2xl h-full px-4 py-2 flex flex-col">
                <ReservationForm />
            </div>
        </div>
    );
};

export default UpsertReservation;
