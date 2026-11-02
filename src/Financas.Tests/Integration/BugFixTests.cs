using Financas.Application.Services.CreditCardServices;
using Financas.Application.Services.EffectiveDateServices;
using Financas.Application.Services.PayableServices.Interfaces;
using Financas.Tests.Infrastructure;
using Financeiro.Application.Models.EffectiveDateModels;
using Financeiro.Application.Models.PayableModels;
using Financeiro.Business.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Financas.Tests.Integration
{
    public class BugFixTests : IntegrationTestBase
    {
        [Fact]
        public async Task EditarCompetencia_ParaUmMesQueJaExiste_EhRecusado()
        {
            await CreateEffectiveDateAsync(CalendarMonth.Oct, 2026);
            var novemberId = await CreateEffectiveDateAsync(CalendarMonth.Nov, 2026);

            var result = await RunAsync(services => services.GetRequiredService<IEffectiveDateServices>()
                .UpdateAsync(novemberId, new EffectiveDateRequest { Month = CalendarMonth.Oct, Year = 2026 }));

            Assert.False(result.IsSuccess);
            Assert.False(result.Error!.IsNotFound);
        }

        [Fact]
        public async Task RegistroInexistente_RetornaNaoEncontrado()
        {
            var result = await RunAsync(services => services.GetRequiredService<ICardExpenseServices>().GetByIdAsync(Guid.NewGuid()));

            Assert.False(result.IsSuccess);
            Assert.True(result.Error!.IsNotFound);
        }

        [Fact]
        public async Task ExcluirCartaoComFaturas_EhRecusadoSemErroDeBanco()
        {
            var cardId = await CreateCreditCardAsync();
            await CreateEffectiveDateAsync(CalendarMonth.Oct, 2026);

            var result = await RunAsync(services => services.GetRequiredService<ICreditCardServices>().DeleteAsync(cardId));

            Assert.False(result.IsSuccess);
            Assert.Contains("fatura", result.Error!.Message);
        }

        [Fact]
        public async Task TransferirParaMesSemFatura_CriaAFaturaComADescricaoDoMesDeDestino()
        {
            var octoberId = await CreateEffectiveDateAsync(CalendarMonth.Oct, 2026);
            await CreateCreditCardAsync("Nubank");
            await CreateEffectiveDateAsync(CalendarMonth.Nov, 2026);
            var november = await GetInvoiceAsync(CalendarMonth.Nov, 2026);
            await AddItemAsync(november.Id, subtotal: 80, quantity: 1, dueDate: new DateOnly(2026, 11, 3));
            var itemId = (await GetInvoiceAsync(CalendarMonth.Nov, 2026)).Expenses.Single().Id;

            var result = await RunAsync(services => services.GetRequiredService<ICardExpenseServices>().TransferItemsAsync(new CardExpenseItemTransferRequest
            {
                CardExpenseItemIds = new List<Guid> { itemId },
                TargetEffectiveDateId = octoberId,
            }));

            Assert.True(result.IsSuccess, result.Error?.Message);
            var october = await GetInvoiceAsync(CalendarMonth.Oct, 2026);
            Assert.Equal($"Fatura Nubank - {CalendarMonth.Oct}/2026", october.Description);
            Assert.Equal(80, october.Total);
            Assert.Empty((await GetInvoiceAsync(CalendarMonth.Nov, 2026)).Expenses);
        }

        [Fact]
        public async Task TransferirParaMesComFatura_MoveOItemEAtualizaAsDuasFaturas()
        {
            await CreateCreditCardAsync();
            var octoberId = await CreateEffectiveDateAsync(CalendarMonth.Oct, 2026);
            await CreateEffectiveDateAsync(CalendarMonth.Nov, 2026);
            var november = await GetInvoiceAsync(CalendarMonth.Nov, 2026);
            await AddItemAsync(november.Id, subtotal: 80, quantity: 1, dueDate: new DateOnly(2026, 11, 3));
            await AddItemAsync(november.Id, subtotal: 20, quantity: 1, dueDate: new DateOnly(2026, 11, 4), description: "Café");
            var itemId = (await GetInvoiceAsync(CalendarMonth.Nov, 2026)).Expenses.Single(x => x.Total == 80).Id;

            var result = await RunAsync(services => services.GetRequiredService<ICardExpenseServices>().TransferItemsAsync(new CardExpenseItemTransferRequest
            {
                CardExpenseItemIds = new List<Guid> { itemId },
                TargetEffectiveDateId = octoberId,
            }));

            Assert.True(result.IsSuccess, result.Error?.Message);
            var october = await GetInvoiceAsync(CalendarMonth.Oct, 2026);
            november = await GetInvoiceAsync(CalendarMonth.Nov, 2026);
            Assert.Equal(itemId, Assert.Single(october.Expenses).Id);
            Assert.Equal(80, october.Total);
            Assert.Equal(20, Assert.Single(november.Expenses).Total);
            Assert.Equal(20, november.Total);
        }

        [Fact]
        public async Task SaldoDaCompetencia_NaoContaOCartaoEmDobro()
        {
            await CreateCreditCardAsync();
            await CreateEffectiveDateAsync(CalendarMonth.Oct, 2026);
            var october = await GetInvoiceAsync(CalendarMonth.Oct, 2026);
            await AddItemAsync(october.Id, subtotal: 100, quantity: 1, dueDate: new DateOnly(2026, 10, 5));

            var list = await RunAsync(services => services.GetRequiredService<IEffectiveDateServices>().GetListAsync());

            Assert.Equal(-100, Assert.Single(list.Value!).Balance);
        }
    }
}
