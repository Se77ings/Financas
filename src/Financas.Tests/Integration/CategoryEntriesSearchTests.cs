using Financas.Application.Services.CategoryServices;
using Financas.Application.Services.PayableServices.Interfaces;
using Financas.Application.Services.ReceivableServices;
using Financas.Tests.Infrastructure;
using Financeiro.Application.Models.CategoryModels;
using Financeiro.Application.Models.EntryModels;
using Financeiro.Application.Models.PayableModels;
using Financeiro.Application.Models.ReceivableModels;
using Financeiro.Business.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Financas.Tests.Integration
{
    public class CategoryEntriesSearchTests : IntegrationTestBase
    {
        [Fact]
        public async Task BuscaPorDescricao_IgnoraMaiusculasEAcentos_ETrazACategoria()
        {
            await CreateCreditCardAsync("Nubank");
            await CreateEffectiveDateAsync(CalendarMonth.Oct, 2026);
            var invoice = await GetInvoiceAsync(CalendarMonth.Oct, 2026);
            var categoryId = await RunAsync(async services =>
                (await services.GetRequiredService<ICategoryService>().AddAsync(new CategoryRequest { Name = "Alimentação" })).Value!.Id);

            await RunAsync(services => services.GetRequiredService<IImmediateExpenseServices>().AddAsync(new ImmediateExpenseRequest
            {
                Description = "Padaria São João",
                DueDate = new DateOnly(2026, 10, 3),
                SubTotal = 30,
                CategoryId = categoryId,
            }));
            await AddItemAsync(invoice.Id, subtotal: 80, quantity: 1, dueDate: new DateOnly(2026, 10, 9), description: "PÃO DE AÇÚCAR");
            await RunAsync(services => services.GetRequiredService<IReceivableServices>().AddAsync(new ReceivableRequest
            {
                Description = "Reembolso pão",
                SubTotal = 15,
                Date = new DateOnly(2026, 10, 12),
            }));
            await RunAsync(services => services.GetRequiredService<IImmediateExpenseServices>().AddAsync(new ImmediateExpenseRequest
            {
                Description = "Farmácia",
                DueDate = new DateOnly(2026, 10, 4),
                SubTotal = 50,
            }));

            var result = await RunAsync(services => services.GetRequiredService<ICategoryManagementService>().SearchEntriesAsync(" pao "));
            var bakery = await RunAsync(services => services.GetRequiredService<ICategoryManagementService>().SearchEntriesAsync("SAO JOAO"));

            Assert.True(result.IsSuccess, result.Error?.Message);
            Assert.Equal(new[] { "Reembolso pão", "PÃO DE AÇÚCAR" }, result.Value!.Select(x => x.Description));
            Assert.All(result.Value!, x => Assert.Null(x.CategoryId));
            var cardItem = result.Value!.Single(x => x.Type == EntryType.CardExpenseItem);
            Assert.Equal("Nubank", cardItem.CreditCardName);
            Assert.Equal(CalendarMonth.Oct, cardItem.Month);

            var expense = Assert.Single(bakery.Value!);
            Assert.Equal(EntryType.ImmediateExpense, expense.Type);
            Assert.Equal(categoryId, expense.CategoryId);
            Assert.Equal("Alimentação", expense.CategoryName);
        }

        [Fact]
        public async Task BuscaComTermoCurto_EhRecusada()
        {
            var result = await RunAsync(services => services.GetRequiredService<ICategoryManagementService>().SearchEntriesAsync(" a "));

            Assert.False(result.IsSuccess);
        }
    }
}
