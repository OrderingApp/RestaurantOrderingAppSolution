'use client';

import { useEffect, useState } from 'react';
import { TransformWrapper, TransformComponent } from 'react-zoom-pan-pinch';
import { cn } from '@/lib/utils';

import TablesHeader from './Header';
import Table from './Table';
import AsidesView from '@/components/shared/views/Asides';
import { CURRENCIES, ORDER_TYPES } from '@/helpers/constants/constants';
import OverviewModal from '@/components/shared/modals/OverviewModal';
import CreateOrder from '../../shared/modals/CreateOrder';
import SplitBill from '@/components/shared/modals/SplitBill';
import useQueryOrders from '@/helpers/queries/orders/useQueryOrders';
import useQueryAreas from '@/helpers/queries/areas/useAreasQuery';
import useQuerySingleOrder from '@/helpers/queries/orders/useQuerySingleOrder';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import { useLanguage } from '@/providers/LanguageProvider';
import languagePacks from '@/helpers/constants/languagePacks';
import { getAggregatedDineInOrdersForTable } from '@/helpers/utils/orderTransforms';
import { getIngredientAnnotations } from '@/helpers/utils/ingredientAnnotations';

const Tables = () => {
    const { language } = useLanguage();
    const [isCreateOrderModalOpen, setIsCreateOrderModalOpen] = useState(false);
    const [isPanning, setIsPanning] = useState(false);
    const [currentTableId, setCurrentTableId] = useState<string | null>(null);
    const [currentAreaId, setCurrentAreaId] = useState<string | null>(null);
    const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
    const [splitBillOrderId, setSplitBillOrderId] = useState<string | null>(
        null
    );

    const toggleCreateOrderModal = (tableId?: string | Event) => {
        // sometimes this is used as an onClick handler and receives the click event
        // guard against DOM/event being passed as tableId
        if (typeof tableId === 'string') setCurrentTableId(tableId);
        setIsCreateOrderModalOpen((prev) => !prev);
    };

    const { data: allOrders } = useQueryOrders({
        queryKeys: [OrdersItems.BY_TYPE, ORDER_TYPES.DINEIN],
    });
    const { data: areas = [] } = useQueryAreas();
    const { data: splitBillOrderData } = useQuerySingleOrder(
        splitBillOrderId || ''
    );

    const activeAreaId =
        currentAreaId && areas.some((area) => area.id === currentAreaId)
            ? currentAreaId
            : (areas[0]?.id ?? '');

    const selectedArea = areas.find((area) => area.id === activeAreaId) ?? null;
    const tables = selectedArea?.tables ?? [];

    const selectedTableGuid = currentTableId
        ? tables?.find((t) => t.id === currentTableId)?.id
        : undefined;

    const ordersForTable = getAggregatedDineInOrdersForTable(
        allOrders || [],
        selectedTableGuid
    );

    const { detailsAside } = languagePacks[language];
    const splitBillLabel = languagePacks[language].splitBillModal.splitBill;
    const isSplitBillOpen = !!splitBillOrderId;

    const receiptLabel = detailsAside.receipt;
    const tableName =
        currentTableId && tables
            ? tables.find((t) => t.id === currentTableId)?.name || ''
            : '';

    useEffect(() => {
        if (!ordersForTable.length) {
            setSelectedOrderId(null);
            return;
        }

        const selectedStillExists = ordersForTable.some(
            (order) => order.id === selectedOrderId
        );

        if (!selectedStillExists) {
            setSelectedOrderId(ordersForTable[0].id);
        }
    }, [ordersForTable, selectedOrderId]);

    const handleOpenSplitBill = () => {
        if (!selectedOrderId) return;
        setSplitBillOrderId(selectedOrderId);
    };

    const handleCloseSplitBill = () => {
        setSplitBillOrderId(null);
    };

    const details = {
        ...detailsMock,
        title: tableName ? `${detailsAside.table} ${tableName}` : '',
        items: ordersForTable.map((o, index) => ({
            id: o.id,
            name: `${receiptLabel} ${index + 1}`,
            price: o.totalAmount || 0,
            currency: 'pln' as keyof typeof CURRENCIES,
            nestedItems: o.orderItems.map((it) => {
                const annotation = getIngredientAnnotations(it);
                return {
                    name: it.menuItem.name,
                    price: it.price,
                    currency: 'pln' as keyof typeof CURRENCIES,
                    quantity: it.quantity,
                    annotation,
                    annotationClassName: annotation
                        ? 'text-dark-gray font-normal'
                        : undefined,
                    onClick: () =>
                        console.log(`item ${it.menuItem.name} clicked`),
                };
            }),
        })),
        onAddNewOrder: toggleCreateOrderModal,
        onSelectItem: setSelectedOrderId,
        selectedItemId: selectedOrderId,
        buttons: [
            {
                children: splitBillLabel,
                onClick: handleOpenSplitBill,
                disabled: !selectedOrderId,
                variant: 'primary' as const,
            },
        ],
    };

    return (
        <>
            <AsidesView
                details={details}
                bottom={bottomMock}
                isBottomAsideShown={true}
            >
                <div className="flex flex-col h-full">
                    <TablesHeader
                        tabs={areas}
                        activeTabValue={activeAreaId}
                        onTabChange={(newAreaId) => {
                            setCurrentAreaId(newAreaId);
                            setCurrentTableId(null);
                        }}
                    />
                    <section className="relative h-full max-h-[559px] z-30">
                        <TransformWrapper
                            initialScale={0.65}
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
                                                table={table}
                                                orders={allOrders || []}
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

            <OverviewModal isOpen={isSplitBillOpen}>
                {splitBillOrderId && (
                    <SplitBill
                        onClose={handleCloseSplitBill}
                        orderId={splitBillOrderId}
                        orderItems={splitBillOrderData?.orderItems ?? []}
                        onSplitSuccess={handleCloseSplitBill}
                    />
                )}
            </OverviewModal>
        </>
    );
};

const detailsMock = {
    served: true,
};

const bottomMock = {
    reservations: [],
};

export default Tables;
