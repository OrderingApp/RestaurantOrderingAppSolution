import { ReactNode } from 'react';
import { createPortal } from 'react-dom';
import { motion } from 'motion/react';

const Modal = ({
    children,
    onClose,
}: {
    children: ReactNode;
    onClose: () => void;
}) =>
    createPortal(
        <motion.div
            className="absolute top-0 left-0 w-full h-full flex items-center justify-center z-50"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            transition={{ duration: 0.2 }}
        >
            <div
                onClick={onClose}
                className="absolute top-0 left-0 w-full h-full bg-[rgba(0,0,0,0.8)]"
            />
            <motion.div
                initial={{ opacity: 0, scale: 0.9 }}
                animate={{ opacity: 1, scale: 1 }}
                exit={{ opacity: 0, scale: 0.9 }}
                transition={{ duration: 0.2, ease: 'easeOut' }}
            >
                {children}
            </motion.div>
        </motion.div>,
        document.body
    );

export default Modal;
