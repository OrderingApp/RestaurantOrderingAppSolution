'use client';

import AsidesView from '@/components/shared/views/Asides';
import { CURRENCIES } from '@/helpers/constants/constants';
import TablesHeader from './Header';

const Tables = () => (
    <AsidesView
        details={detailsMock}
        bottom={bottomMock}
        isBottomAsideShown={true}
    >
        <TablesHeader onTabChange={console.log} />
        <p>elo</p>
    </AsidesView>
);

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

// eslint-disable-next-line
const ingredients = [
    {
        name: 'Mozarella',
        price: 33,
        currency: 'pln' as keyof typeof CURRENCIES,
        isSingleItem: true,
        onClick: () => console.log('ing clicked'),
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
