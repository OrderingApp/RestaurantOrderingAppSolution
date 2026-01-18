'use client';

import { useState } from 'react';
import { TransformWrapper, TransformComponent } from 'react-zoom-pan-pinch';
import { cn } from '@/lib/utils';

import TablesHeader from './Header';
import Table from './Table';
import AsidesView from '@/components/shared/views/Asides';
import { CURRENCIES } from '@/helpers/constants/constants';
import OverviewModal from '@/components/shared/modals/OverviewModal';
import CreateOrder from '../orders/CreateOrder';

const INITIAL_TABLES_DATA = [
    { id: 'table-alpha', layout: [3] },
    { id: 'table-beta', layout: [2, 1] },
    { id: 'table-gamma', layout: [1, 2] },
    { id: 'table-delta', layout: [4, 2, 1] },
    { id: 'table-deltaa', layout: [2, 2, 1] },
];

const Tables = () => {
    const [isPanning, setIsPanning] = useState(false);
    const [isCreateOrderModalOpen, setIsCreateOrderModalOpen] = useState(false);

    const toggleCreateOrderModal = () =>
        setIsCreateOrderModalOpen((prev) => !prev);

    return (
        <>
            <AsidesView
                details={{
                    ...detailsMock,
                    onAddNewOrder: toggleCreateOrderModal,
                }}
                bottom={bottomMock}
                isBottomAsideShown={true}
            >
                <TablesHeader onTabChange={console.log} />

                <section>
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
                                        />
                                    </li>
                                ))}
                            </ul>
                        </TransformComponent>
                    </TransformWrapper>
                </section>
            </AsidesView>

            <OverviewModal isOpen={isCreateOrderModalOpen}>
                <CreateOrder toggleModal={toggleCreateOrderModal} />
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
