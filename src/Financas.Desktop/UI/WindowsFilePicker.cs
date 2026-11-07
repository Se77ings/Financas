namespace Financas.Desktop.UI
{
    public sealed class WindowsFilePicker : IFilePicker
    {
        private readonly Control _owner;

        public WindowsFilePicker(Control owner)
        {
            _owner = owner;
        }

        public Task<string?> PickFolderAsync()
        {
            return OnWindowThread(() =>
            {
                using var dialog = new FolderBrowserDialog
                {
                    Description = "Escolha a pasta dos backups",
                    UseDescriptionForTitle = true
                };

                return dialog.ShowDialog(_owner) == DialogResult.OK ? dialog.SelectedPath : null;
            });
        }

        public Task<string?> PickBackupFileAsync()
        {
            return OnWindowThread(() =>
            {
                using var dialog = new OpenFileDialog
                {
                    Title = "Escolha o backup do Financeiro",
                    Filter = "Backup do Financeiro (*.db)|*.db"
                };

                return dialog.ShowDialog(_owner) == DialogResult.OK ? dialog.FileName : null;
            });
        }

        // As requisições chegam em threads do Kestrel, e os diálogos só abrem na thread da janela.
        private Task<string?> OnWindowThread(Func<string?> showDialog)
        {
            var result = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);

            _owner.BeginInvoke(() =>
            {
                try
                {
                    result.SetResult(showDialog());
                }
                catch (Exception exception)
                {
                    result.SetException(exception);
                }
            });

            return result.Task;
        }
    }
}
