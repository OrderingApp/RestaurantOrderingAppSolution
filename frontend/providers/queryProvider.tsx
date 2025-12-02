'use client';

import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useState, type ReactNode } from 'react';

const QueryProvider = ({ children }: { children: ReactNode }) => {
    const [client] = useState(() => new QueryClient());

    return (
        <QueryClientProvider client={client}>{children}</QueryClientProvider>
    );
};

export default QueryProvider;

//TODO: check if queryClient is actually running on both client instances
