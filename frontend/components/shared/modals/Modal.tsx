'use client';
import { ReactNode } from 'react';
import { createPortal } from 'react-dom';
import { AnimatePresence, motion, type HTMLMotionProps } from 'framer-motion';

interface ModalProps {
    children: ReactNode;
    isOpen?: boolean;
    onClose?: () => void;
    onAnimationComplete?: () => void;
    motionConfig?: {
        backdrop?: Pick<
            HTMLMotionProps<'div'>,
            'initial' | 'animate' | 'exit' | 'transition'
        >;
        content?: Pick<
            HTMLMotionProps<'div'>,
            'initial' | 'animate' | 'exit' | 'transition'
        >;
    };
}

export const MODAL_DEFAULT_MOTION_CONFIG = {
    backdrop: {
        initial: { opacity: 0 },
        animate: { opacity: 1 },
        exit: { opacity: 0 },
        transition: { duration: 0.2 },
    },
    content: {
        initial: { opacity: 0, scale: 0.9 },
        animate: { opacity: 1, scale: 1 },
        exit: { opacity: 0, scale: 0.9 },
        transition: { duration: 0.2, ease: 'easeOut' },
    },
};

const Modal = ({
    children,
    isOpen = true,
    onClose,
    onAnimationComplete,
    motionConfig = MODAL_DEFAULT_MOTION_CONFIG,
}: ModalProps) =>
    createPortal(
        <AnimatePresence onExitComplete={onAnimationComplete}>
            {isOpen && (
                <motion.div
                    key="modal-backdrop"
                    className="absolute top-0 left-0 w-full h-full flex items-center justify-center z-50"
                    {...motionConfig.backdrop}
                >
                    <div
                        onClick={onClose}
                        className="absolute top-0 left-0 w-full h-full bg-[rgba(0,0,0,0.8)]"
                    />
                    <motion.div
                        key="modal-content"
                        {...motionConfig.content}
                        onClick={(e) => e.stopPropagation()} // Prevent backdrop click when clicking content
                    >
                        {children}
                    </motion.div>
                </motion.div>
            )}
        </AnimatePresence>,
        document.body
    );

export default Modal;
