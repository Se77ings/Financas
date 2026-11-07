export interface DesktopBackupStatus {
    folder: string | null;
    lastBackupAt: string | null;
    lastError: string | null;
    isStale: boolean;
}

export interface DesktopUpdateStatus {
    availableVersion: string | null;
    readyToApply: boolean;
}

export interface DesktopBackupFile {
    fileName: string;
    createdAt: string;
    sizeBytes: number;
}

export default interface DesktopStatusView {
    version: string;
    firstRunPending: boolean;
    backup: DesktopBackupStatus;
    update: DesktopUpdateStatus;
}
