import { CreditCardUsage } from "@/models/Dashboard/DashboardView";
import { formatDateBR } from "@/utils/dateFormat";
import DashboardPanel from "./DashboardPanel";
import { formatBRL } from "./chartTheme";

interface CreditCardUsagePanelProps {
    data: CreditCardUsage[];
    monthLabel: string;
}

function usageColor(percentage: number) {
    if (percentage >= 85) return 'bg-red-500';
    if (percentage >= 60) return 'bg-amber-500';
    return 'bg-green-500';
}

export default function CreditCardUsagePanel({ data, monthLabel }: CreditCardUsagePanelProps) {
    return (
        <DashboardPanel
            title={`Cartões · ${monthLabel}`}
            subtitle="Limite comprometido pela fatura da competência e pelas parcelas que ainda vão cair."
            info={
                <>
                    <p>Quanto do limite de cada cartão já está comprometido.</p>
                    <ul className="mt-2 space-y-1">
                        <li><span className="font-semibold text-gray-100">Barra forte</span>: fatura da competência.</li>
                        <li><span className="font-semibold text-gray-400">Barra clara</span>: parcelas que ainda vão cair.</li>
                    </ul>
                    <p className="mt-2">
                        Uso <span className="text-green-400 font-semibold">até 60%</span>,{' '}
                        <span className="text-amber-400 font-semibold">até 85%</span> e{' '}
                        <span className="text-red-400 font-semibold">acima de 85%</span>.
                    </p>
                </>
            }
        >
            {data.length === 0 ? (
                <p className="text-sm text-gray-500 italic py-10 text-center">Nenhum cartão ativo.</p>
            ) : (
                <ul className="flex flex-col gap-4">
                    {data.map(card => {
                        const invoiceShare = card.limit > 0 ? Math.min(100, card.invoiceTotal / card.limit * 100) : 0;
                        const installmentsShare = card.limit > 0 ? Math.min(100 - invoiceShare, card.futureInstallments / card.limit * 100) : 0;

                        return (
                            <li key={card.creditCardId} className="flex flex-col gap-1.5">
                                <div className="flex items-baseline justify-between gap-2">
                                    <span className="text-sm font-semibold text-gray-200">💳 {card.name}</span>
                                    <span className="text-xs font-mono text-gray-400">
                                        {card.usagePercentage.toLocaleString('pt-BR')}% de {formatBRL(card.limit)}
                                    </span>
                                </div>
                                <div className="h-2.5 w-full rounded-full bg-white/5 overflow-hidden flex" title={`Comprometido: ${formatBRL(card.committed)}`}>
                                    <div className={usageColor(card.usagePercentage)} style={{ width: `${invoiceShare}%` }} />
                                    <div className={`${usageColor(card.usagePercentage)} opacity-40`} style={{ width: `${installmentsShare}%` }} />
                                </div>
                                <div className="grid grid-cols-3 gap-2 text-[11px]">
                                    <div>
                                        <p className="text-gray-500">Fatura{card.invoiceDueDate ? ` · vence ${formatDateBR(card.invoiceDueDate)}` : ''}</p>
                                        <p className="font-mono text-gray-200">{formatBRL(card.invoiceTotal)}</p>
                                    </div>
                                    <div>
                                        <p className="text-gray-500">Parcelas futuras</p>
                                        <p className="font-mono text-gray-200">{formatBRL(card.futureInstallments)}</p>
                                    </div>
                                    <div className="text-right">
                                        <p className="text-gray-500">Disponível</p>
                                        <p className={`font-mono ${card.limit - card.committed < 0 ? 'text-red-400' : 'text-gray-200'}`}>
                                            {formatBRL(card.limit - card.committed)}
                                        </p>
                                    </div>
                                </div>
                            </li>
                        );
                    })}
                </ul>
            )}
        </DashboardPanel>
    );
}
