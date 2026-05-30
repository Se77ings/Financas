'use client';

import { useCallback, useEffect, useState } from 'react';
import { centsToValue, formatCurrencyBR, MAX_CENTS, valueToCents } from '@/utils/currency';

const INPUT_CLASS =
    'w-full pl-9 pr-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition text-right font-mono';

type ValueComponentProps = {
    value: number;
    onChange: (value: number) => void;
    readOnly?: boolean;
    required?: boolean;
    className?: string;
    tabIndex?: number;
};

export function ValueComponent({
    value,
    onChange,
    readOnly = false,
    required = false,
    className,
    tabIndex,
}: ValueComponentProps) {
    const [cents, setCents] = useState(() => valueToCents(value));

    useEffect(() => {
        setCents(valueToCents(value));
    }, [value]);

    const updateCents = useCallback(
        (newCents: number) => {
            const clamped = Math.min(Math.max(0, newCents), MAX_CENTS);
            setCents(clamped);
            onChange(centsToValue(clamped));
        },
        [onChange]
    );

    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (readOnly) return;

        if (e.key >= '0' && e.key <= '9') {
            e.preventDefault();
            const digit = parseInt(e.key, 10);
            updateCents(cents * 10 + digit);
            return;
        }

        if (e.key === 'Backspace' || e.key === 'Delete') {
            e.preventDefault();
            updateCents(Math.floor(cents / 10));
            return;
        }

        const allowed = ['Tab', 'ArrowLeft', 'ArrowRight', 'Home', 'End'];
        if (!allowed.includes(e.key) && !e.ctrlKey && !e.metaKey) {
            e.preventDefault();
        }
    };

    return (
        <div className="relative">
            <span className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500 text-sm pointer-events-none select-none">
                R$
            </span>
            <input
                type="text"
                inputMode="numeric"
                readOnly={readOnly}
                required={required}
                value={formatCurrencyBR(cents)}
                onKeyDown={handleKeyDown}
                onChange={() => {}}
                className={`${className ?? INPUT_CLASS}${readOnly ? ' opacity-70 cursor-default' : ''}`}
                placeholder="0,00"
                aria-label="Valor em reais"
                tabIndex={tabIndex}
            />
        </div>
    );
}
