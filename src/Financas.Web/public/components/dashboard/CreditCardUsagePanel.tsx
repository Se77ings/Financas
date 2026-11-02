import { Bar, BarChart, CartesianGrid, Cell, Legend, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import { CreditCardInvoiceMonth, CreditCardUsage } from "@/models/Dashboard/DashboardView";
import { formatDateBR } from "@/utils/dateFormat";
import DashboardPanel from "./DashboardPanel";
import { AXIS_TICK, CHART_COLORS, formatBRL, formatCompactBRL, formatMonthLabel, SERIES_PALETTE, TOOLTIP_STYLE } from "./chartTheme";

interface CreditCardUsagePanelProps {
    data: CreditCardUsage[];
    history: CreditCardInvoiceMonth[];
    monthLabel: string;
}

function usageColor(percentage: number) {
    if (percentage >= 85) return 'text-red-400';
    if (percentage >= 60) return 'text-amber-400';
    return 'text-green-400';
}

export default function CreditCardUsagePanel({ data, history, monthLabel }: CreditCardUsagePanelProps) {
    // A cor de cada cartão vem da ordem alfabética, para não mudar entre os meses nem entre as competências.
    const cards = [...new Map([
        ...history.flatMap(month => month.invoices.map(invoice => [invoice.creditCardId, invoice.name] as const)),
        ...data.map(card => [card.creditCardId, card.name] as const),
    ]).entries()]
        .map(([id, name]) => ({ id, name }))
        .sort((a, b) => a.name.localeCompare(b.name, 'pt-BR'));
    const colorOf = (creditCardId: string) =>
        SERIES_PALETTE[cards.findIndex(card => card.id === creditCardId) % SERIES_PALETTE.length];

    const rows = history.map(month => ({
        label: formatMonthLabel(month.month, month.year),
        isSelected: month.isSelected,
        ...Object.fromEntries(month.invoices.map(invoice => [invoice.creditCardId, invoice.total])),
    }));

    return (
        <DashboardPanel
            title={`Cartões · ${monthLabel}`}
            subtitle="Faturas da competência, das 4 anteriores e da seguinte."
            info={
                <>
                    <p>Total das faturas de cada mês, com uma cor por cartão.</p>
                    <p className="mt-2">A barra em destaque é a competência selecionada; a última, quando existe, é a seguinte.</p>
                    <p className="mt-2">
                        Abaixo, o <span className="font-semibold text-gray-100">disponível</span> de cada cartão: limite menos a fatura e as parcelas que ainda vão cair. A cor indica o uso:{' '}
                        <span className="text-green-400 font-semibold">até 60%</span>,{' '}
                        <span className="text-amber-400 font-semibold">até 85%</span> e{' '}
                        <span className="text-red-400 font-semibold">acima de 85%</span>.
                    </p>
                </>
            }
        >
            {cards.length === 0 ? (
                <p className="text-sm text-gray-500 italic py-10 text-center">Nenhum cartão com fatura.</p>
            ) : (
                <>
                    <div className="h-48">
                        <ResponsiveContainer width="100%" height="100%">
                            <BarChart data={rows} maxBarSize={40} margin={{ top: 8, right: 8, left: 0, bottom: 0 }}>
                                <CartesianGrid stroke={CHART_COLORS.grid} vertical={false} />
                                <XAxis dataKey="label" tick={AXIS_TICK} />
                                <YAxis tick={AXIS_TICK} tickFormatter={formatCompactBRL} width={48} />
                                <Tooltip {...TOOLTIP_STYLE} formatter={(value) => formatBRL(Number(value))} />
                                <Legend wrapperStyle={{ fontSize: 12 }} />
                                {cards.map(card => (
                                    <Bar key={card.id} dataKey={card.id} name={card.name} stackId="invoices" fill={colorOf(card.id)}>
                                        {rows.map(row => (
                                            <Cell key={row.label} fillOpacity={row.isSelected ? 1 : 0.45} />
                                        ))}
                                    </Bar>
                                ))}
                            </BarChart>
                        </ResponsiveContainer>
                    </div>
                    {data.length > 0 && (
                        <ul className="flex flex-col gap-1.5 text-xs">
                            {data.map(card => (
                                <li key={card.creditCardId} className="flex items-center gap-2">
                                    <span className="h-2.5 w-2.5 rounded-sm shrink-0" style={{ backgroundColor: colorOf(card.creditCardId) }} />
                                    <span className="flex-1 truncate text-gray-300">
                                        {card.name}
                                        {card.invoiceDueDate && <span className="text-gray-500"> · vence {formatDateBR(card.invoiceDueDate)}</span>}
                                    </span>
                                    <span className="font-mono text-gray-200 w-24 text-right" title="Fatura da competência">{formatBRL(card.invoiceTotal)}</span>
                                    <span
                                        className={`font-mono w-24 text-right ${usageColor(card.usagePercentage)}`}
                                        title={`Disponível · ${card.usagePercentage.toLocaleString('pt-BR')}% de ${formatBRL(card.limit)} comprometido`}
                                    >
                                        {formatBRL(card.limit - card.committed)}
                                    </span>
                                </li>
                            ))}
                        </ul>
                    )}
                </>
            )}
        </DashboardPanel>
    );
}
