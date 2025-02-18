'use client';

import { useState, type ReactNode, type MouseEvent } from 'react';
import clsx from 'clsx';
import btnStyles from '@/lib/styles/button';
type ButtonProps = {
    children: ReactNode;
    className?: string;
    disabled?: boolean;
    variant?: keyof typeof btnStyles.variants;
    size?: keyof typeof btnStyles.sizes;
    onClick?: (e?: MouseEvent<HTMLButtonElement>) => void;
    action?: () => Promise<void>;
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
                'transition-all duration-200',
                btnStyles.variants[variant],
                btnStyles.sizes[size],
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
