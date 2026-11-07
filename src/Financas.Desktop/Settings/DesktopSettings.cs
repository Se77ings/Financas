namespace Financas.Desktop.Settings
{
    public sealed record DesktopSettings
    {
        public int? Port { get; init; }
        public string? BackupFolder { get; init; }
        public DateTimeOffset? LastBackupAt { get; init; }
        public string? LastBackupError { get; init; }
        public bool FirstRunCompleted { get; init; }
    }
}
