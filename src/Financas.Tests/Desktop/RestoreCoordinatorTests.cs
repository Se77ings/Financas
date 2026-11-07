using Financas.Desktop.Backups;

namespace Financas.Tests.Desktop
{
    public class RestoreCoordinatorTests : IAsyncLifetime
    {
        private readonly DesktopTestEnvironment _environment = new();

        public async Task InitializeAsync()
        {
            await _environment.Startup.RunAsync();
            _environment.UseBackupFolder();
        }

        public Task DisposeAsync()
        {
            _environment.Dispose();
            return Task.CompletedTask;
        }

        private async Task<string> CreateBackupAsync()
        {
            await _environment.Backups.RunAsync();
            return Assert.Single(Directory.GetFiles(_environment.BackupFolder));
        }

        private void AssertRefused(string file, string expectedMessage)
        {
            var result = _environment.Restore.Prepare(file);

            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Error!.Message);
            Assert.False(File.Exists(_environment.Paths.RestorePending));
            Assert.False(File.Exists(_environment.Paths.RestoreIncoming));
        }

        [Fact]
        public void Recusa_arquivo_que_nao_e_banco()
        {
            var file = Path.Combine(_environment.Root, "invalido.db");
            File.WriteAllText(file, "isto não é um banco SQLite, mas tem texto suficiente para o cabeçalho ser lido");

            AssertRefused(file, "não é um backup válido");
        }

        [Fact]
        public void Recusa_arquivo_vazio()
        {
            var file = Path.Combine(_environment.Root, "vazio.db");
            File.WriteAllBytes(file, []);

            AssertRefused(file, "não é um backup válido");
        }

        [Fact]
        public void Recusa_banco_de_outro_programa()
        {
            var file = Path.Combine(_environment.Root, "outro.db");
            _environment.Execute(file, "CREATE TABLE Coisa (Id INTEGER)");

            AssertRefused(file, "não é um backup válido");
        }

        [Fact]
        public async Task Recusa_backup_com_migration_que_o_programa_nao_conhece()
        {
            var file = await CreateBackupAsync();
            _environment.Execute(file, "INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('29990101000000_Futura', '9.0.4')");

            AssertRefused(file, "versão mais nova");
        }

        [Theory]
        [InlineData(@"..\dados\settings.json")]
        [InlineData("financeiro-inexistente.db")]
        public async Task Recusa_nome_fora_da_lista_de_backups(string fileName)
        {
            await CreateBackupAsync();

            var result = _environment.Restore.PrepareFromBackupFolder(fileName);

            Assert.False(result.IsSuccess);
            Assert.True(result.Error!.IsNotFound);
            Assert.False(File.Exists(_environment.Paths.RestorePending));
        }

        [Fact]
        public async Task Backup_valido_fica_pendente_e_copia_o_banco_atual()
        {
            var file = await CreateBackupAsync();

            var result = _environment.Restore.PrepareFromBackupFolder(Path.GetFileName(file));

            Assert.True(result.IsSuccess);
            Assert.True(File.Exists(_environment.Paths.RestorePending));
            Assert.Single(Directory.GetFiles(_environment.Paths.PreRestore));
            Assert.True(_environment.Settings.Current.FirstRunCompleted);
        }

        [Fact]
        public async Task Proxima_abertura_troca_o_banco_e_descarta_o_wal_antigo()
        {
            const string countQuery = "SELECT count(*) FROM Categories";
            var file = await CreateBackupAsync();
            var categoriesInBackup = _environment.Scalar(file, countQuery);

            _environment.Execute(_environment.Paths.Database, "DELETE FROM Categories");
            Assert.True(_environment.Restore.Prepare(file).IsSuccess);

            await _environment.Startup.RunAsync();

            Assert.False(File.Exists(_environment.Paths.RestorePending));
            Assert.Equal(categoriesInBackup, _environment.Scalar(_environment.Paths.Database, countQuery));
            Assert.Equal("ok", _environment.Scalar(_environment.Paths.Database, "PRAGMA integrity_check"));
        }

        [Fact]
        public void Sem_restauracao_pendente_nao_mexe_no_banco()
        {
            var modified = File.GetLastWriteTimeUtc(_environment.Paths.Database);

            _environment.Restore.ApplyPending();

            Assert.Equal(modified, File.GetLastWriteTimeUtc(_environment.Paths.Database));
        }
    }
}
