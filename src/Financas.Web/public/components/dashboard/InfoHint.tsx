import { ReactNode, useState } from "react";

export default function InfoHint({ children }: { children: ReactNode }) {
    const [isOpen, setIsOpen] = useState(false);

    return (
        <div
            className="relative shrink-0"
            onMouseEnter={() => setIsOpen(true)}
            onMouseLeave={() => setIsOpen(false)}
        >
            <button
                type="button"
                onClick={() => setIsOpen(prev => !prev)}
                onBlur={() => setIsOpen(false)}
                className="h-5 w-5 rounded-full border border-white/20 text-[11px] font-bold text-gray-400 hover:text-white hover:border-white/50 transition cursor-pointer flex items-center justify-center"
                aria-label="Sobre este card"
            >
                i
            </button>
            {isOpen && (
                <div className="absolute right-0 top-7 z-40 w-72 p-3 rounded-lg border border-white/10 bg-[#1a1917] shadow-2xl text-xs leading-relaxed text-gray-300 normal-case tracking-normal font-normal">
                    {children}
                </div>
            )}
        </div>
    );
}
