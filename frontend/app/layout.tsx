import { Geist, Geist_Mono } from 'next/font/google';

import type { Metadata } from 'next';
import type { ReactNode } from 'react';

import QueryProvider from '@/providers/queryProvider';
import LanguageProvider from '@/providers/LanguageProvider';
import readLangCookie from '@/actions/readLangCookie';
import languagePacks from '@/helpers/constants/languagePacks';

import './globals.css';
import OrdersProvider from '@/providers/OrdersContext';
import { Toaster } from 'sonner';
import { ReservationProvider } from '@/providers/ReservationsContext';

const geistSans = Geist({
    variable: '--font-geist-sans',
    subsets: ['latin'],
});

const geistMono = Geist_Mono({
    variable: '--font-geist-mono',
    subsets: ['latin'],
});

export const generateMetadata = async (): Promise<Metadata> => {
    const lang = await readLangCookie();
    const { metadata } = languagePacks[lang];

    return metadata;
};

const RootLayout = async ({
    children,
}: Readonly<{
    children: ReactNode;
}>) => {
    const lang = await readLangCookie();

    return (
        <html lang={lang}>
            <body
                className={`${geistSans.variable} ${geistMono.variable} flex justify-center items-center min-h-screen antialiased bg-page-gradient`}
            >
                <div id="root" className="w-full max-w-[64rem] relative">
                    <QueryProvider>
                        <LanguageProvider language={lang}>
                            <OrdersProvider>
                                <ReservationProvider>
                                    {children}
                                </ReservationProvider>
                            </OrdersProvider>
                        </LanguageProvider>
                    </QueryProvider>
                </div>
                <Toaster richColors />
            </body>
        </html>
    );
};

export default RootLayout;
//TODO: add constants for html lang, metaData title etc.
// change fonts

//TODO: add dehydration/hydration from tanstack-query
