using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Financas.Desktop.Hosting;
using Financas.Desktop.UI;
using Microsoft.Extensions.Logging.Abstractions;

namespace Financas.Tests.Desktop
{
    public class DesktopApiHostTests : IAsyncLifetime
    {
        internal sealed class FakeFilePicker : IFilePicker
        {
            public string? Folder { get; set; }
            public string? File { get; set; }

            public Task<string?> PickFolderAsync() => Task.FromResult(Folder);
            public Task<string?> PickBackupFileAsync() => Task.FromResult(File);
        }

        private readonly DesktopTestEnvironment _environment = new();
        private readonly FakeFilePicker _filePicker = new();
        private readonly DesktopLifetime _lifetime = new();
        private readonly DesktopApiHost _host;
        private readonly HttpClient _client;
        private readonly HttpClient _anonymousClient;

        public DesktopApiHostTests()
        {
            _host = new DesktopApiHost(_environment.Paths, _environment.Settings, _filePicker, _lifetime, NullLoggerProvider.Instance);
            _client = new HttpClient();
            _anonymousClient = new HttpClient();
        }

        public async Task InitializeAsync()
        {
            await _host.StartAsync();
            _client.BaseAddress = new Uri(_host.BaseAddress);
            _anonymousClient.BaseAddress = new Uri(_host.BaseAddress);
            _client.DefaultRequestHeaders.Add("Cookie", $"{SessionTokenMiddleware.CookieName}={_host.Token}");
        }

        public async Task DisposeAsync()
        {
            await _host.ShutdownAsync(DesktopExit.RestartAfterRestore);
            _client.Dispose();
            _anonymousClient.Dispose();
            _environment.Dispose();
        }

        [Fact]
        public async Task Sem_cookie_responde_401()
        {
            var response = await _anonymousClient.GetAsync("api/desktop/status");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Sem_cookie_nem_a_pagina_inicial_e_servida()
        {
            var response = await _anonymousClient.GetAsync("");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Recusa_host_diferente_de_127_0_0_1()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "api/desktop/status");
            request.Headers.Host = "site-malicioso.example";

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Api_compartilhada_usa_o_sqlite_da_pasta_de_dados()
        {
            var response = await _client.GetAsync("api/category");

            response.EnsureSuccessStatusCode();
            Assert.Equal(12, (await response.Content.ReadFromJsonAsync<JsonElement>()).GetArrayLength());
        }

        [Fact]
        public async Task Status_segue_o_contrato()
        {
            var status = await _client.GetFromJsonAsync<JsonElement>("api/desktop/status");

            Assert.False(string.IsNullOrEmpty(status.GetProperty("version").GetString()));
            Assert.True(status.GetProperty("firstRunPending").GetBoolean());

            var backup = status.GetProperty("backup");
            Assert.Equal(JsonValueKind.Null, backup.GetProperty("folder").ValueKind);
            Assert.Equal(JsonValueKind.Null, backup.GetProperty("lastBackupAt").ValueKind);
            Assert.Equal(JsonValueKind.Null, backup.GetProperty("lastError").ValueKind);
            Assert.True(backup.GetProperty("isStale").GetBoolean());

            var update = status.GetProperty("update");
            Assert.Equal(JsonValueKind.Null, update.GetProperty("availableVersion").ValueKind);
            Assert.False(update.GetProperty("readyToApply").GetBoolean());
        }

        [Fact]
        public async Task Cancelar_o_seletor_de_pasta_mantem_a_atual()
        {
            var response = await _client.PostAsync("api/desktop/backup/folder", null);

            var backup = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal(JsonValueKind.Null, backup.GetProperty("folder").ValueKind);
        }

        [Fact]
        public async Task Backup_sem_pasta_responde_400_com_mensagem()
        {
            var response = await _client.PostAsync("api/desktop/backup/run", null);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("pasta de backup", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Pasta_escolhida_gera_backup_listado_com_data_local()
        {
            _filePicker.Folder = _environment.BackupFolder;

            var backup = await (await _client.PostAsync("api/desktop/backup/folder", null)).Content.ReadFromJsonAsync<JsonElement>();
            var backups = await _client.GetFromJsonAsync<JsonElement>("api/desktop/backups");

            Assert.False(backup.GetProperty("isStale").GetBoolean());
            Assert.Matches(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$", backup.GetProperty("lastBackupAt").GetString()!);

            var item = Assert.Single(backups.EnumerateArray());
            Assert.StartsWith("financeiro-", item.GetProperty("fileName").GetString());
            Assert.Matches(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$", item.GetProperty("createdAt").GetString()!);
            Assert.True(item.GetProperty("sizeBytes").GetInt64() > 0);
        }

        [Fact]
        public async Task Restaurar_responde_e_so_depois_pede_o_reinicio()
        {
            _filePicker.Folder = _environment.BackupFolder;
            await _client.PostAsync("api/desktop/backup/folder", null);
            var fileName = Path.GetFileName(Assert.Single(Directory.GetFiles(_environment.BackupFolder)));

            var exit = new TaskCompletionSource<DesktopExit>();
            _lifetime.ExitRequested += value => exit.TrySetResult(value);

            var response = await _client.PostAsJsonAsync("api/desktop/backup/restore", new { fileName });

            response.EnsureSuccessStatusCode();
            Assert.Equal(DesktopExit.RestartAfterRestore, await exit.Task.WaitAsync(TimeSpan.FromSeconds(5)));
        }

        [Fact]
        public async Task Cancelar_o_seletor_de_arquivo_nao_inicia_restauracao()
        {
            var result = await (await _client.PostAsync("api/desktop/backup/restore/file", null)).Content.ReadFromJsonAsync<JsonElement>();

            Assert.False(result.GetProperty("started").GetBoolean());
        }

        [Fact]
        public async Task Sem_atualizacao_pronta_responde_400()
        {
            var response = await _client.PostAsync("api/desktop/update/apply", null);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Concluir_primeiro_uso_persiste()
        {
            (await _client.PostAsync("api/desktop/first-run/complete", null)).EnsureSuccessStatusCode();

            Assert.True(_environment.Settings.Current.FirstRunCompleted);
        }

        [Fact]
        public void Reutiliza_a_porta_salva()
        {
            Assert.Equal(_host.Port, _environment.Settings.Current.Port);
        }
    }

    public class DesktopApiHostPortTests
    {
        [Fact]
        public async Task Porta_salva_ocupada_sorteia_outra_e_salva()
        {
            using var environment = new DesktopTestEnvironment();
            using var occupied = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
            occupied.Start();
            var port = ((IPEndPoint)occupied.LocalEndpoint).Port;
            environment.Settings.Update(settings => settings with { Port = port });

            var host = new DesktopApiHost(environment.Paths, environment.Settings, new DesktopApiHostTests.FakeFilePicker(), new DesktopLifetime(), NullLoggerProvider.Instance);
            await host.StartAsync();
            await host.ShutdownAsync(DesktopExit.RestartAfterRestore);

            Assert.NotEqual(port, host.Port);
            Assert.Equal(host.Port, environment.Settings.Current.Port);
        }
    }
}
