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
import { checkMaxAndMinDate, parseIsoDateAndTime } from '@/helpers/utils/dates';
import languagePacks from '@/helpers/constants/languagePacks';
import {
    getReservationSchema,
    type ReservationSchema,
} from '@/helpers/models/reservationForm';
import { ICONS } from '@/helpers/constants/icons/icons';
import { useReservationContext } from '@/providers/ReservationsContext';

const FORM_DEFAULT_VALUES = {
    name: '',
    capacityNeeded: '',
    date: '',
    time: '',
    phoneNumber: '',
};

//TO THINK: should we use custom date picker for better time

const ReservationForm = () => {
    const searchParams = useSearchParams();
    const reservationId = searchParams.get(SEARCH_PARAMS_NAMES.RESERVATION);
    const {
        updateForm,
        setHasUnsavedChanges,
        updateReservationFromDb,
        form: { date: contextDate, selectedTableId },
    } = useReservationContext();

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
                events: { saving, deleting },
            },
        },
    } = languagePacks[language];

    const {
        handleSubmit,
        register,
        reset,
        formState: { errors, isSubmitting, isDirty },
    } = useForm<ReservationSchema>({
        resolver: zodResolver(reservationSchema),
        defaultValues: {
            ...FORM_DEFAULT_VALUES,
            date: contextDate,
        },
    });

    const { data: reservation } = useQueryReservationsById(reservationId ?? '');
    const { mutate: createMutate, isPending: isCreating } =
        useReservationMutation('create');
    const { mutate: updateMutate, isPending: isUpdating } =
        useReservationMutation('update');
    const { mutate: deleteMutate, isPending: isDeleting } =
        useReservationMutation('delete');

    const inFlight = isCreating || isUpdating || isDeleting || isSubmitting;

    const submitFormHandler = async ({
        date,
        time,
        ...data
    }: ReservationSchema) => {
        const dateTimeStr = `${date}T${time}:00`;
        const newReservation = {
            ...data,
            scheduledFor: dateTimeStr,
            tableId: selectedTableId,
        };

        const onSuccessAction = () => {
            setHasUnsavedChanges(false);
            reset();
        };

        if (reservationId) {
            updateMutate(
                { data: newReservation, id: reservationId },
                { onSuccess: onSuccessAction }
            );
        } else {
            createMutate(
                { data: newReservation },
                { onSuccess: onSuccessAction }
            );
        }
    };

    const handleDelete = (e: React.MouseEvent) => {
        e.preventDefault();

        deleteMutate(
            { id: reservationId! },
            {
                onSuccess: () => {
                    setHasUnsavedChanges(false);
                    reset();
                },
            }
        );
    };

    useEffect(() => {
        if (reservationId && reservation) {
            updateReservationFromDb(reservation);

            const { date, time } = parseIsoDateAndTime(
                reservation.scheduledFor
            );

            reset({
                name: reservation.name,
                capacityNeeded: reservation.capacityNeeded.toString(),
                date,
                time,
                phoneNumber: reservation.phoneNumber,
            });
        } else {
            updateReservationFromDb(null);
        }
    }, [reservationId, reservation, reset]);

    useEffect(() => {
        setHasUnsavedChanges(isDirty);
    }, [isDirty, setHasUnsavedChanges]);

    return (
        <div className="bg-white pb-3 min-h-full relative">
            <form
                className="flex flex-col justify-start h-full"
                onSubmit={handleSubmit(submitFormHandler)}
            >
                <h1
                    className={`text-black text-xl font-bold capitalize ${!errors && 'py-4'}`}
                >
                    {reservationId ? editReservation : createReservation}
                </h1>
                <div className="h-full flex flex-col justify-between mt-2 ">
                    <div className="flex flex-col gap-1">
                        <Input
                            type="text"
                            id="name"
                            label={name}
                            icon={<Image src={ICONS.USER} alt="user" />}
                            {...register('name')}
                            errors={errors.name}
                            errorClassName="!text-[11px]"
                            inputClassName="w-full"
                            defaultValue={FORM_DEFAULT_VALUES.name}
                            disabled={inFlight}
                        />

                        <Input
                            type="number"
                            id="capacityNeeded"
                            label={capacityNeeded}
                            icon={<Image src={ICONS.USERS} alt="users" />}
                            {...register('capacityNeeded', {
                                onChange: (e) => {
                                    updateForm('peopleCount', e.target.value);
                                },
                            })}
                            min={1}
                            errors={errors.capacityNeeded}
                            errorClassName="!text-[11px]"
                            inputClassName="w-full hide-input-number-icon"
                            defaultValue={FORM_DEFAULT_VALUES.capacityNeeded}
                            disabled={inFlight}
                        />
                        <Input
                            type="date"
                            id="date"
                            icon={<Image src={ICONS.CALENDAR} alt="date" />}
                            label={date}
                            min={minDateString}
                            max={maxDateString}
                            defaultValue={FORM_DEFAULT_VALUES.date}
                            {...register('date', {
                                onChange: (e) => {
                                    updateForm('date', e.target.value);
                                },
                            })}
                            errors={errors.date}
                            errorClassName="!text-[11px]"
                            inputClassName="w-full [&::-webkit-calendar-picker-indicator]:w-20 [&::-webkit-calendar-picker-indicator]:opacity-0"
                            disabled={inFlight}
                        />
                        <Input
                            type="time"
                            id="time"
                            icon={<Image src={ICONS.TIME} alt="time" />}
                            label={time}
                            {...register('time', {
                                onChange: (e) => {
                                    updateForm('time', e.target.value);
                                },
                            })}
                            min={RESTAURANT_OPENING_HOUR}
                            max={RESTAURANT_CLOSING_HOUR}
                            errors={errors.time}
                            errorClassName="!text-[11px]"
                            inputClassName="w-full [&::-webkit-calendar-picker-indicator]:w-20 [&::-webkit-calendar-picker-indicator]:opacity-0"
                            defaultValue={FORM_DEFAULT_VALUES.time}
                            disabled={inFlight}
                        />
                        <Input
                            type="phoneNumber"
                            id="phoneNumber"
                            label={phoneNumber}
                            icon={<Image src={ICONS.PHONE} alt="phone" />}
                            {...register('phoneNumber')}
                            errors={errors.phoneNumber}
                            errorClassName="!text-[11px]"
                            inputClassName="w-full"
                            defaultValue={FORM_DEFAULT_VALUES.phoneNumber}
                            disabled={inFlight}
                        />
                    </div>
                    <div className="self-end flex justify-between w-full ">
                        {reservationId && (
                            <button
                                onClick={handleDelete}
                                className="bg-danger p-2 rounded-md"
                            >
                                {isDeleting ? (
                                    <span className="text-white text-xs px-2">
                                        {deleting}
                                    </span>
                                ) : (
                                    <Image src={ICONS.DELETE} alt="delete" />
                                )}
                            </button>
                        )}

                        <Button className="mt-2" size="xxs">
                            {isCreating || isUpdating
                                ? saving
                                : reservationId
                                  ? edit
                                  : submit}
                        </Button>
                    </div>
                </div>
            </form>
        </div>
    );
};

export default ReservationForm;
