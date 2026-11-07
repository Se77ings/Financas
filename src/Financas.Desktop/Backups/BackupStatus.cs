using Financas.Desktop.Settings;

namespace Financas.Desktop.Backups
{
    public sealed record BackupStatus(string? Folder, DateTime? LastBackupAt, string? LastError, bool IsStale)
    {
        private static readonly TimeSpan StaleAfter = TimeSpan.FromHours(48);

        public static BackupStatus From(DesktopSettings settings, DateTimeOffset now)
        {
            var isStale = settings.BackupFolder is null
                || settings.LastBackupAt is null
                || now - settings.LastBackupAt > StaleAfter;

            var lastBackupAt = settings.LastBackupAt is { } value ? ToLocalTimestamp(value) : (DateTime?)null;

            return new BackupStatus(settings.BackupFolder, lastBackupAt, settings.LastBackupError, isStale);
        }

        /// <summary>
        /// O front corta o ISO no "T" para mostrar o dia: em UTC, um backup feito à noite apareceria no dia seguinte.
        /// O tipo Unspecified faz o JSON sair sem Z nem offset.
        /// </summary>
        public static DateTime ToLocalTimestamp(DateTimeOffset value)
        {
            var local = value.LocalDateTime;
            return DateTime.SpecifyKind(local.AddTicks(-(local.Ticks % TimeSpan.TicksPerSecond)), DateTimeKind.Unspecified);
        }
    }

    public sealed record BackupFile(string FileName, DateTime CreatedAt, long SizeBytes);
}
