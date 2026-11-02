import { useState } from "react";
import { useRouter } from "next/navigation";
import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip } from "recharts";
import { CategorySpending } from "@/models/Dashboard/DashboardView";
import DashboardPanel from "./DashboardPanel";
import SpendingEntryList from "./SpendingEntryList";
import { formatBRL, SERIES_PALETTE, TOOLTIP_STYLE, UNCATEGORIZED_COLOR } from "./chartTheme";

const keyOf = (item: CategorySpending) => item.categoryId ?? 'uncategorized';

interface CategorySpendingChartProps {
    data: CategorySpending[];
    monthLabel: string;
    effectiveDateId: string | null;
}

export default function CategorySpendingChart({ data, monthLabel, effectiveDateId }: CategorySpendingChartProps) {
    const router = useRouter();
    // A seleção guarda a lista de origem: ao trocar de competência, ela deixa de valer sozinha.
    const [selection, setSelection] = useState<{ source: CategorySpending[]; key: string } | null>(null);
    const selectedKey = selection !== null && selection.source === data ? selection.key : null;

    const toggleCategory = (item: CategorySpending) =>
        setSelection(selectedKey === keyOf(item) ? null : { source: data, key: keyOf(item) });

    const colorOf = (item: CategorySpending, index: number) =>
        item.categoryId === null ? UNCATEGORIZED_COLOR : SERIES_PALETTE[index % SERIES_PALETTE.length];

    return (
        <DashboardPanel
            title={`Gastos por categoria · ${monthLabel}`}
            info={
                <>
                    <p>Divide os gastos da competência por categoria.</p>
                    <p className="mt-2">As <span className="text-blue-400 font-semibold">maiores fatias</span> mostram para onde vai a maior parte do dinheiro.</p>
                    <p className="mt-2">Clique numa fatia ou numa categoria da lista para ver os lançamentos dela.</p>
                    <p className="mt-2 text-gray-400"><span className="text-gray-300 font-semibold">Sem categoria</span> são lançamentos ainda não classificados: use <span className="text-blue-400">Resolver</span>.</p>
                </>
            }
        >
            {data.length === 0 ? (
                <p className="text-sm text-gray-500 italic py-10 text-center">Nenhum gasto nesta competência.</p>
            ) : (
                <>
                    <div className="h-44">
                        <ResponsiveContainer width="100%" height="100%">
                            <PieChart>
                                <Pie
                                    data={data}
                                    dataKey="total"
                                    nameKey="name"
                                    innerRadius="55%"
                                    outerRadius="90%"
                                    paddingAngle={1}
                                    stroke="none"
                                    cursor="pointer"
                                    onClick={(_, index) => toggleCategory(data[index])}
                                >
                                    {data.map((item, index) => (
                                        <Cell
                                            key={keyOf(item)}
                                            fill={colorOf(item, index)}
                                            fillOpacity={selectedKey === null || selectedKey === keyOf(item) ? 1 : 0.35}
                                        />
                                    ))}
                                </Pie>
                                <Tooltip {...TOOLTIP_STYLE} formatter={(value) => formatBRL(Number(value))} />
                            </PieChart>
                        </ResponsiveContainer>
                    </div>
                    <ul className="flex flex-col gap-1.5 text-xs">
                        {data.map((item, index) => (
                            <li key={keyOf(item)} className="flex flex-col">
                                <div
                                    onClick={() => toggleCategory(item)}
                                    className={`flex items-center gap-2 rounded px-1 -mx-1 cursor-pointer hover:bg-white/5 ${selectedKey === keyOf(item) ? 'bg-white/5' : ''}`}
                                >
                                    <span className="h-2.5 w-2.5 rounded-sm shrink-0" style={{ backgroundColor: colorOf(item, index) }} />
                                    <span className="flex-1 truncate text-gray-300">{item.name}</span>
                                    {item.categoryId === null && (
                                        <button
                                            onClick={(e) => {
                                                e.stopPropagation();
                                                const params = new URLSearchParams({ tab: 'uncategorized' });
                                                if (effectiveDateId) params.set('effectiveDateId', effectiveDateId);
                                                router.push(`/categories?${params}`);
                                            }}
                                            className="text-[10px] font-semibold text-blue-400 hover:text-blue-300 cursor-pointer"
                                        >
                                            Resolver
                                        </button>
                                    )}
                                    <span className="text-gray-500 w-12 text-right">{item.percentage.toLocaleString('pt-BR')}%</span>
                                    <span className="font-mono text-gray-200 w-24 text-right">{formatBRL(item.total)}</span>
                                </div>
                                {selectedKey === keyOf(item) && (
                                    <div className="ml-4 mt-1 mb-2 max-h-64 overflow-y-auto">
                                        <SpendingEntryList entries={item.entries} showDate />
                                    </div>
                                )}
                            </li>
                        ))}
                    </ul>
                </>
            )}
        </DashboardPanel>
    );
}
