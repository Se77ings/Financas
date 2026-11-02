import { useState } from "react";

export const DEFAULT_IDEAL_PERCENTAGE = 100;

// O teto é 100% porque acima disso o "ideal" gastaria mais do que a receita disponível e deixaria de ser meta.
function parsePercentage(text: string): number | null {
    if (!/^\d+$/.test(text)) return null;
    const value = Number(text);
    return value >= 1 && value <= 100 ? value : null;
}

function readPercentage(storageKey: string): number {
    try {
        return parsePercentage(localStorage.getItem(storageKey) ?? '') ?? DEFAULT_IDEAL_PERCENTAGE;
    } catch {
        return DEFAULT_IDEAL_PERCENTAGE;
    }
}

function writePercentage(storageKey: string, percentage: number) {
    try {
        if (percentage === DEFAULT_IDEAL_PERCENTAGE) localStorage.removeItem(storageKey);
        else localStorage.setItem(storageKey, String(percentage));
    } catch {
        // Sem acesso ao localStorage a escolha vale só nesta sessão.
    }
}

const draftOf = (percentage: number) => percentage === DEFAULT_IDEAL_PERCENTAGE ? '' : String(percentage);

/** Cada card passa a própria chave, então cada um guarda o seu percentual. */
export function useIdealPercentage(storageKey: string) {
    const [percentage, setPercentage] = useState(() => readPercentage(storageKey));
    // O texto fica separado do percentual aplicado para permitir apagar ou passar por valores inválidos ao digitar.
    const [draft, setDraft] = useState(() => draftOf(percentage));

    const changeDraft = (text: string) => {
        setDraft(text);
        const trimmed = text.trim();
        const next = trimmed === '' ? DEFAULT_IDEAL_PERCENTAGE : parsePercentage(trimmed);
        if (next === null) return;
        setPercentage(next);
        writePercentage(storageKey, next);
    };

    return {
        percentage,
        factor: percentage / 100,
        draft,
        isDraftValid: draft.trim() === '' || parsePercentage(draft.trim()) !== null,
        changeDraft,
        resetDraft: () => setDraft(draftOf(percentage)),
    };
}

export default function IdealPercentageInput({ state }: { state: ReturnType<typeof useIdealPercentage> }) {
    return (
        <label className="block">
            <span className="block text-[10px] uppercase text-gray-500">% da receita</span>
            <input
                type="number"
                inputMode="numeric"
                min={1}
                max={100}
                step={1}
                value={state.draft}
                placeholder={String(DEFAULT_IDEAL_PERCENTAGE)}
                onChange={event => state.changeDraft(event.target.value)}
                onBlur={state.resetDraft}
                title="Inteiro de 1 a 100. Vazio vale 100%."
                className={`mt-0.5 w-16 px-1.5 py-0.5 rounded bg-black/30 border text-sm font-mono text-right text-gray-200 outline-none focus:border-blue-500 ${state.isDraftValid ? 'border-white/10' : 'border-red-500'}`}
            />
        </label>
    );
}
