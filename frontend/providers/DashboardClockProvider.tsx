'use client';

import { createContext, useContext, useEffect, useMemo, useState } from 'react';

const DashboardClockContext = createContext<number | null>(null);

export const DashboardClockProvider = ({
    children,
}: {
    children: React.ReactNode;
}) => {
    const [nowMs, setNowMs] = useState(() => Date.now());

    useEffect(() => {
        const intervalId = window.setInterval(() => {
            setNowMs(Date.now());
        }, 1000);

        return () => window.clearInterval(intervalId);
    }, []);

    const value = useMemo(() => nowMs, [nowMs]);

    return (
        <DashboardClockContext.Provider value={value}>
            {children}
        </DashboardClockContext.Provider>
    );
};

export const useDashboardClock = () => {
    const context = useContext(DashboardClockContext);

    if (context === null) {
        throw new Error(
            'useDashboardClock must be used within DashboardClockProvider'
        );
    }

    return context;
};
