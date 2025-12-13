'use client';

import { useEffect } from 'react';
import Image from 'next/image';
import { useSearchParams } from 'next/navigation';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';

import Input from '../shared/Input/Input';
import Button from '../shared/button/Button';

import { useLanguage } from '@/providers/LanguageProvider';
import { useQueryReservationsById } from '@/helpers/queries/reservations/useQueryReservations';
import useReservationMutation from '@/helpers/queries/reservations/useMutationReservation';

import {
    RESTAURANT_CLOSING_HOUR,
    RESTAURANT_OPENING_HOUR,
    SEARCH_PARAMS_NAMES,
} from '@/helpers/constants/constants';
import { checkMaxAndMinDate } from '@/helpers/utils/dates';
import languagePacks from '@/helpers/constants/languagePacks';
import {
    getReservationSchema,
    type ReservationSchema,
} from '@/helpers/models/reservationForm';
import { ICONS } from '@/helpers/constants/icons/icons';

const formDefaultValues = {
    name: '',
    capacityNeeded: '',
    date: '',
    time: '',
    phoneNumber: '',
};

const ReservationForm = () => {
    const searchParams = useSearchParams();
    const editParam = searchParams.get(SEARCH_PARAMS_NAMES.RESERVATION);

    const { language } = useLanguage();
    const reservationSchema = getReservationSchema(language);

    const { maxDateString, minDateString } = checkMaxAndMinDate();
    const {
        createReservationPage: {
            createReservation,
            editReservation,
            form: {
                submit,
                edit,
                name,
                capacityNeeded,
                date,
                time,
                phoneNumber,
            },
        },
    } = languagePacks[language];

    const {
        handleSubmit,
        register,
        reset,
        formState: { errors },
    } = useForm<ReservationSchema>({
        resolver: zodResolver(reservationSchema),
        defaultValues: formDefaultValues,
    });

    const { data: reservation } = useQueryReservationsById(editParam ?? '');
    const createReservationMutation = useReservationMutation('create');
    const updateReservationMutation = useReservationMutation('update');
    const deleteReservationMutation = useReservationMutation('delete');

    const submitFormHandler = async ({
        date,
        time,
        ...data
    }: ReservationSchema) => {
        const dateTimeStr = `${date}T${time}:00`;
        const newReservation = {
            ...data,
            scheduledFor: dateTimeStr,
        };
        if (editParam) {
            updateReservationMutation.mutate({
                data: newReservation,
                id: editParam,
            });
        } else {
            createReservationMutation.mutate({ data: newReservation });
        }
        reset();
    };

    useEffect(() => {
        if (!editParam)
            return reset({
                name: '',
                date: minDateString,
                phoneNumber: '',
                time: '',
                capacityNeeded: '',
            });

        if (!reservation) return;

        reset({
            name: reservation.name,
            date: reservation.scheduledFor.split('T')[0],
            phoneNumber: reservation.phoneNumber,
            time: reservation.scheduledFor
                .split('T')[1]
                .split('.')[0]
                .slice(0, -3),
            capacityNeeded: reservation.capacityNeeded.toString(),
        });
    }, [editParam, reservation, minDateString, reset]);

    return (
        <div className="bg-white pb-3 min-h-full relative">
            <form
                className="flex flex-col justify-start h-full"
                onSubmit={handleSubmit(submitFormHandler)}
            >
                <h1
                    className={`text-black text-xl font-bold capitalize ${!errors && 'py-4'}`}
                >
                    {editParam ? editReservation : createReservation}
                </h1>
                <div className="h-full flex flex-col justify-between mt-2 ">
                    <div className="flex flex-col gap-1">
                        <Input
                            type="text"
                            id="name"
                            inputSize="xs"
                            label={name}
                            icon={<Image src={ICONS.USER} alt="userIcon" />}
                            {...register('name')}
                            errors={errors.name}
                            errorClassName="!text-[11px]"
                            inputClassName="w-full"
                            defaultValue={formDefaultValues.name}
                        />

                        <Input
                            type="number"
                            id="capacityNeeded"
                            label={capacityNeeded}
                            icon={<Image src={ICONS.USERS} alt="usersIcon" />}
                            {...register('capacityNeeded')}
                            min={1}
                            errors={errors.capacityNeeded}
                            errorClassName="!text-[11px]"
                            inputClassName="w-full hide-input-number-icon"
                            defaultValue={formDefaultValues.capacityNeeded}
                        />
                        <Input
                            type="date"
                            id="date"
                            icon={<Image src={ICONS.CALENDAR} alt="dateIcon" />}
                            label={date}
                            min={minDateString}
                            max={maxDateString}
                            defaultValue={formDefaultValues.date}
                            {...register('date')}
                            errors={errors.date}
                            errorClassName="!text-[11px]"
                            inputClassName="w-full [&::-webkit-calendar-picker-indicator]:w-20 [&::-webkit-calendar-picker-indicator]:opacity-0"
                        />
                        <Input
                            type="time"
                            id="time"
                            icon={<Image src={ICONS.TIME} alt="timeIcon" />}
                            label={time}
                            min={RESTAURANT_OPENING_HOUR}
                            max={RESTAURANT_CLOSING_HOUR}
                            {...register('time')}
                            errors={errors.time}
                            errorClassName="!text-[11px]"
                            inputClassName="w-full [&::-webkit-calendar-picker-indicator]:w-20 [&::-webkit-calendar-picker-indicator]:opacity-0"
                            defaultValue={formDefaultValues.time}
                        />
                        <Input
                            type="phoneNumber"
                            id="phoneNumber"
                            label={phoneNumber}
                            icon={<Image src={ICONS.PHONE} alt="phoneIcon" />}
                            {...register('phoneNumber')}
                            errors={errors.phoneNumber}
                            errorClassName="!text-[11px]"
                            inputClassName="w-full"
                            defaultValue={formDefaultValues.phoneNumber}
                        />
                    </div>
                    <div className="self-end flex justify-between w-full ">
                        {editParam && (
                            <button
                                onClick={() =>
                                    deleteReservationMutation.mutate({
                                        id: editParam,
                                    })
                                }
                                className="bg-danger p-2 rounded-md"
                            >
                                <Image src={ICONS.DELETE} alt="delete" />
                            </button>
                        )}

                        <Button className="mt-2" size="xxs">
                            {editParam ? edit : submit}
                        </Button>
                    </div>
                </div>
            </form>
        </div>
    );
};

export default ReservationForm;

//TODO loading states errors and push notification
