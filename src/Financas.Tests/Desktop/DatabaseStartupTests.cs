using Financas.Application.Services.CategoryServices;
using Financas.Desktop.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Financas.Tests.Desktop
{
    public class DatabaseStartupTests : IDisposable
    {
        private readonly DesktopTestEnvironment _environment = new();

        public void Dispose() => _environment.Dispose();

        private async Task<List<string>> CategoryNamesAsync()
        {
            using var scope = _environment.CreateScope();
            var result = await scope.ServiceProvider.GetRequiredService<ICategoryService>().GetListAsync();
            return result.Value!.Select(category => category.Name).ToList();
        }

        [Fact]
        public async Task Banco_novo_recebe_as_categorias_iniciais()
        {
            await _environment.Startup.RunAsync();

            Assert.Equal(DefaultCategories.Names.Order(), (await CategoryNamesAsync()).Order());
            Assert.Empty(Directory.GetFiles(_environment.Paths.PreMigrate));
        }

        [Fact]
        public async Task Categoria_excluida_nao_volta_na_proxima_abertura()
        {
            await _environment.Startup.RunAsync();
            _environment.Execute(_environment.Paths.Database, "DELETE FROM Categories WHERE Name = 'Lazer'");

            await _environment.Startup.RunAsync();

            Assert.DoesNotContain("Lazer", await CategoryNamesAsync());
            Assert.Equal(DefaultCategories.Names.Count - 1, (await CategoryNamesAsync()).Count);
        }

        [Fact]
        public async Task Sem_migration_pendente_nao_copia_o_banco()
        {
            await _environment.Startup.RunAsync();
            await _environment.Startup.RunAsync();

            Assert.Empty(Directory.GetFiles(_environment.Paths.PreMigrate));
        }

        [Fact]
        public async Task Migration_pendente_em_banco_existente_copia_antes_de_migrar()
        {
            _environment.Execute(_environment.Paths.Database, "CREATE TABLE Marcador (Id INTEGER)");

            await _environment.Startup.RunAsync();

            var copy = Assert.Single(Directory.GetFiles(_environment.Paths.PreMigrate));
            Assert.Equal(0L, _environment.Scalar(copy, "SELECT count(*) FROM Marcador"));
            Assert.Equal(0L, _environment.Scalar(copy, "SELECT count(*) FROM sqlite_master WHERE name = '__EFMigrationsHistory'"));
            Assert.Empty(await CategoryNamesAsync());
        }
    }
}
