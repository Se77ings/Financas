import { useState } from "react";
import type { LegendPayload } from "recharts";

export function useToggleableSeries(initiallyHidden: string[] = []) {
    const [hidden, setHidden] = useState<Set<string>>(new Set(initiallyHidden));

    const isHidden = (dataKey: string) => hidden.has(dataKey);

    const toggle = (payload: LegendPayload) => {
        const key = String(payload.dataKey);
        setHidden(prev => {
            const next = new Set(prev);
            if (next.has(key)) next.delete(key); else next.add(key);
            return next;
        });
    };

    const legendFormatter = (value: string, entry: LegendPayload) => (
        <span style={{ color: isHidden(String(entry.dataKey)) ? '#4b5563' : '#d1d5db', cursor: 'pointer', fontSize: 12 }}>{value}</span>
    );

    return { isHidden, toggle, legendFormatter };
}
