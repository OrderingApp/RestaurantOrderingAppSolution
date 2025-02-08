"use client";

import { useTableQuery } from "../../../helpers/queries/table/useTableQuery";

export default function WaitressPage() {
    const { data: tables, isLoading, isError, error } = useTableQuery();

    if (isLoading) return <p>Loading tables...</p>;

    if (isError) return <p>Error: {error.message}</p>;

    return (
        <div>
            <h1>Tables</h1>
            <ul>
                {tables?.map((table) => (
                    <li key={table.id}>
                        {table.name}
                    </li>
                ))}
            </ul>
        </div>
    );
}