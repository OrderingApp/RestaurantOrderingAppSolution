'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';

import { useLanguage } from '@/providers/LanguageProvider';

import Input from '@/components/shared/Input/Input';
import Button from '@/components/shared/button/Button';

import languagePacks from '@/helpers/constants/languagePacks';
import { COMPANY_NAME } from '@/helpers/constants/constants';
import { getLoginSchema, type LoginSchema } from '@/helpers/models/loginForm';

const Login = () => {
    const { language } = useLanguage();
    const loginSchema = getLoginSchema(language);
    const {
        loginPage: {
            appName,
            form: { login, password, submit },
        },
    } = languagePacks[language];

    const {
        handleSubmit,
        register,
        formState: { errors },
    } = useForm<LoginSchema>({
        resolver: zodResolver(loginSchema),
    });

    const handleFormSubmit = (data: LoginSchema) => {
        console.log(data);
    };

    return (
        <div className="pt-[3.375rem] mx-auto w-max text-white">
            <header className="flex flex-col items-center mb-[5.15625rem]">
                <h1
                    className={`mb-[1.0625rem] font-serif capitalize ${h1Size}`}
                >
                    {COMPANY_NAME}
                </h1>
                <h2 className="uppercase text-xl/9 font-bold">{appName}</h2>
                <div className="mx-[-3.5px] h-[3px] w-full bg-white"></div>
            </header>

            <main>
                <form
                    onSubmit={handleSubmit(handleFormSubmit)}
                    className="flex flex-col gap-[1.0625rem]"
                >
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

                    <Button
                        className="mt-[2.1875rem] self-center uppercase font-bold"
                        size="xl"
                        variant="outline"
                    >
                        {submit}
                    </Button>
                </form>
            </main>
        </div>
    );
};

const h1Size = 'text-[12.5rem]/[12.5rem]';
const inputClasses =
    'w-full text-xl/7 text-white text-center font-bold bg-opacity-30';
const labelClasses = 'text-xl/9 text-center text-white uppercase';

export default Login;
