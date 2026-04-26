'use client';

import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';

import { useLanguage } from '@/providers/LanguageProvider';

import Input from '@/components/shared/Input/Input';
import Button from '@/components/shared/button/Button';

import languagePacks from '@/helpers/constants/languagePacks';
import { getLoginSchema, type LoginSchema } from '@/helpers/models/loginForm';
import {
    AuthClientError,
    completeKeycloakLogin,
    fetchBackendSession,
    hasKeycloakAuthParams,
    clearBackendSession,
    getMissingAuthEnvVars,
    loginWithPin,
    startKeycloakLogin,
} from '@/helpers/utils/keycloakAuth';
import { User } from 'lucide-react';

const Login = () => {
    const [isCheckingKeycloakState, setIsCheckingKeycloakState] =
        useState(true);
    const [isRedirectingToKeycloak, setIsRedirectingToKeycloak] =
        useState(false);
    const [isPinSubmitting, setIsPinSubmitting] = useState(false);
    const [pinErrorMessage, setPinErrorMessage] = useState<string | null>(null);
    const [sessionErrorMessage, setSessionErrorMessage] = useState<
        string | null
    >(null);
    const [isUserAssignedToThisDevice, setIsUserAssignedToThisDevice] =
        useState(false);
    const [sessionCheckTick, setSessionCheckTick] = useState(0);
    const isDevelopment = process.env.NODE_ENV !== 'production';
    const missingAuthEnvVars = getMissingAuthEnvVars();
    const { language } = useLanguage();
    const loginSchema = getLoginSchema(language, isUserAssignedToThisDevice);
    const {
        loginPage: {
            heading: { defaultHeading, userHasBeenAssignedHeading },
            form: {
                pin,
                submit,
                pinLoginFallbackCta,
                keycloakHint,
                keycloakSignInCta,
                pinSubmitting,
                pinError,
                sessionChecking,
                sessionCheckFailed,
                sessionRetryCta,
                missingEnvWarningPrefix,
            },
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

    useEffect(() => {
        let isActive = true;

        const checkExistingLoginState = async () => {
            setSessionErrorMessage(null);

            try {
                if (hasKeycloakAuthParams()) {
                    const completedSession = await completeKeycloakLogin();

                    if (!isActive) {
                        return;
                    }

                    setIsUserAssignedToThisDevice(
                        completedSession?.isDeviceAssigned ?? false
                    );
                    return;
                }

                const session = await fetchBackendSession();

                if (!isActive) {
                    return;
                }

                setIsUserAssignedToThisDevice(
                    session?.isDeviceAssigned ?? false
                );
            } catch (error) {
                if (isActive) {
                    setIsUserAssignedToThisDevice(false);
                    setSessionErrorMessage(sessionCheckFailed);
                }

                console.error('Keycloak login state check failed', error);
            } finally {
                if (isActive) {
                    setIsCheckingKeycloakState(false);
                }
            }
        };

        checkExistingLoginState();

        return () => {
            isActive = false;
        };
    }, [sessionCheckTick, sessionCheckFailed]);

    const handleFormSubmit = async (data: LoginSchema) => {
        if (isUserAssignedToThisDevice) {
            const pinValue = data.pin?.trim() ?? '';
            setIsPinSubmitting(true);
            setPinErrorMessage(null);

            try {
                const session = await loginWithPin(pinValue);

                if (session?.isDeviceAssigned) {
                    setIsUserAssignedToThisDevice(true);
                    return;
                }

                setPinErrorMessage(pinError);
                setIsUserAssignedToThisDevice(false);
            } catch (error) {
                if (!(error instanceof AuthClientError)) {
                    setPinErrorMessage(sessionCheckFailed);
                }

                if ((error as { code?: string })?.code === 'invalid_pin') {
                    setPinErrorMessage(pinError);
                } else {
                    setPinErrorMessage(sessionCheckFailed);
                }
            } finally {
                setIsPinSubmitting(false);
            }

            return;
        }

        try {
            setIsRedirectingToKeycloak(true);
            setSessionErrorMessage(null);
            await startKeycloakLogin();
        } catch (error) {
            if (!(error instanceof AuthClientError)) {
                setSessionErrorMessage(sessionCheckFailed);
            }

            if ((error as { code?: string })?.code === 'missing_env') {
                setSessionErrorMessage(
                    `${missingEnvWarningPrefix} ${missingAuthEnvVars.join(', ')}`
                );
            } else {
                setSessionErrorMessage(sessionCheckFailed);
            }
        } finally {
            setIsRedirectingToKeycloak(false);
        }
    };

    const handleReturnToMainLogin = async () => {
        try {
            await clearBackendSession();
        } finally {
            setIsPinSubmitting(false);
            setPinErrorMessage(null);
            setIsUserAssignedToThisDevice(false);
        }
    };

    if (isCheckingKeycloakState) {
        return (
            <div className="mx-[218px] flex flex-col items-center m-auto p-12 bg-[#0F2027B2] rounded-[20px]">
                <p className="text-white text-xl font-bold">
                    {sessionChecking}
                </p>
            </div>
        );
    }

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
                {isDevelopment && missingAuthEnvVars.length ? (
                    <p className="text-amber-200 text-xs font-semibold text-center mt-2">
                        {missingEnvWarningPrefix}{' '}
                        {missingAuthEnvVars.join(', ')}
                    </p>
                ) : null}
                {sessionErrorMessage ? (
                    <div className="mt-4 flex flex-col items-center gap-2">
                        <p className="text-red-200 text-sm font-semibold text-center">
                            {sessionErrorMessage}
                        </p>
                        <button
                            type="button"
                            className="text-white text-xs underline underline-offset-4"
                            onClick={() =>
                                setSessionCheckTick((prev) => prev + 1)
                            }
                        >
                            {sessionRetryCta}
                        </button>
                    </div>
                ) : null}
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
                            {pinErrorMessage ? (
                                <p className="text-red-200 text-sm font-semibold">
                                    {pinErrorMessage}
                                </p>
                            ) : null}
                            {isPinSubmitting ? (
                                <p className="text-white text-xs">
                                    {pinSubmitting}
                                </p>
                            ) : null}
                            <p className="text-center text-white text-sm max-w-[420px]">
                                {pinHelpText}{' '}
                                <button
                                    type="button"
                                    className="underline underline-offset-4 font-bold"
                                    onClick={handleReturnToMainLogin}
                                >
                                    {switchToMainLoginText}
                                </button>
                            </p>
                        </>
                    ) : (
                        <p className="text-center text-white text-sm max-w-[420px]">
                            {keycloakHint}
                        </p>
                    )}
                    <Button
                        className="mt-8 uppercase font-bold"
                        disabled={isRedirectingToKeycloak || isPinSubmitting}
                    >
                        {isUserAssignedToThisDevice
                            ? submit
                            : keycloakSignInCta}
                    </Button>
                </form>
            </main>
        </div>
    );
};

const labelClasses = 'text-xl/9 text-center text-white uppercase';
const inputClasses =
    'w-full text-xl/7 text-white text-center font-bold bg-opacity-30';

export default Login;
