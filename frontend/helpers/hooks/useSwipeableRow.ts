'use client';

import { useEffect, useRef, useState } from 'react';
import { useSwipeable } from 'react-swipeable';

type SwipeFlashDirection = 'plus' | 'minus' | null;

export const useSwipeableRow = ({
    onSwipeLeft,
    onSwipeRight,
    enabled,
    delta = 25,
    maxOffset = 60,
    flashDurationMs = 220,
}: {
    onSwipeLeft?: () => void;
    onSwipeRight?: () => void;
    enabled?: boolean;
    delta?: number;
    maxOffset?: number;
    flashDurationMs?: number;
}) => {
    const isEnabled = enabled ?? !!(onSwipeLeft || onSwipeRight);

    const [isSwiping, setIsSwiping] = useState(false);
    const [swipeX, setSwipeX] = useState(0);
    const [flashDir, setFlashDir] = useState<SwipeFlashDirection>(null);

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

    const flashIndicator = (dir: Exclude<SwipeFlashDirection, null>) => {
        setFlashDir(dir);
        if (hideTimerRef.current) clearTimeout(hideTimerRef.current);
        hideTimerRef.current = setTimeout(() => {
            setFlashDir(null);
            hideTimerRef.current = null;
        }, flashDurationMs);
    };

    useEffect(() => {
        return () => {
            if (hideTimerRef.current) clearTimeout(hideTimerRef.current);
            if (rafRef.current != null) cancelAnimationFrame(rafRef.current);
        };
    }, []);

    const handlers = useSwipeable({
        onSwiping: isEnabled
            ? (e) => {
                  setIsSwiping(true);

                  const deltaX = typeof e.deltaX === 'number' ? e.deltaX : 0;
                  const clamped = Math.max(
                      -maxOffset,
                      Math.min(maxOffset, deltaX)
                  );
                  setSwipeXThrottled(clamped);
              }
            : undefined,
        onSwipedLeft: isEnabled
            ? () => {
                  setIsSwiping(false);
                  setSwipeXThrottled(0);
                  onSwipeLeft?.();
                  if (onSwipeLeft) flashIndicator('minus');
              }
            : undefined,
        onSwipedRight: isEnabled
            ? () => {
                  setIsSwiping(false);
                  setSwipeXThrottled(0);
                  onSwipeRight?.();
                  if (onSwipeRight) flashIndicator('plus');
              }
            : undefined,
        onTouchEndOrOnMouseUp: isEnabled
            ? () => {
                  setIsSwiping(false);
                  setSwipeXThrottled(0);
              }
            : undefined,
        delta,
        trackMouse: isEnabled,
        preventScrollOnSwipe: isEnabled,
        touchEventOptions: { passive: false },
    });

    const absSwipeX = Math.abs(swipeX);
    const progress = isEnabled
        ? Math.max(0, Math.min(1, absSwipeX / maxOffset))
        : 0;

    const activeDir: SwipeFlashDirection = isEnabled
        ? swipeX > 0
            ? 'plus'
            : swipeX < 0
              ? 'minus'
              : flashDir
        : null;

    return {
        handlers,
        isSwiping,
        swipeX,
        progress,
        activeDir,
    };
};
