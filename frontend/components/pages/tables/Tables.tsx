'use client';

import { useState } from 'react';
import { TransformWrapper, TransformComponent } from 'react-zoom-pan-pinch';
import { cn } from '@/lib/utils';

import TablesHeader from './Header';
import Table from './Table';
import AsidesView from '@/components/shared/views/Asides';
import { CURRENCIES } from '@/helpers/constants/constants';
import OverviewModal from '@/components/shared/modals/OverviewModal';
import CreateOrder from '../../shared/modals/CreateOrder';
import useQueryOrders from '@/helpers/queries/orders/useQueryOrders';
import { useQuery } from '@tanstack/react-query';
import { BACKEND_URL } from '@/helpers/constants/constants';

const INITIAL_TABLES_DATA = [
    { id: 'table-alpha', layout: [3] },
    { id: 'table-beta', layout: [2, 1] },
    { id: 'table-gamma', layout: [1, 2] },
    { id: 'table-delta', layout: [4, 2, 1] },
    { id: 'table-deltaa', layout: [2, 2, 1] },
];

const Tables = () => {
    const [isCreateOrderModalOpen, setIsCreateOrderModalOpen] = useState(false);
    const [isPanning, setIsPanning] = useState(false);
    const [currentTableId, setCurrentTableId] = useState<string | null>(null);
    const toggleCreateOrderModal = (tableId?: string | Event) => {
        // sometimes this is used as an onClick handler and receives the click event
        // guard against DOM/event being passed as tableId
        if (typeof tableId === 'string') setCurrentTableId(tableId);
        setIsCreateOrderModalOpen((prev) => !prev);
    };

    const { data: allOrders } = useQueryOrders();
    const { data: tables } = useQuery({
        queryKey: ['tables'],
        queryFn: async () => {
            const res = await fetch(`${BACKEND_URL}/tables`);
            if (!res.ok) return [];
            return res.json();
        },
    });

    const dineInOrders = (allOrders || []).filter(
        (o) => o.orderType === 'dinein'
    );

    // map currentTableId (ui id) to backend table guid if possible
    const selectedTableGuid = (() => {
        if (!tables) return undefined;
        const match = tables.find((t: { id: string; name?: string }) =>
            t.name?.toLowerCase().includes((currentTableId || '').toLowerCase())
        );
        return match ? match.id : undefined;
    })();

    const ordersForTable = selectedTableGuid
        ? dineInOrders.filter((o) => o.tableId === selectedTableGuid)
        : [];

    const humanize = (id: string | null) =>
        id ? id.replace('table-', 'Stolik ').replace(/-/g, ' ') : '';

    const details = {
        ...detailsMock,
        title: humanize(currentTableId),
        items: ordersForTable.map((o) => ({
            id: o.id,
            name: `Rachunek ${o.id.slice(0, 4)}`,
            price: o.totalAmount || 0,
            currency: 'pln' as keyof typeof CURRENCIES,
            nestedItems: o.orderItems.map((it) => ({
                name: it.menuItem.name,
                price: it.menuItem.price,
                currency: 'pln' as keyof typeof CURRENCIES,
                quantity:
                    (it as unknown as { quantity?: number }).quantity ?? 1,
                onClick: () => {},
            })),
        })),
        onAddNewOrder: toggleCreateOrderModal,
        buttons: detailsMock.buttons?.filter(
            (b) => b.children !== 'otwórz rachunek'
        ),
    };

    return (
        <>
            <AsidesView
                details={details}
                bottom={bottomMock}
                isBottomAsideShown={true}
            >
                <TablesHeader onTabChange={console.log} />

                <section className="relative h-full" style={{ zIndex: 25 }}>
                    <div className="absolute inset-0 bg-[#F7F7F8] z-0" />
                    <TransformWrapper
                        initialScale={0.55}
                        minScale={0.25}
                        maxScale={1.25}
                        centerOnInit={true}
                        limitToBounds={true}
                        onPanningStart={() => setIsPanning(true)}
                        onPanningStop={() => setIsPanning(false)}
                    >
                        <TransformComponent
                            wrapperClass={cn(
                                '!h-full !w-full relative',
                                isPanning ? 'cursor-grabbing' : 'cursor-grab'
                            )}
                        >
                            <ul className="relative z-10 grid h-full w-full grid-cols-[1fr,1fr,1fr] gap-y-[52px] gap-x-24 items-start p-8 pt-20">
                                {INITIAL_TABLES_DATA.map((table) => (
                                    <li key={table.id}>
                                        <Table
                                            id={table.id}
                                            onSelect={(id) =>
                                                setCurrentTableId(id)
                                            }
                                            selected={
                                                currentTableId === table.id
                                            }
                                        />
                                    </li>
                                ))}
                            </ul>
                        </TransformComponent>
                    </TransformWrapper>
                </section>
            </AsidesView>

            <OverviewModal isOpen={isCreateOrderModalOpen}>
                <CreateOrder
                    toggleModal={toggleCreateOrderModal}
                    skipCustomerForm={true}
                    tableId={currentTableId || undefined}
                />
            </OverviewModal>
        </>
    );
};

const items = [
    {
        id: '1st',
        name: 'Rachunek 1',
        price: 33,
        currency: 'pln' as keyof typeof CURRENCIES,
        nestedItems: [
            {
                name: 'rosa 1',
                price: 33,
                currency: 'pln' as keyof typeof CURRENCIES,
                quantity: 1,
                onClick: () => {
                    console.log(`item 1 clicked`);
                },
                isServed: true,
            },
            {
                name: 'rosa 2',
                price: 33,
                currency: 'pln' as keyof typeof CURRENCIES,
                quantity: 2,
                annotation: ['+mozarella', '-mozzarella', '+mozarella'],
                onClick: () => {
                    console.log(`item 2 clicked`);
                },
            },
            {
                name: 'pizza Margheritta',
                price: 33,
                currency: 'pln' as keyof typeof CURRENCIES,
                quantity: 1,
                onClick: () => {
                    console.log(`item 3 clicked`);
                },
                isServed: true,
            },
            {
                name: 'rosa 3',
                price: 33,
                currency: 'pln' as keyof typeof CURRENCIES,
                quantity: 2,
                annotation: ['+mozarella', '-mozzarella'],
                onClick: () => {
                    console.log(`item 4 clicked`);
                },
            },
        ],
    },
    {
        id: '2nd',
        name: 'Rachunek 2',
        price: 33,
        currency: 'pln' as keyof typeof CURRENCIES,
        nestedItems: [
            {
                name: 'rosa 1',
                price: 33,
                currency: 'pln' as keyof typeof CURRENCIES,
                quantity: 1,
                onClick: () => {
                    console.log(`item 1 clicked`);
                },
                isServed: true,
            },
            {
                name: 'rosa 2',
                price: 33,
                currency: 'pln' as keyof typeof CURRENCIES,
                quantity: 2,
                annotation: ['+mozarella', '-mozzarella', '+mozarella'],
                onClick: () => {
                    console.log(`item 2 clicked`);
                },
            },
            {
                name: 'pizza Margheritta',
                price: 33,
                currency: 'pln' as keyof typeof CURRENCIES,
                quantity: 1,
                onClick: () => {
                    console.log(`item 3 clicked`);
                },
                isServed: true,
            },
            {
                name: 'rosa 3',
                price: 33,
                currency: 'pln' as keyof typeof CURRENCIES,
                quantity: 2,
                annotation: ['+mozarella', '-mozzarella'],
                onClick: () => {
                    console.log(`item 4 clicked`);
                },
            },
        ],
    },
    {
        id: '3rd',
        name: 'Rachunek 3',
        price: 33,
        currency: 'pln' as keyof typeof CURRENCIES,
        nestedItems: [
            {
                name: 'rosa 1',
                price: 33,
                currency: 'pln' as keyof typeof CURRENCIES,
                quantity: 1,
                onClick: () => {
                    console.log(`item 1 clicked`);
                },
                isServed: true,
            },
            {
                name: 'rosa 2',
                price: 33,
                currency: 'pln' as keyof typeof CURRENCIES,
                quantity: 2,
                annotation: ['+mozarella', '-mozzarella', '+mozarella'],
                onClick: () => {
                    console.log(`item 2 clicked`);
                },
            },
            {
                name: 'pizza Margheritta',
                price: 33,
                currency: 'pln' as keyof typeof CURRENCIES,
                quantity: 1,
                onClick: () => {
                    console.log(`item 3 clicked`);
                },
                isServed: true,
            },
            {
                name: 'rosa 3',
                price: 33,
                currency: 'pln' as keyof typeof CURRENCIES,
                quantity: 2,
                annotation: ['+mozarella', '-mozzarella'],
                onClick: () => {
                    console.log(`item 4 clicked`);
                },
            },
        ],
    },
];

const mockReservationCards = [
    {
        scheduledFor: '2025-08-10T20:30:00Z',
        name: 'Bob The Builder',
        capacityNeeded: 2,
        phoneNumber: '+15559876543',
        onClick: () => console.log('Clicked reservation 2'),
        className: 'bg-blue-50',
        id: 'res-005',
        isAssigned: 1,
        tableId: 'Tbl-VIP',
    },
];

const buttons = [
    {
        children: 'otwórz rachunek',
        onClick: () => console.log('otworz clicked'),
    },
    { children: 'zamknij rachunek' },
];

const detailsMock = {
    title: 'stolik b2',
    items,
    served: true,
    buttons: buttons,
    onAddNewOrder: () => console.log('essa'),
};

const bottomMock = {
    reservations: mockReservationCards,
};

export default Tables;
