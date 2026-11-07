using Financas.Desktop.Settings;
using Financeiro.Business.Shared.ResultPattern;
using Financeiro.Infra.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Financas.Desktop.Backups
{
    /// <summary>
    /// A troca do banco só acontece na próxima abertura, antes de existir qualquer conexão: com a API no ar,
    /// o arquivo estaria em uso. Aqui o backup é apenas validado e deixado em restore.pending.
    /// </summary>
    public sealed class RestoreCoordinator
    {
        private const string InvalidBackupMessage = "O arquivo escolhido não é um backup válido do Financeiro.";

        private readonly DesktopPaths _paths;
        private readonly DesktopSettingsStore _settings;
        private readonly BackupService _backups;
        private readonly IServiceScopeFactory _scopes;
        private readonly ILogger<RestoreCoordinator> _logger;

        public RestoreCoordinator(
            DesktopPaths paths,
            DesktopSettingsStore settings,
            BackupService backups,
            IServiceScopeFactory scopes,
            ILogger<RestoreCoordinator> logger)
        {
            _paths = paths;
            _settings = settings;
            _backups = backups;
            _scopes = scopes;
            _logger = logger;
        }

        public Result<bool> PrepareFromBackupFolder(string fileName)
        {
            // Procura na listagem da pasta: o nome vem da requisição e não pode apontar para fora dela.
            var path = _backups.FindPath(fileName);

            return path is null
                ? Result<bool>.Failure(Error.NotFound("Backup não encontrado na pasta configurada."))
                : Prepare(path);
        }

        public Result<bool> Prepare(string sourcePath)
        {
            try
            {
                // Valida uma cópia própria: a original pode estar numa pasta sincronizada e mudar até o reinício.
                File.Copy(sourcePath, _paths.RestoreIncoming, overwrite: true);

                var validation = Validate(_paths.RestoreIncoming);
                if (!validation.IsSuccess)
                {
                    File.Delete(_paths.RestoreIncoming);
                    return validation;
                }

                SqliteBackup.Copy(_paths.Database, Path.Combine(_paths.PreRestore, BackupService.NewFileName()));
                File.Move(_paths.RestoreIncoming, _paths.RestorePending, overwrite: true);

                // Quem restaura um backup já tem dados: o passo a passo do primeiro uso perde o sentido.
                _settings.Update(settings => settings with { FirstRunCompleted = true });

                return Result<bool>.Success(true);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or SqliteException)
            {
                _logger.LogWarning(exception, "Falha ao preparar a restauração de {Source}", sourcePath);
                return Result<bool>.Failure(new Error("Não foi possível preparar a restauração. Confira se o arquivo pode ser lido."));
            }
        }

        public void ApplyPending()
        {
            if (!File.Exists(_paths.RestorePending)) return;

            SqliteConnection.ClearAllPools();

            // Os -wal e -shm são do banco antigo. Reaplicados sobre o restaurado, corromperiam o arquivo.
            File.Delete(_paths.Database + "-wal");
            File.Delete(_paths.Database + "-shm");
            File.Move(_paths.RestorePending, _paths.Database, overwrite: true);

            _logger.LogInformation("Backup restaurado");
        }

        private Result<bool> Validate(string path)
        {
            try
            {
                using var connection = new SqliteConnection(SqliteBackup.ConnectionString(path, SqliteOpenMode.ReadWrite));
                connection.Open();

                if (ReadColumn(connection, "PRAGMA integrity_check").FirstOrDefault() != "ok"
                    || !ReadColumn(connection, "SELECT name FROM sqlite_master WHERE type = 'table' AND name = '__EFMigrationsHistory'").Any())
                    return Result<bool>.Failure(new Error(InvalidBackupMessage));

                var applied = ReadColumn(connection, "SELECT MigrationId FROM __EFMigrationsHistory");
                if (applied.Count == 0)
                    return Result<bool>.Failure(new Error(InvalidBackupMessage));

                // Este programa não sabe voltar um banco à frente do código dele: as consultas falhariam em colunas desconhecidas.
                if (applied.Except(KnownMigrations()).Any())
                    return Result<bool>.Failure(new Error("Este backup veio de uma versão mais nova do Financeiro. Atualize o programa antes de restaurar."));

                return Result<bool>.Success(true);
            }
            catch (SqliteException)
            {
                return Result<bool>.Failure(new Error(InvalidBackupMessage));
            }
        }

        private List<string> KnownMigrations()
        {
            using var scope = _scopes.CreateScope();
            return scope.ServiceProvider.GetRequiredService<FinanceiroDBContext>().Database.GetMigrations().ToList();
        }

        private static List<string> ReadColumn(SqliteConnection connection, string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            using var reader = command.ExecuteReader();
            var values = new List<string>();
            while (reader.Read()) values.Add(reader.GetString(0));

            return values;
        }
    }
}
