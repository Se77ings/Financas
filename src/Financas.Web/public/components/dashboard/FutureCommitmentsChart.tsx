import { Bar, BarChart, CartesianGrid, Legend, ReferenceLine, ResponsiveContainer, Tooltip, TooltipContentProps, XAxis, YAxis } from "recharts";
import { FutureCommitments } from "@/models/Dashboard/DashboardView";
import DashboardPanel from "./DashboardPanel";
import { AXIS_TICK, CHART_COLORS, formatBRL, formatCompactBRL, formatMonthLabel, SERIES_PALETTE } from "./chartTheme";
import { useToggleableSeries } from "./useToggleableSeries";

const CARD_RECURRENT_PAYMENTS_KEY = 'cardRecurrentPayments';
const IMMEDIATE_RECURRENT_PAYMENTS_KEY = 'immediateRecurrentPayments';

function CommitmentsTooltip({ active, payload, label }: TooltipContentProps) {
    const entries = (payload ?? []).filter(entry => Number(entry.value) > 0);
    if (!active || entries.length === 0) return null;

    const total = entries.reduce((sum, entry) => sum + Number(entry.value), 0);

    return (
        <div className="rounded-lg border border-white/10 bg-[#1a1917] px-3 py-2 text-xs shadow-2xl">
            <p className="font-semibold text-gray-200 mb-1">{label}</p>
            <ul className="space-y-0.5">
                {entries.map(entry => (
                    <li key={String(entry.dataKey)} className="flex justify-between gap-4">
                        <span style={{ color: entry.color }}>{entry.name}</span>
                        <span className="font-mono text-gray-200">{formatBRL(Number(entry.value))}</span>
                    </li>
                ))}
            </ul>
            <div className="mt-1.5 pt-1.5 border-t border-white/10 flex justify-between gap-4 font-bold">
                <span className="text-gray-100">Total</span>
                <span className="font-mono text-white">{formatBRL(total)}</span>
            </div>
        </div>
    );
}

export default function FutureCommitmentsChart({ data }: { data: FutureCommitments }) {
    const { isHidden, toggle, legendFormatter } = useToggleableSeries();

    const cardNames = [...new Set(data.months.flatMap(month => month.installmentsByCreditCard.map(x => x.creditCardName)))].sort();

    // As chaves das parcelas usam o nome do cartão, prefixado para não colidir com as demais séries.
    const installmentKey = (cardName: string) => `installments:${cardName}`;

    const rows = data.months.map(month => ({
        label: formatMonthLabel(month.month, month.year),
        ...Object.fromEntries(month.installmentsByCreditCard.map(x => [installmentKey(x.creditCardName), x.total])),
        [CARD_RECURRENT_PAYMENTS_KEY]: month.cardRecurrentPayments,
        [IMMEDIATE_RECURRENT_PAYMENTS_KEY]: month.immediateRecurrentPayments,
    }));

    return (
        <DashboardPanel
            title="Compromisso futuro · próximos 6 meses"
            subtitle="Parcelas que ainda vão cair, assinaturas e recorrências à vista, comparadas à receita média."
            className="lg:col-span-2"
            info={
                <>
                    <p>O que já está comprometido nos próximos meses, antes de qualquer gasto novo.</p>
                    <ul className="mt-2 space-y-1">
                        <li><span className="text-blue-400 font-semibold">Parcelas</span> de cada cartão.</li>
                        <li><span className="text-violet-400 font-semibold">Assinaturas</span> dos cartões.</li>
                        <li><span className="text-amber-400 font-semibold">Recorrências à vista</span>, como aluguel e internet.</li>
                    </ul>
                    <p className="mt-2 text-gray-400">Quanto mais perto da <span className="text-blue-400">linha da receita média</span>, menos sobra para os gastos do mês.</p>
                </>
            }
            actions={
                <div className="text-right">
                    <p className="text-[10px] uppercase text-gray-500">Receita média</p>
                    <p className="text-sm font-bold font-mono" style={{ color: CHART_COLORS.income }}>{formatBRL(data.averageIncome)}</p>
                </div>
            }
        >
            <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={rows} margin={{ top: 8, right: 8, left: 0, bottom: 0 }}>
                        <CartesianGrid stroke={CHART_COLORS.grid} vertical={false} />
                        <XAxis dataKey="label" tick={AXIS_TICK} />
                        <YAxis tick={AXIS_TICK} tickFormatter={formatCompactBRL} width={56} />
                        <Tooltip content={(props) => <CommitmentsTooltip {...props} />} cursor={{ fill: 'rgba(255,255,255,0.04)' }} />
                        <Legend onClick={toggle} formatter={legendFormatter} />
                        {data.averageIncome > 0 && (
                            <ReferenceLine y={data.averageIncome} stroke={CHART_COLORS.income} strokeDasharray="6 4" />
                        )}
                        {cardNames.map((cardName, index) => (
                            <Bar
                                key={cardName}
                                dataKey={installmentKey(cardName)}
                                name={`Parcelas · ${cardName}`}
                                stackId="commitments"
                                fill={SERIES_PALETTE[index % SERIES_PALETTE.length]}
                                hide={isHidden(installmentKey(cardName))}
                            />
                        ))}
                        <Bar dataKey={CARD_RECURRENT_PAYMENTS_KEY} name="Assinaturas" stackId="commitments" fill="#a78bfa" hide={isHidden(CARD_RECURRENT_PAYMENTS_KEY)} />
                        <Bar dataKey={IMMEDIATE_RECURRENT_PAYMENTS_KEY} name="Recorrências à vista" stackId="commitments" fill={CHART_COLORS.average} radius={[3, 3, 0, 0]} hide={isHidden(IMMEDIATE_RECURRENT_PAYMENTS_KEY)} />
                    </BarChart>
                </ResponsiveContainer>
            </div>
        </DashboardPanel>
    );
}
