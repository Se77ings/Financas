"use client"

import { useRouter } from "next/navigation";
import EffectiveDateSummary from '@/models/EffectiveDate/EffectiveDateSummary';
import Content from '@/public/components/footer/Content';
import { IonIcon } from '@ionic/react';
import { home, addOutline } from 'ionicons/icons';
import { useEffect, useState } from 'react';
import { getEffectiveDateListAsync } from '@/services/effectiveDateServices';

export default function Home() {
    const totalBalance = 0;
    const totalPayables = 0;
    const totalReceivables = 0;

    const router = useRouter();

    return (
        <div
            className="flex flex-col flex-1 min-h-0 w-full h-full text-gray-100 bg-blue-950">
            <header
                className="px-5 py-3 border-b shrink-0 flex items-center justify-between">
                <div className="flex items-center gap-3">
                    <h1 className="text-sm font-bold tracking-tight">Finanças Pessoais</h1>
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


        </div>

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