namespace Financas.Desktop.Hosting
{
    public enum DesktopExit
    {
        Close,
        RestartAfterRestore,
        RestartWithUpdate
    }

    /// <summary>
    /// A API roda fora da thread da janela e não conhece o formulário: pede o encerramento por aqui.
    /// </summary>
    public sealed class DesktopLifetime
    {
        public event Action<DesktopExit>? ExitRequested;

        public void RequestExit(DesktopExit exit) => ExitRequested?.Invoke(exit);
    }
}
