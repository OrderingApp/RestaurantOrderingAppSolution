'use client';

import clsx from 'clsx';
import { useSearchParams, useRouter } from 'next/navigation';

interface ToggleSwitchProps {
    id: string;
    name: string;
}

const ToggleSwitch = ({ items }: { items: ToggleSwitchProps[] }) => {
    const searchParams = useSearchParams();
    const router = useRouter();
    const order = searchParams.get('orderType') || items[0]?.id;

    const toggleSwitchParams = (id: string) => {
        return () => {
            const newParams = new URLSearchParams(searchParams.toString());
            newParams.set('orderType', id);
            router.push(`?${newParams.toString()}`);
        };
    };

    return (
        <div className="flex bg-white rounded-full shadow-lg items-center h-11">
            {items.map((item) => (
                <button
                    key={item.id}
                    className={clsx(
                        'px-8 h-full rounded-full text-sm',
                        order === item.id ? 'text-white bg-primary' : 'bg-white'
                    )}
                    onClick={toggleSwitchParams(item.id)}
                >
                    {item.name}
                </button>
            ))}
        </div>
    );
};

export default ToggleSwitch;
