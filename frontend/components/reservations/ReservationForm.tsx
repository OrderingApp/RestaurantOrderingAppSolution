'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import Image from 'next/image';

import { useLanguage } from '@/providers/LanguageProvider';
import Input from '../shared/Input/Input';
import Button from '../shared/Button/Button';
import languagePacks from '@/helpers/constants/languagePacks';
import {
    getReservationSchema,
    type ReservationSchema,
} from '@/helpers/models/reservationForm';

import userSvg from '@/public/images/svg/user.svg';
import usersSvg from '@/public/images/svg/users.svg';
import calendarSvg from '@/public/images/svg/calendar.svg';
import timeSvg from '@/public/images/svg/time.svg';
import phoneSvg from '@/public/images/svg/phone.svg';
import {
    RESTAURANT_ClOSING_HOUR,
    RESTAURANT_OPENING_HOUR,
} from '@/helpers/constants/constants';

const ReservationForm = () => {
    const { language } = useLanguage();
    const reservationSchema = getReservationSchema(language);

    const {
        createReservationPage: {
            createReservation,
            form: { submit, personalData, noOfPeople, date, time, phone },
        },
    } = languagePacks[language];

    const {
        handleSubmit,
        register,
        formState: { errors },
    } = useForm<ReservationSchema>({
        resolver: zodResolver(reservationSchema),
    });

    const today = new Date();
    const minDateString = today.toISOString().split('T')[0];
    const threeMonthsLater = new Date();
    threeMonthsLater.setMonth(today.getMonth() + 3);
    const maxDateString = threeMonthsLater.toISOString().split('T')[0];

    const submitFormHandler = async (data: ReservationSchema) => {
        console.log(data);
    };

    return (
        <div className="w-1/2 bg-white py-8 min-h-full relative">
            <h1 className="text-center text-black text-4xl font-bold py-5">
                {createReservation}
            </h1>
            <form
                className="py-2 px-8 flex flex-col justify-start gap-5 "
                onSubmit={handleSubmit(submitFormHandler)}
            >
                <Input
                    type="text"
                    id="personalData"
                    label={personalData}
                    icon={<Image src={userSvg} alt="userIcon" />}
                    {...register('personalData')}
                    errors={errors.personalData}
                    inputClassName="w-full"
                />

                <Input
                    type="number"
                    id="noOfPeople"
                    label={noOfPeople}
                    icon={<Image src={usersSvg} alt="usersIcon" />}
                    {...register('noOfPeople')}
                    min={1}
                    errors={errors.noOfPeople}
                    inputClassName="w-full [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
                />
                <Input
                    type="date"
                    id="date"
                    icon={<Image src={calendarSvg} alt="dateIcon" />}
                    label={date}
                    min={minDateString}
                    max={maxDateString}
                    defaultValue={minDateString}
                    {...register('date')}
                    errors={errors.date}
                    inputClassName="w-full [&::-webkit-calendar-picker-indicator]:w-20 [&::-webkit-calendar-picker-indicator]:opacity-0 
"
                />
                <Input
                    type="time"
                    id="time"
                    icon={<Image src={timeSvg} alt="timeIcon" />}
                    label={time}
                    min={RESTAURANT_OPENING_HOUR}
                    max={RESTAURANT_ClOSING_HOUR}
                    {...register('time')}
                    errors={errors.time}
                    inputClassName="w-full [&::-webkit-calendar-picker-indicator]:w-20 [&::-webkit-calendar-picker-indicator]:opacity-0"
                />
                <Input
                    type="phone"
                    id="phone"
                    label={phone}
                    icon={<Image src={phoneSvg} alt="phoneIcon" />}
                    {...register('phone')}
                    errors={errors.phone}
                    inputClassName="w-full"
                />

                <Button
                    className="relative bottom-[-60] left-0 w-full"
                    size="lg"
                >
                    {submit}
                </Button>
            </form>
        </div>
    );
};

export default ReservationForm;
