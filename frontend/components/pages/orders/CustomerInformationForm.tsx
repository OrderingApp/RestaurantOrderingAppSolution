'use client';

import { useEffect, useState } from 'react';
import { usePathname, useRouter } from 'next/navigation';
import Image from 'next/image';
import { useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';

import DetailsAside from '@/components/shared/asides/Details';
import Button, { ButtonProps } from '@/components/shared/button/Button';
import Input from '@/components/shared/Input/Input';
import { BillProps } from '../../shared/modals/CreateOrder';
import DeliveryInput from './DeliveryInput';

import { getFutureTime } from '@/helpers/utils/dates';
import { ICONS } from '@/helpers/constants/icons/icons';
import languagePacks from '@/helpers/constants/languagePacks';
import { OrderDto } from '@/helpers/interfaces/orders';
import { getOrderDeliverySchema } from '@/helpers/models/orderDeliveryForm';
import { getOrderTakewaySchema } from '@/helpers/models/orderTakewayForm';
import useOrderMutation from '@/helpers/queries/orders/useOrdersMutation';
import { useLanguage } from '@/providers/LanguageProvider';
import MobileTimePicker from '@/components/shared/pickers/MobileTimePicker';
import useAddOrderItemsMutation from '@/helpers/queries/orders/useAddOrderItemsMutation';
import useUpdateCustomerInformationMutation from '@/helpers/queries/customers/useUpdateCustomerInformationMutation';
import { OrdersItems } from '@/helpers/utils/queryKeys';

import { useOrdersContext } from '@/providers/OrdersContext';
import dayjs, { type Dayjs } from 'dayjs';

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
    orderItems: OrderDto['orderItems'];
    editedOrderId?: string;
    editedOrderType?: 'dinein' | 'Takeaway' | 'Delivery';
    editedCustomerInformation?: {
        id?: string;
        phoneNumber: string;
        additionalInstructions?: string | null;
        address?: string | null;
        orderCompletionType?: 'Immediate' | 'Scheduled';
        expectedOrderCompletion?: string;
    };
}

const CustomerInformationForm = ({
    bill,
    orderItems,
    editedOrderId,
    editedOrderType,
    editedCustomerInformation,
}: CustomerInformationFormProps) => {
    const isEditMode = !!editedOrderId;
    const [isDelivery, setIsDelivery] = useState(
        editedOrderType === 'Delivery'
    );
    const [selectedQuickTime, setSelectedQuickTime] = useState<string | null>(
        null
    );
    const [timeValue, setTimeValue] = useState<Dayjs | null>(null);
    const pathname = usePathname();
    const router = useRouter();
    const { language } = useLanguage();
    const { deliveryPrice } = useOrdersContext();
    const queryClient = useQueryClient();

    const {
        generic: { errorMsg },
        ordersPage: {
            orderCustomerInformationForm: {
                title,
                form: {
                    fields: { comment, time, phoneNumber, address },
                },
                buttons: { takeway, delivery },
                toasts: { updateSuccess, updateError },
                aside: {
                    title: asideTitle,
                    buttons: { accept, cancel, discount },
                },
                timeBtns: { asap },
            },
        },
    } = languagePacks[language];

    const deliverySchema = getOrderDeliverySchema(language);
    const takewaySchema = getOrderTakewaySchema(language);

    const createTakewayOrderMutation = useOrderMutation('create', 'Takeaway', {
        redirectOnSettled: false,
        onSuccess: () => router.push('/orders'),
    });
    const createDeliveryOrderMutation = useOrderMutation('create', 'Delivery', {
        redirectOnSettled: false,
        onSuccess: () => router.push('/orders'),
    });

    const addOrderItemsMutation = useAddOrderItemsMutation();
    const updateCustomerInformationMutation =
        useUpdateCustomerInformationMutation();

    const {
        handleSubmit,
        register,
        setValue,
        formState: { errors, isSubmitting },
    } = useForm({
        resolver: zodResolver(isDelivery ? deliverySchema : takewaySchema),
        defaultValues: formDefaultValues,
    });

    const isOrderMutationPending =
        createTakewayOrderMutation.isPending ||
        createDeliveryOrderMutation.isPending ||
        updateCustomerInformationMutation.isPending ||
        addOrderItemsMutation.isPending;

    const isLoading = isSubmitting || isOrderMutationPending;

    const mutationError =
        updateCustomerInformationMutation.error ||
        addOrderItemsMutation.error ||
        (isDelivery
            ? createDeliveryOrderMutation.error
            : createTakewayOrderMutation.error);

    const mutationErrorMessage = !mutationError
        ? null
        : mutationError instanceof Error
          ? mutationError.message
          : errorMsg;

    const setInputValueHandler = (time: string, id: string) => {
        const futureTime = getFutureTime(parseInt(time, 10));

        if (selectedQuickTime === id) {
            setSelectedQuickTime(null);
            setValue('time', '', {
                shouldDirty: true,
                shouldValidate: true,
                shouldTouch: true,
            });
            setTimeValue(null);
        } else {
            setSelectedQuickTime(id);
            setValue('time', futureTime, {
                shouldDirty: true,
                shouldValidate: true,
                shouldTouch: true,
            });

            const [hours, minutes] = futureTime.split(':').map(Number);
            setTimeValue(dayjs().hour(hours).minute(minutes).second(0));
        }
    };

    // CHANGE orderCompletionType

    useEffect(() => {
        if (!isEditMode || !editedCustomerInformation) return;

        setValue('phoneNumber', editedCustomerInformation.phoneNumber || '', {
            shouldDirty: false,
            shouldValidate: false,
        });
        setValue('address', editedCustomerInformation.address || '', {
            shouldDirty: false,
            shouldValidate: false,
        });
        setValue(
            'comment',
            editedCustomerInformation.additionalInstructions || '',
            {
                shouldDirty: false,
                shouldValidate: false,
            }
        );

        const expectedCompletion =
            editedCustomerInformation.expectedOrderCompletion;
        if (expectedCompletion) {
            const parsed = dayjs(expectedCompletion);
            if (parsed.isValid()) {
                const hhmm = parsed.format('HH:mm');
                setValue('time', hhmm, {
                    shouldDirty: false,
                    shouldValidate: false,
                });
                setTimeValue(parsed);
            }
        }

        setSelectedQuickTime(null);
        setIsDelivery(editedOrderType === 'Delivery');
    }, [editedCustomerInformation, editedOrderType, isEditMode, setValue]);

    const submitFormHandler = async (data: FormData) => {
        try {
            const now = new Date();
            const dateStr = [
                now.getFullYear(),
                String(now.getMonth() + 1).padStart(2, '0'),
                String(now.getDate()).padStart(2, '0'),
            ].join('-');
            const dateTimeStr = `${dateStr}T${data.time}:00`;
            const orderCompletionType =
                selectedQuickTime === '10' ? 'Immediate' : 'Scheduled';

            const order: OrderDto = {
                discount: 0,
                customerInformation: {
                    phoneNumber: data.phoneNumber,
                    orderCompletionType,
                    additionalInstructions: data.comment || '',
                    address: isDelivery ? (data.address ?? '') : '',
                    expectedOrderCompletion: dateTimeStr,
                },
                orderItems: orderItems,
            };

            if (isEditMode && editedOrderId) {
                if (!editedCustomerInformation?.id) {
                    toast.error(updateError || errorMsg);
                    return;
                }

                if (orderItems.length > 0) {
                    await addOrderItemsMutation.mutateAsync({
                        orderId: editedOrderId,
                        orderItems,
                    });
                }

                await updateCustomerInformationMutation.mutateAsync({
                    customerInformationId: editedCustomerInformation.id,
                    data: {
                        phoneNumber: order.customerInformation.phoneNumber,
                        additionalInstructions:
                            order.customerInformation.additionalInstructions,
                        address: order.customerInformation.address,
                        orderCompletionType:
                            order.customerInformation.orderCompletionType,
                        expectedOrderCompletion:
                            order.customerInformation.expectedOrderCompletion,
                    },
                });

                await Promise.all([
                    queryClient.invalidateQueries({
                        queryKey: [OrdersItems.BY_ID, editedOrderId],
                    }),
                    queryClient.invalidateQueries({
                        queryKey: [OrdersItems.BY_TYPE],
                    }),
                ]);

                toast.success(updateSuccess);
                router.push('/orders');
                return;
            }

            if (!isDelivery) {
                await createTakewayOrderMutation.mutateAsync({ data: order });
            } else {
                await createDeliveryOrderMutation.mutateAsync({ data: order });
            }
        } catch (error) {
            // Hooks show domain-specific toasts for edit flow; keep a fallback for unexpected create-flow failures.
            if (!isEditMode) {
                toast.error(
                    error instanceof Error && error.message
                        ? error.message
                        : errorMsg
                );
            }
        }
    };

    const buttons: ButtonProps[] = [
        {
            children: discount,
            variant: 'primary',
        },
        {
            children: accept,
            variant: 'primary',
            disabled: isLoading,
            onClick: () => handleSubmit(submitFormHandler)(),
        },
        {
            children: cancel,
            disabled: isLoading,
            onClick: () => router.push(pathname),
            variant: 'tertiary',
        },
    ];

    const timeBtns = [
        {
            id: '10',
            value: asap,
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
                        disabled={isLoading || isEditMode}
                        onClick={() => setIsDelivery(false)}
                    >
                        {takeway}
                    </Button>
                    <Button
                        disabled={isLoading || isEditMode}
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
                    <MobileTimePicker
                        label={time}
                        value={timeValue}
                        minutesStep={5}
                        disabled={!!selectedQuickTime || isLoading}
                        dimLabel={!!selectedQuickTime}
                        errorText={errors.time?.message}
                        onChange={(newValue) => {
                            setTimeValue(newValue);
                            setSelectedQuickTime(null);
                            setValue(
                                'time',
                                newValue ? newValue.format('HH:mm') : '',
                                {
                                    shouldDirty: true,
                                    shouldValidate: true,
                                    shouldTouch: true,
                                }
                            );
                        }}
                    />
                    <div className="flex gap-2">
                        {timeBtns.map((btn) => (
                            <Input
                                type="button"
                                id={btn.id}
                                key={btn.id}
                                value={btn.value}
                                disabled={isLoading}
                                inputClassName={`${selectedQuickTime === btn.id ? 'bg-primary text-white' : 'bg-white'} focus:outline-none focus:ring-0 w-auto text-sm  shadow-xl rounded-xl`}
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
                        icon={<Image src={ICONS.PHONE} alt="phone" />}
                        label={phoneNumber}
                        {...register('phoneNumber')}
                        errors={errors.phoneNumber}
                        disabled={isLoading}
                        inputClassName="w-full"
                    />

                    <DeliveryInput
                        address={address}
                        isDelivery={isDelivery}
                        errors={errors}
                        register={register}
                        setValue={setValue}
                        isDisabled={isLoading}
                    />

                    {mutationErrorMessage && (
                        <p className="rounded-lg bg-red-100 px-3 py-2 text-sm text-red-700">
                            {mutationErrorMessage}
                        </p>
                    )}

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
                            disabled={isLoading}
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
                isDelivery={isDelivery}
                deliveryPrice={deliveryPrice}
            />
        </div>
    );
};

export default CustomerInformationForm;
