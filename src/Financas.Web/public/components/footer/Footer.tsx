"use client";

import { getEffectiveDateListAsync } from '@/services/effectiveDateServices';
import EffectiveDateSummary from "@/models/EffectiveDate/EffectiveDateSummary";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import Content from "./Content";

export default function Footer() {
    const router = useRouter();

    const [effectiveDates, setEffectiveDates] = useState<EffectiveDateSummary[]>([]);

    useEffect(() => {
        getEffectiveDateListAsync()
            .then((dates) => {
                setEffectiveDates(dates);
            }).catch((error) => {
                console.error('Erro ao buscar datas:', error);
            })
    }, [])

    function getEffectiveDates(): void {
        let effectiveDates: EffectiveDateSummary[] = [];

        setEffectiveDates(effectiveDates);
    }

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
            <div className="flex flex-grow">
                <div className="flex flex-row flex-grow-1">
                    {effectiveDates.map((effectiveDate) => {
                        return <button key={effectiveDate.id} onClick={() => goToEffectiveDate(effectiveDate.id)} className="flex ">
                            <Content key={effectiveDate.id}>
                                {effectiveDate.display}
                            </Content>
                        </button>
                    })}
                </div>

            </div>

            <button
                type="button"
                onClick={() => router.push("/effective-date")}
                className="flex items-center gap-2 px-3 py-2 rounded-lg text-sm font-medium transition-colors hover:bg-white/10 cursor-pointer"
            >
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="w-5 h-5">
                    <path d="M12 5v14" />
                    <path d="M5 12h14" />
                </svg>
                Novo
            </button>
        </footer>
    );
}