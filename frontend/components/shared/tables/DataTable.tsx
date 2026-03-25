'use client';

import Image from 'next/image';
import { ReactNode, useState } from 'react';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import clsx from 'clsx';
import {
    ColumnDef,
    flexRender,
    getCoreRowModel,
    getPaginationRowModel,
    useReactTable,
    SortingState,
    getSortedRowModel,
    getFilteredRowModel,
} from '@tanstack/react-table';

import { useLanguage } from '@/providers/LanguageProvider';
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
    TableFooter,
} from '@/components/ui/table';
import Button from '../button/Button';
import Input from '../Input/Input';

import languagePacks from '@/helpers/constants/languagePacks';
import { getPaginationRange } from '@/helpers/utils/utils';
import searchSvg from '@/public/images/svg/search.svg';
import LoadingSpinner from '../states/LoadingSpinner';

interface DataTableProps<TData, TValue> {
    columns: ColumnDef<TData, TValue>[];
    data: TData[];
    pagination?: boolean;
    paginationType?: 'descriptive';
    searchBar?: boolean;
    searchPlaceholder?: string;
    noResultsFallback?: ReactNode;
    error?: string;
    loadingState?: ReactNode;
    isError?: boolean;
    isLoading?: boolean;
}

export const DataTable = <TData, TValue>({
    columns,
    data,
    pagination,
    paginationType = 'descriptive',
    searchBar,
    searchPlaceholder,
    noResultsFallback,
    error,
    loadingState,
    isError,
    isLoading,
}: DataTableProps<TData, TValue>) => {
    const [sorting, setSorting] = useState<SortingState>([]);
    const [globalFilter, setGlobalFilter] = useState('');

    const { language } = useLanguage();
    const {
        generic: {
            options: {
                previousPage: { [paginationType]: previousPageBtnLabel },
                nextPage: { [paginationType]: nextPageBtnLabel },
            },
            searchPlaceholder: fallbackSearchPlaceholder,
            noResults,
            errorMsg,
        },
    } = languagePacks[language];

    const table = useReactTable({
        data,
        columns,
        getCoreRowModel: getCoreRowModel(),
        onSortingChange: setSorting,
        getSortedRowModel: getSortedRowModel(),
        onGlobalFilterChange: setGlobalFilter,
        getFilteredRowModel: getFilteredRowModel(),
        globalFilterFn: (row, _, filterValue) => {
            const searchIn =
                // eslint-disable-next-line @typescript-eslint/no-explicit-any
                (row.original as any)._search ||
                JSON.stringify(row.original).toLowerCase();

            return searchIn.includes(filterValue.toLowerCase());
        },
        state: {
            sorting,
            globalFilter,
        },
        ...(pagination
            ? {
                  getPaginationRowModel: getPaginationRowModel(),
              }
            : {}),
    });

    const getPages = () => {
        const totalPages = table.getPageCount();
        const currentPage = table.getState().pagination.pageIndex;
        const pages = getPaginationRange(currentPage, totalPages);

        return pages;
    };

    return (
        <>
            {searchBar && (
                <Input
                    value={globalFilter ?? ''}
                    placeholder={searchPlaceholder ?? fallbackSearchPlaceholder}
                    icon={<Image src={searchSvg} alt="" />}
                    className="w-4/5 max-w-[630px] mb-5"
                    inputClassName="w-full pl-8"
                    iconClassName="left-3 !top-1/2 -translate-y-1/2 w-max"
                    onChange={(e) => table.setGlobalFilter(e.target.value)}
                />
            )}

            <div className="overflow-hidden rounded-md border">
                <Table>
                    <TableHeader>
                        {table.getHeaderGroups().map((headerGroup) => (
                            <TableRow key={headerGroup.id}>
                                {headerGroup.headers.map((header) => {
                                    return (
                                        <TableHead
                                            key={header.id}
                                            className="py-3"
                                        >
                                            {header.isPlaceholder
                                                ? null
                                                : flexRender(
                                                      header.column.columnDef
                                                          .header,
                                                      header.getContext()
                                                  )}
                                        </TableHead>
                                    );
                                })}
                            </TableRow>
                        ))}
                    </TableHeader>

                    <TableBody>
                        {isLoading ? (
                            <TableRow>
                                <TableCell
                                    colSpan={columns.length}
                                    className="h-24"
                                >
                                    {loadingState ?? (
                                        <LoadingSpinner className="mx-auto" />
                                    )}
                                </TableCell>
                            </TableRow>
                        ) : isError ? (
                            <TableRow>
                                <TableCell
                                    colSpan={columns.length}
                                    className="h-24 text-danger text-center"
                                >
                                    {error ?? errorMsg}
                                </TableCell>
                            </TableRow>
                        ) : table.getRowModel().rows?.length ? (
                            table.getRowModel().rows.map((row) => (
                                <TableRow
                                    key={row.id}
                                    data-state={
                                        row.getIsSelected() && 'selected'
                                    }
                                >
                                    {row.getVisibleCells().map((cell) => (
                                        <TableCell key={cell.id}>
                                            {flexRender(
                                                cell.column.columnDef.cell,
                                                cell.getContext()
                                            )}
                                        </TableCell>
                                    ))}
                                </TableRow>
                            ))
                        ) : (
                            <TableRow>
                                <TableCell
                                    colSpan={columns.length}
                                    className="h-24 text-center"
                                >
                                    {noResultsFallback ?? noResults}
                                </TableCell>
                            </TableRow>
                        )}
                    </TableBody>

                    {pagination && (
                        <TableFooter>
                            <TableRow>
                                <TableCell
                                    colSpan={columns.length}
                                    className="py-3"
                                >
                                    <div className="flex items-center justify-between">
                                        <Button
                                            {...PAGINATION_BTN_PROPS}
                                            onClick={() => table.previousPage()}
                                            disabled={
                                                !table.getCanPreviousPage()
                                            }
                                        >
                                            <ChevronLeft
                                                size={
                                                    PAGINATION_BTN_CHEVRON_SIZE
                                                }
                                            />
                                            {previousPageBtnLabel}
                                        </Button>

                                        <div className="flex items-center gap-1">
                                            {getPages().map(
                                                ({
                                                    index,
                                                    isActive,
                                                    disabled,
                                                    label,
                                                }) => (
                                                    <button
                                                        key={index}
                                                        onClick={() =>
                                                            table.setPageIndex(
                                                                index
                                                            )
                                                        }
                                                        className={clsx(
                                                            'px-2 py-1 font-semibold',
                                                            isActive
                                                                ? 'bg-quaternary text-white rounded-lg'
                                                                : 'text-dark-gray'
                                                        )}
                                                        disabled={disabled}
                                                    >
                                                        {label}
                                                    </button>
                                                )
                                            )}
                                        </div>

                                        <Button
                                            {...PAGINATION_BTN_PROPS}
                                            onClick={() => table.nextPage()}
                                            disabled={!table.getCanNextPage()}
                                        >
                                            {nextPageBtnLabel}
                                            <ChevronRight
                                                size={
                                                    PAGINATION_BTN_CHEVRON_SIZE
                                                }
                                            />
                                        </Button>
                                    </div>
                                </TableCell>
                            </TableRow>
                        </TableFooter>
                    )}
                </Table>
            </div>
        </>
    );
};

const PAGINATION_BTN_PROPS = {
    variant: 'none',
    size: 'xxs',
    className: 'flex text-xs font-semibold',
} as const;

const PAGINATION_BTN_CHEVRON_SIZE = 16;
