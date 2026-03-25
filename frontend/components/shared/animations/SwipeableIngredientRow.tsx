'use client';

import { useState, useRef, type ReactNode } from 'react';
import { useSwipeable } from 'react-swipeable';

export interface SwipeableIngredientRowProps {
    children: ReactNode;
    className?: string;
    onSwipeLeft?: () => void;
    onSwipeRight?: () => void;
}

const SwipeableIngredientRow = ({
    children,
    className,
    onSwipeLeft,
    onSwipeRight,
}: SwipeableIngredientRowProps) => {
    const [isSwiping, setIsSwiping] = useState(false);
    const [swipeX, setSwipeX] = useState(0);
    const [flashDir, setFlashDir] = useState<'plus' | 'minus' | null>(null);

    const hideTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);
    const rafRef = useRef<number | null>(null);
    const pendingXRef = useRef(0);

    const setSwipeXThrottled = (nextX: number) => {
        pendingXRef.current = nextX;
        if (rafRef.current != null) return;

        rafRef.current = requestAnimationFrame(() => {
            rafRef.current = null;
            setSwipeX(pendingXRef.current);
        });
    };

    const flashIndicator = (dir: 'plus' | 'minus') => {
        setFlashDir(dir);
        if (hideTimerRef.current) clearTimeout(hideTimerRef.current);
        hideTimerRef.current = setTimeout(() => {
            setFlashDir(null);
            hideTimerRef.current = null;
        }, 220);
    };

    const absSwipeX = Math.abs(swipeX);
    const progress = Math.max(0, Math.min(1, absSwipeX / 60));
    const activeDir: 'plus' | 'minus' | null =
        swipeX > 0 ? 'plus' : swipeX < 0 ? 'minus' : flashDir;

    const handlers = useSwipeable({
        onSwiping: (e) => {
            setIsSwiping(true);

            // react-swipeable provides deltaX; keep this defensive
            const deltaX = typeof e.deltaX === 'number' ? e.deltaX : 0;
            const clamped = Math.max(-60, Math.min(60, deltaX));
            setSwipeXThrottled(clamped);
        },
        onSwipedLeft: () => {
            setIsSwiping(false);
            setSwipeXThrottled(0);
            onSwipeLeft?.();
            flashIndicator('minus');
        },
        onSwipedRight: () => {
            setIsSwiping(false);
            setSwipeXThrottled(0);
            onSwipeRight?.();
            flashIndicator('plus');
        },
        onTouchEndOrOnMouseUp: () => {
            // If user releases without triggering a swipe threshold
            setIsSwiping(false);
            setSwipeXThrottled(0);
            // do not flash on cancel
        },
        delta: 25,
        trackMouse: true,
        preventScrollOnSwipe: true,
        touchEventOptions: { passive: false },
    });

    return (
        <li
            {...handlers}
            className={
                className
                    ? `relative overflow-hidden ${className}`
                    : 'relative overflow-hidden'
            }
        >
            {/* Reveal background (smooth scaleX, anchored to edge) */}
            {activeDir && (
                <div
                    className={
                        activeDir === 'plus'
                            ? 'pointer-events-none absolute inset-y-0 left-0 w-24 bg-success'
                            : 'pointer-events-none absolute inset-y-0 right-0 w-24 bg-danger'
                    }
                    style={{
                        transform:
                            activeDir === 'plus'
                                ? `scaleX(${progress})`
                                : `scaleX(${progress})`,
                        transformOrigin:
                            activeDir === 'plus'
                                ? 'left center'
                                : 'right center',
                        willChange: 'transform',
                    }}
                />
            )}

            <div
                className={
                    isSwiping
                        ? 'w-full flex items-center justify-between gap-2'
                        : 'w-full flex items-center justify-between gap-2 transition-transform duration-150 ease-out'
                }
                style={{ transform: `translateX(${swipeX}px)` }}
            >
                {children}
            </div>

            {/* Icon pinned to the edge; fades in as you swipe */}
            {activeDir && (
                <span
                    className={
                        activeDir === 'plus'
                            ? 'pointer-events-none absolute left-0 top-0 h-full w-24 flex items-center justify-center text-white font-semibold'
                            : 'pointer-events-none absolute right-0 top-0 h-full w-24 flex items-center justify-center text-white font-semibold'
                    }
                    style={{
                        opacity: 0.35 + 0.65 * progress,
                        transform: `translateZ(0) scale(${0.9 + 0.1 * progress})`,
                        willChange: 'opacity, transform',
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

export default SwipeableIngredientRow;
