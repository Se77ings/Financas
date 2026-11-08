import { Bar, LabelList, TooltipContentProps } from "recharts";
import { CHART_COLORS, formatBRL } from "./chartTheme";

/** Chave do total de cada linha: quem usa o rótulo precisa preencher esse campo nos dados do gráfico. */
export const STACK_TOTAL_KEY = 'stackTotal';

/** Tooltip das barras empilhadas: as séries com valor e, por último, o total. */
export function StackedTotalTooltip({ active, payload, label }: TooltipContentProps) {
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

export const STACKED_TOOLTIP_CURSOR = { fill: 'rgba(255,255,255,0.04)' };

/**
 * Barra de altura zero no topo da pilha, só para levar o rótulo do total. Precisa ser a última do
 * `stackId` e é uma função, não um componente: o Recharts só reconhece os próprios elementos como filhos do gráfico.
 */
export function stackTotalBar(stackId: string) {
    return (
        <Bar key={STACK_TOTAL_KEY} dataKey={() => 0} stackId={stackId} legendType="none" tooltipType="none" isAnimationActive={false}>
            <LabelList
                dataKey={STACK_TOTAL_KEY}
                position="top"
                fill={CHART_COLORS.axis}
                fontSize={10}
                formatter={(value) => Number(value) > 0 ? formatBRL(Number(value)) : ''}
            />
        </Bar>
    );
}
