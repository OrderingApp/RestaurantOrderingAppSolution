'use client';
import React, {
    createContext,
    useContext,
    useState,
    useEffect,
    ReactNode,
} from 'react';
import useQueryAreas from '@/helpers/queries/areas/useAreasQuery';

interface ReservationContextType {
    // Dane i ładowanie
    localAreas: any[];
    isLoading: boolean;
    isFetching: boolean;
    // Stan formularza
    form: {
        date: string;
        time: string;
        peopleCount: number;
        selectedTableId: string | null;
    };
    hasUnsavedChanges: boolean;
    // Funkcje
    updateForm: (field: string, value: any) => void;
    addLocalReservation: (tableId: string, tableName: string) => void;
    clearLocalReservations: () => void;
    setHasUnsavedChanges: React.Dispatch<React.SetStateAction<boolean>>;
}

const ReservationContext = createContext<ReservationContextType | undefined>(
    undefined
);

export const ReservationProvider = ({ children }: { children: ReactNode }) => {
    // 1. Grupujemy dane formularza w jeden obiekt state
    const [form, setForm] = useState({
        date: new Date().toISOString().split('T')[0],
        time: null,
        peopleCount: null,
        selectedTableId: null as string | null,
    });

    const [localAreas, setLocalAreas] = useState<any[]>([]);
    const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);

    // Zapytanie reaguje na form.date
    const { data: dbAreas, isLoading, isFetching } = useQueryAreas(form.date);

    useEffect(() => {
        if (dbAreas) setLocalAreas(dbAreas);
    }, [dbAreas]);

    // 2. Jedna funkcja do aktualizacji dowolnego pola w formularzu
    const updateForm = (field: string, value: any) => {
        setForm((prev) => ({ ...prev, [field]: value }));
        // Jeśli zmieniamy datę, warto zresetować zaznaczony stolik
        if (field === 'date')
            setForm((prev) => ({
                ...prev,
                date: value,
                selectedTableId: null,
            }));
    };

    // 3. Logika dodawania rezerwacji korzysta z aktualnego stanu 'form'
    const addLocalReservation = (tableId: string, tableName: string) => {
        const newRes = {
            id: `temp-${Date.now()}`,
            name: tableName,
            scheduledFor: `${form.date}T${form.time}:00`,
            capacityNeeded: form.peopleCount,
            tableId,
        };

        setLocalAreas((prev) =>
            prev.map((area) => ({
                ...area,
                tables: area.tables.map((table: any) =>
                    table.id === tableId
                        ? {
                              ...table,
                              reservations: [
                                  ...(table.reservations || []),
                                  newRes,
                              ],
                          }
                        : table
                ),
            }))
        );

        setForm((prev) => ({ ...prev, selectedTableId: tableId }));
        setHasUnsavedChanges(true);
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
