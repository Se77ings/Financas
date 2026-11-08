import EffectiveDateSummary from "@/models/EffectiveDate/EffectiveDateSummary";
import { formatCalendarMonthAbbrev } from "@/utils/calendarMonth";

interface PeriodTabsProps {
    years: number[];
    /** Nulo em "Todos". */
    year: number | null;
    onYearChange: (year: number | null) => void;
    months: EffectiveDateSummary[];
    month: number | null;
    onMonthChange: (month: number) => void;
}

export default function PeriodTabs({ years, year, onYearChange, months, month, onMonthChange }: PeriodTabsProps) {
    const today = new Date();

    return (
        <div className="flex flex-wrap items-center gap-x-6 gap-y-2">
            <TabGroup>
                {years.map(item => (
                    <Tab key={item} isActive={item === year} isCurrent={item === today.getFullYear()} onClick={() => onYearChange(item)}>
                        {item}
                    </Tab>
                ))}
                <Tab isActive={year === null} onClick={() => onYearChange(null)}>Todos</Tab>
            </TabGroup>

            {year !== null && (
                <TabGroup>
                    {months.map(item => (
                        <Tab
                            key={item.id}
                            isActive={item.month === month}
                            isCurrent={item.month === today.getMonth() + 1 && item.year === today.getFullYear()}
                            onClick={() => onMonthChange(item.month)}
                        >
                            {formatCalendarMonthAbbrev(item.month)}
                        </Tab>
                    ))}
                </TabGroup>
            )}
        </div>
    );
}

function TabGroup({ children }: { children: React.ReactNode }) {
    return <div className="flex flex-wrap items-center gap-1 p-1 rounded-lg bg-black/20 border border-white/5">{children}</div>;
}

function Tab({ isActive, isCurrent, onClick, children }: { isActive: boolean; isCurrent?: boolean; onClick: () => void; children: React.ReactNode }) {
    return (
        <button
            type="button"
            onClick={onClick}
            title={isCurrent ? 'Atual' : undefined}
            className={`relative px-2.5 py-1 rounded text-[11px] font-mono uppercase tracking-widest transition-colors cursor-pointer ${isActive ? 'bg-blue-600 text-white font-bold' : 'text-gray-400 hover:bg-white/10 hover:text-white'}`}
        >
            {children}
            {isCurrent && (
                <span className="absolute bottom-0.5 left-1/2 -translate-x-1/2 h-0.5 w-4 rounded-full bg-yellow-400" />
            )}
        </button>
    );
}
