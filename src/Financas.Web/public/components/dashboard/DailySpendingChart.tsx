import { useEffect, useState } from "react";
import { Bar, CartesianGrid, ComposedChart, Legend, Line, ReferenceLine, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import { DailySpending } from "@/models/Dashboard/DashboardView";
import { getCategoriesAsync } from "@/services/categoryServices";
import DashboardPanel from "./DashboardPanel";
import CategoryFilterMenu, { CategoryOption } from "./CategoryFilterMenu";
import { AXIS_TICK, CHART_COLORS, formatBRL, formatCompactBRL, formatDayMonth, TOOLTIP_STYLE } from "./chartTheme";
import { useToggleableSeries } from "./useToggleableSeries";
import { useCategoryFilter } from "./useCategoryFilter";

const FILTER_STORAGE_KEY = 'dashboard.dailySpending.excludedCategories';

export default function DailySpendingChart({ data }: { data: DailySpending }) {
    const { isHidden, toggle, legendFormatter } = useToggleableSeries();
    const filter = useCategoryFilter(FILTER_STORAGE_KEY);
    const [categoryOptions, setCategoryOptions] = useState<CategoryOption[]>([]);

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

    const totals = data.days.map(day => day.byCategory
        .filter(amount => filter.isIncluded(amount.categoryId))
        .reduce((sum, amount) => sum + amount.total, 0));
    const averageDaily = totals.length === 0 ? 0 : totals.reduce((sum, total) => sum + total, 0) / totals.length;

    const rows = data.days.map((day, index) => ({
        label: formatDayMonth(day.date),
        total: totals[index],
        average: averageDaily,
        ideal: day.idealDaily,
    }));

    const latestIdeal = [...data.days].reverse().find(day => day.idealDaily !== null)?.idealDaily ?? null;

    return (
        <DashboardPanel
            title="Gastos por dia · últimos 30 dias"
            subtitle={filter.excludedCount > 0
                ? 'Exibindo só as categorias selecionadas. Clique na legenda para ocultar séries.'
                : 'Clique na legenda para exibir ou ocultar cada série.'}
            info={
                <>
                    <p>Quanto você gastou em cada dia dos últimos 30 dias.</p>
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
                        <Bar dataKey="total" name="Gasto do dia" fill={CHART_COLORS.spending} radius={[3, 3, 0, 0]} hide={isHidden('total')} />
                        <Line dataKey="average" name="Média diária" stroke={CHART_COLORS.average} strokeWidth={2} dot={false} hide={isHidden('average')} />
                        <Line dataKey="ideal" name="Ideal diário" type="stepAfter" stroke={CHART_COLORS.ideal} strokeWidth={2} strokeDasharray="6 4" dot={false} connectNulls={false} hide={isHidden('ideal')} />
                    </ComposedChart>
                </ResponsiveContainer>
            </div>
        </DashboardPanel>
    );
}
