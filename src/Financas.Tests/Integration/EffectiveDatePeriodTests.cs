using Financas.Application.Services.EffectiveDateServices;
using Financas.Tests.Infrastructure;
using Financeiro.Application.Models.EffectiveDateModels;
using Financeiro.Business.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Financas.Tests.Integration
{
    public class EffectiveDatePeriodTests : IntegrationTestBase
    {
        [Fact]
        public async Task Detalhe_TrazOMesCivilDaCompetenciaEOPeriodoDasComprasDeCadaFatura()
        {
            await CreateCreditCardAsync();
            var novemberId = await CreateEffectiveDateAsync(CalendarMonth.Nov, 2026);
            var invoice = await GetInvoiceAsync(CalendarMonth.Nov, 2026);
            await AddItemAsync(invoice.Id, subtotal: 50, quantity: 1, dueDate: new DateOnly(2026, 10, 28), description: "Mercado");
            await AddItemAsync(invoice.Id, subtotal: 80, quantity: 1, dueDate: new DateOnly(2026, 10, 3), description: "Farmácia");
            await AddItemAsync(invoice.Id, subtotal: 20, quantity: 1, dueDate: new DateOnly(2026, 10, 15), description: "Padaria");

            var view = await GetViewAsync(novemberId);

            Assert.Equal(new DateOnly(2026, 11, 1), view.PeriodStart);
            Assert.Equal(new DateOnly(2026, 11, 30), view.PeriodEnd);
            var card = Assert.Single(view.CardExpense);
            Assert.Equal(new DateOnly(2026, 11, 5), card.DueDate);
            Assert.Equal(3, card.Items.Count);
            Assert.Equal(new DateOnly(2026, 10, 3), card.FirstItemDate);
            Assert.Equal(new DateOnly(2026, 10, 28), card.LastItemDate);
        }

        [Fact]
        public async Task Detalhe_FaturaSemItensNaoTemPeriodoDeCompras()
        {
            await CreateCreditCardAsync();
            var februaryId = await CreateEffectiveDateAsync(CalendarMonth.Feb, 2028);

            var view = await GetViewAsync(februaryId);

            Assert.Equal(new DateOnly(2028, 2, 1), view.PeriodStart);
            Assert.Equal(new DateOnly(2028, 2, 29), view.PeriodEnd);
            var card = Assert.Single(view.CardExpense);
            Assert.Null(card.FirstItemDate);
            Assert.Null(card.LastItemDate);
        }

        private Task<EffectiveDateView> GetViewAsync(Guid id) => RunAsync(async services =>
        {
            var result = await services.GetRequiredService<IEffectiveDateServices>().GetByIdAsync(id);
            Assert.True(result.IsSuccess, result.Error?.Message);
            return result.Value!;
        });
    }
}
