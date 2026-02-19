interface TableProps {
    id: string;
    onSelect?: (id: string) => void;
    selected?: boolean;
}

const Table = ({ id, onSelect, selected }: TableProps) => {
    const label = id.replace('table-', 'Stolik ').replace(/-/g, ' ');

    return (
        <div
            className={`p-6 border rounded-2xl bg-white shadow-md w-40 h-28 flex items-center justify-center cursor-pointer transition-shadow ${
                selected ? 'border-black shadow-lg' : 'border-dark-gray'
            }`}
            onClick={() => onSelect?.(id)}
        >
            <span className="font-semibold capitalize text-center">
                {label}
            </span>
        </div>
    );
};

export default Table;
