using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using Financas.API.Core;
using Financas.Desktop.Backups;
using Financas.Desktop.Controllers;
using Financas.Desktop.Database;
using Financas.Desktop.Settings;
using Financas.Desktop.UI;
using Financas.Desktop.Updates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Financas.Desktop.Hosting
{
    public sealed class DesktopApiHost
    {
        public const string Host = "127.0.0.1";

        private static readonly TimeSpan CloseBackupTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan StopTimeout = TimeSpan.FromSeconds(5);

        private readonly DesktopPaths _paths;
        private readonly DesktopSettingsStore _settings;
        private readonly IFilePicker _filePicker;
        private readonly DesktopLifetime _lifetime;
        private readonly ILoggerProvider _logs;
        private WebApplication? _app;

        public DesktopApiHost(DesktopPaths paths, DesktopSettingsStore settings, IFilePicker filePicker, DesktopLifetime lifetime, ILoggerProvider logs)
        {
            _paths = paths;
            _settings = settings;
            _filePicker = filePicker;
            _lifetime = lifetime;
            _logs = logs;
        }

        public string Token { get; } = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        public int Port { get; private set; }

        public string BaseAddress => $"http://{Host}:{Port}/";

        public async Task StartAsync()
        {
            Port = ResolvePort();

            var app = Build();
            await app.Services.GetRequiredService<DatabaseStartup>().RunAsync();
            await app.StartAsync();
            _app = app;
        }

        public async Task ShutdownAsync(DesktopExit exit)
        {
            if (_app is null) return;

            var backups = _app.Services.GetRequiredService<BackupService>();
            var updates = _app.Services.GetRequiredService<UpdateService>();

            // Na restauração o banco atual já foi copiado e será substituído; um backup agora só empurraria outro para fora da retenção.
            if (exit != DesktopExit.RestartAfterRestore)
            {
                await backups.RunOnCloseAsync(CloseBackupTimeout);
                updates.ApplyOnExit(restart: exit == DesktopExit.RestartWithUpdate);
            }

            using var timeout = new CancellationTokenSource(StopTimeout);
            await _app.StopAsync(timeout.Token);
            await _app.DisposeAsync();
            _app = null;

            SqliteConnection.ClearAllPools();
        }

        private WebApplication Build()
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ContentRootPath = AppContext.BaseDirectory,
                EnvironmentName = Environments.Production
            });

            // Variável de ambiente ou appsettings ao lado do executável não podem redirecionar o banco nem abrir a API.
            builder.Configuration.Sources.Clear();
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "Sqlite",
                ["ConnectionStrings:DefaultConnection"] = $"Data Source={_paths.Database}",
                // Sem isto, um site poderia apontar o próprio domínio para 127.0.0.1 e falar com a API (DNS rebinding).
                ["AllowedHosts"] = Host
            });

            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(_logs);
            builder.Logging.AddFilter("Microsoft", LogLevel.Warning);

            builder.WebHost.ConfigureKestrel(kestrel => kestrel.Listen(IPAddress.Loopback, Port));

            builder.Services.AddFinanceiroApi(builder.Configuration).AddApplicationPart(typeof(DesktopController).Assembly);

            builder.Services.AddSingleton(_paths);
            builder.Services.AddSingleton(_settings);
            builder.Services.AddSingleton(_filePicker);
            builder.Services.AddSingleton(_lifetime);
            builder.Services.AddSingleton<BackupService>();
            builder.Services.AddSingleton<RestoreCoordinator>();
            builder.Services.AddSingleton<DatabaseStartup>();
            builder.Services.AddSingleton<UpdateService>();
            builder.Services.AddHostedService(provider => provider.GetRequiredService<UpdateService>());
            builder.Services.AddHostedService<BackupScheduler>();

            var app = builder.Build();

            app.UseMiddleware<SessionTokenMiddleware>(Token);
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.MapControllers();

            return app;
        }

        /// <summary>
        /// A porta faz parte da origem, e o localStorage com as preferências é por origem: só troca se estiver ocupada.
        /// </summary>
        private int ResolvePort()
        {
            if (_settings.Current.Port is int saved && TryBind(saved, out _)) return saved;

            if (!TryBind(0, out var port)) throw new InvalidOperationException("Nenhuma porta livre em 127.0.0.1.");

            _settings.Update(settings => settings with { Port = port });
            return port;
        }

        private static bool TryBind(int port, out int boundPort)
        {
            var listener = new TcpListener(IPAddress.Loopback, port);

            try
            {
                listener.Start();
                boundPort = ((IPEndPoint)listener.LocalEndpoint).Port;
                return true;
            }
            catch (SocketException)
            {
                boundPort = 0;
                return false;
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
