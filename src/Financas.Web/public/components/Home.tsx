"use client"

import { useRouter } from "next/navigation";
import { useState } from 'react';
import CreditCardListModal from "@/public/components/CreditCardListModal";
import SubscriptionListModal from "@/public/components/SubscriptionListModal";

export default function Home() {
    const totalBalance = 0;
    const totalPayables = 0;
    const totalReceivables = 0;

    const router = useRouter();
    const [showSettingsDropdown, setShowSettingsDropdown] = useState(false);
    const [showCardModal, setShowCardModal] = useState(false);
    const [showImmediateRecurrences, setShowImmediateRecurrences] = useState(false);

    return (
        <div
            className="flex flex-col flex-1 min-h-0 w-full h-full text-gray-100 bg-blue-950">
            <header
                className="px-5 py-3 border-b shrink-0 flex items-center justify-between">
                <div className="flex items-center gap-3">
                    <h1 className="text-sm font-bold tracking-tight">Finanças Pessoais</h1>
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
                <div className="flex flex-col gap-6 h-full">
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                        <SummaryCard
                            label="Saldo Total (todas)"
                            value={totalBalance.toString()}
                            valueClass={totalBalance >= 0 ? 'text-green-400' : 'text-orange-400'}
                        />
                        <SummaryCard
                            label="Total Recebíveis"
                            value={totalReceivables.toString()}
                            valueClass="text-blue-400"
                        />
                        <SummaryCard
                            label="Total Obrigações"
                            value={totalPayables.toString()}
                            valueClass="text-red-400"
                        />
                    </div>


                    <div className="flex flex-col items-center justify-center flex-grow py-16 gap-3 rounded-2xl border-2 border-dashed">
                        <p className="text-gray-400 text-sm">Dashboard futuro</p>
                    </div>
                </div>

            </main>

            {showCardModal && (
                <CreditCardListModal onClose={() => setShowCardModal(false)} />
            )}

            {showImmediateRecurrences && (
                <SubscriptionListModal onClose={() => setShowImmediateRecurrences(false)} />
            )}        </div>

    );

    function SummaryCard({
        label,
        value,
        valueClass,
    }: {
        label: string;
        value: string;
        valueClass: string;
    }) {
        return (
            <div
                className="p-4 rounded-lg border border-white/5"
                style={{ backgroundColor: 'rgba(0,0,0,0.20)' }}
            >
                <p className="text-xs text-gray-400 uppercase tracking-wider mb-1">{label}</p>
                <p className={`text-xl font-bold font-mono ${valueClass}`}>{value}</p>
            </div>
        );
    }
}