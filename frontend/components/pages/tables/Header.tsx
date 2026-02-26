import { useEffect, useRef, useState } from 'react';
import { cn } from '@/lib/utils';

import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';

export type TabValue = (typeof tabsMock)[number]['value'];
interface TablesHeaderProps {
    onTabChange: (newTabValue: TabValue) => void;
}

const TablesHeader = ({ onTabChange }: TablesHeaderProps) => {
    const [isDesktopAndOverflowing, setIsDesktopAndOverflowing] =
        useState(false);
    const [activeTabValue, setActiveTabValue] = useState(tabsMock[0].value);
    const tabsRef = useRef<HTMLDivElement>(null);

    // Check overflow
    useEffect(() => {
        const checkOverflow = () => {
            if (!tabsRef.current) return;

            const isTouchDevice =
                'ontouchstart' in window || navigator.maxTouchPoints > 0;

            if (isTouchDevice) return setIsDesktopAndOverflowing(false);

            const isOverflowing =
                tabsRef.current.scrollWidth > tabsRef.current.clientWidth;

            setIsDesktopAndOverflowing(isOverflowing);
        };

        checkOverflow();
        window.addEventListener('resize', checkOverflow);

        return () => {
            window.removeEventListener('resize', checkOverflow);
        };
    }, []);

    // Scroll to active tab
    useEffect(() => {
        if (!tabsRef.current || !activeTabValue) return;

        const activeTabTrigger = tabsRef.current.querySelector(
            `[data-state="active"]`
        );

        if (!activeTabTrigger) return;

        activeTabTrigger.scrollIntoView({
            behavior: 'smooth',
            inline: 'nearest',
        });
    }, [activeTabValue]);

    const handleTabChange = (newTabValue: TabValue) => {
        setActiveTabValue(newTabValue);
        onTabChange(newTabValue);
    };

    return (
        <header
            style={{ boxShadow: '0px 4px 4px 0px #00000040' }}
            className={cn(
                'w-full flex justify-between items-center gap-9',
                isDesktopAndOverflowing ? 'mb-3.5' : 'mb-2'
            )}
        >
            {/* [PRIO-1] TODO: add skeleton for tabs -- blocked by lack of API endpoint */}
            <Tabs
                ref={tabsRef}
                value={activeTabValue}
                onValueChange={handleTabChange}
                className="w-full"
            >
                <TabsList
                    className={cn(
                        'p-0 bg-transparent text-black text-sm mx-auto',
                        isDesktopAndOverflowing ? '-mb-3.5' : '-mb-2'
                    )}
                >
                    {tabsMock.map(({ label, value }) => (
                        <TabsTrigger
                            key={value}
                            value={value}
                            className="px-6 font-semibold rounded-none flex items-center justify-center min-w-[120px] h-full data-[state=active]:bg-quaternary data-[state=active]:text-white"
                        >
                            {label}
                        </TabsTrigger>
                    ))}
                </TabsList>
            </Tabs>
        </header>
    );
};

const tabsMock = [
    { label: 'Bar', value: 'bar' },
    { label: 'Kominek', value: 'kominek' },
    { label: 'Bilardownia', value: 'bilardownia' },
    { label: 'Góra', value: 'góra' },
    { label: 'Ogródek', value: 'ogródek' },
    { label: 'Przód', value: 'przód' },
];

export default TablesHeader;
