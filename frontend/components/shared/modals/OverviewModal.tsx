'use client';
import { ReactNode, useEffect, useState } from 'react';
import { createPortal } from 'react-dom';
import { AnimatePresence, motion, type HTMLMotionProps } from 'framer-motion';

interface OverviewModalProps {
    children: ReactNode;
    isOpen?: boolean;
    motionConfig?: Pick<
        HTMLMotionProps<'div'>,
        'initial' | 'animate' | 'exit' | 'transition'
    >;
    onAnimationComplete?: () => void;
}

export const OVERVIEW_MODAL_DEFAULT_MOTION_CONFIG = {
    initial: { opacity: 0, scale: 0.95 },
    animate: { opacity: 1, scale: 1 },
    exit: { opacity: 0, scale: 0.95 },
    transition: { duration: 0.2, ease: 'easeOut' },
};

const OverviewModal = ({
    children,
    isOpen = true,
    motionConfig = OVERVIEW_MODAL_DEFAULT_MOTION_CONFIG,
    onAnimationComplete,
}: OverviewModalProps) => {
    const [mounted, setMounted] = useState(false);

    useEffect(() => {
        setMounted(true);
    }, []);

    if (!mounted) return null;

    const modalRoot = document.getElementById('root');

    if (!modalRoot) return null;

    return createPortal(
        <AnimatePresence onExitComplete={onAnimationComplete}>
            {isOpen && (
                <motion.div
                    key="overview-modal"
                    className="absolute top-0 left-0 pl-4 h-full bg-light-gray w-full rounded-3xl overflow-x-hidden z-50"
                    {...motionConfig}
                >
                    {children}
                </motion.div>
            )}
        </AnimatePresence>,
        modalRoot
    );
};

export default OverviewModal;
