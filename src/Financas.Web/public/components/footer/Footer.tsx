"use client";

import { getEffectiveDateListAsync, addEffectiveDateAsync } from '@/services/effectiveDateServices';
import EffectiveDateSummary from "@/models/EffectiveDate/EffectiveDateSummary";
import { useRouter, useSearchParams } from "next/navigation";
import { useEffect, useState } from "react";
import Content from "./Content";

export default function Footer() {
    const router = useRouter();
    const searchParams = useSearchParams();
    const activeId = searchParams?.get("id");

    const [effectiveDates, setEffectiveDates] = useState<EffectiveDateSummary[]>([]);
    const [isCreating, setIsCreating] = useState(false);

    useEffect(() => {
        getEffectiveDateListAsync()
            .then((dates) => {
                setEffectiveDates(dates);
            }).catch((error) => {
                console.error('Erro ao buscar datas:', error);
            })
    }, [])

    const handleCreateNextEffectiveDate = async () => {
        if (isCreating) return;
        setIsCreating(true);

        try {
            let nextMonth = new Date().getMonth() + 1;
            let nextYear = new Date().getFullYear();

            if (effectiveDates.length > 0) {
                let maxDate = effectiveDates[0];
                for (let i = 1; i < effectiveDates.length; i++) {
                    const item = effectiveDates[i];
                    if (item.year > maxDate.year || (item.year === maxDate.year && item.month > maxDate.month)) {
                        maxDate = item;
                    }
                }

                nextMonth = maxDate.month + 1;
                nextYear = maxDate.year;
                if (nextMonth > 12) {
                    nextMonth = 1;
                    nextYear += 1;
                }
            }

            await addEffectiveDateAsync({ month: nextMonth, year: nextYear });

            const updatedDates = await getEffectiveDateListAsync();
            setEffectiveDates(updatedDates);

            const newCompetency = updatedDates.find(d => d.month === nextMonth && d.year === nextYear);
            if (newCompetency) {
                goToEffectiveDate(newCompetency.id);
            }
        } catch (error) {
            alert(error instanceof Error ? error.message : "Erro ao criar próxima competência");
        } finally {
            setIsCreating(false);
        }
    };

    function goToEffectiveDate(id: string) {
        router.push(`/effective-date?id=${encodeURIComponent(id)}`);
    }

    return (
        <footer className=" h-11 border-t flex items-stretch overflow-hidden shrink-0">
            <button type="button" onClick={() => router.push("/")} className="flex items-center gap-2 px-3 py-2 rounded-lg text-sm font-medium transition-colors hover:bg-white/10 cursor-pointer">
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="w-5 h-5">
                    <path d="M3 9.5L12 3l9 6.5V20a1 1 0 0 1-1 1h-5a1 1 0 0 1-1-1v-5H10v5a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V9.5z" />
                </svg>
                Home
            </button>
            <div className="flex flex-grow overflow-x-auto">
                <div className="flex flex-row items-stretch">
                    {effectiveDates.map((effectiveDate) => {
                        const isActive = effectiveDate.id === activeId;
                        return (
                            <button
                                key={effectiveDate.id}
                                onClick={() => goToEffectiveDate(effectiveDate.id)}
                                className={`flex items-stretch transition-colors ${isActive ? 'bg-blue-900/10' : ''}`}
                            >
                                <Content isActive={isActive}>
                                    {effectiveDate.display}
                                </Content>
                            </button>
                        );
                    })}
                </div>
            </div>

            <button
                type="button"
                onClick={handleCreateNextEffectiveDate}
                disabled={isCreating}
                className="flex items-center gap-2 px-3 py-2 rounded-lg text-sm font-medium transition-colors hover:bg-white/10 cursor-pointer disabled:opacity-50"
            >
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="w-5 h-5">
                    <path d="M12 5v14" />
                    <path d="M5 12h14" />
                </svg>
                {isCreating ? "Criando..." : "Novo"}
            </button>
        </footer>
    );
}