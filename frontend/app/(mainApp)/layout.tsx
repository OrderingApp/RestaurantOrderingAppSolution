'use client';

import { usePathname } from 'next/navigation';
import Image from 'next/image';
import Link from 'next/link';
import clsx from 'clsx';
import { type ReactNode } from 'react';

import { useLanguage } from '@/providers/LanguageProvider';
import languagePacks from '@/helpers/constants/languagePacks';
import { MENU_BAR_ICONS } from '@/helpers/constants/constants';
import { camelToKebab } from '@/helpers/utils/utils';

const ProtectedLayout = ({ children }: Readonly<{ children: ReactNode }>) => {
    const pathname = usePathname();
    const { language } = useLanguage();
    const { menuBar } = languagePacks[language];
    const menuBarEntries = Object.entries(menuBar) as Array<
        [keyof typeof menuBar, string]
    >;

    return (
        <div className="py-5 pl-4 pr-2 flex min-h-[48rem] max-h-screen">
            <aside className="pt-11 pb-6 px-4 bg-[#000] bg-opacity-40 rounded-l-3xl">
                <ul className="flex flex-col items-center gap-8 h-full">
                    {menuBarEntries.map(([key, name]) => {
                        const opacity =
                            !pathname.substring(1).startsWith(key) &&
                            'opacity-50';

                        return (
                            <li
                                key={key}
                                className={clsx(
                                    opacity,
                                    key === 'settings' && 'mt-auto'
                                )}
                            >
                                <Link
                                    href={`/${camelToKebab(key)}`}
                                    className={clsx(
                                        'flex flex-col items-center text-sm text-white capitalize hocus:opacity-90 transition-opacity duration-300',
                                        opacity
                                    )}
                                >
                                    <Image src={MENU_BAR_ICONS[key]} alt="" />
                                    {name}
                                </Link>
                            </li>
                        );
                    })}
                </ul>
            </aside>

            <main className="relative bg-light-gray w-full rounded-r-3xl overflow-x-hidden">
                {children}
            </main>
        </div>
    );
};

export default ProtectedLayout;
