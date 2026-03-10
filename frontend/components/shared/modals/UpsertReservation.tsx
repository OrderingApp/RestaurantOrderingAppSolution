import Image from 'next/image';
import React, { useEffect, useState } from 'react';
import { useSearchParams } from 'next/navigation';

import ReservationForm from '@/components/reservations/ReservationForm';

import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Skeleton } from '@/components/ui/skeleton';
import { toast } from 'sonner';

import { useReservationContext } from '@/providers/ReservationsContext';

import { ICONS } from '@/helpers/constants/icons/icons';
import { SEARCH_PARAMS_NAMES } from '@/helpers/constants/constants';
import { formatDate } from '@/helpers/utils/dates';
import useLanguage from '@/helpers/hooks/useLanguage';
import languagePacks from '@/helpers/constants/languagePacks';
import { AreaReservation } from '@/helpers/queries/areas/useAreasQuery';

const MODAL_WIDTH = '830px';
const MODAL_HEIGHT = '510px';

interface ExpandableTableRowProps {
    tableId: string;
    tableName: string;
    reservations?: AreaReservation[];
    editedReservationId?: string | null;
}

const ExpandableTableRow = ({
    tableId,
    tableName,
    reservations,
    editedReservationId,
}: ExpandableTableRowProps) => {
    const [isOpenTable, setIsOpenTable] = useState(false);

    const { language } = useLanguage();

    const {
        createReservationPage: {
            reservationsList: {
                tableExpanded: { messageNoReservations },
            },
            toasts: {
                error: { missingDateTimeAndGuests },
            },
        },
    } = languagePacks[language];

    const {
        addLocalReservation,
        updateForm,
        form: { date, time, peopleCount, selectedTableId },
        removeLocalReservation,
    } = useReservationContext();

    const isSelected = selectedTableId === tableId;

    const addTableReservation = (e: React.MouseEvent) => {
        e.stopPropagation();
        if (!date || !time || !peopleCount) {
            toast.error(missingDateTimeAndGuests);
            return;
        }

        updateForm('selectedTableId', tableId);
        addLocalReservation(tableId, tableName);

        setIsOpenTable(true);
    };

    useEffect(() => {
        if (isSelected) {
            setIsOpenTable(true);
        }
    }, [isSelected]);

    return (
        <div className="border-b border-gray-100 last:border-0">
            <div
                onClick={() => setIsOpenTable(!isOpenTable)}
                className={`grid grid-cols-3 px-6 py-3 cursor-pointer transition-colors text-sm items-center ${isOpenTable ? 'bg-primary text-white' : 'bg-white text-black'}`}
            >
                <div className="flex items-center gap-2 font-medium col-start-1">
                    {tableName}
                </div>
                <div className="col-start-3 flex justify-end">
                    <button
                        onClick={addTableReservation}
                        className={`${isOpenTable ? 'bg-white' : 'bg-primary'} w-6 h-6 flex items-center justify-center rounded-md transition-all z-10`}
                    >
                        <Image
                            className="w-4"
                            src={
                                isOpenTable
                                    ? ICONS.ADD_CIRCLE
                                    : ICONS.ADD_CIRCLE_WHITE
                            }
                            alt="addcircle"
                        />
                    </button>
                </div>
            </div>

            {isOpenTable &&
                reservations?.map((res) => (
                    <div
                        className="bg-[#E6E6E6] px-8 pr-2 py-2 text-sm text-gray-600 border-t border-gray-100 shadow-inner grid grid-cols-3"
                        key={res.id}
                    >
                        <div className="text-left flex items-center">
                            <p className="text-xs">{tableName}</p>
                        </div>
                        <div className="text-center flex items-center justify-center">
                            <p className="text-xs">{res.capacityNeeded}</p>
                        </div>
                        <div className="text-right flex justify-end gap-2 items-center">
                            <p className="text-xs">
                                {
                                    formatDate(
                                        new Date(res.scheduledFor),
                                        language
                                    ).time
                                }
                            </p>
                            <button
                                onClick={() => {
                                    if (
                                        editedReservationId &&
                                        res.id !== editedReservationId
                                    ) {
                                        return;
                                    }

                                    removeLocalReservation(tableId, res.id);
                                }}
                                disabled={
                                    !!editedReservationId &&
                                    res.id !== editedReservationId
                                }
                                className="bg-danger w-6 h-6 flex items-center justify-center rounded-md transition-all disabled:opacity-40 disabled:cursor-not-allowed"
                            >
                                <Image
                                    src={ICONS.DELETE}
                                    alt="delete"
                                    className="w-4 h-4"
                                />
                            </button>
                        </div>
                    </div>
                ))}

            {isOpenTable && reservations?.length === 0 && (
                <div className="bg-[#E6E6E6] px-8 py-2 text-sm text-gray-600 border-t border-gray-100 shadow-inner">
                    <p className="text-xs italic">{messageNoReservations}</p>
                </div>
            )}
        </div>
    );
};

const UpsertReservation = () => {
    const [activeTabValue, setActiveTabValue] = useState('');
    const searchParams = useSearchParams();
    const { language } = useLanguage();
    const editedReservationId = searchParams.get(
        SEARCH_PARAMS_NAMES.RESERVATION
    );

    const {
        createReservationPage: {
            reservationsList: { guests, time, table },
        },
    } = languagePacks[language];

    const {
        localAreas,
        isFetching,
        form: { selectedTableId },
    } = useReservationContext();

    useEffect(() => {
        if (!activeTabValue && localAreas?.length > 0) {
            setActiveTabValue(localAreas[0].id);
        }
    }, [localAreas, activeTabValue]);

    useEffect(() => {
        if (!selectedTableId || !localAreas?.length) return;

        const foundArea = localAreas.find((area) =>
            area.tables.some((table) => table.id === selectedTableId)
        );

        if (foundArea && foundArea.id !== activeTabValue) {
            setActiveTabValue(foundArea.id);
        }
    }, [selectedTableId, localAreas, activeTabValue]);

    return (
        <div
            className="bg-reservation-gradient rounded-2xl relative flex"
            style={{ width: MODAL_WIDTH, height: MODAL_HEIGHT }}
        >
            <div className="w-1/2">
                <div className="bg-white mt-[0.4rem] h-full rounded-tl-2xl rounded-bl-2xl px-6 py-4 flex flex-col">
                    <ReservationForm />
                </div>
            </div>

            <div className="w-1/2 bg-white mt-[0.4rem] h-full rounded-tr-2xl rounded-br-2xl border-l border-gray-100 overflow-scroll">
                {isFetching && <Skeleton></Skeleton>}
                <Tabs
                    value={activeTabValue}
                    onValueChange={setActiveTabValue}
                    className=""
                >
                    <TabsList className="flex w-full p-0 justify-start flex-wrap bg-gray-100/50 rounded-lg h-full">
                        {localAreas?.map(({ name, id }) => (
                            <TabsTrigger
                                key={id}
                                onClick={() => setActiveTabValue(id)}
                                value={id}
                                className="flex-1 px-6 py-2 rounded-none text-xs
                                    text-black transition-all duration-200
                                    hover:bg-gray-200/50 hover:text-gray-900
                                    data-[state=active]:bg-primary
                                    data-[state=active]:text-white 
                                    data-[state=active]:shadow-sm"
                            >
                                {name}
                            </TabsTrigger>
                        ))}
                    </TabsList>
                    <div className="grid grid-cols-3 bg-primary text-white px-6 py-2  text-xs font-medium">
                        <div>{table}</div>
                        <div className="text-center">{guests}</div>
                        <div className="text-right">{time}</div>
                    </div>

                    {localAreas?.map((area) => (
                        <TabsContent
                            className="m-0"
                            key={area.id}
                            value={area.id}
                        >
                            {area.tables.map((table) => (
                                <ExpandableTableRow
                                    tableId={table.id}
                                    key={table.id}
                                    tableName={table.name}
                                    reservations={table.reservations}
                                    editedReservationId={editedReservationId}
                                />
                            ))}
                        </TabsContent>
                    ))}
                </Tabs>
            </div>
        </div>
    );
};

export default UpsertReservation;
