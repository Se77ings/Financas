using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Financeiro.Infra.Sqlite
{
    /// <summary>
    /// Usada só pelo dotnet ef neste projeto; o arquivo do banco não é criado ao gerar migrations.
    /// </summary>
    public class FinanceiroSqliteDesignTimeFactory : IDesignTimeDbContextFactory<FinanceiroDBContext>
    {
        public FinanceiroDBContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<FinanceiroDBContext>();
            options.UseFinanceiroSqlite("Data Source=design.db");

            return new FinanceiroDBContext(options.Options);
        }
    }
}
