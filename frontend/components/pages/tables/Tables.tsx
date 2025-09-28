'use client';

import { useState } from 'react';
// import { DndContext } from '@dnd-kit/core';
import { TransformWrapper, TransformComponent } from 'react-zoom-pan-pinch';
import { cn } from '@/lib/utils';

import TablesHeader from './Header';
import Table from './Table';
import AsidesView from '@/components/shared/views/Asides';
import { CURRENCIES } from '@/helpers/constants/constants';

export interface TableData {
    id: string;
    layout: number[]; // This now defines rows/seats within the table
    x?: number; // Keep for future canvas dragging
    y?: number; // Keep for future canvas dragging
}

const INITIAL_TABLES_DATA: TableData[] = [
    { id: 'table-alpha', layout: [3], x: 0, y: 0 },
    { id: 'table-beta', layout: [2, 1], x: 250, y: 0 },
    { id: 'table-gamma', layout: [1, 2], x: 350, y: 0 },
    // ... rest of your tables
];

// const INITIAL_TABLES_DATA = [
//     { id: 'table-alpha', layout: [3] },
//     { id: 'table-beta', layout: [2, 1] },
//     { id: 'table-gamma', layout: [1, 2] },
//     { id: 'table-delta', layout: [4, 2, 1] },
//     { id: 'table-deltaa', layout: [2, 2, 1] },
// ];

const Tables = () => {
    const [isPanning, setIsPanning] = useState(false);
    const [isEditing, setIsEditing] = useState(false);
    const [isSeatDragging, setIsSeatDragging] = useState(false);

    const handleSeatDragChange = (isDragging: boolean) => {
        setIsSeatDragging(isDragging);
    };

    const detailsMockExtended = {
        ...detailsMock,
        buttons: [
            {
                children: isEditing ? 'Zapisz edycję' : 'Edytuj',
                onClick: () => setIsEditing((p) => !p),
            },
            ...detailsMock.buttons,
        ],
    };
    return (
        <AsidesView
            details={detailsMockExtended}
            bottom={bottomMock}
            isBottomAsideShown={true}
        >
            <TablesHeader onTabChange={console.log} />

            <section>
                {/* <DndContext> */}
                <TransformWrapper
                    initialScale={0.55}
                    minScale={0.25}
                    maxScale={1.25}
                    centerOnInit={true}
                    limitToBounds={true}
                    onPanningStart={() => setIsPanning(true)}
                    onPanningStop={() => setIsPanning(false)}
                    panning={{
                        disabled: isSeatDragging,
                        velocityDisabled: isSeatDragging,
                    }}
                    pinch={{ disabled: isSeatDragging }}
                    wheel={{ disabled: isSeatDragging }}
                    doubleClick={{ disabled: isSeatDragging }}
                >
                    <TransformComponent
                        wrapperClass={cn(
                            '!h-full !w-full',
                            isPanning ? 'cursor-grabbing' : 'cursor-grab'
                        )}
                    >
                        {/* automate grid cols depending on the tables? so e.g.
                        semi-smart distribution, or baed on the user input how
                        he put it in edit mode? */}
                        <ul className="grid grid-cols-[1fr,1fr,1fr] gap-y-[52px] gap-x-24 items-center p-8 pt-20">
                            {INITIAL_TABLES_DATA.map((table) => (
                                <li key={table.id}>
                                    <Table
                                        id={table.id}
                                        layout={table.layout}
                                        onSeatDragChange={handleSeatDragChange}
                                    />
                                </li>
                            ))}
                        </ul>
                    </TransformComponent>
                </TransformWrapper>
                {/* </DndContext> */}
            </section>
        </AsidesView>
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
        capacityNeeded: 2,
        dateTime: '2025-08-10T20:30:00Z',
        subtitle: 'Online Request',
        onClick: () => console.log('Clicked reservation 2'),
        className: 'bg-blue-50',

        id: 'res-002',
        phoneNumber: '+15559876543',
        name: 'Bob The Builder',
        isAssigned: 1,
        tableId: 'Tbl-A3',
    },
    {
        capacityNeeded: 5,
        dateTime: '2025-08-11T18:45:00Z',
        subtitle: 'VIP Guest',
        onClick: () => console.log('Clicked reservation 5'),
        className: 'bg-purple-100 shadow-lg',

        id: 'res-005',
        phoneNumber: '+15553334444',
        name: 'Elon Musk',
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
};

const bottomMock = {
    reservations: mockReservationCards,
};

export default Tables;
