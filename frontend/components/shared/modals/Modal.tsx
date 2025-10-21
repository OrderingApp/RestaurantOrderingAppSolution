import { ReactNode } from 'react';
import { createPortal } from 'react-dom';

const Modal = ({
    children,
    onClose,
}: {
    children: ReactNode;
    onClose: () => void;
}) =>
    createPortal(
        <div className="absolute top-0 left-0 w-full h-full flex items-center justify-center z-50">
            <div
                onClick={onClose}
                className="absolute top-0 left-0 w-full h-full bg-[rgba(0,0,0,0.8)]"
            />
            {children}
        </div>,
        document.body
    );

export default Modal;
