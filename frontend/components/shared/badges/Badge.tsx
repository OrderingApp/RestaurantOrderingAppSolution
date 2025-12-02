import { ReactNode } from 'react';

const BADGE_VARIANTS = {
    success: 'text-white bg-success',
    warning: 'text-white bg-warning',
    danger: 'text-white bg-danger',
    'danger-dark': 'text-white bg-danger-dark',
} as const;

const BADGE_SIZES = {
    md: 'py-1.5 px-3 rounded-2xl',
} as const;

export type BadgeVariantKeys = keyof typeof BADGE_VARIANTS;
export type BadgeSizeKeys = keyof typeof BADGE_SIZES;

export interface BadgeProps {
    children: ReactNode;
    variant?: BadgeVariantKeys;
    size?: BadgeSizeKeys;
}

export const Badge = ({
    children,
    variant = 'success',
    size = 'md',
}: BadgeProps) => (
    <span className={`${BADGE_VARIANTS[variant]} ${BADGE_SIZES[size]}`}>
        {children}
    </span>
);

export default Badge;
