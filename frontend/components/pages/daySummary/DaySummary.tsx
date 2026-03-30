'use client';

import { useLanguage } from '@/providers/LanguageProvider';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '@/components/ui/tabs';
import languagePacks from '@/helpers/constants/languagePacks';
import OrdersTable from './OrdersTable';
import DaySummaryTab from './DaySummaryTab';

const DaySummary = () => {
    const { language } = useLanguage();
    const {
        daySummaryPage: {
            tabs: { allOrders, summary },
            heading,
        },
    } = languagePacks[language];

    const tabs = [allOrders, summary];

    return (
        <div className="py-8 px-5">
            <Tabs defaultValue={allOrders}>
                <nav>
                    <TabsList className="gap-2.5 mb-8">
                        {tabs.map((label) => (
                            <TabsTrigger
                                key={label}
                                value={label}
                                className="py-3 px-6 font-semibold text-black shadow-[0_4px_4px_0_#00000040] rounded-xl data-[state=active]:bg-quaternary data-[state=active]:text-white"
                            >
                                {label}
                            </TabsTrigger>
                        ))}
                    </TabsList>
                </nav>

                <TabsContent value={allOrders}>
                    <header>
                        <h1 className="sr-only">{heading}</h1>
                    </header>

                    <section>
                        <OrdersTable />
                    </section>
                </TabsContent>
                <TabsContent value={summary}>
                    <header>
                        <h1 className="sr-only">{heading}</h1>
                    </header>

                    <section>
                        <DaySummaryTab />
                    </section>
                </TabsContent>
            </Tabs>
        </div>
    );
};

export default DaySummary;
