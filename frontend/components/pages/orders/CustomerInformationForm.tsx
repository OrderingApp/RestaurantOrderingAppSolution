'use client';

import { useState } from 'react';
import { usePathname, useRouter } from 'next/navigation';
import Image from 'next/image';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';

import DetailsAside from '@/components/shared/asides/Details';
import Button, { ButtonProps } from '@/components/shared/button/Button';
import Input from '@/components/shared/Input/Input';
import { BillProps } from './CreateOrder';
import DeliveryInput from './DeliveryInput';

import { getFutureTime } from '@/helpers/utils/dates';
import { ICONS } from '@/helpers/constants/icons/icons';
import languagePacks from '@/helpers/constants/languagePacks';
import { OrderDto } from '@/helpers/interfaces/orders';
import { getOrderDeliverySchema } from '@/helpers/models/orderDeliveryForm';
import { getOrderTakewaySchema } from '@/helpers/models/orderTakewayForm';
import useOrderMutation from '@/helpers/queries/orders/useOrdersMutation';
import { useLanguage } from '@/providers/LanguageProvider';

import { useOrdersContext } from '@/providers/OrdersContext';

const formDefaultValues = {
    date: '',
    time: '',
    phoneNumber: '',
    address: '',
    comment: '',
};

interface FormData {
    time: string;
    phoneNumber: string;
    address?: string;
    comment: string;
}

interface CustomerInformationFormProps {
    bill: BillProps[];
    orderItems: { menuItemId: string }[];
}

const CustomerInformationForm = ({
    bill,
    orderItems,
}: CustomerInformationFormProps) => {
    const [isDelivery, setIsDelivery] = useState<boolean>(false);
    const [isCustomDate, setIsCustomDate] = useState<string | null>(null);
    const pathname = usePathname();
    const router = useRouter();
    const { language } = useLanguage();
    const { deliveryPrice } = useOrdersContext();

    const {
        ordersPage: {
            orderCustomerInformationForm: {
                title,
                form: {
                    fields: { comment, time, phoneNumber, address },
                },
                buttons: { takeway, delivery },
                aside: {
                    title: asideTitle,
                    buttons: { accept, cancel },
                },
            },
        },
    } = languagePacks[language];

    const deliverySchema = getOrderDeliverySchema(language);
    const takewaySchema = getOrderTakewaySchema(language);

    const createTakewayOrderMutation = useOrderMutation('create', 'Takeaway');
    const createDeliveryOrderMutation = useOrderMutation('create', 'Delivery');

    const {
        handleSubmit,
        register,
        setValue,
        formState: { errors },
    } = useForm({
        resolver: zodResolver(isDelivery ? deliverySchema : takewaySchema),
        defaultValues: formDefaultValues,
    });

    const setInputValueHandler = (time: string, id: string) => {
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
                additionalInstructions: data.comment,
                address: isDelivery ? data.address : '',
                expectedOrderCompletion: dateTimeStr,
            },
            orderItems: orderItems,
            deliveryPrice: deliveryPrice ? deliveryPrice : 0,
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
                                    setInputValueHandler(
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

                    <div className="flex flex-col">
                        <label
                            htmlFor="comment"
                            className="font-bold text-sm ml-2 mb-1"
                        >
                            {comment}
                        </label>
                        <textarea
                            {...register('comment')}
                            id="comment"
                            className={`${errors.comment && 'bg-red-200'} h-40 rounded-xl bg-[#E6E6E6] text-[#2B5162] p-2 focus:outline-none focus:ring-2 focus:ring-blue-500`}
                        />
                        {errors && (
                            <p className="text-red-500 text-[10px] md:text-[12px] px-2">
                                {errors.comment?.message}
                            </p>
                        )}
                    </div>
                </form>
            </div>
            <DetailsAside
                title={asideTitle}
                items={bill}
                price={3}
                currency="pln"
                buttons={buttons}
                button={button}
                isDelivery={isDelivery}
                deliveryPrice={deliveryPrice}
            />
        </div>
    );
};

export default CustomerInformationForm;
