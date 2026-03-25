import Image from 'next/image';
import type { StaticImport } from 'next/dist/shared/lib/get-img-props';
import clsx from 'clsx';

import type { View } from './types';
import { VIEWS } from './types';

export type ActionButton = {
    icon: StaticImport;
    color: string;
    alt: string;
    onClick: () => void;
};

const ActionsBar = ({
    view,
    buttons,
}: {
    view: View;
    buttons: ActionButton[];
}) => (
    <ul
        className={clsx(
            'flex gap-4 py-3 justify-between mt-6',
            view === VIEWS.INFO ? 'px-12' : 'px-6'
        )}
    >
        {buttons.map((btn) => (
            <li key={btn.alt}>
                <button
                    onClick={btn.onClick}
                    style={{ backgroundColor: btn.color }}
                    className="w-16 h-16 rounded-lg shadow-md flex justify-center items-center hover:opacity-90 transition-opacity"
                >
                    <Image className="w-8 h-8" src={btn.icon} alt={btn.alt} />
                </button>
            </li>
        ))}
    </ul>
);

export default ActionsBar;
