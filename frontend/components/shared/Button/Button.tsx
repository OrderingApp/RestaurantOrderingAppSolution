'use client';

import { useState, type ReactNode, type MouseEvent } from 'react';
import clsx from 'clsx';

type ButtonProps = {
    children: ReactNode;
    className?: string;
    disabled?: boolean;
    variant?: 'primary' | 'success' | 'danger' | 'outline';
    size?: 'sm' | 'md' | 'lg';
    onClick?: (e?: MouseEvent<HTMLButtonElement>) => void;
    action?: () => Promise<void>;
};

const variantClasses = {
    primary: 'bg-[#2B5162] text-white',
    success: 'bg-[#2B622F] text-white',
    danger: 'bg-[#F20707] text-white ',
    outline: 'bg-white shadow-lg text-black border border-gray-200',
};

const sizeClasses = {
    sm: 'px-2 py-1 text-sm',
    md: 'px-4 py-2 text-base',
    lg: 'px-6 py-6 text-2xl rounded-3xl',
};

const Button = ({
    children,
    onClick,
    action,
    variant = 'primary',
    size = 'md',
    className = '',
    disabled = false,
    ...props
}: ButtonProps) => {
    const [isLoading, setIsLoading] = useState(false);

    const handleClick = async (e: MouseEvent<HTMLButtonElement>) => {
        if (onClick) return onClick(e);
        if (!action) return;
        setIsLoading(true);

        try {
            await action();
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <button
            className={clsx(
                ' transition-all duration-200',
                variantClasses[variant],
                sizeClasses[size],
                { 'opacity-50 cursor-not-allowed': disabled || isLoading },
                className
            )}
            onClick={handleClick}
            disabled={disabled || isLoading}
            {...props}
        >
            {isLoading ? 'to add spinner' : children}
        </button>
    );
};

export default Button;
//TODO: add a loading spinner before {children} if disabled, maybe accept a spinner? flag to do so only when needed

// <img src="https://bazaikon.com/naszapaczka/{name} />
