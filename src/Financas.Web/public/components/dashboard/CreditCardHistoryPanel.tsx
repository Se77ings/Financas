import { Bar, BarChart, CartesianGrid, Cell, Legend, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import { CreditCardInvoicePeriod } from "@/models/Dashboard/DashboardView";
import DashboardPanel from "./DashboardPanel";
import { AXIS_TICK, CHART_COLORS, formatCompactBRL, formatMonthLabel, SERIES_PALETTE } from "./chartTheme";
import { STACK_TOTAL_KEY, StackedTotalTooltip, STACKED_TOOLTIP_CURSOR, stackTotalBar } from "./StackedTotal";
import { useDragScroll } from "./useDragScroll";

// Abaixo disso as colunas de ano ficam espremidas: o gráfico passa a ser mais largo que o quadro e se arrasta.
const YEAR_COLUMN_MIN_WIDTH = 96;

interface CreditCardHistoryPanelProps {
    history: CreditCardInvoicePeriod[];
    /** Nulo em todos os anos: uma coluna por ano. */
    monthLabel: string | null;
}

export default function CreditCardHistoryPanel({ history, monthLabel }: CreditCardHistoryPanelProps) {
    const drag = useDragScroll<HTMLDivElement>();
    const isYearly = monthLabel === null;

    // A cor de cada cartão vem da ordem alfabética, para não mudar entre os meses nem entre as competências.
    const cards = [...new Map(history.flatMap(period => period.invoices.map(invoice => [invoice.creditCardId, invoice.name] as const))).entries()]
        .map(([id, name]) => ({ id, name }))
        .sort((a, b) => a.name.localeCompare(b.name, 'pt-BR'));
    const colorOf = (creditCardId: string) =>
        SERIES_PALETTE[cards.findIndex(card => card.id === creditCardId) % SERIES_PALETTE.length];

    // Todo cartão entra em toda linha, mesmo zerado: sem o valor, o Recharts não desenha a barra e a pilha perde o rótulo do total.
    const rows = history.map(period => ({
        label: period.month === null ? String(period.year) : formatMonthLabel(period.month, period.year),
        isSelected: isYearly || period.isSelected,
        ...Object.fromEntries(cards.map(card => [card.id, 0])),
        ...Object.fromEntries(period.invoices.map(invoice => [invoice.creditCardId, invoice.total])),
        [STACK_TOTAL_KEY]: period.invoices.reduce((sum, invoice) => sum + invoice.total, 0),
    }));

    return (
        <DashboardPanel
            title={isYearly ? 'Cartões · por ano' : `Cartões · ${monthLabel}`}
            subtitle={isYearly ? 'Soma das faturas de cada ano.' : 'Faturas da competência, das 4 anteriores e da seguinte.'}
            info={
                isYearly ? (
                    <>
                        <p>Total das faturas de cada ano, com uma cor por cartão.</p>
                        <p className="mt-2">Com muitos anos, arraste o gráfico para os lados.</p>
                    </>
                ) : (
                    <>
                        <p>Total das faturas de cada mês, com uma cor por cartão.</p>
                        <p className="mt-2">A barra em destaque é a competência selecionada; a última, quando existe, é a seguinte.</p>
                    </>
                )
            }
        >
            {cards.length === 0 ? (
                <p className="text-sm text-gray-500 italic py-10 text-center">Nenhum cartão com fatura.</p>
            ) : (
                <div
                    {...drag}
                    className={`overflow-x-auto overflow-y-hidden select-none ${isYearly ? 'cursor-grab active:cursor-grabbing' : ''}`}
                >
                    <div className="h-48" style={isYearly ? { minWidth: rows.length * YEAR_COLUMN_MIN_WIDTH } : undefined}>
                        <ResponsiveContainer width="100%" height="100%">
                            <BarChart data={rows} maxBarSize={40} margin={{ top: 20, right: 8, left: 0, bottom: 0 }}>
                                <CartesianGrid stroke={CHART_COLORS.grid} vertical={false} />
                                <XAxis dataKey="label" tick={AXIS_TICK} />
                                <YAxis tick={AXIS_TICK} tickFormatter={formatCompactBRL} width={48} />
                                <Tooltip content={(props) => <StackedTotalTooltip {...props} />} cursor={STACKED_TOOLTIP_CURSOR} />
                                <Legend wrapperStyle={{ fontSize: 12 }} />
                                {cards.map(card => (
                                    <Bar key={card.id} dataKey={card.id} name={card.name} stackId="invoices" fill={colorOf(card.id)}>
                                        {rows.map(row => (
                                            <Cell key={row.label} fillOpacity={row.isSelected ? 1 : 0.45} />
                                        ))}
                                    </Bar>
                                ))}
                                {stackTotalBar("invoices")}
                            </BarChart>
                        </ResponsiveContainer>
                    </div>
                </div>
            )}
        </DashboardPanel>
    );
}
