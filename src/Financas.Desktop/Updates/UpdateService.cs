using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Velopack;
using Velopack.Sources;

namespace Financas.Desktop.Updates
{
    public sealed record UpdateStatus(string? AvailableVersion, bool ReadyToApply);

    public sealed class UpdateService : BackgroundService
    {
        private const string RepositoryUrl = "https://github.com/Se77ings/Financas";

        private readonly ILogger<UpdateService> _logger;
        private UpdateManager? _manager;
        private volatile string? _availableVersion;
        private volatile VelopackAsset? _readyRelease;

        public UpdateService(ILogger<UpdateService> logger)
        {
            _logger = logger;
        }

        public UpdateStatus GetStatus() => new(_availableVersion, _readyRelease is not null);

        /// <summary>
        /// Só engatilha o atualizador: ele espera este processo encerrar para trocar os arquivos.
        /// </summary>
        public void ApplyOnExit(bool restart)
        {
            if (_manager is null || _readyRelease is null) return;

            try
            {
                _manager.WaitExitThenApplyUpdates(_readyRelease, silent: true, restart: restart);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Falha ao preparar a atualização");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            try
            {
                var manager = new UpdateManager(new GithubSource(RepositoryUrl, null, false));
                if (!manager.IsInstalled) return;

                _manager = manager;

                if (manager.UpdatePendingRestart is { } pending)
                {
                    MarkReady(pending);
                    return;
                }

                var update = await manager.CheckForUpdatesAsync();
                if (update is null) return;

                _availableVersion = update.TargetFullRelease.Version.ToString();
                await manager.DownloadUpdatesAsync(update, cancelToken: stoppingToken);
                MarkReady(update.TargetFullRelease);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                // Sem internet o programa segue normal; atualizar nunca pode atrapalhar o uso.
                _logger.LogWarning(exception, "Falha ao procurar atualização");
            }
        }

        private void MarkReady(VelopackAsset release)
        {
            _availableVersion = release.Version.ToString();
            _readyRelease = release;
        }
    }
}
