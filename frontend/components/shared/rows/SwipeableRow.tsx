'use client';

import { type ReactNode } from 'react';
import { useSwipeableRow } from '@/helpers/hooks/useSwipeableRow';

type SwipeableRenderState = {
    shouldShowAction: boolean;
    isSwiping: boolean;
    swipeX: number;
    progress: number;
    activeDir: 'plus' | 'minus' | null;
    revealedWidth: number;
};

const SwipeableRow = ({
    children,
    className,
    contentClassName,
    onSwipeLeft,
    onSwipeRight,
}: {
    children: ReactNode | ((state: SwipeableRenderState) => ReactNode);
    className?: string;
    contentClassName?: string;
    onSwipeLeft?: () => void;
    onSwipeRight?: () => void;
}) => {
    const { handlers, isSwiping, swipeX, progress, activeDir } =
        useSwipeableRow({
            onSwipeLeft,
            onSwipeRight,
        });

    const revealedWidth = Math.abs(swipeX);
    const shouldShowAction = !!activeDir && revealedWidth > 0;

    const renderState: SwipeableRenderState = {
        shouldShowAction,
        isSwiping,
        swipeX,
        progress,
        activeDir,
        revealedWidth,
    };

    return (
        <li
            {...handlers}
            className={
                className
                    ? `relative overflow-hidden ${className}`
                    : 'relative overflow-hidden'
            }
        >
            {shouldShowAction && (
                <div
                    className={
                        activeDir === 'plus'
                            ? 'pointer-events-none absolute inset-y-0 left-0 bg-success'
                            : 'pointer-events-none absolute inset-y-0 right-0 bg-danger'
                    }
                    style={{
                        width: `${revealedWidth}px`,
                        willChange: 'width',
                    }}
                />
            )}

            <div
                className={
                    isSwiping
                        ? contentClassName
                            ? `w-full bg-inherit ${contentClassName}`
                            : 'w-full bg-inherit'
                        : contentClassName
                          ? `w-full bg-inherit transition-transform duration-150 ease-out ${contentClassName}`
                          : 'w-full bg-inherit transition-transform duration-150 ease-out'
                }
                style={{ transform: `translateX(${swipeX}px)` }}
            >
                {typeof children === 'function' ? children(renderState) : children}
            </div>

            {shouldShowAction && (
                <span
                    className={
                        activeDir === 'plus'
                            ? 'pointer-events-none absolute left-0 top-0 h-full flex items-center justify-center overflow-hidden text-white font-semibold'
                            : 'pointer-events-none absolute right-0 top-0 h-full flex items-center justify-center overflow-hidden text-white font-semibold'
                    }
                    style={{
                        width: `${revealedWidth}px`,
                        opacity: 0.35 + 0.65 * progress,
                        transform: `translateZ(0) scale(${0.9 + 0.1 * progress})`,
                        willChange: 'width, opacity, transform',
                        transition: isSwiping
                            ? 'none'
                            : 'opacity 150ms ease-out, transform 150ms ease-out',
                    }}
                >
                    {activeDir === 'plus' ? '+' : '-'}
                </span>
            )}
        </li>
    );
};

export default SwipeableRow;