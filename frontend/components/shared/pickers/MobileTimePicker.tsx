'use client';

import type { Dayjs } from 'dayjs';
import { MobileTimePicker as MuiMobileTimePicker } from '@mui/x-date-pickers';
import clsx from 'clsx';

interface MobileTimePickerProps {
    label?: string;
    value: Dayjs | null;
    onChange: (newValue: Dayjs | null) => void;
    disabled?: boolean;
    errorText?: string;
    minutesStep?: number;
    format?: string;
    className?: string;
    labelClassName?: string;
    dimLabel?: boolean;
    minTime?: Dayjs;
    maxTime?: Dayjs;
}

const MobileTimePicker = ({
    label,
    value,
    onChange,
    disabled = false,
    errorText,
    minutesStep = 5,
    format = 'HH:mm',
    className,
    labelClassName,
    dimLabel = false,
    minTime,
    maxTime,
}: MobileTimePickerProps) => {
    const hasError = Boolean(errorText);

    return (
        <div
            className={clsx('relative flex flex-col w-full', className)}
            style={{ touchAction: 'none' }}
        >
            {label && (
                <label
                    htmlFor="time"
                    className={clsx(
                        'pl-2 font-semibold text-sm mb-1',
                        dimLabel ? 'text-black/50' : 'text-gray-700',
                        labelClassName
                    )}
                >
                    {label}
                </label>
            )}

            <MuiMobileTimePicker
                value={value}
                onChange={onChange}
                ampm={false}
                format={format}
                minutesStep={minutesStep}
                disabled={disabled}
                minTime={minTime}
                maxTime={maxTime}
                slotProps={{
                    textField: {
                        fullWidth: true,
                        error: hasError,
                        helperText: errorText,
                        placeholder: '',
                        FormHelperTextProps: {
                            sx: {
                                color: 'var(--danger) !important',
                                fontSize: '0.75rem',
                                fontWeight: '500',
                                marginLeft: '8px',
                                marginTop: '4px',
                            },
                        },
                        InputProps: {
                            sx: {
                                borderRadius: '9999px',
                                height: '40px',
                                transition: 'all 0.2s ease',
                                backgroundColor: hasError
                                    ? 'var(--danger-light) !important'
                                    : '#E6E6E6 !important',
                                '& fieldset': {
                                    border: 'none !important',
                                },
                                '&:hover fieldset': {
                                    border: 'none !important',
                                },
                                '&.Mui-focused fieldset': {
                                    border: 'none !important',
                                },
                                '& .MuiInputBase-input': {
                                    paddingLeft: '24px !important',
                                    color: hasError
                                        ? '#FFFFFF !important'
                                        : '#2B5162',
                                    '&::placeholder': {
                                        color: 'transparent',
                                        opacity: 0,
                                    },
                                },
                            },
                        },
                        inputProps: {
                            placeholder: '',
                        },
                    },
                }}
            />
        </div>
    );
};

export default MobileTimePicker;
