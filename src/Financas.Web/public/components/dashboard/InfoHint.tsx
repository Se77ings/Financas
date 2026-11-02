import { ReactNode, useState } from "react";

type InfoHintProps = {
    children: ReactNode;
    align?: "left" | "right";
    label?: string;
};

export default function InfoHint({ children, align = "right", label = "Sobre este card" }: InfoHintProps) {
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
                aria-label={label}
            >
                i
            </button>
            {isOpen && (
                <div className={`absolute ${align === "left" ? "left-0" : "right-0"} top-7 z-40 w-72 p-3 rounded-lg border border-white/10 bg-[#1a1917] shadow-2xl text-xs leading-relaxed text-gray-300 normal-case tracking-normal font-normal`}>
                    {children}
                </div>
            )}
        </div>
    );
}
