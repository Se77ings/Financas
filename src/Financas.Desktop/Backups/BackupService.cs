using Financas.Desktop.Settings;
using Financeiro.Business.Shared.ResultPattern;
using Microsoft.Extensions.Logging;

namespace Financas.Desktop.Backups
{
    public sealed class BackupService
    {
        private const int Retention = 30;
        private const string BackupPattern = "financeiro-*.db";
        private const string TemporaryPattern = BackupPattern + ".tmp";

        private static readonly TimeSpan BackupInterval = TimeSpan.FromHours(24);

        private readonly DesktopPaths _paths;
        private readonly DesktopSettingsStore _settings;
        private readonly ILogger<BackupService> _logger;
        private readonly SemaphoreSlim _gate = new(1, 1);

        public BackupService(DesktopPaths paths, DesktopSettingsStore settings, ILogger<BackupService> logger)
        {
            _paths = paths;
            _settings = settings;
            _logger = logger;
        }

        public static string NewFileName() => $"financeiro-{DateTime.Now:yyyy-MM-dd_HHmmss}.db";

        public BackupStatus GetStatus() => BackupStatus.From(_settings.Current, DateTimeOffset.Now);

        public bool IsDue(DateTimeOffset now)
        {
            var settings = _settings.Current;

            return settings.BackupFolder is not null
                && (settings.LastBackupAt is null || now - settings.LastBackupAt > BackupInterval);
        }

        /// <summary>
        /// Escolher a pasta já faz um backup, que confirma se ela é gravável; a falha fica em lastError.
        /// </summary>
        public async Task<BackupStatus> SetFolderAsync(string folder)
        {
            _settings.Update(settings => settings with { BackupFolder = folder, LastBackupError = null });
            await RunAsync();
            return GetStatus();
        }

        public async Task<Result<BackupStatus>> RunAsync()
        {
            var folder = _settings.Current.BackupFolder;
            if (folder is null)
                return Result<BackupStatus>.Failure(new Error("Escolha a pasta de backup antes de fazer um backup."));

            await _gate.WaitAsync();
            try
            {
                await Task.Run(() => CopyToFolder(folder));
                _settings.Update(settings => settings with { LastBackupAt = DateTimeOffset.Now, LastBackupError = null });
                return Result<BackupStatus>.Success(GetStatus());
            }
            catch (Exception exception)
            {
                // Pasta de rede fora do ar ou pendrive removido não podem derrubar o programa.
                _logger.LogWarning(exception, "Falha no backup em {Folder}", folder);

                var message = $"Não foi possível fazer o backup em {folder}. Confira se a pasta existe e pode ser gravada.";
                _settings.Update(settings => settings with { LastBackupError = message });
                return Result<BackupStatus>.Failure(new Error(message));
            }
            finally
            {
                _gate.Release();
            }
        }

        public async Task RunOnCloseAsync(TimeSpan timeout)
        {
            if (_settings.Current.BackupFolder is null) return;

            try
            {
                await RunAsync().WaitAsync(timeout);
            }
            catch (TimeoutException)
            {
                // A cópia fica em arquivo .tmp, que o próximo backup limpa; a pasta nunca vê um backup pela metade.
                _logger.LogWarning("O backup ao fechar passou de {Timeout} e foi abandonado", timeout);
            }
        }

        public Result<List<BackupFile>> List()
        {
            var folder = _settings.Current.BackupFolder;
            if (folder is null || !Directory.Exists(folder))
                return Result<List<BackupFile>>.Success([]);

            try
            {
                return Result<List<BackupFile>>.Success(FindBackups(folder)
                    .Select(file => new BackupFile(file.Name, BackupStatus.ToLocalTimestamp(file.LastWriteTimeUtc), file.Length))
                    .ToList());
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                _logger.LogWarning(exception, "Falha ao listar os backups de {Folder}", folder);
                return Result<List<BackupFile>>.Failure(new Error($"Não foi possível ler a pasta de backup {folder}."));
            }
        }

        /// <returns><c>null</c> se o nome não for de um backup da pasta atual.</returns>
        public string? FindPath(string fileName)
        {
            var folder = _settings.Current.BackupFolder;
            if (folder is null || !Directory.Exists(folder)) return null;

            return FindBackups(folder).FirstOrDefault(file => file.Name == fileName)?.FullName;
        }

        private void CopyToFolder(string folder)
        {
            Directory.CreateDirectory(folder);

            var target = Path.Combine(folder, NewFileName());
            var temporary = target + ".tmp";

            // O sincronizador da pasta (Drive, OneDrive) só pode enxergar o backup completo.
            SqliteBackup.Copy(_paths.Database, temporary);
            File.Move(temporary, target, overwrite: true);

            ApplyRetention(folder);
        }

        private void ApplyRetention(string folder)
        {
            var obsolete = FindBackups(folder).Skip(Retention).Concat(new DirectoryInfo(folder).GetFiles(TemporaryPattern));

            foreach (var file in obsolete)
            {
                try
                {
                    file.Delete();
                }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
                {
                    // O backup novo já está salvo; o arquivo antigo sai na próxima rodada.
                    _logger.LogWarning(exception, "Falha ao apagar {File}", file.FullName);
                }
            }
        }

        private static IEnumerable<FileInfo> FindBackups(string folder)
        {
            return new DirectoryInfo(folder).GetFiles(BackupPattern).OrderByDescending(file => file.LastWriteTimeUtc);
        }
    }
}
