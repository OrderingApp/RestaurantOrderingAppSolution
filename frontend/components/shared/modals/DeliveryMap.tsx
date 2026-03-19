import Image from 'next/image';
import { useEffect, useState, useRef } from 'react';

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
    address: string;
    onAddressChange: (nextAddress: string) => void;
}

const initial = {
    lat: 50.05598658820353,
    lng: 21.61245102578422,
};

const DISTANCE_UNIT = 'km';
const CURRENCY = 'zł';
const MAX_DELIVERY_DISTANCE_KM = 10;

const DeliveryMap = ({
    onClose,
    address,
    onAddressChange,
}: DeliveryMapProps) => {
    const [inputAddress, setInputAddress] = useState<string>(address);
    const [isQrCodeModalOpen, setIsQrCodeModalOpen] = useState<boolean>(false);
    const autocompleteRef = useRef<google.maps.places.Autocomplete | null>(
        null
    );

    const { Image: QrImage } = useQRCode();
    const { language } = useLanguage();
    const {
        deliveryMapModal: { deliveryTitle },
    } = languagePacks[language];

    const {
        destination,    
        directions,
        distance,
        deliveryPrice,
        isLoaded,
        handleRoute,
        qrCode,
        routeError,
    } = useDeliveryLocation({
        address: inputAddress,
    });

    useEffect(() => {
        if (!address) return;
        handleRoute();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [address]);

    useEffect(() => {
        if (!isLoaded || !window.google) return;

        const inputElement = document.getElementById(
            'autocomplete-address-input'
        ) as HTMLInputElement;

        if (!inputElement) return;

        const deliveryAreaCircle = new window.google.maps.Circle({
            center: initial,
            radius: MAX_DELIVERY_DISTANCE_KM * 1000,
        });
        const deliveryAreaBounds = deliveryAreaCircle.getBounds() ?? undefined;

        autocompleteRef.current = new window.google.maps.places.Autocomplete(
            inputElement,
            {
                bounds: deliveryAreaBounds,
                strictBounds: true,
                componentRestrictions: { country: 'pl' },
                fields: ['formatted_address', 'name'],
                types: ['address'],
            }
        );

        autocompleteRef.current.addListener('place_changed', () => {
            const place = autocompleteRef.current?.getPlace();

            if (place?.formatted_address) {
                setInputAddress(place.formatted_address);
                onAddressChange(place.formatted_address);
            } else if (place?.name) {
                setInputAddress(place.name);
                onAddressChange(place.name);
            }
        });

        return () => {
            if (autocompleteRef.current) {
                window.google.maps.event.clearInstanceListeners(
                    autocompleteRef.current
                );
            }
        };
    }, [isLoaded]);

    if (!isLoaded) return <Skeleton />;

    return (
        <Modal onClose={onClose}>
            <div className="relative rounded-lg overflow-hidden">
                <div className="flex absolute top-0 left-0 bg-primary h-16 w-full z-10 items-center px-3 gap-4 rounded-t-lg ">
                    <Input
                        type="text"
                        id="autocomplete-address-input"
                        value={inputAddress}
                        icon={
                            <Image
                                onClick={() => handleRoute()}
                                src={ICONS.MAP_MARKER}
                                alt="usersIcon"
                            />
                        }
                        iconClassName="top-[50%] -translate-y-1/2 cursor-pointer"
                        onChange={(e) => {
                            setInputAddress(e.target.value);
                            onAddressChange(e.target.value);
                        }}
                        onKeyDown={(e) => {
                            if (e.key === 'Enter') {
                                handleRoute();
                            }
                        }}
                        inputClassName={`w-[400px]`}
                        placeholder=""
                    />

                    <div className="bg-white w-32 py-[1px] rounded-md flex justify-between">
                        <div className="flex px-1">
                            <p className="text-xs self-end">
                                {deliveryPrice}
                                {CURRENCY}
                            </p>
                        </div>
                        <div className="flex flex-col items-center px-1">
                            <p className="text-sm font-bold">{deliveryTitle}</p>
                            <p className="text-xs self-end">
                                {distance}
                                {DISTANCE_UNIT}
                            </p>
                        </div>
                    </div>

                    {routeError && (
                        <p className="max-w-72 rounded-md bg-danger-light px-2 py-1 text-xs text-danger mr-2">
                            {routeError}
                        </p>
                    )}

                    {/* ADD QRCODE ICON */}
                    {qrCode && (
                        <button
                            type="button"
                            className="bg-white px-2 py-1 rounded text-xs font-bold"
                            onClick={() => setIsQrCodeModalOpen(true)}
                        >
                            CODE
                        </button>
                    )}

                    <button
                        type="button"
                        onClick={onClose}
                        className="absolute top-1/2 -translate-y-1/2 right-2"
                    >
                        <Image
                            src={ICONS.CLOSE_WHITE}
                            alt="closeIcon"
                            className="w-10 h-10"
                        />
                    </button>
                </div>

                <div className="w-[950px] h-[600px] mt-1 rounded-lg overflow-hidden">
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

            <Modal
                isOpen={isQrCodeModalOpen}
                onClose={() => setIsQrCodeModalOpen(false)}
            >
                <div className="flex flex-col items-center gap-4 bg-white p-4 rounded-lg z-40 relative">
                    <Image
                        className="absolute top-0 right-0 w-6 h-6 cursor-pointer"
                        alt="closeIcon"
                        src={ICONS.CLOSE}
                        onClick={() => setIsQrCodeModalOpen(false)}
                    />
                    <QrImage text={qrCode} />
                </div>
            </Modal>
        </Modal>
    );
};

export default DeliveryMap;
