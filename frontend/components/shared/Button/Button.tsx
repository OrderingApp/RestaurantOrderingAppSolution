'use client';

import { useState, type ReactNode, type MouseEvent } from 'react';

type ButtonProps = {
    children: ReactNode;
    className?: string;
    disabled?: boolean;
} & (
    | { onClick: (e?: MouseEvent<HTMLButtonElement>) => void; action?: never }
    | { action: () => Promise<void>; onClick?: never }
);

export const Button = ({
    children,
    onClick,
    action,
    className = '',
    disabled = false,
    ...props
}: ButtonProps) => {
    const [isLoading, setIsLoading] = useState(false);

    const handleClick = async (e: MouseEvent<HTMLButtonElement>) => {
        if (onClick) return onClick(e);

        setIsLoading(true);

        try {
            await action();
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <button
            className={`btn ${className}`}
            onClick={handleClick}
            disabled={disabled || isLoading}
            {...props}
        >
            {children}
        </button>
    );
};

//TODO: add a loading spinner before {children} if disabled, maybe accept a spinner? flag to do so only when needed

// <img src="https://bazaikon.com/naszapaczka/{name} />
