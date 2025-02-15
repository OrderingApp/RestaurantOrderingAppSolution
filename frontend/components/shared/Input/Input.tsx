import clsx from 'clsx';

type InputProps = {
    id?: string;
    label?: string;
    type: string;
    placeholder?: string;
    autoCapitalize?: string;
    autoCorrect?: string;
    disabled?: boolean;
    variant?: 'primary';
    size?: 'sm' | 'md' | 'lg';
    labelClassName?: string;
    inputClassName?: string;
    errorClassName?: string;
    icon?: React.ReactElement;
    onChange?: React.ChangeEventHandler<HTMLInputElement>;
    props?: React.HTMLAttributes<HTMLInputElement>;
    errors?: { type: string; message?: string };
};

const variantClasses = {
    primary: {
        label: 'text-black',
        input: 'bg-[#E6E6E6] text-[#2B5162]',
    },
};
const sizeClasses = {
    sm: {
        label: 'text-sm',
        input: 'px-4 py-3 text-md',
        error: 'text-sm',
    },
    md: {
        label: 'text-md',
        input: 'px-4 py-2 text-md',
        error: 'text-sm',
    },
    lg: {
        label: 'text-md',
        input: 'px-4 py-3 text-md',
        error: 'text-md',
    },
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
    size = 'sm',
    labelClassName,
    inputClassName,
    errorClassName,
    icon,
    onChange,
    props,
    errors,
}: InputProps) => (
    <div className="flex flex-col relative">
        {label && (
            <label
                htmlFor={id}
                className={clsx(
                    'pl-2 font-semibold text-black',
                    variantClasses[variant].label,
                    sizeClasses[size].label,
                    labelClassName
                )}
            >
                {label}
            </label>
        )}

        <input
            {...{
                id,
                type,
                placeholder,
                onChange,
                autoCapitalize,
                autoCorrect,
                disabled,
                ...props,
            }}
            className={clsx(
                'px-4  w-40 py-2 shadow-sm text-black rounded-full focus:outline-none focus:ring-2 focus:ring-blue-500',
                variantClasses[variant].input,
                sizeClasses[size].input,
                inputClassName
            )}
        />

        {icon && <span className="absolute top-8 right-4">{icon}</span>}

        {errors && (
            <p
                className={clsx(
                    'text-red-500 text-[10px] md:text-[12px] px-2 ',
                    sizeClasses[size].error,
                    errorClassName
                )}
            >
                {errors.message}
            </p>
        )}
    </div>
);

export default Input;
