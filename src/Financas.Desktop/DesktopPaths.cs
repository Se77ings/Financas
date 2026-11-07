namespace Financas.Desktop
{
    public sealed class DesktopPaths
    {
        public DesktopPaths(string root)
        {
            Root = root;
        }

        // Fora de %LOCALAPPDATA%\Financeiro porque a desinstalação do Velopack apaga aquela pasta.
        public static DesktopPaths Default() => new(Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Financeiro.Dados"));

        public string Root { get; }
        public string Database => Path.Combine(Root, "financeiro.db");
        public string Settings => Path.Combine(Root, "settings.json");
        public string WebView => Path.Combine(Root, "webview");
        public string PreMigrate => Path.Combine(Root, "pre-migrate");
        public string PreRestore => Path.Combine(Root, "pre-restore");
        public string Logs => Path.Combine(Root, "logs");
        public string RestoreIncoming => Path.Combine(Root, "restore.incoming");
        public string RestorePending => Path.Combine(Root, "restore.pending");

        public void EnsureCreated()
        {
            Directory.CreateDirectory(Root);
            Directory.CreateDirectory(PreMigrate);
            Directory.CreateDirectory(PreRestore);
            Directory.CreateDirectory(Logs);
        }
    }
}
