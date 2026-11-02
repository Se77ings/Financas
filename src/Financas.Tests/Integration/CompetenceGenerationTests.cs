using Financas.Application.Services.EffectiveDateServices;
using Financas.Application.Services.RecurrentPaymentServices;
using Financas.Tests.Infrastructure;
using Financeiro.Application.Models.RecurrentPaymentModels;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Shared;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Financas.Tests.Integration
{
    public class CompetenceGenerationTests : IntegrationTestBase
    {
        [Fact]
        public async Task ItemParcelado_LancaUmaParcelaEmCadaFaturaEAtualizaOsTotais()
        {
            await CreateCreditCardAsync();
            await CreateEffectiveDateAsync(CalendarMonth.Oct, 2026);
            await CreateEffectiveDateAsync(CalendarMonth.Nov, 2026);
            await CreateEffectiveDateAsync(CalendarMonth.Dec, 2026);

            var october = await GetInvoiceAsync(CalendarMonth.Oct, 2026);
            await AddItemAsync(october.Id, subtotal: 100, quantity: 3, dueDate: new DateOnly(2026, 10, 5));

            foreach (var (month, installment, dueDate) in new[]
            {
                (CalendarMonth.Oct, 1, new DateOnly(2026, 10, 5)),
                (CalendarMonth.Nov, 2, new DateOnly(2026, 11, 5)),
                (CalendarMonth.Dec, 3, new DateOnly(2026, 12, 5)),
            })
            {
                var invoice = await GetInvoiceAsync(month, 2026);
                var item = Assert.Single(invoice.Expenses);

                Assert.Equal(100, invoice.Total);
                Assert.Equal(installment, item.CurrentInstallment);
                Assert.Equal(dueDate, item.DueDate);
            }
        }

        [Fact]
        public async Task NovaCompetencia_LevaAParcelaSeguinteComADataDoMesSeguinte()
        {
            await CreateCreditCardAsync();
            await CreateEffectiveDateAsync(CalendarMonth.Oct, 2026);
            var october = await GetInvoiceAsync(CalendarMonth.Oct, 2026);
            await AddItemAsync(october.Id, subtotal: 100, quantity: 3, dueDate: new DateOnly(2026, 10, 5));

            await CreateEffectiveDateAsync(CalendarMonth.Nov, 2026);

            var november = await GetInvoiceAsync(CalendarMonth.Nov, 2026);
            var carried = Assert.Single(november.Expenses);
            Assert.Equal(2, carried.CurrentInstallment);
            Assert.Equal(new DateOnly(2026, 11, 5), carried.DueDate);
            Assert.Equal(november.Id, carried.CardExpenseId);
            Assert.Equal(100, november.Total);
        }

        [Fact]
        public async Task PrimeiraCompetencia_GeraFaturaERecorrenciaAVistaMesmoSemMesAnterior()
        {
            await CreateCreditCardAsync();
            await AddRecurrentPaymentAsync(creditCardId: null, value: 99.99m, start: new DateOnly(2026, 10, 5));

            await CreateEffectiveDateAsync(CalendarMonth.Oct, 2026);

            await GetInvoiceAsync(CalendarMonth.Oct, 2026);
            var immediate = await RunAsync(services => services.GetRequiredService<FinanceiroDBContext>()
                .Set<ImmediateExpense>().AsNoTracking().SingleAsync());
            Assert.Equal(99.99m, immediate.Total);
            Assert.Equal(new DateOnly(2026, 10, 5), immediate.DueDate);
        }

        [Fact]
        public async Task SincronizarCartoes_CriaAFaturaComAssinaturasEParcelasDoMesAnterior()
        {
            // As competências existem antes do cartão, então nenhuma fatura é gerada na criação delas.
            await CreateEffectiveDateAsync(CalendarMonth.Sep, 2026);
            var octoberId = await CreateEffectiveDateAsync(CalendarMonth.Oct, 2026);
            var septemberId = (await RunAsync(services => services.GetRequiredService<FinanceiroDBContext>().EffectiveDate
                .AsNoTracking().SingleAsync(x => x.Month == CalendarMonth.Sep))).Id;
            var cardId = await CreateCreditCardAsync();
            await AddRecurrentPaymentAsync(creditCardId: cardId, value: 29.90m, start: new DateOnly(2026, 9, 10));

            await SyncAsync(septemberId);
            var september = await GetInvoiceAsync(CalendarMonth.Sep, 2026);
            await AddItemAsync(september.Id, subtotal: 100, quantity: 3, dueDate: new DateOnly(2026, 9, 5));

            await SyncAsync(octoberId);

            var october = await GetInvoiceAsync(CalendarMonth.Oct, 2026);
            Assert.Contains(october.Expenses, x => x.Total == 29.90m);
            var carried = Assert.Single(october.Expenses, x => x.QuantityInstallments == 3);
            Assert.Equal(2, carried.CurrentInstallment);
            Assert.Equal(new DateOnly(2026, 10, 5), carried.DueDate);
        }

        private Task AddRecurrentPaymentAsync(Guid? creditCardId, decimal value, DateOnly start) => RunAsync(async services =>
        {
            var result = await services.GetRequiredService<IRecurrentPaymentServices>().AddAsync(new RecurrentPaymentRequest
            {
                Description = creditCardId is null ? "Internet" : "Streaming",
                Value = value,
                FrequencyType = FrequencyType.Mensal,
                CreditCardId = creditCardId,
                StartDate = start,
                IsActive = true,
            });
            Assert.True(result.IsSuccess, result.Error?.Message);
            return result;
        });

        private Task SyncAsync(Guid effectiveDateId) => RunAsync(async services =>
        {
            var result = await services.GetRequiredService<IEffectiveDateServices>().SyncMissingCardInvoicesAsync(effectiveDateId);
            Assert.True(result.IsSuccess, result.Error?.Message);
            return result;
        });
    }
}
