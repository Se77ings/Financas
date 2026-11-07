import { getAsync, postAsync, readErrorAsync } from "./baseServices";
import DesktopStatusView, { DesktopBackupFile, DesktopBackupStatus } from "@/models/Desktop/DesktopStatusView";

export async function getDesktopStatusAsync(): Promise<DesktopStatusView> {
    const response = await getAsync(`api/desktop/status`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Não foi possível ler a situação do programa'));
    }

    return await response.json();
}

export async function chooseBackupFolderAsync(): Promise<DesktopBackupStatus> {
    const response = await postAsync(`api/desktop/backup/folder`, {});

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Não foi possível escolher a pasta de backup'));
    }

    return await response.json();
}

export async function runBackupAsync(): Promise<DesktopBackupStatus> {
    const response = await postAsync(`api/desktop/backup/run`, {});

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Não foi possível fazer o backup'));
    }

    return await response.json();
}

export async function getBackupsAsync(): Promise<DesktopBackupFile[]> {
    const response = await getAsync(`api/desktop/backups`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Não foi possível listar os backups'));
    }

    return await response.json();
}

export async function restoreBackupAsync(fileName: string): Promise<void> {
    const response = await postAsync(`api/desktop/backup/restore`, { fileName });

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Não foi possível restaurar o backup'));
    }
}

/** Devolve false se a pessoa cancelou o seletor de arquivo: nesse caso o programa não reinicia. */
export async function restoreBackupFromFileAsync(): Promise<boolean> {
    const response = await postAsync(`api/desktop/backup/restore/file`, {});

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Não foi possível restaurar o arquivo'));
    }

    const data: { started: boolean } = await response.json();
    return data.started;
}

export async function applyUpdateAsync(): Promise<void> {
    const response = await postAsync(`api/desktop/update/apply`, {});

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Não foi possível aplicar a atualização'));
    }
}

export async function completeFirstRunAsync(): Promise<void> {
    const response = await postAsync(`api/desktop/first-run/complete`, {});

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Não foi possível concluir a configuração'));
    }
}
