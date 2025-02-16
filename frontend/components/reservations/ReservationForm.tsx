'use client';

import { useForm } from 'react-hook-form';
import { reservationSchema } from '@/helpers/models/reservationForm';
import { zodResolver } from '@hookform/resolvers/zod';

import Input from '../shared/Input/Input';
import Button from '../shared/Button/Button';
import Image from 'next/image';

import dateSvg from '@/public/images/svg/date.svg';
import timeSvg from '@/public/images/svg/time.svg';

type FormValues = {
    name: string;
    numberOfPeople: number;
    date: Date;
    time: string;
    phone: number;
};

const ReservationForm = () => {
    const {
        handleSubmit,
        register,
        formState: { errors },
    } = useForm<FormValues>({
        resolver: zodResolver(reservationSchema),
    });

    const submitFormHandler = async (data: FormValues) => {
        console.log(data);
    };
    return (
        <div className="w-1/2 bg-white py-8 min-h-full relative">
            <h1 className="text-center text-black text-4xl font-bold py-5">
                Stwórz Rezerwacjee
            </h1>
            <form
                className="py-2 px-8 flex flex-col justify-start gap-5 "
                onSubmit={handleSubmit(submitFormHandler)}
            >
                <Input
                    type="text"
                    id="name"
                    label="Dane"
                    props={{ ...register('name') }}
                    errors={errors.name}
                    inputClassName="w-full"
                />

                <Input
                    type="number"
                    id="numberOfPeople"
                    label="Ilość Osób"
                    props={{ ...register('numberOfPeople') }}
                    errors={errors.numberOfPeople}
                    inputClassName="w-full"
                />
                <Input
                    type="date"
                    id="date"
                    icon={<Image src={dateSvg} alt="dateIcon" />}
                    label="Data"
                    props={{ ...register('date') }}
                    errors={errors.date}
                    inputClassName="w-full [&::-webkit-calendar-picker-indicator]:w-20 [&::-webkit-calendar-picker-indicator]:opacity-0 
"
                />
                <Input
                    type="time"
                    id="time"
                    icon={<Image src={timeSvg} alt="timeIcon" />}
                    label="Godzina"
                    props={{ ...register('time') }}
                    errors={errors.time}
                    inputClassName="w-full [&::-webkit-calendar-picker-indicator]:w-20 [&::-webkit-calendar-picker-indicator]:opacity-0"
                />
                <Input
                    type="phone"
                    id="phone"
                    label="Nummer Telefonu"
                    props={{ ...register('phone') }}
                    errors={errors.phone}
                    inputClassName="w-full"
                />

                <Button
                    className="relative bottom-[-60] left-0 w-full"
                    size="lg"
                >
                    Zarezerwuj Stolik
                </Button>
            </form>
        </div>
    );
};

export default ReservationForm;
