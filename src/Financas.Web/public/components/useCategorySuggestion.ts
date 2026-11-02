import { useEffect, useRef, useState } from "react";
import { suggestCategoryAsync } from "@/services/categorySuggestionServices";

const SUGGESTION_DELAY_MS = 250;

export function useCategorySuggestion(
    description: string,
    categoryId: string | null,
    setCategoryId: (categoryId: string | null) => void,
) {
    const [isSuggested, setIsSuggested] = useState(false);
    // Lançamento aberto já com categoria conta como escolha manual: a sugestão nunca a sobrescreve.
    const isManualRef = useRef(categoryId !== null);
    const setCategoryIdRef = useRef(setCategoryId);

    useEffect(() => {
        setCategoryIdRef.current = setCategoryId;
    });

    useEffect(() => {
        if (isManualRef.current) return;

        let cancelled = false;
        const timer = setTimeout(() => {
            suggestCategoryAsync(description).then(suggestion => {
                if (cancelled || isManualRef.current) return;
                setCategoryIdRef.current(suggestion);
                setIsSuggested(suggestion !== null);
            });
        }, SUGGESTION_DELAY_MS);

        return () => {
            cancelled = true;
            clearTimeout(timer);
        };
    }, [description]);

    const selectManually = (selected: string | null) => {
        isManualRef.current = selected !== null;
        setIsSuggested(false);
        setCategoryId(selected);
    };

    const reset = () => {
        isManualRef.current = false;
        setIsSuggested(false);
    };

    return { isSuggested, selectManually, reset };
}
