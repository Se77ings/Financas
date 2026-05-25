'use client';

import { useEffect, useRef, useState } from 'react';

export type SaveMode = 'close' | 'continue';

type SaveButtonProps = {
    mode: SaveMode;
    onModeChange: (mode: SaveMode) => void;
    onClick: () => void;
    isSubmitting?: boolean;
    disabled?: boolean;
    className?: string;
};

const LABELS: Record<SaveMode, string> = {
    close: 'Salvar',
    continue: 'Salvar e continuar',
};

function ChevronDownIcon() {
    return (
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
            <polyline points="6 9 12 15 18 9" />
        </svg>
    );
}

export function SaveButton({
    mode,
    onModeChange,
    onClick,
    isSubmitting = false,
    disabled = false,
    className = 'bg-green-700 hover:bg-green-600 disabled:bg-green-900/50',
}: SaveButtonProps) {
    const [isOpen, setIsOpen] = useState(false);
    const containerRef = useRef<HTMLDivElement>(null);

    const alternateMode: SaveMode = mode === 'close' ? 'continue' : 'close';

    useEffect(() => {
        if (!isOpen) return;

        const handleClickOutside = (event: MouseEvent) => {
            if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, [isOpen]);

    const handleSelectAlternate = () => {
        onModeChange(alternateMode);
        setIsOpen(false);
    };

    return (
        <div ref={containerRef} className="relative flex flex-1 min-w-0">
            <button
                type="button"
                onClick={onClick}
                disabled={disabled || isSubmitting}
                className={`flex-1 min-w-0 px-4 py-2 rounded-l text-white transition text-sm font-medium disabled:cursor-not-allowed ${className}`}
            >
                {isSubmitting ? 'Salvando...' : LABELS[mode]}
            </button>
            <button
                type="button"
                onClick={() => setIsOpen(prev => !prev)}
                disabled={disabled || isSubmitting}
                aria-label="Outras opções de salvar"
                aria-expanded={isOpen}
                className={`px-2.5 py-2 rounded-r border-l border-white/20 text-white transition disabled:cursor-not-allowed ${className}`}
            >
                <ChevronDownIcon />
            </button>

            {isOpen && (
                <div
                    role="menu"
                    className="absolute bottom-full right-0 mb-1 min-w-[180px] py-1 bg-[#1a1917] border border-white/10 rounded-lg shadow-xl z-10"
                >
                    <button
                        type="button"
                        role="menuitem"
                        onClick={handleSelectAlternate}
                        className="w-full text-left px-4 py-2 text-sm text-gray-200 hover:bg-white/10 transition"
                    >
                        {LABELS[alternateMode]}
                    </button>
                </div>
            )}
        </div>
    );
}
