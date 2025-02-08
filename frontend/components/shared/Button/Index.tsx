import { type ReactNode } from 'react';

type ButtonProps = {
    onClick?: () => void;
    className?: string;
    children: ReactNode;
};

export const Button = ({
    children,
    onClick = () => {},
    className,
}: ButtonProps) => {
    return (
        <button className={`btn ${className || ''}`} onClick={() => onClick()}>
            {children}
        </button>
    );
};
