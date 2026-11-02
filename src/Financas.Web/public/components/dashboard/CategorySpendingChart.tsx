import { useRouter } from "next/navigation";
import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip } from "recharts";
import { CategorySpending } from "@/models/Dashboard/DashboardView";
import DashboardPanel from "./DashboardPanel";
import { formatBRL, SERIES_PALETTE, TOOLTIP_STYLE, UNCATEGORIZED_COLOR } from "./chartTheme";

interface CategorySpendingChartProps {
    data: CategorySpending[];
    monthLabel: string;
}

export default function CategorySpendingChart({ data, monthLabel }: CategorySpendingChartProps) {
    const router = useRouter();

    const colorOf = (item: CategorySpending, index: number) =>
        item.categoryId === null ? UNCATEGORIZED_COLOR : SERIES_PALETTE[index % SERIES_PALETTE.length];

    return (
        <DashboardPanel
            title={`Gastos por categoria · ${monthLabel}`}
            info={
                <>
                    <p>Divide os gastos da competência por categoria.</p>
                    <p className="mt-2">As <span className="text-blue-400 font-semibold">maiores fatias</span> mostram para onde vai a maior parte do dinheiro.</p>
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
                                <Pie data={data} dataKey="total" nameKey="name" innerRadius="55%" outerRadius="90%" paddingAngle={1} stroke="none">
                                    {data.map((item, index) => (
                                        <Cell key={item.categoryId ?? 'uncategorized'} fill={colorOf(item, index)} />
                                    ))}
                                </Pie>
                                <Tooltip {...TOOLTIP_STYLE} formatter={(value) => formatBRL(Number(value))} />
                            </PieChart>
                        </ResponsiveContainer>
                    </div>
                    <ul className="flex flex-col gap-1.5 text-xs">
                        {data.map((item, index) => (
                            <li key={item.categoryId ?? 'uncategorized'} className="flex items-center gap-2">
                                <span className="h-2.5 w-2.5 rounded-sm shrink-0" style={{ backgroundColor: colorOf(item, index) }} />
                                <span className="flex-1 truncate text-gray-300">{item.name}</span>
                                {item.categoryId === null && (
                                    <button
                                        onClick={() => router.push('/categories')}
                                        className="text-[10px] font-semibold text-blue-400 hover:text-blue-300 cursor-pointer"
                                    >
                                        Resolver
                                    </button>
                                )}
                                <span className="text-gray-500 w-12 text-right">{item.percentage.toLocaleString('pt-BR')}%</span>
                                <span className="font-mono text-gray-200 w-24 text-right">{formatBRL(item.total)}</span>
                            </li>
                        ))}
                    </ul>
                </>
            )}
        </DashboardPanel>
    );
}
