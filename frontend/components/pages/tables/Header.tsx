import { useEffect, useRef, useState } from 'react';
import Image from 'next/image';
import { cn } from '@/lib/utils';

import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import Button from '@/components/shared/Button/Button';

import EditIcon from '@/public/images/svg/edit.svg';

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
            className={cn(
                'fixed flex justify-center items-center gap-9 pt-4 px-5 z-50 backdrop-blur-sm',
                isDesktopAndOverflowing ? 'mb-3.5' : 'mb-2'
            )}
        >
            {/* [PRIO-1] TODO: add skeleton for tabs -- blocked by lack of API endpoint */}
            <Tabs
                ref={tabsRef}
                value={activeTabValue}
                onValueChange={handleTabChange}
                className={cn(
                    'max-w-[547px] pb-2 overflow-x-auto scrollbar',
                    isDesktopAndOverflowing ? '-mb-3.5' : '-mb-2'
                )}
            >
                <TabsList className="p-0 h-auto bg-white rounded-3xl text-black text-sm">
                    {tabsMock.map(({ label, value }) => (
                        <TabsTrigger
                            key={value}
                            value={value}
                            className="justify-start py-4 px-8 font-semibold rounded-3xl data-[state=active]:bg-quaternary data-[state=active]:text-white"
                        >
                            {label}
                        </TabsTrigger>
                    ))}
                </TabsList>
            </Tabs>

            <Button
                variant="quaternary"
                size="xxs"
                className="rounded-xl aspect-square h-[46px]"
            >
                <Image src={EditIcon} alt="Edit tables" />
            </Button>
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
