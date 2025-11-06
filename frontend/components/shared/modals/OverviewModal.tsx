'use client';
import { ReactNode } from 'react';
import { createPortal } from 'react-dom';

const OverviewModal = ({ children }: { children: ReactNode }) => {
    const modalRoot = document.getElementById('modal-root');
    if (!modalRoot) return null;
    return createPortal(
        <div className="absolute top-0 left-0 pl-4  h-[768px]  bg-light-gray w-full rounded-3xl overflow-x-hidden z-10">
            {children}
        </div>,
        modalRoot!
    );
};

export default OverviewModal;
