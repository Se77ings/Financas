import { SpendingEntry } from "@/models/Dashboard/DashboardView";
import { formatBRL, formatDayMonth } from "./chartTheme";

interface SpendingEntryListProps {
    entries: SpendingEntry[];
    showDate?: boolean;
}

export default function SpendingEntryList({ entries, showDate = false }: SpendingEntryListProps) {
    if (entries.length === 0)
        return <p className="text-xs text-gray-500 italic py-2">Nenhum gasto.</p>;

    return (
        <ul className="flex flex-col divide-y divide-white/5 text-xs">
            {entries.map((entry, index) => (
                <li key={index} className="flex items-center gap-2 py-1.5">
                    {showDate && <span className="text-gray-500 font-mono w-10 shrink-0">{formatDayMonth(entry.date)}</span>}
                    <span className="flex-1 truncate text-gray-200" title={entry.description}>{entry.description}</span>
                    {entry.installment && <span className="text-[10px] text-gray-400 font-mono">{entry.installment}</span>}
                    <span className={`text-[10px] px-1.5 py-0.5 rounded shrink-0 ${entry.creditCardName ? 'bg-purple-500/15 text-purple-300' : 'bg-white/5 text-gray-400'}`}>
                        {entry.creditCardName ?? 'À vista'}
                    </span>
                    <span className="font-mono text-gray-100 w-24 text-right shrink-0">{formatBRL(entry.total)}</span>
                </li>
            ))}
        </ul>
    );
}
