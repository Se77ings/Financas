using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Financeiro.Infra.Context
{
    public static class DatabaseRegistration
    {
        // Só o nome: o projeto das migrations do SQLite referencia o Infra, então a referência inversa seria circular.
        private const string SqliteMigrationsAssembly = "Financeiro.Infra.Sqlite";

        public static IServiceCollection AddFinanceiroDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var provider = configuration["Database:Provider"] ?? "SqlServer";
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            return provider switch
            {
                "SqlServer" => services.AddDbContext<FinanceiroDBContext>(options => options.UseSqlServer(connectionString)),
                "Sqlite" => services.AddDbContext<FinanceiroDBContext>(options => options.UseFinanceiroSqlite(connectionString)),
                _ => throw new InvalidOperationException($"Database:Provider inválido: '{provider}'. Use SqlServer ou Sqlite.")
            };
        }

        public static DbContextOptionsBuilder UseFinanceiroSqlite(this DbContextOptionsBuilder options, SqliteConnection connection)
        {
            return options.UseSqlite(connection, sqlite => sqlite.MigrationsAssembly(SqliteMigrationsAssembly));
        }

        public static DbContextOptionsBuilder UseFinanceiroSqlite(this DbContextOptionsBuilder options, string? connectionString)
        {
            return options.UseSqlite(connectionString, sqlite => sqlite.MigrationsAssembly(SqliteMigrationsAssembly));
        }
    }
}
