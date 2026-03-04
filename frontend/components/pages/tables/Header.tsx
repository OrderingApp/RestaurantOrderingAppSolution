import { useEffect, useRef, useState } from 'react';

import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Area } from '@/helpers/queries/areas/useAreasQuery';

export type TabValue = string;

interface TablesHeaderProps {
    tabs: Area[];
    activeTabValue: TabValue;
    onTabChange: (newTabValue: TabValue) => void;
}

const SCROLLBAR_GAP = 6;

const TablesHeader = ({
    tabs,
    activeTabValue,
    onTabChange,
}: TablesHeaderProps) => {
    const [isOverflowing, setIsOverflowing] = useState(false);
    const [scrollWidth, setScrollWidth] = useState(0);

    const scrollRef = useRef<HTMLDivElement>(null);
    const trackRef = useRef<HTMLDivElement>(null);
    const scrollSource = useRef<'tabs' | 'track' | 'programmatic' | null>(null);

    // Check overflow & measure scroll width
    useEffect(() => {
        const el = scrollRef.current;
        if (!el) return;

        const check = () => {
            setIsOverflowing(el.scrollWidth > el.clientWidth);
            setScrollWidth(el.scrollWidth);
        };

        check();
        const observer = new ResizeObserver(check);
        observer.observe(el);

        return () => observer.disconnect();
    }, []);

    // Sync: tabs → track
    useEffect(() => {
        const el = scrollRef.current;
        const track = trackRef.current;
        if (!el || !track) return;

        const onScroll = () => {
            if (scrollSource.current === 'track') return;
            scrollSource.current = 'tabs';
            track.scrollLeft = el.scrollLeft;
            requestAnimationFrame(() => {
                if (scrollSource.current === 'tabs')
                    scrollSource.current = null;
            });
        };

        el.addEventListener('scroll', onScroll, { passive: true });
        return () => el.removeEventListener('scroll', onScroll);
    }, [isOverflowing]);

    // Sync: track → tabs
    useEffect(() => {
        const el = trackRef.current;
        const container = scrollRef.current;
        if (!el || !container) return;

        const onScroll = () => {
            if (
                scrollSource.current === 'tabs' ||
                scrollSource.current === 'programmatic'
            )
                return;
            scrollSource.current = 'track';
            container.scrollLeft = el.scrollLeft;
            requestAnimationFrame(() => {
                if (scrollSource.current === 'track')
                    scrollSource.current = null;
            });
        };

        el.addEventListener('scroll', onScroll, { passive: true });
        return () => el.removeEventListener('scroll', onScroll);
    }, [isOverflowing]);

    // Scroll active tab into view
    useEffect(() => {
        const container = scrollRef.current;
        if (!container) return;

        const activeTrigger = container.querySelector<HTMLElement>(
            `[data-state="active"]`
        );
        if (!activeTrigger) return;

        const containerRect = container.getBoundingClientRect();
        const triggerRect = activeTrigger.getBoundingClientRect();

        const targetScrollLeft = Math.max(
            0,
            Math.min(
                triggerRect.left -
                    containerRect.left +
                    container.scrollLeft -
                    containerRect.width / 2 +
                    triggerRect.width / 2,
                container.scrollWidth - container.clientWidth
            )
        );

        if (Math.abs(container.scrollLeft - targetScrollLeft) < 1) return;

        scrollSource.current = 'programmatic';

        container.scrollTo({ left: targetScrollLeft, behavior: 'smooth' });

        const fallbackResetTimeout = window.setTimeout(() => {
            scrollSource.current = null;
        }, 350);

        const onScrollEnd = () => {
            window.clearTimeout(fallbackResetTimeout);
            scrollSource.current = null;
            container.removeEventListener('scrollend', onScrollEnd);
        };
        container.addEventListener('scrollend', onScrollEnd, { once: true });

        return () => {
            window.clearTimeout(fallbackResetTimeout);
            scrollSource.current = null;
            container.removeEventListener('scrollend', onScrollEnd);
        };
    }, [activeTabValue]);

    return (
        <>
            <header
                style={{ boxShadow: '0px 4px 4px 0px #00000040' }}
                className="flex w-full items-center"
            >
                <Tabs
                    value={activeTabValue}
                    onValueChange={onTabChange}
                    className="w-full"
                >
                    <div
                        ref={scrollRef}
                        className="w-full overflow-x-auto scrollbar-none"
                    >
                        <TabsList className="h-auto w-full rounded-none bg-transparent p-0 text-sm text-black">
                            {tabs.map((tab) => (
                                <TabsTrigger
                                    key={tab.id}
                                    value={tab.id}
                                    className="min-w-[120px] rounded-none px-6 py-3 font-semibold data-[state=active]:bg-quaternary data-[state=active]:text-white"
                                >
                                    {tab.name}
                                </TabsTrigger>
                            ))}
                        </TabsList>
                    </div>
                </Tabs>
            </header>

            {isOverflowing && (
                <div
                    ref={trackRef}
                    className="pointer-fine-scrollbar w-full overflow-x-auto"
                    style={{ marginTop: SCROLLBAR_GAP }}
                >
                    <div style={{ width: scrollWidth, height: 1 }} />
                </div>
            )}
        </>
    );
};

export default TablesHeader;
