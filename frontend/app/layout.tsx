import { Titillium_Web } from 'next/font/google';

import type { Metadata } from 'next';
import type { ReactNode } from 'react';

import MuiLocalizationProvider from '@/providers/LocalizationProvider';

import QueryProvider from '@/providers/queryProvider';
import LanguageProvider from '@/providers/LanguageProvider';
import readLangCookie from '@/actions/readLangCookie';
import languagePacks from '@/helpers/constants/languagePacks';

import './globals.css';
import OrdersProvider from '@/providers/OrdersContext';
import { Toaster } from 'sonner';
import { ReservationProvider } from '@/providers/ReservationsContext';

const titilliumWeb = Titillium_Web({
    variable: '--font-titillium-web',
    subsets: ['latin'],
    weight: ['400', '700'],
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
                className={`${titilliumWeb.variable} flex justify-center items-center min-h-screen antialiased bg-page-gradient`}
            >
                <div id="root" className="w-full max-w-[64rem] relative">
                    <QueryProvider>
                        <LanguageProvider language={lang}>
                            <MuiLocalizationProvider>
                                <OrdersProvider>
                                    <ReservationProvider>
                                        {children}
                                    </ReservationProvider>
                                </OrdersProvider>
                            </MuiLocalizationProvider>
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
