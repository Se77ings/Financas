using System.Diagnostics;
using Financas.Desktop.Hosting;
using Financas.Desktop.Settings;
using Velopack;

namespace Financas.Desktop
{
    internal static class Program
    {
        private const string SingleInstanceMutex = @"Local\Financeiro.Desktop";

        [STAThread]
        private static void Main()
        {
            // Precisa vir antes de tudo: nos eventos de instalação e atualização, o Velopack encerra o processo aqui.
            VelopackApp.Build().Run();

            using var mutex = new Mutex(initiallyOwned: true, SingleInstanceMutex, out var isFirstInstance);
            if (!isFirstInstance)
            {
                MessageBox.Show("O Financeiro já está aberto.", "Financeiro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ApplicationConfiguration.Initialize();

            var paths = DesktopPaths.Default();
            paths.EnsureCreated();

            using var logs = new FileLoggerProvider(paths.Logs);
            var restart = false;

            using (var form = new MainForm(paths, new DesktopSettingsStore(paths.Settings), logs))
            {
                System.Windows.Forms.Application.Run(form);
                restart = form.RestartRequested;
            }

            // A nova instância desistiria ao encontrar o Mutex ocupado.
            mutex.ReleaseMutex();

            if (restart) Process.Start(Environment.ProcessPath!);
        }
    }
}
