import { useState } from "react";
import DesktopStatusView from "@/models/Desktop/DesktopStatusView";
import { applyUpdateAsync } from "@/services/desktopServices";

interface DesktopStatusBannerProps {
    status: DesktopStatusView;
    onOpenBackup: () => void;
}

export default function DesktopStatusBanner({ status, onOpenBackup }: DesktopStatusBannerProps) {
    const [isRestarting, setIsRestarting] = useState(false);
    const [updateError, setUpdateError] = useState<string | null>(null);

    const { backup, update } = status;

    const handleApplyUpdate = async () => {
        setIsRestarting(true);
        setUpdateError(null);
        try {
            await applyUpdateAsync();
        } catch (err) {
            setUpdateError(err instanceof Error ? err.message : "Erro ao aplicar a atualização");
            setIsRestarting(false);
        }
    };

    const backupWarning = backup.lastError
        ? `O último backup falhou: ${backup.lastError}`
        : backup.isStale
            ? backup.folder ? "Seu backup está desatualizado." : "Escolha uma pasta para guardar seus backups."
            : null;

    if (!backupWarning && !update.readyToApply) return null;

    return (
        <div className="shrink-0 border-b border-white/10 divide-y divide-white/5 text-xs">
            {backupWarning && (
                <div className="px-5 py-1.5 flex items-center justify-between gap-4 bg-orange-900/20 text-orange-300">
                    <span className="min-w-0 truncate" title={backupWarning}>{backupWarning}</span>
                    <button
                        onClick={onOpenBackup}
                        className="font-semibold underline hover:text-white transition cursor-pointer shrink-0"
                    >
                        Abrir backup
                    </button>
                </div>
            )}
            {update.readyToApply && (
                <div className="px-5 py-1.5 flex items-center justify-between gap-4 bg-blue-900/30 text-blue-200">
                    <span className="min-w-0 truncate">
                        {updateError ?? `A versão ${update.availableVersion} está pronta para instalar.`}
                    </span>
                    <button
                        onClick={handleApplyUpdate}
                        disabled={isRestarting}
                        className="font-semibold underline hover:text-white disabled:opacity-50 disabled:cursor-not-allowed transition cursor-pointer shrink-0"
                    >
                        {isRestarting ? "Reiniciando..." : "Reiniciar agora"}
                    </button>
                </div>
            )}
        </div>
    );
}
