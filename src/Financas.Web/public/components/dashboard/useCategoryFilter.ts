import { useState } from "react";

const UNCATEGORIZED_KEY = 'uncategorized';

const keyOf = (categoryId: string | null) => categoryId ?? UNCATEGORIZED_KEY;

function readExcluded(storageKey: string): Set<string> {
    try {
        const stored = localStorage.getItem(storageKey);
        return new Set(stored ? JSON.parse(stored) as string[] : []);
    } catch {
        return new Set();
    }
}

function writeExcluded(storageKey: string, excluded: Set<string>) {
    try {
        localStorage.setItem(storageKey, JSON.stringify([...excluded]));
    } catch {
        // Sem acesso ao localStorage (ex.: navegação privada) o filtro vale só nesta sessão.
    }
}

/**
 * Guarda as categorias excluídas (e não as incluídas) para que categorias criadas depois
 * já apareçam sem precisar reconfigurar o filtro.
 */
export function useCategoryFilter(storageKey: string) {
    const [excluded, setExcluded] = useState<Set<string>>(() => readExcluded(storageKey));

    const update = (next: Set<string>) => {
        setExcluded(next);
        writeExcluded(storageKey, next);
    };

    const isIncluded = (categoryId: string | null) => !excluded.has(keyOf(categoryId));

    const toggle = (categoryId: string | null) => {
        const next = new Set(excluded);
        const key = keyOf(categoryId);
        if (next.has(key)) next.delete(key); else next.add(key);
        update(next);
    };

    const includeAll = () => update(new Set());

    const excludeAll = (categoryIds: (string | null)[]) => update(new Set(categoryIds.map(keyOf)));

    return { isIncluded, toggle, includeAll, excludeAll, excludedCount: excluded.size };
}
