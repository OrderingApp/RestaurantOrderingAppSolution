import { useEffect, useState } from 'react';
import { type LucideIcon } from 'lucide-react';
import { Clock3, Frown, Smile } from 'lucide-react';

import { cn } from '@/lib/utils';
import {
    AreaTable,
    TABLE_STATUSES,
    TableStatus,
} from '@/helpers/queries/areas/useAreasQuery';
import { COMPANYS_CURRENCY } from '@/helpers/constants/constants';
import languagePacks, {
    type TableDescriptionKey,
} from '@/helpers/constants/languagePacks';
import { Order } from '@/helpers/interfaces/orders';
import { getAggregatedDineInOrdersForTable } from '@/helpers/utils/orderTransforms';
import { useLanguage } from '@/providers/LanguageProvider';
import {
    formatElapsedTime,
    formatReservationHour,
    getElapsedSecondsFromTimestamp,
    getRemainingSecondsUntilTimestamp,
} from '@/helpers/utils/dates';

interface TableProps {
    table: AreaTable;
    orders?: Order[];
    onSelect?: (id: string) => void;
    selected?: boolean;
}

const getTableTimerSeconds = (isReserved: boolean, statusTimestamp?: string) =>
    isReserved
        ? getRemainingSecondsUntilTimestamp(statusTimestamp)
        : getElapsedSecondsFromTimestamp(statusTimestamp);

const TABLE_DESCRIPTION_UI_CONFIG: Record<
    TableDescriptionKey,
    {
        colorClass: string;
        Icon: LucideIcon;
    }
> = {
    available: {
        colorClass: 'text-success',
        Icon: Smile,
    },
    closed: {
        colorClass: 'text-danger-dark',
        Icon: Frown,
    },
};

const TABLE_STATUS_CONFIG: Record<
    TableStatus,
    {
        borderColor: string;
        showTimer?: boolean;
        showDescription?: TableDescriptionKey;
    }
> = {
    [TABLE_STATUSES.Available]: {
        borderColor: 'var(--success)',
        showDescription: 'available',
    },
    [TABLE_STATUSES.Reserved]: { borderColor: '#2C5364', showTimer: true },
    [TABLE_STATUSES.Ongoing]: { borderColor: '#008080', showTimer: true },
    [TABLE_STATUSES.PendingServingOrderItems]: {
        borderColor: '#CD5700',
        showTimer: true,
    },
    [TABLE_STATUSES.PendingPayment]: {
        borderColor: '#C70039',
        showTimer: true,
    },
    [TABLE_STATUSES.Closed]: {
        borderColor: 'var(--danger)',
        showDescription: 'closed',
    },
};

const Table = ({ table, orders, onSelect, selected }: TableProps) => {
    const { language } = useLanguage();
    const { detailsAside, tablePage } = languagePacks[language];

    const status: TableStatus = table.status;
    const isReserved = status === TABLE_STATUSES.Reserved;
    const statusTimestamp = table.reservations?.[0]?.scheduledFor;

    const statusConfig = TABLE_STATUS_CONFIG[status];
    const descriptionKey = statusConfig.showDescription;
    const descriptionText = descriptionKey
        ? tablePage.tableCard.descriptions[descriptionKey]
        : null;
    const descriptionUiConfig = descriptionKey
        ? TABLE_DESCRIPTION_UI_CONFIG[descriptionKey]
        : null;

    const aggregatedOrdersForTable = getAggregatedDineInOrdersForTable(
        orders ?? [],
        table.id
    );
    const totalBalanceForTable = aggregatedOrdersForTable.reduce(
        (sum, order) => sum + (order.totalAmount || 0),
        0
    );
    const displayBalance = `${totalBalanceForTable.toFixed(2)} ${COMPANYS_CURRENCY.toUpperCase()}`;
    const displayReceiptsCount = `${aggregatedOrdersForTable.length}`;
    const displayReservationPersonName = table.reservations?.[0]?.name ?? '-';
    const displayReservationPeopleCount =
        table.reservations?.[0]?.capacityNeeded?.toString() ?? '-';
    const displayReservationHour = formatReservationHour(
        table.reservations?.[0]?.scheduledFor
    );

    const shouldShowDetails =
        status !== TABLE_STATUSES.Available && status !== TABLE_STATUSES.Closed;

    const [timerSeconds, setTimerSeconds] = useState(() =>
        getTableTimerSeconds(isReserved, statusTimestamp)
    );

    useEffect(() => {
        setTimerSeconds(getTableTimerSeconds(isReserved, statusTimestamp));

        if (!statusConfig.showTimer) return;

        const intervalId = window.setInterval(() => {
            if (isReserved) {
                setTimerSeconds((prev) => Math.max(0, prev - 1));
                return;
            }

            setTimerSeconds((prev) => prev + 1);
        }, 1000);

        return () => window.clearInterval(intervalId);
    }, [isReserved, statusConfig.showTimer, statusTimestamp]);

    return (
        <button
            type="button"
            className={cn(
                'border-2 rounded-2xl bg-white shadow-md w-56 min-h-48 pt-4 pl-4 flex flex-col cursor-pointer transition-shadow',
                selected && 'shadow-lg ring-2 ring-black/15'
            )}
            style={{ borderColor: statusConfig.borderColor }}
            onClick={() => onSelect?.(table.id)}
            aria-pressed={selected}
        >
            <div className="w-full pr-4 pb-4 flex flex-col items-center gap-1">
                <span className="font-semibold text-lg leading-5">
                    {`${detailsAside.table} ${table.name}`}
                </span>

                {descriptionText && (
                    <p
                        className={cn(
                            'flex items-center justify-center gap-1 text-center',
                            descriptionUiConfig?.colorClass
                        )}
                    >
                        {descriptionText}
                        {descriptionUiConfig && (
                            <descriptionUiConfig.Icon
                                size={16}
                                className={descriptionUiConfig.colorClass}
                            />
                        )}
                    </p>
                )}

                {shouldShowDetails && (
                    <div className="flex flex-col text-center">
                        {isReserved ? (
                            <>
                                <span>
                                    <span>
                                        {tablePage.tableCard.reservationName}:{' '}
                                    </span>

                                    <span className="font-semibold">
                                        {displayReservationPersonName}
                                    </span>
                                </span>
                                <span>
                                    <span>
                                        {tablePage.tableCard.personsCountName}:{' '}
                                    </span>
                                    <span className="font-semibold">
                                        {displayReservationPeopleCount}
                                    </span>
                                </span>
                                <span>
                                    <span>
                                        {tablePage.tableCard.hourName}:{' '}
                                    </span>
                                    <span className="font-semibold">
                                        {displayReservationHour}
                                    </span>
                                </span>
                            </>
                        ) : (
                            <>
                                <span>
                                    <span>
                                        {tablePage.tableCard.balanceName}:{' '}
                                    </span>
                                    <span className="font-semibold">
                                        {displayBalance}
                                    </span>
                                </span>
                                <span>
                                    <span>
                                        {tablePage.tableCard.receiptsCountName}:{' '}
                                    </span>
                                    <span className="font-semibold">
                                        {displayReceiptsCount}
                                    </span>
                                </span>
                            </>
                        )}
                    </div>
                )}
            </div>

            {statusConfig.showTimer && (
                <div className="mt-auto w-full flex justify-end pt-1 text-sm text-gray-700">
                    {isReserved ? (
                        <div
                            className="grid grid-cols-[1fr,auto] items-center p-1.5 gap-x-1 text-sm border-t-2 border-l-2 rounded-tl-xl"
                            style={{
                                borderColor: statusConfig.borderColor,
                            }}
                        >
                            <span>
                                {tablePage.tableCard.reservationInName}:{' '}
                            </span>
                            <Clock3
                                size={16}
                                style={{ color: statusConfig.borderColor }}
                            />
                            <span className="col-span-full text-center">
                                {formatElapsedTime(timerSeconds)}
                            </span>
                        </div>
                    ) : (
                        <div className="flex items-center gap-1 pr-2 pb-2">
                            <span>{formatElapsedTime(timerSeconds)}</span>
                            <Clock3
                                size={16}
                                style={{ color: statusConfig.borderColor }}
                            />
                        </div>
                    )}
                </div>
            )}
        </button>
    );
};

export default Table;
