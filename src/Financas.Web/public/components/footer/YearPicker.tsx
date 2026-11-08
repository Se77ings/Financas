interface YearPickerProps {
    years: number[];
    value: number;
    onChange: (year: number) => void;
}

/** Só passa pelos anos que têm competência; para cima vai ao ano seguinte, como num contador. */
export default function YearPicker({ years, value, onChange }: YearPickerProps) {
    const index = years.indexOf(value);
    const previous = index > 0 ? years[index - 1] : null;
    const next = index >= 0 && index < years.length - 1 ? years[index + 1] : null;
    const isCurrent = value === new Date().getFullYear();

    return (
        <div className="flex items-stretch shrink-0 border-l border-white/5">
            <div
                title={isCurrent ? 'Ano atual' : undefined}
                className="relative flex items-center justify-center w-16 text-xs font-mono font-bold tracking-widest text-gray-200 select-none"
            >
                {value}
                {isCurrent && (
                    <span className="absolute bottom-1 left-1/2 -translate-x-1/2 h-0.5 w-4 rounded-full bg-yellow-400" />
                )}
            </div>
            <div className="flex flex-col">
                <ChevronButton direction="up" target={next} onChange={onChange} />
                <ChevronButton direction="down" target={previous} onChange={onChange} />
            </div>
        </div>
    );
}

function ChevronButton({ direction, target, onChange }: { direction: 'up' | 'down'; target: number | null; onChange: (year: number) => void }) {
    return (
        <button
            type="button"
            onClick={() => target !== null && onChange(target)}
            disabled={target === null}
            title={target !== null ? String(target) : undefined}
            className="flex flex-1 items-center justify-center px-2 text-gray-400 transition-colors hover:bg-white/10 hover:text-white cursor-pointer disabled:opacity-25 disabled:cursor-default disabled:hover:bg-transparent"
        >
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" className="w-3.5 h-3.5">
                <path d={direction === 'up' ? 'M6 15l6-6 6 6' : 'M6 9l6 6 6-6'} />
            </svg>
        </button>
    );
}
