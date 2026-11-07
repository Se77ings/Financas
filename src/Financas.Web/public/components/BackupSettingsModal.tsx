import { useEffect, useState } from "react";
import DesktopStatusView, { DesktopBackupFile, DesktopBackupStatus } from "@/models/Desktop/DesktopStatusView";
import {
    chooseBackupFolderAsync,
    getBackupsAsync,
    getDesktopStatusAsync,
    restoreBackupAsync,
    restoreBackupFromFileAsync,
    runBackupAsync,
} from "@/services/desktopServices";
import { ConfirmDialog } from "@/public/components/ConfirmDialog";
import { formatDateBR } from "@/utils/dateFormat";

interface BackupSettingsModalProps {
    onClose: () => void;
}

interface RestoreRequest {
    /** Sem nome, o arquivo é escolhido no seletor do Windows depois da confirmação. */
    fileName: string | null;
    message: string;
}

const RESTORE_WARNING = "Os dados atuais serão substituídos e o programa vai reiniciar.";

function formatFileSize(sizeBytes: number): string {
    const megabytes = sizeBytes / (1024 * 1024);
    if (megabytes >= 1) return `${megabytes.toLocaleString('pt-BR', { maximumFractionDigits: 1 })} MB`;
    return `${Math.ceil(sizeBytes / 1024).toLocaleString('pt-BR')} KB`;
}

export default function BackupSettingsModal({ onClose }: BackupSettingsModalProps) {
    const [status, setStatus] = useState<DesktopStatusView | null>(null);
    const [backups, setBackups] = useState<DesktopBackupFile[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [isWorking, setIsWorking] = useState(false);
    const [restoreRequest, setRestoreRequest] = useState<RestoreRequest | null>(null);
    const [isRestoring, setIsRestoring] = useState(false);
    const [isRestarting, setIsRestarting] = useState(false);

    useEffect(() => {
        const showError = (err: unknown) => setError(err instanceof Error ? err.message : "Erro ao carregar os backups");

        // Separados porque uma pasta inacessível falha a lista, mas a pessoa ainda precisa ver a tela para trocá-la.
        Promise.all([
            getDesktopStatusAsync().then(setStatus).catch(showError),
            getBackupsAsync().then(setBackups).catch(showError),
        ]).finally(() => setIsLoading(false));
    }, []);

    const handleBackupChange = async (action: () => Promise<DesktopBackupStatus>) => {
        setIsWorking(true);
        setError(null);
        try {
            const backup = await action();
            setStatus(current => current && { ...current, backup });
            setBackups(await getBackupsAsync());
        } catch (err) {
            setError(err instanceof Error ? err.message : "Erro ao atualizar o backup");
        } finally {
            setIsWorking(false);
        }
    };

    const handleConfirmRestore = async () => {
        if (!restoreRequest) return;

        setIsRestoring(true);
        setError(null);
        try {
            const started = restoreRequest.fileName
                ? await restoreBackupAsync(restoreRequest.fileName).then(() => true)
                : await restoreBackupFromFileAsync();
            setIsRestarting(started);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Erro ao restaurar o backup");
        } finally {
            setIsRestoring(false);
            setRestoreRequest(null);
        }
    };

    const backup = status?.backup;

    return (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 backdrop-blur-sm">
            <div className="bg-[#1a1917] rounded-xl border border-white/5 w-[650px] max-w-full shadow-2xl flex flex-col max-h-[85vh] overflow-hidden animate-in fade-in zoom-in duration-200">

                <div className="p-4 border-b border-white/5 flex justify-between items-center bg-[#151413] shrink-0">
                    <h3 className="font-bold text-sm uppercase tracking-wider text-blue-400 flex items-center gap-2">
                        💾 Backup
                    </h3>
                    {!isRestarting && (
                        <button
                            onClick={onClose}
                            className="text-gray-500 hover:text-white transition cursor-pointer"
                        >
                            ✕
                        </button>
                    )}
                </div>

                {isRestarting ? (
                    <div className="p-10 text-center text-sm text-gray-300">
                        Restaurando o backup. O programa vai reiniciar em instantes.
                    </div>
                ) : (
                    <>
                        <div className="flex-grow p-5 overflow-y-auto min-h-0 space-y-5">
                            {error && (
                                <div className="p-3 bg-red-900/20 border border-red-500/30 rounded text-red-400 text-sm">
                                    {error}
                                </div>
                            )}

                            {backup?.lastError && (
                                <div className="p-3 bg-orange-900/20 border border-orange-500/30 rounded text-orange-300 text-sm">
                                    O último backup falhou: {backup.lastError}
                                </div>
                            )}

                            {isLoading ? (
                                <div className="text-center py-12 text-gray-500 text-sm">Carregando backups...</div>
                            ) : (
                                <>
                                    <div className="flex justify-between items-center gap-4">
                                        <div className="min-w-0">
                                            <p className="text-xs font-semibold text-gray-400 mb-1">Pasta de backup</p>
                                            <p className="text-sm text-gray-200 break-all">{backup?.folder ?? "Nenhuma pasta escolhida"}</p>
                                        </div>
                                        <button
                                            onClick={() => handleBackupChange(chooseBackupFolderAsync)}
                                            disabled={isWorking}
                                            className="px-3 py-1.5 rounded border border-white/10 text-white hover:bg-white/5 disabled:opacity-50 disabled:cursor-not-allowed transition text-xs font-semibold cursor-pointer shrink-0"
                                        >
                                            Trocar pasta
                                        </button>
                                    </div>

                                    <div className="flex justify-between items-center gap-4">
                                        <div>
                                            <p className="text-xs font-semibold text-gray-400 mb-1">Último backup</p>
                                            <p className="text-sm text-gray-200">{backup?.lastBackupAt ? formatDateBR(backup.lastBackupAt) : "Nenhum backup feito"}</p>
                                        </div>
                                        <button
                                            onClick={() => handleBackupChange(runBackupAsync)}
                                            disabled={isWorking || !backup?.folder}
                                            className="px-3 py-1.5 bg-blue-700 hover:bg-blue-600 disabled:bg-blue-900/50 disabled:cursor-not-allowed transition rounded text-xs font-bold shadow cursor-pointer text-white shrink-0"
                                        >
                                            {isWorking ? "Aguarde..." : "Fazer backup agora"}
                                        </button>
                                    </div>

                                    {backups.length === 0 ? (
                                        <div className="text-center py-10 border-2 border-dashed border-white/5 rounded-lg text-gray-500 text-sm italic">
                                            Nenhum backup nesta pasta.
                                        </div>
                                    ) : (
                                        <div className="border border-white/5 rounded-lg overflow-hidden bg-black/20">
                                            <table className="w-full text-left text-sm">
                                                <thead className="text-[10px] text-gray-500 uppercase border-b border-white/5 bg-[#151413]">
                                                    <tr>
                                                        <th className="px-4 py-2.5">Arquivo</th>
                                                        <th className="px-4 py-2.5 text-center">Data</th>
                                                        <th className="px-4 py-2.5 text-right">Tamanho</th>
                                                        <th className="px-4 py-2.5 w-24"></th>
                                                    </tr>
                                                </thead>
                                                <tbody className="divide-y divide-white/5 font-mono text-xs">
                                                    {backups.map((file) => (
                                                        <tr key={file.fileName} className="hover:bg-white/5">
                                                            <td className="px-4 py-3 text-gray-200 break-all">{file.fileName}</td>
                                                            <td className="px-4 py-3 text-center text-gray-400">{formatDateBR(file.createdAt)}</td>
                                                            <td className="px-4 py-3 text-right text-gray-400 whitespace-nowrap">{formatFileSize(file.sizeBytes)}</td>
                                                            <td className="px-4 py-3 text-center">
                                                                <button
                                                                    onClick={() => setRestoreRequest({
                                                                        fileName: file.fileName,
                                                                        message: `Restaurar o backup de ${formatDateBR(file.createdAt)}? ${RESTORE_WARNING}`,
                                                                    })}
                                                                    disabled={isWorking}
                                                                    className="text-blue-400 hover:text-blue-300 disabled:opacity-50 transition font-sans font-semibold cursor-pointer"
                                                                >
                                                                    Restaurar
                                                                </button>
                                                            </td>
                                                        </tr>
                                                    ))}
                                                </tbody>
                                            </table>
                                        </div>
                                    )}

                                    <div className="flex justify-between items-center gap-4">
                                        <p className="text-xs text-gray-400">
                                            Trocou de computador? Escolha um arquivo de backup guardado em outro lugar.
                                        </p>
                                        <button
                                            onClick={() => setRestoreRequest({
                                                fileName: null,
                                                message: `Depois de confirmar, escolha o arquivo de backup. ${RESTORE_WARNING}`,
                                            })}
                                            disabled={isWorking}
                                            className="px-3 py-1.5 rounded border border-white/10 text-white hover:bg-white/5 disabled:opacity-50 disabled:cursor-not-allowed transition text-xs font-semibold cursor-pointer shrink-0"
                                        >
                                            Restaurar de um arquivo
                                        </button>
                                    </div>
                                </>
                            )}
                        </div>

                        <div className="p-4 border-t border-white/5 bg-[#151413] flex justify-between items-center shrink-0">
                            <span className="text-xs text-gray-500">{status && `Versão ${status.version}`}</span>
                            <button
                                onClick={onClose}
                                className="px-4 py-1.5 rounded border border-white/10 text-white hover:bg-white/5 transition text-xs font-semibold cursor-pointer"
                            >
                                Fechar
                            </button>
                        </div>
                    </>
                )}

                <ConfirmDialog
                    isOpen={restoreRequest !== null}
                    onClose={() => setRestoreRequest(null)}
                    onConfirm={handleConfirmRestore}
                    loading={isRestoring}
                    title="Restaurar backup"
                    message={restoreRequest?.message ?? ""}
                    confirmLabel="Restaurar"
                />
            </div>
        </div>
    );
}
