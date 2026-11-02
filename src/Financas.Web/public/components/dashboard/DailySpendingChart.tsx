import { useEffect, useState } from "react";
import { Bar, CartesianGrid, Cell, ComposedChart, Legend, Line, ReferenceLine, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import { DailySpending, DailySpendingDay } from "@/models/Dashboard/DashboardView";
import { getCategoriesAsync } from "@/services/categoryServices";
import { formatDateBR } from "@/utils/dateFormat";
import DashboardPanel from "./DashboardPanel";
import CategoryFilterMenu, { CategoryOption } from "./CategoryFilterMenu";
import SpendingEntryList from "./SpendingEntryList";
import { AXIS_TICK, CHART_COLORS, formatBRL, formatCompactBRL, formatDayMonth, TOOLTIP_STYLE } from "./chartTheme";
import { useToggleableSeries } from "./useToggleableSeries";
import { useCategoryFilter } from "./useCategoryFilter";

const FILTER_STORAGE_KEY = 'dashboard.dailySpending.excludedCategories';
const RANGE_STORAGE_KEY = 'dashboard.dailySpending.range';

type Range = 'lastDays' | 'selectedMonth';

function readRange(): Range {
    try {
        return localStorage.getItem(RANGE_STORAGE_KEY) === 'selectedMonth' ? 'selectedMonth' : 'lastDays';
    } catch {
        return 'lastDays';
    }
}

function writeRange(range: Range) {
    try {
        localStorage.setItem(RANGE_STORAGE_KEY, range);
    } catch {
        // Sem acesso ao localStorage a escolha vale só nesta sessão.
    }
}

interface DailySpendingChartProps {
    data: DailySpending;
    monthLabel: string;
}

export default function DailySpendingChart({ data, monthLabel }: DailySpendingChartProps) {
    const { isHidden, toggle, legendFormatter } = useToggleableSeries();
    const filter = useCategoryFilter(FILTER_STORAGE_KEY);
    const [categoryOptions, setCategoryOptions] = useState<CategoryOption[]>([]);
    const [range, setRange] = useState<Range>(readRange);
    // A seleção guarda a lista de origem: ao trocar de competência ou de período, ela deixa de valer sozinha.
    const [selection, setSelection] = useState<{ source: DailySpendingDay[]; date: string } | null>(null);
    const selectedDate = selection !== null && selection.source === data[range] ? selection.date : null;

    useEffect(() => {
        getCategoriesAsync()
            .then(categories => setCategoryOptions([
                ...[...categories]
                    .sort((a, b) => a.name.localeCompare(b.name, 'pt-BR'))
                    .map(category => ({ id: category.id, name: category.name })),
                { id: null, name: 'Sem categoria' },
            ]))
            .catch(err => console.error('Erro ao carregar categorias:', err));
    }, []);

    const changeRange = (next: Range) => {
        setRange(next);
        writeRange(next);
    };

    const days = data[range].map(day => ({
        ...day,
        entries: day.entries.filter(entry => filter.isIncluded(entry.categoryId)),
    }));
    const totals = days.map(day => day.entries.reduce((sum, entry) => sum + entry.total, 0));
    const averageDaily = totals.length === 0 ? 0 : totals.reduce((sum, total) => sum + total, 0) / totals.length;

    const rows = days.map((day, index) => ({
        date: day.date,
        label: formatDayMonth(day.date),
        total: totals[index],
        average: averageDaily,
        ideal: day.idealDaily,
    }));

    const latestIdeal = [...days].reverse().find(day => day.idealDaily !== null)?.idealDaily ?? null;
    const selectedDay = days.find(day => day.date === selectedDate);
    const toggleDay = (date: string) => setSelection(selectedDate === date ? null : { source: data[range], date });

    return (
        <DashboardPanel
            title={`Gastos por dia · ${range === 'lastDays' ? 'últimos 30 dias' : monthLabel}`}
            subtitle={filter.excludedCount > 0
                ? 'Exibindo só as categorias selecionadas. Clique num dia para ver os gastos.'
                : 'Clique num dia para ver os gastos, ou na legenda para ocultar séries.'}
            info={
                <>
                    <p>Quanto você gastou em cada dia dos últimos 30 dias ou do mês da competência selecionada.</p>
                    <p className="mt-2">Clique numa <span className="text-red-400 font-semibold">barra</span> para ver a lista do que foi gasto no dia.</p>
                    <ul className="mt-2 space-y-1">
                        <li><span className="text-red-400 font-semibold">Barras</span>: gastos à vista e itens de cartão do dia.</li>
                        <li><span className="text-amber-400 font-semibold">Média</span>: quanto gastou, em média, por dia.</li>
                        <li><span className="text-green-400 font-semibold">Ideal</span>: quanto pode gastar por dia sem passar da receita do mês.</li>
                    </ul>
                    <p className="mt-2">Use <span className="text-blue-400 font-semibold">🏷️ Categorias</span> para deixar só os gastos do dia a dia, tirando contas como aluguel. A escolha fica salva neste navegador.</p>
                    <p className="mt-2 text-gray-400">Média acima do ideal indica um ritmo maior do que a receita comporta.</p>
                </>
            }
            actions={
                <div className="flex items-start gap-4 text-right">
                    <div className="flex rounded border border-white/10 overflow-hidden text-[11px] font-semibold">
                        {([['lastDays', '30 dias'], ['selectedMonth', 'Mês']] as const).map(([value, label]) => (
                            <button
                                key={value}
                                onClick={() => changeRange(value)}
                                className={`px-2 py-1 cursor-pointer transition ${range === value ? 'bg-blue-600 text-white' : 'text-gray-400 hover:bg-white/10'}`}
                            >
                                {label}
                            </button>
                        ))}
                    </div>
                    <CategoryFilterMenu
                        options={categoryOptions}
                        isIncluded={filter.isIncluded}
                        onToggle={filter.toggle}
                        onIncludeAll={filter.includeAll}
                        onExcludeAll={() => filter.excludeAll(categoryOptions.map(option => option.id))}
                        excludedCount={filter.excludedCount}
                    />
                    <div>
                        <p className="text-[10px] uppercase text-gray-500">Média diária</p>
                        <p className="text-sm font-bold font-mono" style={{ color: CHART_COLORS.average }}>{formatBRL(averageDaily)}</p>
                    </div>
                    <div>
                        <p className="text-[10px] uppercase text-gray-500">Ideal diário</p>
                        <p className="text-sm font-bold font-mono" style={{ color: CHART_COLORS.ideal }}>
                            {latestIdeal === null ? '—' : formatBRL(latestIdeal)}
                        </p>
                    </div>
                </div>
            }
        >
            <div className="h-72">
                <ResponsiveContainer width="100%" height="100%">
                    <ComposedChart data={rows} margin={{ top: 8, right: 8, left: 0, bottom: 0 }}>
                        <CartesianGrid stroke={CHART_COLORS.grid} vertical={false} />
                        <XAxis dataKey="label" tick={AXIS_TICK} interval="preserveStartEnd" minTickGap={16} />
                        <YAxis tick={AXIS_TICK} tickFormatter={formatCompactBRL} width={56} />
                        <Tooltip {...TOOLTIP_STYLE} formatter={(value) => formatBRL(Number(value))} />
                        <Legend onClick={toggle} formatter={legendFormatter} />
                        <ReferenceLine y={0} stroke={CHART_COLORS.grid} />
                        <Bar
                            dataKey="total"
                            name="Gasto do dia"
                            fill={CHART_COLORS.spending}
                            radius={[3, 3, 0, 0]}
                            hide={isHidden('total')}
                            cursor="pointer"
                            onClick={(_, index) => toggleDay(rows[index].date)}
                        >
                            {rows.map(row => (
                                <Cell key={row.date} fillOpacity={selectedDate === null || selectedDate === row.date ? 1 : 0.35} />
                            ))}
                        </Bar>
                        <Line dataKey="average" name="Média diária" stroke={CHART_COLORS.average} strokeWidth={2} dot={false} hide={isHidden('average')} />
                        <Line dataKey="ideal" name="Ideal diário" type="stepAfter" stroke={CHART_COLORS.ideal} strokeWidth={2} strokeDasharray="6 4" dot={false} connectNulls={false} hide={isHidden('ideal')} />
                    </ComposedChart>
                </ResponsiveContainer>
            </div>
            {selectedDay && (
                <div className="border-t border-white/5 pt-3">
                    <div className="flex items-center justify-between mb-1">
                        <p className="text-xs font-semibold text-gray-300">Gastos de {formatDateBR(selectedDay.date)}</p>
                        <button onClick={() => setSelection(null)} className="text-[11px] text-gray-500 hover:text-gray-300 cursor-pointer">Fechar</button>
                    </div>
                    <SpendingEntryList entries={selectedDay.entries} />
                </div>
            )}
        </DashboardPanel>
    );
}
