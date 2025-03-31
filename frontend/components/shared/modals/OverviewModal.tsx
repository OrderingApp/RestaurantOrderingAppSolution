import { ReactNode } from 'react';
import { createPortal } from 'react-dom';

const OverviewModal = ({ children }: { children: ReactNode }) => {
    const modalRoot = document.getElementById('root') as HTMLDivElement;
    return createPortal(
        <div className="absolute top-0 left-0 pl-4 pr-2 h-[768px]  bg-light-gray w-full rounded-3xl overflow-x-hidden ">
            {children}
        </div>,
        modalRoot!
    );
};

export default OverviewModal;
