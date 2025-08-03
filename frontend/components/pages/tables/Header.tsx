import Image from 'next/image';

import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import Button from '@/components/shared/button/Button';

import EditIcon from '@/public/images/svg/edit.svg';
import { useEffect, useRef, useState } from 'react';
import { cn } from '@/lib/utils';

interface TablesHeaderProps {
    onTabChange: (newTab: (typeof tabsMock)[number]['value']) => void;
}

const TablesHeader = ({ onTabChange }: TablesHeaderProps) => {
    const [isDesktopAndOverflowing, setIsDesktopAndOverflowing] =
        useState(false);
    const tabsRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (!tabsRef.current) return;

        const isTouchDevice =
            'ontouchstart' in window || navigator.maxTouchPoints > 0;

        if (isTouchDevice) return;

        const isOverflowing =
            tabsRef.current.scrollWidth > tabsRef.current.clientWidth;

        setIsDesktopAndOverflowing(isOverflowing);
    }, []);

    return (
        <header
            className={cn(
                'flex justify-center items-center gap-9',
                isDesktopAndOverflowing ? 'mb-3.5' : 'mb-2'
            )}
        >
            {/* [PRIO-1] TODO: add skeleton for tags */}
            <Tabs
                ref={tabsRef}
                defaultValue={tabsMock[0].value}
                onValueChange={onTabChange}
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
];

export default TablesHeader;
