import { useState } from "react";

export interface CategoryOption {
    id: string | null;
    name: string;
}

interface CategoryFilterMenuProps {
    options: CategoryOption[];
    isIncluded: (categoryId: string | null) => boolean;
    onToggle: (categoryId: string | null) => void;
    onIncludeAll: () => void;
    onExcludeAll: () => void;
    excludedCount: number;
}

export default function CategoryFilterMenu({ options, isIncluded, onToggle, onIncludeAll, onExcludeAll, excludedCount }: CategoryFilterMenuProps) {
    const [isOpen, setIsOpen] = useState(false);

    return (
        <div className="relative shrink-0">
            <button
                type="button"
                onClick={() => setIsOpen(prev => !prev)}
                className={`px-2 py-1 rounded border text-[11px] font-semibold transition cursor-pointer ${excludedCount > 0
                    ? 'border-blue-500/50 text-blue-300 bg-blue-900/20'
                    : 'border-white/10 text-gray-400 hover:text-white'}`}
                title="Escolher as categorias exibidas"
            >
                🏷️ Categorias{excludedCount > 0 ? ` · ${options.length - excludedCount}/${options.length}` : ''}
            </button>
            {isOpen && (
                <>
                    <div className="fixed inset-0 z-30" onClick={() => setIsOpen(false)} />
                    <div className="absolute right-0 top-8 z-40 w-64 rounded-lg border border-white/10 bg-[#1a1917] shadow-2xl">
                        <div className="flex justify-between px-3 py-2 border-b border-white/5 text-[11px]">
                            <button onClick={onIncludeAll} className="text-blue-400 hover:text-blue-300 cursor-pointer">Marcar todas</button>
                            <button onClick={onExcludeAll} className="text-gray-400 hover:text-white cursor-pointer">Desmarcar todas</button>
                        </div>
                        <ul className="max-h-72 overflow-y-auto py-1">
                            {options.map(option => (
                                <li key={option.id ?? 'uncategorized'}>
                                    <label className="flex items-center gap-2 px-3 py-1.5 text-xs text-gray-200 hover:bg-white/5 cursor-pointer select-none">
                                        <input
                                            type="checkbox"
                                            checked={isIncluded(option.id)}
                                            onChange={() => onToggle(option.id)}
                                            className="h-3.5 w-3.5 accent-blue-500"
                                        />
                                        <span className={option.id === null ? 'italic text-gray-400' : ''}>{option.name}</span>
                                    </label>
                                </li>
                            ))}
                        </ul>
                    </div>
                </>
            )}
        </div>
    );
}
