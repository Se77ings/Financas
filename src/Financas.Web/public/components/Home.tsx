"use client"

import { useRouter } from "next/navigation";
import { useEffect, useState } from 'react';
import CreditCardListModal from "@/public/components/CreditCardListModal";
import RecurrentPaymentListModal from "@/public/components/RecurrentPaymentListModal";
import EffectiveDateSummary from "@/models/EffectiveDate/EffectiveDateSummary";
import DashboardView from "@/models/Dashboard/DashboardView";
import { getEffectiveDateListAsync } from "@/services/effectiveDateServices";
import { getDashboardAsync } from "@/services/dashboardServices";
import { formatEffectiveDateLabel } from "@/utils/calendarMonth";
import { formatBRL } from "@/public/components/dashboard/chartTheme";
import InfoHint from "@/public/components/dashboard/InfoHint";
import DailySpendingChart from "@/public/components/dashboard/DailySpendingChart";
import SpendingPaceChart from "@/public/components/dashboard/SpendingPaceChart";
import CategorySpendingChart from "@/public/components/dashboard/CategorySpendingChart";
import CreditCardUsagePanel from "@/public/components/dashboard/CreditCardUsagePanel";
import FutureCommitmentsChart from "@/public/components/dashboard/FutureCommitmentsChart";
import MonthlyHistoryChart from "@/public/components/dashboard/MonthlyHistoryChart";

export default function Home() {
    const [effectiveDates, setEffectiveDates] = useState<EffectiveDateSummary[]>([]);
    const [selectedEffectiveDateId, setSelectedEffectiveDateId] = useState<string | null>(null);
    const [dashboard, setDashboard] = useState<DashboardView | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        getEffectiveDateListAsync()
            .then(dates => {
                setEffectiveDates(dates);
                const now = new Date();
                const current = dates.find(d => d.month === now.getMonth() + 1 && d.year === now.getFullYear());
                const initial = current ?? dates[dates.length - 1];
                if (initial) setSelectedEffectiveDateId(initial.id);
                else setIsLoading(false);
            })
            .catch(err => {
                setError(err instanceof Error ? err.message : 'Erro ao carregar competências');
                setIsLoading(false);
            });
    }, []);

    useEffect(() => {
        if (!selectedEffectiveDateId) return;
        setIsLoading(true);
        setError(null);
        getDashboardAsync(selectedEffectiveDateId)
            .then(setDashboard)
            .catch(err => setError(err instanceof Error ? err.message : 'Erro ao carregar o dashboard'))
            .finally(() => setIsLoading(false));
    }, [selectedEffectiveDateId]);

    const selectedEffectiveDate = effectiveDates.find(d => d.id === selectedEffectiveDateId);
    const monthLabel = selectedEffectiveDate ? formatEffectiveDateLabel(selectedEffectiveDate.month, selectedEffectiveDate.year) : '';
    const totals = dashboard?.totals;

    const router = useRouter();
    const [showSettingsDropdown, setShowSettingsDropdown] = useState(false);
    const [showCardModal, setShowCardModal] = useState(false);
    const [showImmediateRecurrences, setShowImmediateRecurrences] = useState(false);

    return (
        <div
            className="flex flex-col flex-1 min-h-0 w-full h-full text-gray-100 bg-blue-950">
            <header
                className="px-5 py-3 border-b shrink-0 flex items-center justify-between">
                <div className="flex items-center gap-4">
                    <h1 className="text-sm font-bold tracking-tight">Finanças Pessoais</h1>
                    <label className="flex items-center gap-2 text-xs text-gray-400">
                        Competência
                        <select
                            value={selectedEffectiveDateId ?? ''}
                            onChange={(e) => setSelectedEffectiveDateId(e.target.value)}
                            disabled={effectiveDates.length === 0}
                            className="px-2 py-1 bg-[#0f0e0c] border border-white/10 rounded text-white text-xs uppercase font-mono focus:outline-none focus:border-blue-500"
                        >
                            {effectiveDates.map(d => (
                                <option key={d.id} value={d.id}>{formatEffectiveDateLabel(d.month, d.year)}</option>
                            ))}
                        </select>
                    </label>
                    {isLoading && <span className="text-xs text-gray-500">Carregando...</span>}
                </div>

                <div className="relative">
                    <button
                        onClick={() => setShowSettingsDropdown(!showSettingsDropdown)}
                        className="p-1.5 rounded-lg text-gray-400 hover:text-white hover:bg-white/10 transition cursor-pointer flex items-center justify-center"
                        title="Configurações"
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="w-5 h-5">
                            <circle cx="12" cy="12" r="3"/>
                            <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 1 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 1 1-2.83-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 1 1 2.83-2.83l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 1 1 2.83 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z"/>
                        </svg>
                    </button>
                    {showSettingsDropdown && (
                        <div className="absolute right-0 mt-2 w-60 bg-[#1a1917] border border-white/10 rounded-lg shadow-2xl z-50 overflow-hidden animate-in fade-in slide-in-from-top-1 duration-150">
                            <button
                                onClick={() => {
                                    setShowSettingsDropdown(false);
                                    setShowCardModal(true);
                                }}
                                className="w-full px-4 py-2.5 text-left text-xs font-semibold text-gray-200 hover:bg-blue-600 hover:text-white transition flex items-center gap-2 cursor-pointer"
                            >
                                💳 Configurar Cartão
                            </button>
                            <button
                                onClick={() => {
                                    setShowSettingsDropdown(false);
                                    setShowImmediateRecurrences(true);
                                }}
                                className="w-full px-4 py-2.5 text-left text-xs font-semibold text-gray-200 hover:bg-blue-600 hover:text-white transition flex items-center gap-2 cursor-pointer"
                            >
                                🔁 Gerenciar Recorrências à vista
                            </button>
                            <button
                                onClick={() => {
                                    setShowSettingsDropdown(false);
                                    router.push("/categories");
                                }}
                                className="w-full px-4 py-2.5 text-left text-xs font-semibold text-gray-200 hover:bg-blue-600 hover:text-white transition flex items-center gap-2 cursor-pointer"
                            >
                                🏷️ Gerenciar Categorias
                            </button>
                            <button
                                onClick={() => {
                                    setShowSettingsDropdown(false);
                                    router.push("/wage-history");
                                }}
                                className="w-full px-4 py-2.5 text-left text-xs font-semibold text-gray-200 hover:bg-blue-600 hover:text-white transition flex items-center gap-2 cursor-pointer"
                            >
                                💼 Histórico de Salários
                            </button>
                        </div>
                    )}
                </div>
            </header>
            <main className="flex-1 overflow-auto p-5">
                <div className="flex flex-col gap-4">
                    {error && (
                        <div className="p-3 bg-red-900/20 border border-red-500/30 rounded text-red-400 text-sm">
                            {error}
                        </div>
                    )}

                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                        <SummaryCard
                            label="Saldo Total (todas)"
                            value={totals ? formatBRL(totals.balance) : '—'}
                            valueClass={(totals?.balance ?? 0) >= 0 ? 'text-green-400' : 'text-orange-400'}
                            info={<p>Tudo o que <span className="text-blue-400 font-semibold">entrou</span> menos tudo o que <span className="text-red-400 font-semibold">saiu</span>, somando todas as competências.</p>}
                        />
                        <SummaryCard
                            label="Total Recebíveis"
                            value={totals ? formatBRL(totals.income) : '—'}
                            valueClass="text-blue-400"
                            info={<p>Soma de todos os <span className="text-blue-400 font-semibold">recebimentos</span> lançados, em todas as competências.</p>}
                        />
                        <SummaryCard
                            label="Total Obrigações"
                            value={totals ? formatBRL(totals.expenses) : '—'}
                            valueClass="text-red-400"
                            info={<p>Soma de todos os <span className="text-red-400 font-semibold">gastos à vista</span> e <span className="text-red-400 font-semibold">itens de cartão</span> lançados, em todas as competências.</p>}
                        />
                    </div>

                    {!isLoading && effectiveDates.length === 0 && (
                        <div className="flex flex-col items-center justify-center py-16 gap-3 rounded-2xl border-2 border-dashed border-white/10">
                            <p className="text-gray-400 text-sm">Crie a primeira competência na barra inferior para ver o dashboard.</p>
                        </div>
                    )}

                    {dashboard && (
                        <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
                            <div className="lg:col-span-3 min-w-0">
                                <DailySpendingChart data={dashboard.dailySpending} />
                            </div>
                            <SpendingPaceChart data={dashboard.spendingPace} monthLabel={monthLabel} />
                            <CategorySpendingChart data={dashboard.spendingByCategory} monthLabel={monthLabel} />
                            <CreditCardUsagePanel data={dashboard.creditCards} monthLabel={monthLabel} />
                            <FutureCommitmentsChart data={dashboard.futureCommitments} />
                            <MonthlyHistoryChart data={dashboard.monthlyHistory} />
                        </div>
                    )}
                </div>

            </main>

            {showCardModal && (
                <CreditCardListModal onClose={() => setShowCardModal(false)} />
            )}

            {showImmediateRecurrences && (
                <RecurrentPaymentListModal onClose={() => setShowImmediateRecurrences(false)} />
            )}        </div>

    );

    function SummaryCard({
        label,
        value,
        valueClass,
        info,
    }: {
        label: string;
        value: string;
        valueClass: string;
        info: React.ReactNode;
    }) {
        return (
            <div
                className="p-4 rounded-lg border border-white/5"
                style={{ backgroundColor: 'rgba(0,0,0,0.20)' }}
            >
                <div className="flex items-start justify-between gap-2 mb-1">
                    <p className="text-xs text-gray-400 uppercase tracking-wider">{label}</p>
                    <InfoHint>{info}</InfoHint>
                </div>
                <p className={`text-xl font-bold font-mono ${valueClass}`}>{value}</p>
            </div>
        );
    }
}