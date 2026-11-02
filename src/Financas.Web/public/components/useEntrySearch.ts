import { useCallback, useEffect, useRef, useState } from "react";
import EntryView from "@/models/Entry/EntryView";
import { searchEntriesAsync } from "@/services/categoryServices";

// Mesmo mínimo exigido pela API.
export const MIN_SEARCH_LENGTH = 2;
const DEBOUNCE_MS = 300;

interface SearchState {
    term: string;
    entries: EntryView[];
    error: string;
}

/** Busca os lançamentos pela descrição; `isActive` é falso enquanto o termo tem menos de 2 caracteres. */
export function useEntrySearch(query: string) {
    const [search, setSearch] = useState<SearchState | null>(null);
    // Respostas que chegam depois de uma busca mais nova são descartadas.
    const latestTermRef = useRef("");

    const term = query.trim();
    const isActive = term.length >= MIN_SEARCH_LENGTH;

    const runSearch = useCallback(async (value: string) => {
        latestTermRef.current = value;
        try {
            const entries = await searchEntriesAsync(value);
            if (latestTermRef.current === value) setSearch({ term: value, entries, error: "" });
        } catch (err) {
            if (latestTermRef.current === value)
                setSearch({ term: value, entries: [], error: err instanceof Error ? err.message : "Erro ao buscar lançamentos." });
        }
    }, []);

    useEffect(() => {
        if (!isActive) return;
        const timer = setTimeout(() => runSearch(term), DEBOUNCE_MS);
        return () => clearTimeout(timer);
    }, [term, isActive, runSearch]);

    const current = isActive && search?.term === term ? search : null;

    return {
        isActive,
        isSearching: isActive && current === null,
        entries: current?.entries ?? [],
        error: current?.error ?? "",
        refresh: () => isActive ? runSearch(term) : Promise.resolve(),
    };
}
