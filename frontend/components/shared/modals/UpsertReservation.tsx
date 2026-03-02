import Image from 'next/image';
import ReservationForm from '@/components/reservations/ReservationForm';
import { ICONS } from '@/helpers/constants/icons/icons';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useState } from 'react';

// Jeśli używasz ikon od Shadcn, polecam dodać strzałkę
import { formatDate } from '@/helpers/utils/dates';
import useLanguage from '@/helpers/hooks/useLanguage';
import { useReservationContext } from '@/providers/ReservationsContext';
import { Skeleton } from '@/components/ui/skeleton';

const ExpandableTableRow = ({
    tableId,
    tableName,
    reservations,
}: {
    tableId: string;
    tableName: string;
    reservations?: { peopleCount: number; time: string }[];
}) => {
    const [isOpen, setIsOpen] = useState(false);
    const { language } = useLanguage();

    // Pobieramy akcję z Contextu
    const {
        addLocalReservation,
        updateForm,
        form: { date, time, peopleCount, selectedTableId },
        setHasUnsavedChanges,
    } = useReservationContext();

    // Sprawdzamy czy ten stolik jest aktualnie kliknięty do dodawania
    // const isSelectedForAdding = selectedTableId === tableId;

    const addTableReservation = () => {
        if (!date || !time || !peopleCount) {
            alert('Proszę uzupełnić datę, godzinę i liczbę osób');
            return;
        }
        updateForm('selectedTableId', tableId);
        addLocalReservation(tableId, tableName);

        setIsOpen(true);
    };

    return (
        <div className="border-b border-gray-100 last:border-0">
            <div
                onClick={() => setIsOpen(!isOpen)}
                className={`grid grid-cols-3 px-6 py-3 cursor-pointer transition-colors text-sm items-center ${isOpen ? 'bg-primary text-white' : 'bg-white text-black'}`}
            >
                <div className="flex items-center gap-2 font-medium col-start-1">
                    {tableName}
                </div>
                <div className="col-start-3 flex justify-end">
                    <button
                        onClick={(e) => {
                            e.stopPropagation();
                            addTableReservation();
                        }}
                        className={`${isOpen ? 'bg-white' : 'bg-primary'} w-6 h-6 flex items-center justify-center rounded-md transition-all z-10`}
                    >
                        <Image
                            className="w-4"
                            src={
                                isOpen
                                    ? ICONS.ADD_CIRCLE
                                    : ICONS.ADD_CIRCLE_WHITE
                            }
                            alt="addcircle"
                        />
                    </button>
                </div>
            </div>

            {isOpen &&
                reservations?.map((res, index) => (
                    <div className="bg-[#E6E6E6] px-8 py-2 text-sm text-gray-600 border-t border-gray-100 shadow-inner grid grid-cols-3 ">
                        <div className="text-left">
                            <p className="text-xs">{tableName}</p>
                        </div>
                        <div className="text-center">
                            <p className="text-xs">{res.capacityNeeded}</p>
                        </div>
                        <div className="text-right">
                            <p className="text-xs">
                                {
                                    formatDate(
                                        new Date(res.scheduledFor),
                                        language
                                    ).time
                                }
                            </p>
                        </div>
                    </div>
                ))}
            {isOpen && reservations?.length === 0 && (
                <div className="bg-[#E6E6E6] px-8 py-2 text-sm text-gray-600 border-t border-gray-100 shadow-inner">
                    <p className="text-xs italic">Brak rezerwacji</p>
                </div>
            )}
        </div>
    );
};

const MODAL_WIDTH = '830px';

const UpsertReservation = ({ onClose }: { onClose: () => void }) => {
    const [activeTabValue, setActiveTabValue] = useState(
        '0209c7a5-4f60-4b33-894a-4a7002eadfbb'
    );

    // Zaciągamy z Contextu to co potrzebne
    const { localAreas, isLoading, isFetching } = useReservationContext();

    // if (isLoading) {
    //     return (
    //         <div className="p-8 text-center w-full">
    //             Ładowanie stref i stolików...
    //         </div>
    //     );
    // }

    return (
        <div
            className="bg-reservation-gradient rounded-2xl relative h-[500px] flex"
            style={{ width: MODAL_WIDTH }}
        >
            <div className="w-1/2">
                <div className="bg-white mt-[0.4rem] h-full rounded-tl-2xl rounded-bl-2xl px-6 py-4 flex flex-col">
                    <ReservationForm />
                </div>
            </div>

            <div className="w-1/2 bg-white mt-[0.4rem] h-full rounded-tr-2xl rounded-br-2xl border-l border-gray-100 overflow-scroll">
                {isFetching && <Skeleton></Skeleton>}
                <Tabs defaultValue={activeTabValue} className="">
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
                        {/* {tabsMock.map(({ label, value }) => (
                            <TabsTrigger
                                key={value}
                                onClick={() => setActiveTabValue(value)}
                                value={value}
                                className="
                                    flex-1 px-6 py-2 rounded-none text-xs
                                    text-black transition-all duration-200
                                    hover:bg-gray-200/50 hover:text-gray-900
                                    data-[state=active]:bg-primary
                                    data-[state=active]:text-white 
                                    data-[state=active]:shadow-sm
                                "
                            >
                                {label}
                            </TabsTrigger>
                        ))} */}
                    </TabsList>
                    <div className="grid grid-cols-3 bg-primary text-white px-6 py-2  text-xs font-medium">
                        <div>Stolik</div>
                        <div className="text-center">Ilość osób</div>
                        <div className="text-right">Godzina</div>
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
