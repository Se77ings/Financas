using System.Text.Json;
using Financas.Desktop.Backups;
using Financas.Desktop.Settings;
using Microsoft.Data.Sqlite;

namespace Financas.Tests.Desktop
{
    public class BackupServiceTests : IAsyncLifetime
    {
        private readonly DesktopTestEnvironment _environment = new();

        public Task InitializeAsync() => _environment.Startup.RunAsync();

        public Task DisposeAsync()
        {
            _environment.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Sem_pasta_configurada_devolve_falha()
        {
            var result = await _environment.Backups.RunAsync();

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task Backup_contem_o_que_ainda_esta_no_wal_e_nao_deixa_temporario()
        {
            _environment.UseBackupFolder();
            using var keepsWalOpen = new SqliteConnection(SqliteBackup.ConnectionString(_environment.Paths.Database));
            keepsWalOpen.Open();
            using var read = keepsWalOpen.CreateCommand();
            read.CommandText = "SELECT count(*) FROM Categories";
            read.ExecuteScalar();
            _environment.Execute(_environment.Paths.Database, "UPDATE Categories SET Description = 'Teste' WHERE Name = 'Lazer'");
            Assert.Equal("wal", _environment.Scalar(_environment.Paths.Database, "PRAGMA journal_mode"));
            Assert.True(new FileInfo(_environment.Paths.Database + "-wal").Length > 0);

            var result = await _environment.Backups.RunAsync();

            Assert.True(result.IsSuccess);
            var file = Assert.Single(Directory.GetFiles(_environment.BackupFolder));
            Assert.EndsWith(".db", file);
            Assert.Equal(1L, _environment.Scalar(file, "SELECT count(*) FROM Categories WHERE Description = 'Teste'"));
            Assert.NotNull(_environment.Settings.Current.LastBackupAt);
            Assert.Null(_environment.Settings.Current.LastBackupError);
        }

        [Fact]
        public async Task Mantem_so_os_30_mais_novos_e_limpa_temporario_abandonado()
        {
            _environment.UseBackupFolder();

            for (var day = 1; day <= 32; day++)
            {
                var old = Path.Combine(_environment.BackupFolder, $"financeiro-2020-{day:000}_000000.db");
                File.WriteAllText(old, "x");
                File.SetLastWriteTimeUtc(old, new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(day));
            }

            var other = Path.Combine(_environment.BackupFolder, "outro-arquivo.db");
            File.WriteAllText(other, "x");
            File.WriteAllText(Path.Combine(_environment.BackupFolder, "financeiro-2020-01-01_000000.db.tmp"), "x");

            await _environment.Backups.RunAsync();

            var backups = _environment.Backups.List().Value!;
            Assert.Equal(30, backups.Count);
            Assert.DoesNotContain(backups, backup => backup.FileName == "financeiro-2020-003_000000.db");
            Assert.Contains(backups, backup => backup.FileName == "financeiro-2020-004_000000.db");
            Assert.True(File.Exists(other));
            Assert.Empty(Directory.GetFiles(_environment.BackupFolder, "*.tmp"));
        }

        [Fact]
        public async Task Falha_fica_em_lastError_sem_lancar_excecao()
        {
            var notAFolder = Path.Combine(_environment.Root, "arquivo");
            File.WriteAllText(notAFolder, "x");
            _environment.Settings.Update(settings => settings with { BackupFolder = notAFolder });

            var result = await _environment.Backups.RunAsync();

            Assert.False(result.IsSuccess);
            Assert.Equal(result.Error!.Message, _environment.Backups.GetStatus().LastError);
        }

        [Fact]
        public async Task Backup_bem_sucedido_limpa_o_erro_anterior()
        {
            _environment.UseBackupFolder();
            _environment.Settings.Update(settings => settings with { LastBackupError = "erro antigo" });

            await _environment.Backups.RunAsync();

            Assert.Null(_environment.Backups.GetStatus().LastError);
        }

        [Fact]
        public async Task Lista_o_mais_novo_primeiro()
        {
            _environment.UseBackupFolder();
            var older = Path.Combine(_environment.BackupFolder, "financeiro-2020-01-01_000000.db");
            File.WriteAllText(older, "x");
            File.SetLastWriteTimeUtc(older, new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            await _environment.Backups.RunAsync();

            var backups = _environment.Backups.List().Value!;
            Assert.Equal(2, backups.Count);
            Assert.Equal("financeiro-2020-01-01_000000.db", backups[1].FileName);
        }

        [Fact]
        public async Task Escolher_pasta_ja_faz_o_primeiro_backup()
        {
            var status = await _environment.Backups.SetFolderAsync(_environment.BackupFolder);

            Assert.False(status.IsStale);
            Assert.Single(Directory.GetFiles(_environment.BackupFolder));
        }

        [Theory]
        [InlineData(25, true)]
        [InlineData(23, false)]
        public void Fica_devido_depois_de_24_horas(int hoursAgo, bool expected)
        {
            var now = DateTimeOffset.Now;
            _environment.UseBackupFolder();
            _environment.Settings.Update(settings => settings with { LastBackupAt = now.AddHours(-hoursAgo) });

            Assert.Equal(expected, _environment.Backups.IsDue(now));
        }

        [Fact]
        public void Sem_pasta_nunca_fica_devido()
        {
            Assert.False(_environment.Backups.IsDue(DateTimeOffset.Now));
        }

        [Fact]
        public void Datas_saem_em_hora_local_sem_offset()
        {
            var value = new DateTimeOffset(2026, 11, 7, 1, 30, 15, 500, TimeSpan.Zero);

            var json = JsonSerializer.Serialize(BackupStatus.ToLocalTimestamp(value));

            Assert.Equal($"\"{value.LocalDateTime:yyyy-MM-ddTHH:mm:ss}\"", json);
        }
    }

    public class BackupStatusTests
    {
        private static readonly DateTimeOffset Now = new(2026, 11, 6, 12, 0, 0, TimeSpan.Zero);

        [Fact]
        public void Sem_pasta_esta_desatualizado()
        {
            var settings = new DesktopSettings { LastBackupAt = Now };

            Assert.True(BackupStatus.From(settings, Now).IsStale);
        }

        [Fact]
        public void Sem_nenhum_backup_esta_desatualizado()
        {
            var settings = new DesktopSettings { BackupFolder = "pasta" };

            Assert.True(BackupStatus.From(settings, Now).IsStale);
        }

        [Theory]
        [InlineData(47, false)]
        [InlineData(48, false)]
        [InlineData(49, true)]
        public void Desatualiza_com_mais_de_48_horas(int hoursAgo, bool expected)
        {
            var settings = new DesktopSettings { BackupFolder = "pasta", LastBackupAt = Now.AddHours(-hoursAgo) };

            Assert.Equal(expected, BackupStatus.From(settings, Now).IsStale);
        }
    }
}
