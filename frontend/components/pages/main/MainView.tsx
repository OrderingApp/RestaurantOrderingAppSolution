'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';

import useLanguage from '@/helpers/hooks/useLanguage';
import languagePacks from '@/helpers/constants/languagePacks';
import {
    ORDER_STATUSES,
    ORDER_TYPES,
    PAYMENT_STATUSES,
    SEARCH_PARAMS_NAMES,
} from '@/helpers/constants/constants';
import { formatDate, todayISO } from '@/helpers/utils/dates';
import {
    getDineInOrders,
    sortOrdersByCreatedAt,
} from '@/helpers/utils/orderTransforms';

import useQueryOrders from '@/helpers/queries/orders/useQueryOrders';
import useQueryOrdersByType from '@/helpers/queries/orders/useQueryOrdersByType';
import useQueryReservations from '@/helpers/queries/reservations/useQueryReservations';
import useQueryAreas from '@/helpers/queries/areas/useAreasQuery';
import ReservationsColumn, {
    DineInOrdersColumn,
    NonDineInColumn,
} from '@/components/shared/columns/Columns';
import { DashboardClockProvider } from '@/providers/DashboardClockProvider';

const MainView = () => {
    const router = useRouter();
    const { language } = useLanguage();
    const { mainPage } = languagePacks[language];
    const [now, setNow] = useState(new Date());

    useEffect(() => {
        const interval = setInterval(() => setNow(new Date()), 30_000);
        return () => clearInterval(interval);
    }, []);

    const dateInfo = formatDate(now, language);
    const today = todayISO();

    const { data: reservations = [] } = useQueryReservations(today);

    const { data: allOrders = [] } = useQueryOrders({ queryKeys: [] });

    const { data: deliveryOrders = [] } = useQueryOrdersByType({
        type: ORDER_TYPES.DELIVERY,
        statuses: [ORDER_STATUSES.ONGOING, ORDER_STATUSES.COMPLETED],
        paymentStatuses: [
            PAYMENT_STATUSES.UNPAID,
            PAYMENT_STATUSES.PARTIALPAID,
            PAYMENT_STATUSES.PAID,
        ],
    });

    const { data: pickupOrders = [] } = useQueryOrdersByType({
        type: ORDER_TYPES.TAKEAWAY,
        statuses: [ORDER_STATUSES.ONGOING, ORDER_STATUSES.COMPLETED],
        paymentStatuses: [
            PAYMENT_STATUSES.UNPAID,
            PAYMENT_STATUSES.PARTIALPAID,
            PAYMENT_STATUSES.PAID,
        ],
    });

    const { data: areas = [] } = useQueryAreas();

    const tableNameMap = (() => {
        const map = new Map<string, string>();
        for (const area of areas) {
            for (const table of area.tables) {
                map.set(table.id, table.name);
            }
        }
        return map;
    })();

    const dineInOrders = sortOrdersByCreatedAt(
        getDineInOrders(allOrders).filter(
            (o) =>
                o.orderStatus === ORDER_STATUSES.ONGOING ||
                o.orderStatus === ORDER_STATUSES.COMPLETED
        )
    );

    const openReservation = (id: string) => {
        router.push(
            `/reservations?${SEARCH_PARAMS_NAMES.MODAL}=true&${SEARCH_PARAMS_NAMES.RESERVATION}=${id}&${SEARCH_PARAMS_NAMES.FROM}=main`
        );
    };

    const openOrderOptions = (orderId: string) => {
        router.push(
            `/orders?${SEARCH_PARAMS_NAMES.ORDER_ID}=${orderId}&${SEARCH_PARAMS_NAMES.FROM}=main`
        );
    };

    const goToTables = () => router.push('/tables');

    return (
        <div className="flex flex-col h-full">
            <header className="flex items-center justify-between px-6 py-3 bg-white shadow-[0_2px_8px_rgba(0,0,0,0.10)]">
                <div className="flex items-center gap-2 bg-[#2C5364] px-3 py-1 rounded-full">
                    <span className=" text-white text-sm rounded-full font-semibold tracking-wide">
                        {dateInfo.date} – {dateInfo.day}.{dateInfo.monthNumber}.
                        {dateInfo.year}
                    </span>
                    <span className="text-white text-md px-5 py-2 rounded-full font-semibold tracking-wide tabular-nums">
                        {dateInfo.time}
                    </span>
                </div>
                <button className="w-9 h-9 rounded-full border-2 border-[#2C5364] flex items-center justify-center text-[#2C5364] font-bold text-sm hover:bg-[#2C5364] hover:text-white transition-colors shadow-sm">
                    i
                </button>
            </header>

            {/* Columns */}
            <DashboardClockProvider>
                <section className="flex-1 grid grid-cols-4 gap-4 px-6 pt-4 pb-4 overflow-hidden">
                    <ReservationsColumn
                        reservations={reservations}
                        title={mainPage.reservations}
                        language={language}
                        onCardClick={openReservation}
                    />

                    <DineInOrdersColumn
                        orders={dineInOrders}
                        tables={tableNameMap}
                        title={mainPage.orders}
                        billCountLabel={mainPage.billCount}
                        onCardClick={goToTables}
                    />

                    <NonDineInColumn
                        orders={deliveryOrders}
                        title={mainPage.deliveries}
                        type={ORDER_TYPES.DELIVERY}
                        asapLabel={mainPage.asap}
                        language={language}
                        onCardClick={openOrderOptions}
                    />

                    <NonDineInColumn
                        orders={pickupOrders}
                        title={mainPage.pickups}
                        type={ORDER_TYPES.TAKEAWAY}
                        asapLabel={mainPage.asap}
                        language={language}
                        onCardClick={openOrderOptions}
                    />
                </section>
            </DashboardClockProvider>
        </div>
    );
};

export default MainView;
