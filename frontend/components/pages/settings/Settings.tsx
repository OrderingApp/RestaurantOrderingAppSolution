'use client';

import getSettingsItems from '@/helpers/utils/settings';
import { useLanguage } from '@/providers/LanguageProvider';
import Link from 'next/link';

const Settings = () => {
    const { language } = useLanguage();
    const settingsItems = getSettingsItems(language);

    return (
        <div className="grid grid-cols-3 gap-8 auto-rows-fr p-6">
            {settingsItems.map(({ label, icon: Icon, href }) => (
                <Link
                    key={label}
                    href={`/settings/${href}`}
                    className="flex flex-col gap-3 items-center justify-center py-4 px-8 text-center text-xl font-semibold  shadow-[0_2px_5px_0_#00000080] rounded-2xl"
                >
                    {label}
                    <Icon size={60} />
                </Link>
            ))}
        </div>
    );
};

export default Settings;
