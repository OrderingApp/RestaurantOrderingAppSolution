import Image from 'next/image';
import { useEffect, useState } from 'react';

import { DirectionsRenderer, GoogleMap, Marker } from '@react-google-maps/api';
import { useQRCode } from 'next-qrcode';
import { Skeleton } from '@/components/ui/skeleton';

import Modal from './Modal';
import Input from '../Input/Input';

import useDeliveryLocation from '@/helpers/hooks/useDeliveryLocation';
import { ICONS } from '@/helpers/constants/icons/icons';
import { useLanguage } from '@/providers/LanguageProvider';
import languagePacks from '@/helpers/constants/languagePacks';

interface DeliveryMapProps {
    onClose: () => void;
    addres: string;
}
const initial = {
    lat: 50.05598658820353,
    lng: 21.61245102578422,
};

const DeliveryMap = ({ onClose, addres }: DeliveryMapProps) => {
    const [inputAdress, setInputAddress] = useState<string>(addres);
    const [qrCodeModal, setQrCodeModal] = useState<boolean>(false);

    const { Image: QrImage } = useQRCode();

    const { language } = useLanguage();

    const {
        delivertyMapModal: { deliveryTitle },
    } = languagePacks[language];

    const {
        destination,
        directions,
        distance,
        deliveryPrice,
        isLoaded,
        handleRoute,
        qrCode,
    } = useDeliveryLocation({
        addres: inputAdress,
    });

    useEffect(() => {
        if (!addres) return;
        handleRoute();
    }, [addres, handleRoute]);

    if (!isLoaded) return <Skeleton />;

    return (
        <Modal onClose={onClose}>
            <div className="relative rounded-lg">
                <div className="flex absolute top-0 left-0 bg-primary h-16 w-full z-10 items-center px-3 gap-4 rounded-t-lg ">
                    <Input
                        type="address"
                        id="address"
                        defaultValue={addres}
                        icon={
                            <Image
                                onClick={() => handleRoute()}
                                src={ICONS.MAP_MARKER}
                                alt="usersIcon"
                            />
                        }
                        iconClassName="top-[50%] -translate-y-1/2 "
                        onChange={(e) => setInputAddress(e.target.value)}
                        inputClassName={`w-[400px]`}
                    />
                    <div className="bg-white w-32  py-[1px] rounded-md flex justify-between">
                        <div className="flex px-1">
                            <p className="text-[10px] self-end">
                                {deliveryPrice}zł
                            </p>
                        </div>
                        <div className="flex flex-col items-center px-1">
                            <p className="text-sm font-bold">{deliveryTitle}</p>
                            <p className="text-[10px] self-end">{distance}km</p>
                        </div>
                    </div>

                    {qrCode && (
                        <button onClick={() => setQrCodeModal(true)}>
                            CODE
                        </button>
                    )}

                    <button
                        onClick={onClose}
                        className="absolute top-1/2 -translate-y-1/2 right-2"
                    >
                        <Image
                            src={ICONS.CLOSE_WHITE}
                            alt="closeIcon"
                            className="w-10 h-10"
                            onClick={onClose}
                        />
                    </button>
                </div>
                <div className="w-[900px] h-[600px] mt-1 rounded-lg overflow-hidden">
                    <GoogleMap
                        center={initial}
                        options={{
                            fullscreenControl: false,
                            streetViewControl: false,
                        }}
                        zoom={13}
                        mapContainerStyle={{ width: '100%', height: '100%' }}
                    >
                        <Marker position={initial} />
                        {destination && <Marker position={destination} />}
                        {directions && (
                            <DirectionsRenderer directions={directions} />
                        )}
                    </GoogleMap>
                </div>
            </div>
            {qrCodeModal && (
                <Modal onClose={() => setQrCodeModal(false)}>
                    <div className="flex flex-col items-center gap-4 bg-white p-4 rounded-lg z-40 relative">
                        <Image
                            className="absolute top-0 right-0 w-6 h-6"
                            alt="closeIcon"
                            src={ICONS.CLOSE}
                            onClick={() => setQrCodeModal(false)}
                        />
                        <QrImage text={qrCode} />
                    </div>
                </Modal>
            )}
        </Modal>
    );
};

export default DeliveryMap;

//TODO - add qr code icon if we want qr code add error if delivery address is too far
