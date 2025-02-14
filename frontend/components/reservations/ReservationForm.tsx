'use client';
import { reservationSchema } from '@/utils/models/reservationForm';
import { zodResolver } from '@hookform/resolvers/zod';
import React from 'react';
import { useForm } from 'react-hook-form';
import Input from '../shared/Input/Input';
import { Button } from '../shared/Button/Button';

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
        formState: { errors, isSubmitting },
    } = useForm<FormValues>({
        resolver: zodResolver(reservationSchema),
    });

    const submitFormHandler = async (data: FormValues) => {
        console.log(data);
    };
    return (
        <div className="w-1/2 bg-white py-8 min-h-full relative">
            <h1 className="text-center text-black text-4xl font-bold py-5">
                Stwórz Rezerwacje
            </h1>
            <form
                className="py-2 px-8 flex flex-col justify-start gap-5 "
                onSubmit={handleSubmit(submitFormHandler)}
            >
                <Input
                    type="text"
                    id="name"
                    label="Dane"
                    props={{ ...register('name' as const) }}
                    errors={errors.name}
                    inputClassName="w-full"
                />

                <Input
                    type="number"
                    id="numberOfPeople"
                    label="Ilość Osób"
                    props={{ ...register('numberOfPeople' as const) }}
                    errors={errors.numberOfPeople}
                    inputClassName="w-full"
                />
                <Input
                    type="date"
                    id="date"
                    icon={
                        <svg
                            width="22"
                            height="22"
                            viewBox="0 0 22 22"
                            fill="none"
                            xmlns="http://www.w3.org/2000/svg"
                        >
                            <path
                                d="M17.309 2.33298H15.983V0.957976C15.9949 0.846402 15.9832 0.733575 15.9486 0.626825C15.9141 0.520076 15.8575 0.42179 15.7824 0.338353C15.7074 0.254916 15.6157 0.188193 15.5132 0.142519C15.4107 0.0968457 15.2997 0.0732422 15.1875 0.0732422C15.0753 0.0732422 14.9644 0.0968457 14.8619 0.142519C14.7594 0.188193 14.6677 0.254916 14.5927 0.338353C14.5176 0.42179 14.461 0.520076 14.4264 0.626825C14.3919 0.733575 14.3802 0.846402 14.392 0.957976V2.33298H7.49704V0.957976C7.50893 0.846402 7.49721 0.733575 7.46264 0.626825C7.42808 0.520076 7.37145 0.42179 7.29643 0.338353C7.22141 0.254916 7.12967 0.188193 7.02719 0.142519C6.9247 0.0968457 6.81375 0.0732422 6.70154 0.0732422C6.58934 0.0732422 6.47838 0.0968457 6.3759 0.142519C6.27341 0.188193 6.18167 0.254916 6.10665 0.338353C6.03163 0.42179 5.975 0.520076 5.94044 0.626825C5.90588 0.733575 5.89416 0.846402 5.90604 0.957976V2.33298H4.58004C3.50558 2.35426 2.4834 2.80079 1.73768 3.57463C0.991962 4.34847 0.583562 5.38646 0.602041 6.46098V17.468C0.583562 18.5425 0.991962 19.5805 1.73768 20.3543C2.4834 21.1282 3.50558 21.5747 4.58004 21.596H17.309C18.3835 21.5747 19.4057 21.1282 20.1514 20.3543C20.8971 19.5805 21.3055 18.5425 21.287 17.468V6.46098C21.3055 5.38646 20.8971 4.34847 20.1514 3.57463C19.4057 2.80079 18.3835 2.35426 17.309 2.33298ZM19.696 17.468C19.707 18.1127 19.4619 18.7355 19.0145 19.1998C18.567 19.6641 17.9537 19.9321 17.309 19.945H4.58004C3.93534 19.9321 3.32205 19.6641 2.8746 19.1998C2.42716 18.7355 2.18206 18.1127 2.19304 17.468V10.589H19.693L19.696 17.468ZM19.696 8.93698H2.19304V6.46098C2.18152 5.8159 2.42638 5.1926 2.87389 4.72785C3.32139 4.26309 3.93499 3.99485 4.58004 3.98198H5.90604V5.35998C5.89416 5.47155 5.90588 5.58438 5.94044 5.69113C5.975 5.79788 6.03163 5.89616 6.10665 5.9796C6.18167 6.06304 6.27341 6.12976 6.3759 6.17543C6.47838 6.22111 6.58934 6.24471 6.70154 6.24471C6.81375 6.24471 6.9247 6.22111 7.02719 6.17543C7.12967 6.12976 7.22141 6.06304 7.29643 5.9796C7.37145 5.89616 7.42808 5.79788 7.46264 5.69113C7.49721 5.58438 7.50893 5.47155 7.49704 5.35998V3.98198H14.392V5.35998C14.3802 5.47155 14.3919 5.58438 14.4264 5.69113C14.461 5.79788 14.5176 5.89616 14.5927 5.9796C14.6677 6.06304 14.7594 6.12976 14.8619 6.17543C14.9644 6.22111 15.0753 6.24471 15.1875 6.24471C15.2997 6.24471 15.4107 6.22111 15.5132 6.17543C15.6157 6.12976 15.7074 6.06304 15.7824 5.9796C15.8575 5.89616 15.9141 5.79788 15.9486 5.69113C15.9832 5.58438 15.9949 5.47155 15.983 5.35998V3.98198H17.309C17.9537 3.99485 18.567 4.26282 19.0145 4.72714C19.4619 5.19146 19.707 5.81424 19.696 6.45898V8.93698ZM16.252 18.019V16.919C16.2496 16.7758 16.3041 16.6376 16.4034 16.5345C16.5027 16.4314 16.6389 16.3719 16.782 16.369H17.843C17.9862 16.3719 18.1223 16.4314 18.2217 16.5345C18.321 16.6376 18.3755 16.7758 18.373 16.919V18.019C18.3755 18.1621 18.321 18.3004 18.2217 18.4035C18.1223 18.5066 17.9862 18.5661 17.843 18.569H16.778C16.6356 18.565 16.5005 18.5051 16.402 18.4021C16.3035 18.2991 16.2496 18.1614 16.252 18.019Z"
                                fill="#2B5162"
                            />
                        </svg>
                    }
                    label="Data"
                    props={{ ...register('date' as const) }}
                    errors={errors.date}
                    inputClassName="w-full custom-date"
                />
                <Input
                    type="time"
                    id="time"
                    icon={
                        <svg
                            width="24"
                            height="24"
                            viewBox="0 0 24 24"
                            fill="none"
                            xmlns="http://www.w3.org/2000/svg"
                        >
                            <path
                                d="M22.1021 12.0181C22.1021 14.0453 21.5009 16.0271 20.3746 17.7127C19.2483 19.3983 17.6475 20.712 15.7746 21.4878C13.9016 22.2636 11.8407 22.4666 9.85238 22.0711C7.86407 21.6756 6.0377 20.6994 4.60421 19.2659C3.17072 17.8324 2.1945 16.006 1.79901 14.0177C1.40351 12.0294 1.60649 9.9685 2.38229 8.09556C3.15809 6.22262 4.47186 4.62179 6.15746 3.4955C7.84306 2.36922 9.8248 1.76807 11.8521 1.76807C13.1981 1.76807 14.531 2.03319 15.7746 2.5483C17.0181 3.06341 18.1481 3.81842 19.0999 4.77022C20.0517 5.72202 20.8067 6.85197 21.3218 8.09556C21.8369 9.33915 22.1021 10.672 22.1021 12.0181Z"
                                stroke="#2B5162"
                                stroke-width="2"
                                stroke-linecap="round"
                                stroke-linejoin="round"
                            />
                            <path
                                d="M14.9271 16.1182L12.4521 13.6442C12.0679 13.2598 11.8521 12.7386 11.8521 12.1952V5.86816"
                                stroke="#2B5162"
                                stroke-width="2"
                                stroke-linecap="round"
                                stroke-linejoin="round"
                            />
                        </svg>
                    }
                    label="Godzina"
                    props={{ ...register('time' as const) }}
                    errors={errors.time}
                    inputClassName="w-full custom-date"
                />
                <Input
                    type="phone"
                    id="phone"
                    label="Nummer Telefonu"
                    props={{ ...register('phone' as const) }}
                    errors={errors.phone}
                    inputClassName="w-full"
                />

                <Button
                    className="relative bottom-[-60] left-0 w-full "
                    size="lg"
                >
                    Zarezerwuj Stolik
                </Button>
            </form>
        </div>
    );
};

export default ReservationForm;
