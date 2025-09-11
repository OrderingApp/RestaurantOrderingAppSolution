import Image from 'next/image';
import Button from '../button/Button';
import cornerIcon from '@/public/images/svg/corner-down-right.svg';

const OrderOptionsModal = ({ onClose }: { onClose: () => void }) => {
    return (
        <div
            className="absolute top-0 right-[232px] w-full h-full bg-[rgba(0,0,0,0.5)]"
            onClick={onClose}
        >
            <div className="bg-slate-300 w-[400px] h-[400px] absolute right-0 bottom-0 rounded-s-2xl flex justify-center items-end gap-3 p-6 ">
                <Image
                    onClick={onClose}
                    className="absolute left-2 top-2"
                    src={cornerIcon}
                    alt="corner icon"
                />
                <Button variant="primary">Edytuj</Button>
                <Button variant="danger">Usuń</Button>
            </div>
        </div>
    );
};

export default OrderOptionsModal;
