'use client';

import { useState } from 'react';
import Image from 'next/image';

import Input from '@/components/shared/Input/Input';
import DeliveryMap from '@/components/shared/modals/DeliveryMap';

import { ICONS } from '@/helpers/constants/icons/icons';
import useDeliveryLocation from '@/helpers/hooks/useDeliveryLocation';
import { FieldErrors, UseFormRegister } from 'react-hook-form';

interface DeliveryInputProps {
    isDelivery: boolean;
    register: UseFormRegister<{
        date: string;
        time: string;
        phoneNumber: string;
        address: string;
        comment: string;
    }>;
    errors: FieldErrors<{
        date: string;
        time: string;
        phoneNumber: string;
        address: string;
        comment: string;
    }>;
    address: string;
}

const DeliveryInput = ({
    isDelivery,
    register,
    errors,
    address,
}: DeliveryInputProps) => {
    const [addres, setAddres] = useState<string>('');
    const [showMap, setShowMap] = useState<boolean>(false);

    const { handleRoute } = useDeliveryLocation({ addres });

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
                            alt="usersIcon"
                        />
                    }
                    iconClassName="w-4 h-4"
                    label={address}
                    {...register('address')}
                    onChange={(e) => setAddres(e.target.value)}
                    errors={errors.address}
                    labelClassName={`${!isDelivery && 'text-[rgba(0,0,0,0.5)]'}`}
                    inputClassName={`w-full ${!isDelivery && 'opacity-90'}`}
                    disabled={!isDelivery}
                />

                <button
                    disabled={!isDelivery}
                    className={`${errors.address ? 'self-center' : 'self-end'} bg-white shadow-xl p-2 rounded-lg ${!isDelivery && 'opacity-50 cursor-not-allowed'}`}
                    onClick={() => setShowMap(true)}
                >
                    <Image src={ICONS.MAP} alt="mapIcon" className="w-6 h-6" />
                </button>
            </div>
            {showMap && (
                <DeliveryMap
                    onClose={() => setShowMap(false)}
                    addres={addres}
                />
            )}
        </>
    );
};

export default DeliveryInput;
