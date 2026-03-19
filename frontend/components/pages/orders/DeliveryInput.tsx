'use client';

import { useState } from 'react';
import Image from 'next/image';

import Input from '@/components/shared/Input/Input';
import DeliveryMap from '@/components/shared/modals/DeliveryMap';

import { ICONS } from '@/helpers/constants/icons/icons';
import useDeliveryLocation from '@/helpers/hooks/useDeliveryLocation';
import { FieldErrors, UseFormRegister, UseFormSetValue } from 'react-hook-form';

type FormValues = {
    date: string;
    time: string;
    phoneNumber: string;
    address: string;
    comment: string;
};

interface DeliveryInputProps {
    isDelivery: boolean;
    isDisabled?: boolean;
    register: UseFormRegister<FormValues>;
    setValue: UseFormSetValue<FormValues>;
    errors: FieldErrors<FormValues>;
    address: string;
}

const DeliveryInput = ({
    isDelivery,
    isDisabled = false,
    register,
    setValue,
    errors,
    address,
}: DeliveryInputProps) => {
    const [addressValue, setAddressValue] = useState('');
    const [isMapShown, setIsMapShown] = useState(false);
    const { ...addressRegister } = register('address');

    const { handleRoute } = useDeliveryLocation({ address: addressValue });

    const handleAddressChange = (nextAddress: string) => {
        setAddressValue(nextAddress);
        setValue('address', nextAddress, {
            shouldDirty: true,
            shouldValidate: true,
            shouldTouch: true,
        });
    };

    return (
        <>
            <div className="relative w-full flex gap-2 items-center">
                <Input
                    type="address"
                    id="address"
                    className="w-full"
                    icon={
                        <Image
                            onClick={() => handleRoute()}
                            src={ICONS.MAP_MARKER}
                            alt="users"
                        />
                    }
                    iconClassName="w-4 h-4"
                    label={address}
                    {...addressRegister}
                    onChange={(e) => handleAddressChange(e.target.value)}
                    errors={errors.address}
                    labelClassName={`${!isDelivery && 'text-[rgba(0,0,0,0.5)]'}`}
                    inputClassName={`w-full ${!isDelivery && 'opacity-90'}`}
                    disabled={!isDelivery || isDisabled}
                />

                <button
                    type="button"
                    disabled={!isDelivery || isDisabled}
                    className={`${errors.address ? 'self-center' : 'self-end'} bg-white shadow-xl p-2 rounded-lg ${(!isDelivery || isDisabled) && 'opacity-50 cursor-not-allowed'}`}
                    onClick={() => setIsMapShown(true)}
                >
                    <Image src={ICONS.MAP} alt="map" className="w-6 h-6" />
                </button>
            </div>
            {isMapShown && (
                <DeliveryMap
                    onClose={() => setIsMapShown(false)}
                    address={addressValue}
                    onAddressChange={handleAddressChange}
                />
            )}
        </>
    );
};

export default DeliveryInput;
