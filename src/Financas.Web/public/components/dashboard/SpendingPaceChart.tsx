import { CartesianGrid, Legend, Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import { SpendingPace } from "@/models/Dashboard/DashboardView";
import DashboardPanel from "./DashboardPanel";
import IdealPercentageInput, { DEFAULT_IDEAL_PERCENTAGE, useIdealPercentage } from "./IdealPercentageInput";
import { AXIS_TICK, CHART_COLORS, formatBRL, formatCompactBRL, TOOLTIP_STYLE } from "./chartTheme";
import { useToggleableSeries } from "./useToggleableSeries";

const PERCENTAGE_STORAGE_KEY = 'dashboard.spendingPace.idealPercentage';

interface SpendingPaceChartProps {
    data: SpendingPace;
    monthLabel: string;
}

export default function SpendingPaceChart({ data, monthLabel }: SpendingPaceChartProps) {
    const { isHidden, toggle, legendFormatter } = useToggleableSeries();
    const idealPercentage = useIdealPercentage(PERCENTAGE_STORAGE_KEY);
    const { percentage, factor } = idealPercentage;

    const idealDaily = data.idealDaily * factor;
    const days = data.days.map(day => ({ ...day, idealAccumulated: day.idealAccumulated * factor }));

    const lastDay = [...days].reverse().find(day => day.accumulated !== null);
    const difference = lastDay ? (lastDay.accumulated ?? 0) - lastDay.idealAccumulated : null;

    return (
        <DashboardPanel
            title={`Ritmo do mês · ${monthLabel}`}
            subtitle="Gasto acumulado comparado ao ritmo ideal e ao mês anterior."
            className="lg:col-span-2"
            info={
                <>
                    <p>Acompanha o gasto do mês somado dia a dia.</p>
                    <ul className="mt-2 space-y-1">
                        <li><span className="text-red-400 font-semibold">Gasto acumulado</span>: tudo o que já gastou até cada dia.</li>
                        <li><span className="text-green-400 font-semibold">Ritmo ideal</span>: até onde poderia ter chegado naquele dia para terminar o mês gastando o percentual escolhido da receita disponível (receita menos recorrências e parcelas).</li>
                        <li><span className="text-gray-400 font-semibold">Mês anterior</span>: o mesmo acumulado no mês passado.</li>
                    </ul>
                    <p className="mt-2">Em <span className="text-green-400 font-semibold">% da receita</span> defina quanto da receita disponível o ritmo ideal e o ideal diário devem considerar, de 1 a 100. Vazio vale 100%. A escolha fica salva neste navegador.</p>
                    <p className="mt-2 text-gray-400">Linha <span className="text-red-400">vermelha</span> acima da <span className="text-green-400">verde</span> significa gasto acima do ideal.</p>
                </>
            }
            actions={
                <div className="flex items-start gap-4 text-right">
                    <IdealPercentageInput state={idealPercentage} />
                    <div>
                        <p className="text-[10px] uppercase text-gray-500">Ideal diário</p>
                        <p className="text-sm font-bold font-mono" style={{ color: CHART_COLORS.ideal }}>{formatBRL(idealDaily)}</p>
                    </div>
                    {difference !== null && (
                        <div>
                            <p className="text-[10px] uppercase text-gray-500">{difference > 0 ? 'Acima do ideal' : 'Abaixo do ideal'}</p>
                            <p className={`text-sm font-bold font-mono ${difference > 0 ? 'text-red-400' : 'text-green-400'}`}>
                                {formatBRL(Math.abs(difference))}
                            </p>
                        </div>
                    )}
                </div>
            }
        >
            <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                    <LineChart data={days} margin={{ top: 8, right: 8, left: 0, bottom: 0 }}>
                        <CartesianGrid stroke={CHART_COLORS.grid} vertical={false} />
                        <XAxis dataKey="day" tick={AXIS_TICK} interval="preserveStartEnd" minTickGap={12} />
                        <YAxis tick={AXIS_TICK} tickFormatter={formatCompactBRL} width={56} />
                        <Tooltip {...TOOLTIP_STYLE} labelFormatter={(day) => `Dia ${day}`} formatter={(value) => formatBRL(Number(value))} />
                        <Legend onClick={toggle} formatter={legendFormatter} />
                        <Line dataKey="accumulated" name="Gasto acumulado" stroke={CHART_COLORS.spending} strokeWidth={2.5} dot={false} connectNulls={false} hide={isHidden('accumulated')} />
                        <Line dataKey="idealAccumulated" name={percentage === DEFAULT_IDEAL_PERCENTAGE ? 'Ritmo ideal' : `Ritmo ideal (${percentage}%)`} stroke={CHART_COLORS.ideal} strokeWidth={2} strokeDasharray="6 4" dot={false} hide={isHidden('idealAccumulated')} />
                        <Line dataKey="previousMonthAccumulated" name="Mês anterior" stroke={CHART_COLORS.previous} strokeWidth={1.5} strokeDasharray="2 3" dot={false} connectNulls={false} hide={isHidden('previousMonthAccumulated')} />
                    </LineChart>
                </ResponsiveContainer>
            </div>
        </DashboardPanel>
    );
}
