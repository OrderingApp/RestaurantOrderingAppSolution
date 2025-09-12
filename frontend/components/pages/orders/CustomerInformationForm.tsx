'use client';
import DetailsAside from '@/components/shared/asides/Details';
import Button from '@/components/shared/button/Button';
import Input from '@/components/shared/Input/Input';
import { ICONS } from '@/helpers/constants/icons/icons';
import languagePacks from '@/helpers/constants/languagePacks';
import { getOrderDeliverySchema } from '@/helpers/models/orderDeliveryForm';
import { getOrderTakewaySchema } from '@/helpers/models/orderTakewayForm';
import { useLanguage } from '@/providers/LanguageProvider';
import { zodResolver } from '@hookform/resolvers/zod';
import Image from 'next/image';
import { usePathname, useRouter } from 'next/navigation';
import { useState } from 'react';
import { useForm } from 'react-hook-form';

const formDefaultValues = {
    name: '',
    date: '',
    time: '',
    phoneNumber: '',
    address: '',
};

const CustomerInformationForm = ({ bill }: { bill: unknown }) => {
    const [isDelivery, setIsDelivery] = useState(false);
    const { language } = useLanguage();
    const pathname = usePathname();
    const router = useRouter();
    const deliverySchema = getOrderDeliverySchema(language);
    const takewaySchema = getOrderTakewaySchema(language);

    const {
        ordersPage: {
            orderCustomerInformationForm: {
                title,
                form: {
                    fields: { name, time, phoneNumber, address },
                },
                buttons: { takeway, delivery },
                aside: {
                    title: asideTitle,
                    buttons: { accept, cancel },
                },
            },
        },
    } = languagePacks[language];

    const {
        handleSubmit,
        register,
        formState: { errors },
    } = useForm({
        resolver: zodResolver(isDelivery ? deliverySchema : takewaySchema),
        defaultValues: formDefaultValues,
    });

    const submitFormHandler = (data) => {
        console.log(data);
    };

    const buttons = [
        {
            children: 'Dodaj zniżkę',
            variant: 'primary',
        },
        {
            children: accept,
            variant: 'primary',
            onClick: () => handleSubmit(submitFormHandler)(),
        },
        {
            children: cancel,
            onClick: () => router.push(pathname),
            variant: 'tertiary',
        },
    ];

    return (
        <div className="bg-light-gray w-full rounded-3xl h-full flex flex-row ">
            <div className="px-3 py-8">
                <h2 className="text-[2.5rem] py-2 font-semibold">{title}</h2>
                <div className="flex gap-5 mt-5">
                    <Button
                        variant={isDelivery ? 'outline' : 'primary'}
                        onClick={() => setIsDelivery(false)}
                    >
                        {takeway}
                    </Button>
                    <Button
                        onClick={() => setIsDelivery(true)}
                        variant={isDelivery ? 'primary' : 'outline'}
                    >
                        {delivery}
                    </Button>
                </div>
                <form
                    onSubmit={handleSubmit(submitFormHandler)}
                    className="flex flex-col gap-5 mt-16"
                >
                    <Input
                        type="text"
                        id="name"
                        icon={<Image src={ICONS.USER} alt="userIcon" />}
                        label={name}
                        {...register('name')}
                        errors={errors.name}
                        inputClassName="w-full"
                    />
                    <Input
                        type="time"
                        id="time"
                        icon={<Image src={ICONS.TIME} alt="timeIcon" />}
                        label={time}
                        {...register('time')}
                        errors={errors.time}
                        inputClassName="w-full [&::-webkit-calendar-picker-indicator]:w-20 [&::-webkit-calendar-picker-indicator]:opacity-0"
                    />
                    <Input
                        type="phone"
                        id="phoneNumber"
                        icon={<Image src={ICONS.PHONE} alt="phoneIcon" />}
                        label={phoneNumber}
                        {...register('phoneNumber')}
                        errors={errors.phoneNumber}
                        inputClassName="w-full"
                    />
                    <Input
                        type="address"
                        id="address"
                        icon={<Image src={ICONS.MARKER} alt="usersIcon" />}
                        iconClassName="w-4 h-4"
                        label={address}
                        {...register('address')}
                        errors={errors.address}
                        labelClassName={`${!isDelivery && 'text-[rgba(0,0,0,0.5)]'}`}
                        inputClassName={`w-full ${!isDelivery && 'opacity-90'}`}
                        disabled={!isDelivery}
                    />
                </form>
            </div>
            <DetailsAside
                title={asideTitle}
                items={bill}
                price={3}
                currency="pln"
                buttons={buttons}
            />
        </div>
    );
};

export default CustomerInformationForm;
