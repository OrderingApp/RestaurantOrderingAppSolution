'use client';

import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';

import { useLanguage } from '@/providers/LanguageProvider';

import Input from '@/components/shared/Input/Input';
import Button from '@/components/shared/button/Button';

import languagePacks from '@/helpers/constants/languagePacks';
import { getLoginSchema, type LoginSchema } from '@/helpers/models/loginForm';
import { User } from 'lucide-react';

const Login = () => {
    // TODO: REMOVE OR CHANGE ACCORIDNG TO BACKEND
    // mock zeby ten pin wpisac - i guess w local storage bedziemy trzymac czy ktos sie juz logowal w obrebie dzisiejszej dniowki

    const [isUserAssignedToThisDevice, setIsUserAssignedToThisDevice] =
        useState(true);
    const { language } = useLanguage();
    const loginSchema = getLoginSchema(language, isUserAssignedToThisDevice);
    const {
        loginPage: {
            heading: { defaultHeading, userHasBeenAssignedHeading },
            form: { login, password, pin, submit, pinLoginFallbackCta },
        },
    } = languagePacks[language];
    const [pinHelpText, switchToMainLoginText = ''] = pinLoginFallbackCta
        .split('||')
        .map((text) => text.trim());

    const {
        handleSubmit,
        reset,
        register,
        formState: { errors },
    } = useForm<LoginSchema>({
        resolver: zodResolver(loginSchema),
    });

    useEffect(() => {
        reset({
            login: '',
            password: '',
            pin: '',
        });
    }, [isUserAssignedToThisDevice, reset]);

    const handleFormSubmit = (data: LoginSchema) => {
        console.log(data);
    };

    return (
        <div className="mx-[218px] flex flex-col items-center m-auto p-12 bg-[#0F2027B2] rounded-[20px]">
            <header className="flex flex-col items-center gap-4">
                <div className="bg-[#284C5C80] aspect-square w-[179px] rounded-[75px] flex items-center justify-center shadow-[0px 4px 4px 0px #00000040]">
                    <User size={120} color="white" />
                </div>
                <h1 className="text-3xl uppercase text-white font-bold">
                    {isUserAssignedToThisDevice
                        ? userHasBeenAssignedHeading
                        : defaultHeading}
                </h1>
            </header>
            <main>
                <form
                    onSubmit={handleSubmit(handleFormSubmit)}
                    className="flex flex-col items-center gap-3 mt-8"
                >
                    {isUserAssignedToThisDevice ? (
                        <>
                            <Input
                                type="password"
                                id="pin"
                                label={pin}
                                {...register('pin')}
                                errors={errors.pin}
                                labelClassName={labelClasses}
                                inputClassName={inputClasses}
                            />
                            <p className="text-center text-white text-sm max-w-[420px]">
                                {pinHelpText}{' '}
                                <button
                                    type="button"
                                    className="underline underline-offset-4 font-bold"
                                    onClick={() =>
                                        // TODO: tu tez trzeba bd dorobic logike usuneicia z localstorage (prawdopodobnie) info o zalogowanym uzytkowniku
                                        setIsUserAssignedToThisDevice(false)
                                    }
                                >
                                    {switchToMainLoginText}
                                </button>
                            </p>
                        </>
                    ) : (
                        <>
                            <Input
                                type="text"
                                id="login"
                                label={login}
                                {...register('login')}
                                errors={errors.login}
                                labelClassName={labelClasses}
                                inputClassName={inputClasses}
                            />
                            <Input
                                type="password"
                                id="password"
                                label={password}
                                {...register('password')}
                                errors={errors.password}
                                labelClassName={labelClasses}
                                inputClassName={inputClasses}
                            />
                        </>
                    )}
                    <Button className="mt-8 uppercase font-bold">
                        {submit}
                    </Button>
                </form>
            </main>
        </div>
    );
};

const inputClasses =
    'w-full text-xl/7 text-white text-center font-bold bg-opacity-30';
const labelClasses = 'text-xl/9 text-center text-white uppercase';

export default Login;
