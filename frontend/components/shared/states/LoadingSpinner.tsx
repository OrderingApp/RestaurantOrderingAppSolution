import { BasicStyles } from '@/lib/types/types';
import clsx from 'clsx';

const SPINNER_VARIANTS: Record<keyof BasicStyles['variants'], string> = {
    primary: 'border-primary',
    secondary: 'border-secondary',
    tertiary: 'border-tertiary',
    quaternary: 'border-quaternary',
    success: 'border-success',
    danger: 'border-danger',
} as const;

interface LoadingSpinnerProps {
    size?: number;
    stroke?: number;
    variant?: keyof typeof SPINNER_VARIANTS;
    className?: string;
}

const LoadingSpinner = ({
    size = 24,
    stroke = 3,
    variant = 'primary',
    className = '',
}: LoadingSpinnerProps) => (
    <div
        className={clsx(
            'animate-spin rounded-full border-t-transparent',
            SPINNER_VARIANTS[variant],
            className
        )}
        style={{
            width: size,
            height: size,
            borderWidth: stroke,
        }}
    ></div>
);

export default LoadingSpinner;
