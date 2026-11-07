using Microsoft.Extensions.Hosting;

namespace Financas.Desktop.Backups
{
    public sealed class BackupScheduler : BackgroundService
    {
        private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(1);

        private readonly BackupService _backups;

        public BackupScheduler(BackupService backups)
        {
            _backups = backups;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Sai do trecho síncrono: o host só termina de subir quando este método devolve o primeiro Task.
            await Task.Yield();

            using var timer = new PeriodicTimer(CheckInterval);

            do
            {
                if (_backups.IsDue(DateTimeOffset.Now)) await _backups.RunAsync();
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}
