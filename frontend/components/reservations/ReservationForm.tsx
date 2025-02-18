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

import dateSvg from '@/public/images/svg/date.svg';
import timeSvg from '@/public/images/svg/time.svg';

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
                    props={{ ...register('personalData') }}
                    errors={errors.personalData}
                    inputClassName="w-full"
                />

                <Input
                    type="number"
                    id="noOfPeople"
                    label={noOfPeople}
                    props={{ ...register('noOfPeople') }}
                    errors={errors.noOfPeople}
                    inputClassName="w-full"
                />
                <Input
                    type="date"
                    id="date"
                    icon={<Image src={dateSvg} alt="dateIcon" />}
                    label={date}
                    props={{ ...register('date') }}
                    errors={errors.date}
                    inputClassName="w-full [&::-webkit-calendar-picker-indicator]:w-20 [&::-webkit-calendar-picker-indicator]:opacity-0
"
                />
                <Input
                    type="time"
                    id="time"
                    icon={<Image src={timeSvg} alt="timeIcon" />}
                    label={time}
                    props={{ ...register('time') }}
                    errors={errors.time}
                    inputClassName="w-full [&::-webkit-calendar-picker-indicator]:w-20 [&::-webkit-calendar-picker-indicator]:opacity-0"
                />
                <Input
                    type="phone"
                    id="phone"
                    label={phone}
                    props={{ ...register('phone') }}
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

//TODO: make date without a year i guess or only accept current year + next, set curr as default, make time without pm/am, accept only actual time +x if current date, if next day etc accept all time restaurant is open?
