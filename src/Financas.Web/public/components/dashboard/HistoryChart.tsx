import { Bar, CartesianGrid, ComposedChart, Legend, Line, ReferenceLine, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import { HistoryPeriod } from "@/models/Dashboard/DashboardView";
import DashboardPanel from "./DashboardPanel";
import { AXIS_TICK, CHART_COLORS, formatBRL, formatCompactBRL, formatMonthLabel, TOOLTIP_STYLE } from "./chartTheme";
import { useToggleableSeries } from "./useToggleableSeries";

/** `year` nulo em todos os anos: uma barra por ano. */
export default function HistoryChart({ data, year }: { data: HistoryPeriod[]; year: number | null }) {
    const { isHidden, toggle, legendFormatter } = useToggleableSeries();
    const period = year === null ? 'ano' : 'mês';

    const rows = data.map(item => ({
        label: item.month === null ? String(item.year) : formatMonthLabel(item.month, item.year),
        income: item.income,
        expenses: item.expenses,
        balance: item.balance,
    }));

    return (
        <DashboardPanel
            title={year === null ? 'Histórico anual' : `Histórico mensal · ${year}`}
            subtitle={year === null ? 'Receitas, despesas e saldo de cada ano.' : `Receitas, despesas e saldo das competências de ${year}.`}
            className="lg:col-span-3"
            info={
                <>
                    <p>{year === null ? 'Compara os anos entre si.' : `Compara as competências de ${year}.`}</p>
                    <ul className="mt-2 space-y-1">
                        <li><span className="text-blue-400 font-semibold">Receitas</span> e <span className="text-red-400 font-semibold">despesas</span> de cada {period}.</li>
                        <li><span className="text-green-400 font-semibold">Saldo</span>: o que sobrou no {period}.</li>
                    </ul>
                    <p className="mt-2 text-gray-400">Saldo abaixo de zero significa que gastou mais do que recebeu naquele {period}.</p>
                </>
            }
        >
            <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                    <ComposedChart data={rows} margin={{ top: 8, right: 8, left: 0, bottom: 0 }}>
                        <CartesianGrid stroke={CHART_COLORS.grid} vertical={false} />
                        <XAxis dataKey="label" tick={AXIS_TICK} />
                        <YAxis tick={AXIS_TICK} tickFormatter={formatCompactBRL} width={56} />
                        <Tooltip {...TOOLTIP_STYLE} formatter={(value) => formatBRL(Number(value))} />
                        <Legend onClick={toggle} formatter={legendFormatter} />
                        <ReferenceLine y={0} stroke={CHART_COLORS.grid} />
                        <Bar dataKey="income" name="Receitas" fill={CHART_COLORS.income} radius={[3, 3, 0, 0]} hide={isHidden('income')} />
                        <Bar dataKey="expenses" name="Despesas" fill={CHART_COLORS.spending} radius={[3, 3, 0, 0]} hide={isHidden('expenses')} />
                        <Line dataKey="balance" name="Saldo" stroke={CHART_COLORS.ideal} strokeWidth={2.5} dot={{ r: 3 }} hide={isHidden('balance')} />
                    </ComposedChart>
                </ResponsiveContainer>
            </div>
        </DashboardPanel>
    );
}
