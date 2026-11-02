import { ReactNode } from "react";
import InfoHint from "./InfoHint";

interface DashboardPanelProps {
    title: string;
    subtitle?: string;
    actions?: ReactNode;
    info?: ReactNode;
    className?: string;
    children: ReactNode;
}

export default function DashboardPanel({ title, subtitle, actions, info, className = '', children }: DashboardPanelProps) {
    return (
        <section className={`p-4 rounded-xl border border-white/5 bg-black/20 flex flex-col gap-3 min-w-0 ${className}`}>
            <header className="flex items-start justify-between gap-3">
                <div>
                    <h2 className="text-xs font-bold uppercase tracking-wider text-gray-300">{title}</h2>
                    {subtitle && <p className="text-[11px] text-gray-500 mt-0.5">{subtitle}</p>}
                </div>
                <div className="flex items-start gap-3">
                    {actions}
                    {info && <InfoHint>{info}</InfoHint>}
                </div>
            </header>
            {children}
        </section>
    );
}
