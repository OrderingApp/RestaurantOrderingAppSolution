import { cn } from '@/lib/utils';

interface TableProps {
    id: string;
    name: string;
    capacity: number;
    onSelect?: (id: string) => void;
    selected?: boolean;
}

const Table = ({ id, name, capacity, onSelect, selected }: TableProps) => (
    <button
        type="button"
        className={cn(
            'p-6 border rounded-2xl bg-white shadow-md w-40 h-28 flex flex-col items-center justify-center cursor-pointer transition-shadow',
            selected ? 'border-black shadow-lg' : 'border-dark-gray'
        )}
        onClick={() => onSelect?.(id)}
        aria-pressed={selected}
    >
        <span className="font-semibold text-center text-lg">{name}</span>
        <span className="text-sm text-gray-600">{capacity}</span>
    </button>
);

export default Table;
