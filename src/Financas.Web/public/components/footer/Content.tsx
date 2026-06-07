import { useState } from "react";
import { ReactNode } from "react";

export default function Content({
    children,
    isActive
}: {
    children: ReactNode;
    isActive?: boolean;
}) {
    return (
        <div className={`flex h-full min-w-20 px-3 flex-row items-center border ${isActive ? 'border-solid border-blue-500 bg-blue-900/20 text-blue-400 font-bold' : 'border-dashed border-white/5 text-gray-400'} text-[10px] uppercase tracking-widest justify-center select-none font-mono transition-all`}>
            {children}
        </div>
    )
}
