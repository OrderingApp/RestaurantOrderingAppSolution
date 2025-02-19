import clsx from 'clsx';
import { inputStyles } from '@/lib/styles/input';
type InputProps = React.InputHTMLAttributes<HTMLInputElement> & {
    label?: string;
    variant?: keyof typeof inputStyles.variants;
    inputSize?: keyof typeof inputStyles.sizes;
    labelClassName?: string;
    inputClassName?: string;
    errorClassName?: string;
    icon?: React.ReactElement;
    errors?: { type: string; message?: string };
};

const Input = ({
    id,
    label,
    type,
    placeholder,
    autoCapitalize = 'none',
    autoCorrect = 'on',
    disabled = false,
    variant = 'primary',
    inputSize = 'sm',
    inputClassName,
    labelClassName,
    errorClassName,
    icon,
    errors,
    ...props
}: InputProps) => (
    <div className="flex flex-col relative">
        {label && (
            <label
                htmlFor={id}
                className={clsx(
                    'pl-2 font-semibold text-black',
                    inputStyles.sizes[inputSize].label,
                    labelClassName
                )}
            >
                {label}
            </label>
        )}

        {icon && <span className="absolute top-8 right-4">{icon}</span>}

        <input
            {...{
                id,
                type,
                placeholder,
                autoCapitalize,
                autoCorrect,
                disabled,
                ...props,
            }}
            className={clsx(
                'px-4  w-40 py-2 shadow-sm text-black rounded-full focus:outline-none focus:ring-2 focus:ring-blue-500',
                inputStyles.variants[variant],
                errors && 'bg-red-200',
                inputStyles.sizes[inputSize].input,
                inputClassName
            )}
        />

        {errors && (
            <p
                className={clsx(
                    'text-red-500 text-[10px] md:text-[12px] px-2 ',
                    inputStyles.sizes[inputSize].error,
                    errorClassName
                )}
            >
                {errors.message}
            </p>
        )}
    </div>
);

export default Input;
