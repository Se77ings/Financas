using Financas.Application.Services.CategoryServices;
using Financas.Application.Services.EffectiveDateServices;
using Financas.Tests.Infrastructure;
using Financeiro.Application.Models.CategoryModels;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Shared;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Financas.Tests.Integration
{
    public class SqliteMigrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task Migrations_CobremTodoOModelo()
        {
            var hasPendingChanges = await RunAsync(services =>
                Task.FromResult(services.GetRequiredService<FinanceiroDBContext>().Database.HasPendingModelChanges()));

            Assert.False(hasPendingChanges);
        }

        [Fact]
        public async Task CriarCategoria_ComNomeDiferenteSoNasMaiusculas_EhRecusado()
        {
            await RunAsync(async services =>
            {
                var categories = services.GetRequiredService<ICategoryService>();
                var first = await categories.AddAsync(new CategoryRequest { Name = "Moradia" });
                Assert.True(first.IsSuccess, first.Error?.Message);

                var duplicate = await categories.AddAsync(new CategoryRequest { Name = "MORADIA" });

                Assert.False(duplicate.IsSuccess);
                return duplicate;
            });
        }

        [Fact]
        public async Task ExcluirCompetencia_ComFaturaVazia_RemoveAFaturaEmCascata()
        {
            await CreateCreditCardAsync();
            var effectiveDateId = await CreateEffectiveDateAsync(CalendarMonth.Nov, 2026);
            await GetInvoiceAsync(CalendarMonth.Nov, 2026);

            var result = await RunAsync(services => services.GetRequiredService<IEffectiveDateServices>().DeleteAsync(effectiveDateId));

            Assert.True(result.IsSuccess, result.Error?.Message);
            await RunAsync(async services =>
            {
                var context = services.GetRequiredService<FinanceiroDBContext>();
                Assert.False(await context.EffectiveDate.AnyAsync());
                Assert.False(await context.Payable.AnyAsync());
                Assert.False(await context.Payable.OfType<CardExpense>().AnyAsync());
                return true;
            });
        }
    }
}
