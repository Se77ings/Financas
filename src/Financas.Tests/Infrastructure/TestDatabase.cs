using Financas.Application.Configuration;
using Financeiro.Infra.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Financas.Tests.Infrastructure
{
    /// <summary>
    /// Banco SQLite em memória com os mesmos services e repositórios da API. Cada <see cref="CreateScope"/>
    /// equivale a uma requisição: um DbContext novo, como no ASP.NET.
    /// </summary>
    public sealed class TestDatabase : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ServiceProvider _provider;

        public TestDatabase()
        {
            // O banco em memória do SQLite só existe enquanto esta conexão estiver aberta.
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<FinanceiroDBContext>(options => options.UseFinanceiroSqlite(_connection));
            services.RegisterServices();
            _provider = services.BuildServiceProvider();

            using var scope = CreateScope();
            scope.ServiceProvider.GetRequiredService<FinanceiroDBContext>().Database.Migrate();
        }

        public IServiceScope CreateScope() => _provider.CreateScope();

        public async Task<T> RunAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using var scope = CreateScope();
            return await action(scope.ServiceProvider);
        }

        public void Dispose()
        {
            _provider.Dispose();
            _connection.Dispose();
        }
    }
}
