'use client';

import React, {
    createContext,
    useContext,
    useState,
    useEffect,
    ReactNode,
} from 'react';

import useQueryAreas, {
    Area,
    AreaReservation,
} from '@/helpers/queries/areas/useAreasQuery';

import { Reservation } from '@/helpers/queries/reservations/useQueryReservations';
import { parseIsoDateAndTime } from '@/helpers/utils/dates';

export interface ReservationTable {
    id: string;
    name: string;
    reservations?: Reservation[];
}
export interface ReservationForm {
    date: string;
    time: string | null;
    peopleCount: number | null;
    selectedTableId: string | null;
    reservationId: string | null;
}

interface ReservationContextType {
    localAreas: Area[];
    isLoading: boolean;
    isFetching: boolean;
    hasUnsavedChanges: boolean;
    form: ReservationForm;
    updateForm: <K extends keyof ReservationForm>(
        field: K,
        value: ReservationForm[K]
    ) => void;
    addLocalReservation: (tableId: string, tableName: string) => void;
    removeLocalReservation: (tableId: string, reservationId: string) => void;
    clearLocalReservations: () => void;
    updateReservationFromDb: (reservation: AreaReservation | null) => void;
    setHasUnsavedChanges: React.Dispatch<React.SetStateAction<boolean>>;
}

const ReservationContext = createContext<ReservationContextType | undefined>(
    undefined
);

const INITIAL_FORM_STATE: ReservationForm = {
    date: new Date().toISOString().split('T')[0],
    time: null,
    peopleCount: null,
    selectedTableId: null,
    reservationId: null,
};

const TEMP_DRAFT_RESERVATION_ID = 'temp-draft-reservation';

export const ReservationProvider = ({ children }: { children: ReactNode }) => {
    const [form, setForm] = useState<ReservationForm>(INITIAL_FORM_STATE);
    const [localAreas, setLocalAreas] = useState<Area[]>([]);
    const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);

    const {
        data: dbAreas,
        isLoading,
        isFetching,
    } = useQueryAreas({ date: form.date });

    useEffect(() => {
        if (dbAreas) setLocalAreas(dbAreas);
    }, [dbAreas]);

    const updateForm = <K extends keyof ReservationForm>(
        field: K,
        value: ReservationForm[K]
    ) => {
        setForm((prev) => {
            const newState = { ...prev, [field]: value };
            if (field === 'date') {
                newState.selectedTableId = null;
            }
            return newState;
        });
    };

    const addLocalReservation = (tableId: string, tableName: string) => {
        const currentReservationId =
            form.reservationId || TEMP_DRAFT_RESERVATION_ID;

        const newReservation: AreaReservation = {
            id: currentReservationId,
            name: tableName,
            phoneNumber: '',
            scheduledFor: `${form.date}T${form.time}:00`,
            capacityNeeded: form.peopleCount!,
            tableId: tableId,
        };

        setLocalAreas((prevAreas) =>
            prevAreas.map((area) => ({
                ...area,
                tables: area.tables.map((table) => {
                    const cleanedReservations = (
                        table.reservations || []
                    ).filter((res) => res.id !== currentReservationId);

                    if (table.id === tableId) {
                        return {
                            ...table,
                            reservations: [
                                ...cleanedReservations,
                                newReservation,
                            ],
                        };
                    }

                    return { ...table, reservations: cleanedReservations };
                }),
            }))
        );

        updateForm('selectedTableId', tableId);
        setHasUnsavedChanges(true);
    };

    const removeLocalReservation = (tableId: string, reservationId: string) => {
        const canDeleteReservation = form.reservationId
            ? reservationId === form.reservationId
            : reservationId === TEMP_DRAFT_RESERVATION_ID;

        if (!canDeleteReservation) {
            return;
        }

        setLocalAreas((prevAreas) =>
            prevAreas.map((area) => ({
                ...area,
                tables: area.tables.map((table) =>
                    table.id === tableId
                        ? {
                              ...table,
                              reservations: (table.reservations || []).filter(
                                  (res) => res.id !== reservationId
                              ),
                          }
                        : table
                ),
            }))
        );
        setHasUnsavedChanges(true);
    };

    const updateReservationFromDb = (reservation: AreaReservation | null) => {
        if (!reservation) {
            setForm({
                ...INITIAL_FORM_STATE,
                date: new Date().toISOString().split('T')[0],
            });
            return;
        }

        const { date, time } = parseIsoDateAndTime(reservation.scheduledFor);

        setForm({
            date,
            time,
            peopleCount: reservation.capacityNeeded,
            selectedTableId: reservation.tableId,
            reservationId: reservation.id,
        });
    };

    const clearLocalReservations = () => {
        if (dbAreas) {
            setLocalAreas(dbAreas);
            setHasUnsavedChanges(false);
        }
    };

    return (
        <ReservationContext.Provider
            value={{
                localAreas,
                isLoading,
                isFetching,
                form,
                hasUnsavedChanges,
                updateForm,
                addLocalReservation,
                setHasUnsavedChanges,
                clearLocalReservations,
                removeLocalReservation,
                updateReservationFromDb,
            }}
        >
            {children}
        </ReservationContext.Provider>
    );
};

export const useReservationContext = () => {
    const context = useContext(ReservationContext);
    if (!context)
        throw new Error('useReservationContext must be used within Provider');
    return context;
};
