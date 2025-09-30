'use client';
import DetailsAside from '@/components/shared/asides/Details';
import Button, { ButtonProps } from '@/components/shared/button/Button';
import Input from '@/components/shared/Input/Input';
import { ICONS } from '@/helpers/constants/icons/icons';
import languagePacks from '@/helpers/constants/languagePacks';
import { OrderDto } from '@/helpers/interfaces/orders';
import { getOrderDeliverySchema } from '@/helpers/models/orderDeliveryForm';
import { getOrderTakewaySchema } from '@/helpers/models/orderTakewayForm';
import useOrderMutation from '@/helpers/queries/orders/useOrdersMutation';
import { useLanguage } from '@/providers/LanguageProvider';
import { zodResolver } from '@hookform/resolvers/zod';
import Image from 'next/image';
import { usePathname, useRouter } from 'next/navigation';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { BillProps } from './CreateOrder';
import { getFutureTime } from '@/helpers/utils/dates';
import DeliveryInput from './DeliveryInput';

const formDefaultValues = {
    name: '',
    date: '',
    time: '',
    phoneNumber: '',
    address: '',
};

interface FormData {
    name: string;
    time: string;
    phoneNumber: string;
    address?: string;
}

const CustomerInformationForm = ({
    bill,
    orderItems,
}: {
    bill: BillProps[];

    orderItems: { menuItemId: string }[];
}) => {
    const [isDelivery, setIsDelivery] = useState(false);
    const [isCustomDate, setIsCustomDate] = useState<string | null>(null);
    const { language } = useLanguage();
    const pathname = usePathname();
    const router = useRouter();
    const deliverySchema = getOrderDeliverySchema(language);
    const takewaySchema = getOrderTakewaySchema(language);

    const createTakewayOrderMutation = useOrderMutation('create', 'Takeaway');
    const createDeliveryOrderMutation = useOrderMutation('create', 'Delivery');

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
        setValue,
        formState: { errors },
    } = useForm({
        resolver: zodResolver(isDelivery ? deliverySchema : takewaySchema),
        defaultValues: formDefaultValues,
    });

    const timeButtonFn = (time: string, id: string) => {
        setValue('time', getFutureTime(parseInt(time)), {
            shouldDirty: true,
            shouldValidate: true,
        });
        if (isCustomDate === id) {
            setIsCustomDate(null);
            setValue('time', '');
        } else {
            setIsCustomDate(id);
        }
    };

    const submitFormHandler = (data: FormData) => {
        const today = new Date();
        const dateStr = today.toISOString().split('T')[0];
        const dateTimeStr = `${dateStr}T${data.time}:00`;

        const order: OrderDto = {
            createdAt: dateTimeStr,
            discount: 0,
            customerInformation: {
                phoneNumber: data.phoneNumber,
                orderCompletionType: 'Immediate',
                preferredPaymentMethod: 'Card',
                additionalInstructions: '',
                address: isDelivery ? data.address : '',
                expectedOrderCompletion: dateTimeStr,
            },
            orderItems: orderItems,
        };

        if (!isDelivery) {
            createTakewayOrderMutation.mutate({ data: order });
        } else {
            createDeliveryOrderMutation.mutate({ data: order });
        }
    };

    const buttons: ButtonProps[] = [
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

    const button = {
        children: 'Info',
        variant: 'tertiary' as const,
    };

    const timeBtns = [
        {
            id: '10',
            value: 'Jak Najszybciej',
        },
        {
            id: '15',
            value: '15min',
        },
        {
            id: '20',
            value: '20min',
        },
        {
            id: '30',
            value: '30min',
        },
        {
            id: '60',
            value: '60min',
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
                    className="flex flex-col gap-5 mt-6"
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
                        disabled={!!isCustomDate}
                        labelClassName={`${isCustomDate && 'text-[rgba(0,0,0,0.5)]'}`}
                        inputClassName={`w-full [&::-webkit-calendar-picker-indicator]:w-20 [&::-webkit-calendar-picker-indicator]:opacity-0 ${isCustomDate && 'opacity-90'} `}
                    />
                    <div className="flex gap-2">
                        {timeBtns.map((btn) => (
                            <Input
                                type="button"
                                id={btn.id}
                                key={btn.id}
                                value={btn.value}
                                inputClassName={`${isCustomDate === btn.id ? 'bg-primary text-white' : 'bg-white'} focus:outline-none focus:ring-0 w-auto text-sm  shadow-xl rounded-xl`}
                                onClick={(e) =>
                                    timeButtonFn(
                                        e.currentTarget.id as string,
                                        btn.id
                                    )
                                }
                            />
                        ))}
                    </div>
                    <Input
                        type="phone"
                        id="phoneNumber"
                        icon={<Image src={ICONS.PHONE} alt="phoneIcon" />}
                        label={phoneNumber}
                        {...register('phoneNumber')}
                        errors={errors.phoneNumber}
                        inputClassName="w-full"
                    />
                    <DeliveryInput
                        address={address}
                        isDelivery={isDelivery}
                        errors={errors}
                        register={register}
                    />
                </form>
            </div>
            <DetailsAside
                title={asideTitle}
                items={bill}
                price={3}
                currency="pln"
                buttons={buttons}
                button={button}
            />
        </div>
    );
};

export default CustomerInformationForm;
