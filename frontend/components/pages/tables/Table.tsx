import Seat from './Seat';

interface TableProps {
    id: string;
    layout: number[];
}

const Table = ({ id, layout }: TableProps) => {
    return (
        <ul className="p-6 border border-dashed border-dark-gray rounded-2xl bg-white shadow-md w-fit flex flex-col gap-6">
            {layout.map((seatCountInRow, rowIndex) => (
                <li key={rowIndex} className="flex gap-6 items-center">
                    {Array.from({ length: seatCountInRow }).map(
                        (_, seatIndex) => (
                            <Seat key={`${id}-${rowIndex}-${seatIndex}`} />
                        )
                    )}
                </li>
            ))}
        </ul>
    );
};

export default Table;
