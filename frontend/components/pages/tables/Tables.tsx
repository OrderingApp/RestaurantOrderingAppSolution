'use client';

import { useMemo, useState } from 'react';
import { TransformWrapper, TransformComponent } from 'react-zoom-pan-pinch';
import { cn } from '@/lib/utils';

import TablesHeader from './Header';
import Table from './Table';
import AsidesView from '@/components/shared/views/Asides';
import { CURRENCIES, ORDER_TYPES } from '@/helpers/constants/constants';
import OverviewModal from '@/components/shared/modals/OverviewModal';
import CreateOrder from '../../shared/modals/CreateOrder';
import useQueryOrders from '@/helpers/queries/orders/useQueryOrders';
import useQueryTables from '@/helpers/queries/tables/useQueryTables';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import { useLanguage } from '@/providers/LanguageProvider';
import languagePacks from '@/helpers/constants/languagePacks';

const Tables = () => {
    const { language } = useLanguage();
    const [isCreateOrderModalOpen, setIsCreateOrderModalOpen] = useState(false);
    const [isPanning, setIsPanning] = useState(false);
    const [currentTableId, setCurrentTableId] = useState<string | null>(null);

    const toggleCreateOrderModal = (tableId?: string | Event) => {
        // sometimes this is used as an onClick handler and receives the click event
        // guard against DOM/event being passed as tableId
        if (typeof tableId === 'string') setCurrentTableId(tableId);
        setIsCreateOrderModalOpen((prev) => !prev);
    };

    const { data: allOrders } = useQueryOrders({
        queryKeys: [OrdersItems.BY_TYPE, ORDER_TYPES.DINEIN],
    });
    const { data: tables } = useQueryTables();

    const dineInOrders = useMemo(
        () =>
            (allOrders || []).filter(
                (o) =>
                    o.orderType.toLowerCase() ===
                    ORDER_TYPES.DINEIN.toLowerCase()
            ),
        [allOrders]
    );

    // map currentTableId to backend table guid
    const selectedTableGuid = currentTableId
        ? tables?.find((t) => t.id === currentTableId)?.id
        : undefined;

    const ordersForTable = selectedTableGuid
        ? dineInOrders.filter((o) => o.tableId === selectedTableGuid)
        : [];

    const { detailsAside } = languagePacks[language];
    const receiptLabel = detailsAside.receipt;
    const tableName =
        currentTableId && tables
            ? tables.find((t) => t.id === currentTableId)?.name || ''
            : '';

    const details = {
        ...detailsMock,
        title: tableName ? `${detailsAside.table} ${tableName}` : '',
        items: ordersForTable.map((o, index) => ({
            id: o.id,
            name: `${receiptLabel} ${index + 1}`,
            price: o.totalAmount || 0,
            currency: 'pln' as keyof typeof CURRENCIES,
            nestedItems: o.orderItems.map((it) => ({
                name: it.menuItem.name,
                price: it.menuItem.price,
                currency: 'pln' as keyof typeof CURRENCIES,
                quantity: 1,
                onClick: () => console.log(`item ${it.menuItem.name} clicked`),
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
                <div className="flex flex-col h-full">
                    <TablesHeader onTabChange={console.log} />
                    <section className="relative h-full max-h-[559px] z-30">
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
                                    isPanning
                                        ? 'cursor-grabbing'
                                        : 'cursor-grab'
                                )}
                            >
                                <ul className="relative z-10 grid h-full w-full grid-cols-[1fr,1fr,1fr] gap-y-[52px] gap-x-24 items-start p-8 pt-20">
                                    {tables?.map((table) => (
                                        <li key={table.id}>
                                            <Table
                                                id={table.id}
                                                name={`${detailsAside.table} ${table.name}`}
                                                capacity={table.capacity}
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
                </div>
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
