using System.Reflection;
using Financas.API.Controllers;
using Financas.Desktop.Backups;
using Financas.Desktop.Hosting;
using Financas.Desktop.Settings;
using Financas.Desktop.UI;
using Financas.Desktop.Updates;
using Financeiro.Business.Shared.ResultPattern;
using Microsoft.AspNetCore.Mvc;

namespace Financas.Desktop.Controllers
{
    public sealed record DesktopStatus(string Version, bool FirstRunPending, BackupStatus Backup, UpdateStatus Update);

    public sealed record RestoreBackupRequest(string FileName);

    public sealed record RestoreFileResult(bool Started);

    [ApiController]
    [Route("api/desktop")]
    public class DesktopController : ApiControllerBase
    {
        private static readonly string Version = typeof(DesktopController).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;

        private readonly BackupService _backups;
        private readonly RestoreCoordinator _restore;
        private readonly UpdateService _updates;
        private readonly DesktopSettingsStore _settings;
        private readonly DesktopLifetime _lifetime;
        private readonly IFilePicker _filePicker;

        public DesktopController(
            BackupService backups,
            RestoreCoordinator restore,
            UpdateService updates,
            DesktopSettingsStore settings,
            DesktopLifetime lifetime,
            IFilePicker filePicker)
        {
            _backups = backups;
            _restore = restore;
            _updates = updates;
            _settings = settings;
            _lifetime = lifetime;
            _filePicker = filePicker;
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var status = new DesktopStatus(Version, !_settings.Current.FirstRunCompleted, _backups.GetStatus(), _updates.GetStatus());
            return QueryResult(Result<DesktopStatus>.Success(status));
        }

        [HttpPost("backup/folder")]
        public async Task<IActionResult> ChooseBackupFolderAsync()
        {
            var folder = await _filePicker.PickFolderAsync();
            var status = folder is null ? _backups.GetStatus() : await _backups.SetFolderAsync(folder);

            return QueryResult(Result<BackupStatus>.Success(status));
        }

        [HttpPost("backup/run")]
        public async Task<IActionResult> RunBackupAsync()
        {
            return QueryResult(await _backups.RunAsync());
        }

        [HttpGet("backups")]
        public IActionResult GetBackups()
        {
            return QueryResult(_backups.List());
        }

        [HttpPost("backup/restore")]
        public IActionResult Restore([FromBody] RestoreBackupRequest request)
        {
            var result = _restore.PrepareFromBackupFolder(request.FileName);
            if (result.IsSuccess) ExitAfterResponse(DesktopExit.RestartAfterRestore);

            return CommandResult(result);
        }

        [HttpPost("backup/restore/file")]
        public async Task<IActionResult> RestoreFileAsync()
        {
            var file = await _filePicker.PickBackupFileAsync();
            if (file is null) return QueryResult(Result<RestoreFileResult>.Success(new RestoreFileResult(false)));

            var result = _restore.Prepare(file);
            if (!result.IsSuccess) return CommandResult(result);

            ExitAfterResponse(DesktopExit.RestartAfterRestore);
            return QueryResult(Result<RestoreFileResult>.Success(new RestoreFileResult(true)));
        }

        [HttpPost("update/apply")]
        public IActionResult ApplyUpdate()
        {
            if (!_updates.GetStatus().ReadyToApply)
                return CommandResult(Result<bool>.Failure(new Error("Nenhuma atualização pronta para aplicar.")));

            ExitAfterResponse(DesktopExit.RestartWithUpdate);
            return CommandResult(Result<bool>.Success(true));
        }

        [HttpPost("first-run/complete")]
        public IActionResult CompleteFirstRun()
        {
            _settings.Update(settings => settings with { FirstRunCompleted = true });
            return CommandResult(Result<bool>.Success(true));
        }

        // Fechar antes de responder deixaria o front sem saber se o pedido deu certo.
        private void ExitAfterResponse(DesktopExit exit)
        {
            Response.OnCompleted(() =>
            {
                _lifetime.RequestExit(exit);
                return Task.CompletedTask;
            });
        }
    }
}
