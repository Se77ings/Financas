using Financas.Application.Configuration;
using Financas.Desktop;
using Financas.Desktop.Backups;
using Financas.Desktop.Database;
using Financas.Desktop.Settings;
using Financeiro.Infra.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Financas.Tests.Desktop
{
    /// <summary>
    /// Pasta de dados temporária com os mesmos serviços do programa, sem janela e sem tocar no %LOCALAPPDATA% real.
    /// </summary>
    public sealed class DesktopTestEnvironment : IDisposable
    {
        private readonly ServiceProvider _provider;

        public DesktopTestEnvironment()
        {
            Root = Path.Combine(Path.GetTempPath(), $"financeiro-tests-{Guid.NewGuid():N}");
            BackupFolder = Path.Combine(Root, "backups");
            Paths = new DesktopPaths(Path.Combine(Root, "dados"));
            Paths.EnsureCreated();
            Settings = new DesktopSettingsStore(Paths.Settings);

            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
            services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
            services.AddDbContext<FinanceiroDBContext>(options => options.UseFinanceiroSqlite($"Data Source={Paths.Database}"));
            services.RegisterServices();
            services.AddSingleton(Paths);
            services.AddSingleton(Settings);
            services.AddSingleton<BackupService>();
            services.AddSingleton<RestoreCoordinator>();
            services.AddSingleton<DatabaseStartup>();
            _provider = services.BuildServiceProvider();
        }

        public string Root { get; }
        public string BackupFolder { get; }
        public DesktopPaths Paths { get; }
        public DesktopSettingsStore Settings { get; }

        public BackupService Backups => _provider.GetRequiredService<BackupService>();
        public RestoreCoordinator Restore => _provider.GetRequiredService<RestoreCoordinator>();
        public DatabaseStartup Startup => _provider.GetRequiredService<DatabaseStartup>();

        public IServiceScope CreateScope() => _provider.CreateScope();

        public void UseBackupFolder()
        {
            Directory.CreateDirectory(BackupFolder);
            Settings.Update(settings => settings with { BackupFolder = BackupFolder });
        }

        public void Execute(string databasePath, string sql)
        {
            using var connection = new SqliteConnection(SqliteBackup.ConnectionString(databasePath));
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        public object? Scalar(string databasePath, string sql)
        {
            using var connection = new SqliteConnection(SqliteBackup.ConnectionString(databasePath));
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            return command.ExecuteScalar();
        }

        public void Dispose()
        {
            _provider.Dispose();
            SqliteConnection.ClearAllPools();

            if (Directory.Exists(Root)) Directory.Delete(Root, recursive: true);
        }
    }
}
