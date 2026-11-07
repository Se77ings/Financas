"use client";

import { getEffectiveDateListAsync, addEffectiveDateAsync } from '@/services/effectiveDateServices';
import EffectiveDateSummary from "@/models/EffectiveDate/EffectiveDateSummary";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { useEffect, useRef, useState } from "react";
import Content from "./Content";

const SCROLL_THRESHOLD = 8;
const HIDE_ON_SCROLL_PATH = '/';
const SETUP_PATH = '/setup';

/**
 * Só no dashboard. As páginas rolam dentro dos próprios containers, não na janela, e o evento de scroll
 * não sobe pela árvore: por isso a captura no documento, com a última posição guardada por elemento.
 */
function useHideOnScroll(ignoreInside: React.RefObject<HTMLElement | null>) {
    // Guardado com a rota: ao navegar, a barra volta a aparecer sem precisar zerar o estado.
    const [hiddenOn, setHiddenOn] = useState<string | null>(null);
    const pathname = usePathname();

    useEffect(() => {
        if (pathname !== HIDE_ON_SCROLL_PATH) return;

        const lastTop = new WeakMap<EventTarget, number>();

        const onScroll = (event: Event) => {
            const target = event.target;
            const element = target instanceof Document ? target.scrollingElement : target as HTMLElement | null;
            if (!element || ignoreInside.current?.contains(element)) return;

            const top = element.scrollTop;
            const delta = top - (lastTop.get(element) ?? 0);
            if (Math.abs(delta) < SCROLL_THRESHOLD) return;

            lastTop.set(element, top);
            setHiddenOn(delta > 0 && top > 0 ? pathname : null);
        };

        document.addEventListener('scroll', onScroll, { capture: true, passive: true });
        return () => document.removeEventListener('scroll', onScroll, { capture: true });
    }, [ignoreInside, pathname]);

    return hiddenOn === pathname;
}

export default function Footer() {
    const router = useRouter();
    const pathname = usePathname();
    const searchParams = useSearchParams();
    const activeId = searchParams?.get("id");

    const [effectiveDates, setEffectiveDates] = useState<EffectiveDateSummary[]>([]);
    const [isCreating, setIsCreating] = useState(false);
    const footerRef = useRef<HTMLElement>(null);
    const hidden = useHideOnScroll(footerRef);

    const today = new Date();
    const currentMonth = today.getMonth() + 1;
    const currentYear = today.getFullYear();

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

    // No passo a passo, os atalhos desta barra tirariam a pessoa do fluxo antes de terminar.
    if (pathname.startsWith(SETUP_PATH)) return null;

    return (
        <footer
            ref={footerRef}
            className={`flex items-stretch overflow-hidden shrink-0 transition-[height,border-width] duration-200 ${hidden ? 'h-0 border-t-0' : 'h-11 border-t'}`}
        >
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
                        const isCurrent = effectiveDate.month === currentMonth && effectiveDate.year === currentYear;
                        return (
                            <button
                                key={effectiveDate.id}
                                onClick={() => goToEffectiveDate(effectiveDate.id)}
                                className={`flex items-stretch transition-colors ${isActive ? 'bg-blue-900/10' : ''}`}
                            >
                                <Content isActive={isActive} isCurrent={isCurrent}>
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