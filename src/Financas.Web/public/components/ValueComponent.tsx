'use client';

import { useCallback, useEffect, useState, useRef, useLayoutEffect } from 'react';
import { centsToValue, formatCurrencyBR, MAX_CENTS, valueToCents } from '@/utils/currency';

const useIsomorphicLayoutEffect = typeof window !== 'undefined' ? useLayoutEffect : useEffect;

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
    const inputRef = useRef<HTMLInputElement>(null);
    const cursorRef = useRef<{ digitCount: number } | null>(null);

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

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (readOnly) return;

        const newValue = e.target.value;
        const selectionStart = e.target.selectionStart || 0;

        // Count how many digits are before the cursor in the raw typed value
        let digitCount = 0;
        for (let i = 0; i < selectionStart; i++) {
            if (newValue[i] >= '0' && newValue[i] <= '9') {
                digitCount++;
            }
        }

        // Save the digit count to restore cursor position after render.
        // If the cursor is at the end of the typed value, we'll restore it to the end of the formatted value.
        const isAtEnd = selectionStart === newValue.length;
        cursorRef.current = { digitCount: isAtEnd ? -1 : digitCount };

        // Extract digits only
        const cleanValue = newValue.replace(/\D/g, '');
        const newCents = cleanValue ? parseInt(cleanValue, 10) : 0;
        updateCents(newCents);
    };

    useIsomorphicLayoutEffect(() => {
        if (!inputRef.current || !cursorRef.current) return;

        const input = inputRef.current;
        const formatted = formatCurrencyBR(cents);
        const { digitCount } = cursorRef.current;

        let newCursorPosition = formatted.length;

        if (digitCount !== -1) {
            // Find the position in the formatted string that has `digitCount` digits before it
            newCursorPosition = 0;
            let countedDigits = 0;
            while (newCursorPosition < formatted.length && countedDigits < digitCount) {
                if (formatted[newCursorPosition] >= '0' && formatted[newCursorPosition] <= '9') {
                    countedDigits++;
                }
                newCursorPosition++;
            }
        }

        // Set the selection range to the new cursor position
        input.setSelectionRange(newCursorPosition, newCursorPosition);
        cursorRef.current = null;
    }, [cents]);

    const handleFocus = (e: React.FocusEvent<HTMLInputElement>) => {
        if (readOnly) return;
        const target = e.target;
        setTimeout(() => {
            target.select();
        }, 0);
    };

    return (
        <div className="relative">
            <span className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500 text-sm pointer-events-none select-none">
                R$
            </span>
            <input
                ref={inputRef}
                type="text"
                inputMode="numeric"
                readOnly={readOnly}
                required={required}
                value={formatCurrencyBR(cents)}
                onChange={handleChange}
                onFocus={handleFocus}
                className={`${className ?? INPUT_CLASS}${readOnly ? ' opacity-70 cursor-default' : ''}`}
                placeholder="0,00"
                aria-label="Valor em reais"
                tabIndex={tabIndex}
            />
        </div>
    );
}
