using Financas.Application.Services.CategoryServices;
using Financas.Desktop.Backups;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Financas.Desktop.Database
{
    public sealed class DatabaseStartup
    {
        private readonly DesktopPaths _paths;
        private readonly RestoreCoordinator _restore;
        private readonly IServiceScopeFactory _scopes;
        private readonly ILogger<DatabaseStartup> _logger;

        public DatabaseStartup(DesktopPaths paths, RestoreCoordinator restore, IServiceScopeFactory scopes, ILogger<DatabaseStartup> logger)
        {
            _paths = paths;
            _restore = restore;
            _scopes = scopes;
            _logger = logger;
        }

        /// <summary>
        /// Precisa terminar antes de a API aceitar requisições. A restauração vem primeiro porque um backup
        /// pode ser mais antigo que o programa e também passar pelas migrations.
        /// </summary>
        public async Task RunAsync()
        {
            _restore.ApplyPending();

            // Só em banco novo: uma categoria que o usuário excluiu não pode voltar a cada atualização.
            var isNewDatabase = !File.Exists(_paths.Database);

            using var scope = _scopes.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<FinanceiroDBContext>().Database;

            if (!isNewDatabase && database.GetPendingMigrations().Any())
                SqliteBackup.Copy(_paths.Database, Path.Combine(_paths.PreMigrate, BackupService.NewFileName()));

            database.Migrate();

            if (isNewDatabase)
                await DefaultCategories.CreateAsync(scope.ServiceProvider.GetRequiredService<ICategoryService>(), _logger);
        }
    }
}
