import { ReactNode } from "react";

export default function Content({
    children,
    isActive,
    isCurrent
}: {
    children: ReactNode;
    isActive?: boolean;
    isCurrent?: boolean;
}) {
    const stateClasses = isActive
        ? 'border-solid border-blue-500 bg-blue-900/20 text-blue-400 font-bold'
        : isCurrent
            ? 'border-dashed border-yellow-500/60 bg-yellow-500/10 text-yellow-300'
            : 'border-dashed border-white/5 text-gray-400';

    return (
        <div
            title={isCurrent ? 'Competência atual' : undefined}
            className={`relative flex h-full w-full px-1 flex-row items-center border ${stateClasses} text-[10px] uppercase tracking-widest justify-center select-none font-mono transition-all`}
        >
            {children}
            {isActive && isCurrent && (
                <span className="absolute bottom-1 left-1/2 -translate-x-1/2 h-0.5 w-4 rounded-full bg-yellow-400" />
            )}
        </div>
    )
}
