'use client';

import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import type { ReactNode } from 'react';
import 'dayjs/locale/pl';

export default function MuiLocalizationProvider({
    children,
}: {
    children: ReactNode;
}) {
    return (
        <LocalizationProvider dateAdapter={AdapterDayjs} adapterLocale="pl">
            {children}
        </LocalizationProvider>
    );
}
