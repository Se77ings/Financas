export interface SortState<K extends string> {
    key: K;
    ascending: boolean;
}

export function nextSort<K extends string>(current: SortState<K>, key: K): SortState<K> {
    return { key, ascending: current.key === key ? !current.ascending : true };
}

/** Texto compara em pt-BR; datas vêm como yyyy-mm-dd, então a ordem do texto é a da data. */
export function sortRows<K extends string, T extends Record<K, string | number>>(rows: T[], sort: SortState<K>): T[] {
    return [...rows].sort((a, b) => {
        const first = a[sort.key];
        const second = b[sort.key];
        const order = typeof first === 'number' && typeof second === 'number'
            ? first - second
            : String(first).localeCompare(String(second), 'pt-BR');
        return sort.ascending ? order : -order;
    });
}

interface SortableHeaderProps<K extends string> {
    label: string;
    sortKey: K;
    sort: SortState<K>;
    onSort: (next: SortState<K>) => void;
    className?: string;
}

export default function SortableHeader<K extends string>({ label, sortKey, sort, onSort, className = '' }: SortableHeaderProps<K>) {
    const isActive = sort.key === sortKey;

    return (
        <th className={`px-4 py-3 ${className}`}>
            <button
                type="button"
                onClick={() => onSort(nextSort(sort, sortKey))}
                className="inline-flex items-center gap-1 uppercase hover:text-white cursor-pointer"
            >
                {label}
                <span className="w-2">{isActive ? (sort.ascending ? '▲' : '▼') : ''}</span>
            </button>
        </th>
    );
}
