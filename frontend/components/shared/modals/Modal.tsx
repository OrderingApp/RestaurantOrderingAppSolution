import { ReactNode } from 'react';
import { createPortal } from 'react-dom';

const Modal = ({
    children,
    onClose,
}: {
    children: ReactNode;
    onClose: () => void;
}) => {
    return createPortal(
        <div
            onClick={onClose}
            className="absolute top-0 left-0 w-full h-full bg-[rgba(0,0,0,0.8)] flex items-center justify-center"
        >
            {children}
        </div>,
        document.body
    );
};

export default Modal;
