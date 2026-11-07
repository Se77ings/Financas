using System.Diagnostics;
using Financas.Desktop.Hosting;
using Financas.Desktop.Settings;
using Financas.Desktop.UI;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Financas.Desktop
{
    public sealed class MainForm : Form
    {
        private const string RuntimeDownloadUrl = "https://developer.microsoft.com/microsoft-edge/webview2/";

        private readonly DesktopPaths _paths;
        private readonly DesktopApiHost _api;
        private readonly ILogger _logger;
        private readonly WebView2 _webView = new() { Dock = DockStyle.Fill };
        private DesktopExit _exit = DesktopExit.Close;
        private bool _closing;
        private bool _readyToClose;

        public MainForm(DesktopPaths paths, DesktopSettingsStore settings, ILoggerProvider logs)
        {
            _paths = paths;
            _logger = logs.CreateLogger(nameof(MainForm));

            var lifetime = new DesktopLifetime();
            lifetime.ExitRequested += OnExitRequested;
            _api = new DesktopApiHost(paths, settings, new WindowsFilePicker(this), lifetime, logs);

            Text = "Financeiro";
            Icon = Icon.ExtractAssociatedIcon(Environment.ProcessPath!);
            ClientSize = new Size(1280, 800);
            MinimumSize = new Size(800, 600);
            StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(_webView);

            Load += OnLoad;
            FormClosing += OnFormClosing;
        }

        public bool RestartRequested => _exit == DesktopExit.RestartAfterRestore;

        private async void OnLoad(object? sender, EventArgs e)
        {
            try
            {
                // Fora da thread da janela: as migrations podem demorar e ela ficaria sem responder.
                await Task.Run(() => _api.StartAsync());
                await OpenWebViewAsync();
            }
            catch (WebView2RuntimeNotFoundException exception)
            {
                _logger.LogError(exception, "WebView2 Runtime ausente");
                ShowRuntimeMissing();
                Close();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Falha ao abrir o programa");
                MessageBox.Show($"Não foi possível abrir o Financeiro. Os detalhes estão em {_paths.Logs}.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private async Task OpenWebViewAsync()
        {
            var environment = await CoreWebView2Environment.CreateAsync(userDataFolder: _paths.WebView);
            await _webView.EnsureCoreWebView2Async(environment);

            var webView = _webView.CoreWebView2;
#if !DEBUG
            webView.Settings.AreDevToolsEnabled = false;
            webView.Settings.AreDefaultContextMenusEnabled = false;
#endif
            webView.Settings.IsStatusBarEnabled = false;

            var cookie = webView.CookieManager.CreateCookie(SessionTokenMiddleware.CookieName, _api.Token, DesktopApiHost.Host, "/");
            cookie.IsHttpOnly = true;
            cookie.SameSite = CoreWebView2CookieSameSiteKind.Strict;
            webView.CookieManager.AddOrUpdateCookie(cookie);

            webView.NavigationStarting += OnNavigationStarting;
            webView.NewWindowRequested += OnNewWindowRequested;
            webView.Navigate(_api.BaseAddress);
        }

        private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri.StartsWith(_api.BaseAddress, StringComparison.OrdinalIgnoreCase)) return;

            e.Cancel = true;
            OpenInBrowser(e.Uri);
        }

        private void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
            OpenInBrowser(e.Uri);
        }

        // Só http e https: outros esquemas podem acionar programas do computador.
        private static void OpenInBrowser(string address)
        {
            if (Uri.TryCreate(address, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
        }

        private void ShowRuntimeMissing()
        {
            var answer = MessageBox.Show(
                "O Financeiro precisa do Microsoft Edge WebView2 Runtime, que não está instalado neste computador. Abrir a página de instalação?",
                Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (answer == DialogResult.Yes) OpenInBrowser(RuntimeDownloadUrl);
        }

        private void OnExitRequested(DesktopExit exit)
        {
            BeginInvoke(() =>
            {
                _exit = exit;
                Close();
            });
        }

        private async void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_readyToClose) return;

            e.Cancel = true;
            if (_closing) return;

            _closing = true;
            Hide();

            try
            {
                await _api.ShutdownAsync(_exit);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Falha ao encerrar a API");
            }

            _readyToClose = true;
            Close();
        }
    }
}
