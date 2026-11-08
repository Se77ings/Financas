using Financas.Application.Services.DashboardServices;
using Financas.Tests.Infrastructure;
using Financeiro.Business.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Financas.Tests.Integration
{
    public class DashboardTests : IntegrationTestBase
    {
        [Fact]
        public async Task HistoricoDosCartoes_TemQuatroMesesAnterioresEOSeguinte()
        {
            await CreateCreditCardAsync("Nubank");
            foreach (var month in new[] { CalendarMonth.May, CalendarMonth.Jun, CalendarMonth.Jul, CalendarMonth.Aug, CalendarMonth.Sep, CalendarMonth.Oct, CalendarMonth.Nov })
            {
                await CreateEffectiveDateAsync(month, 2026);
                var invoice = await GetInvoiceAsync(month, 2026);
                await AddItemAsync(invoice.Id, subtotal: (int)month * 10, quantity: 1, dueDate: new DateOnly(2026, (int)month, 5));
            }

            var dashboard = (await RunAsync(services => services.GetRequiredService<IDashboardService>().GetAsync(2026, CalendarMonth.Sep, new DateOnly(2026, 9, 20)))).Value!;

            Assert.Equal(
                new CalendarMonth?[] { CalendarMonth.May, CalendarMonth.Jun, CalendarMonth.Jul, CalendarMonth.Aug, CalendarMonth.Sep, CalendarMonth.Oct },
                dashboard.CreditCardHistory.Select(x => x.Month));
            var september = Assert.Single(dashboard.CreditCardHistory, x => x.IsSelected);
            Assert.Equal(CalendarMonth.Sep, september.Month);
            var invoiceAmount = Assert.Single(september.Invoices);
            Assert.Equal("Nubank", invoiceAmount.Name);
            Assert.Equal(90, invoiceAmount.Total);
        }

        [Fact]
        public async Task GastosPorDiaEPorCategoria_TrazemOsLancamentos()
        {
            await CreateCreditCardAsync("Nubank");
            await CreateEffectiveDateAsync(CalendarMonth.Oct, 2026);
            var october = await GetInvoiceAsync(CalendarMonth.Oct, 2026);
            await AddItemAsync(october.Id, subtotal: 50, quantity: 3, dueDate: new DateOnly(2026, 10, 7), description: "Tênis");

            var dashboard = (await RunAsync(services => services.GetRequiredService<IDashboardService>().GetAsync(2026, CalendarMonth.Oct, new DateOnly(2026, 10, 20)))).Value!;

            Assert.NotNull(dashboard.DailySpending);
            Assert.Equal(31, dashboard.DailySpending.SelectedMonth.Count);
            Assert.Equal(30, dashboard.DailySpending.LastDays.Count);
            var entry = Assert.Single(dashboard.DailySpending.SelectedMonth.Single(x => x.Date == new DateOnly(2026, 10, 7)).Entries);
            Assert.Equal("Tênis", entry.Description);
            Assert.Equal("Nubank", entry.CreditCardName);
            Assert.Equal("1/3", entry.Installment);
            Assert.Equal(50, Assert.Single(Assert.Single(dashboard.SpendingByCategory).Entries).Total);
        }

        [Fact]
        public async Task AnoSelecionado_LimitaOsTotaisEOHistoricoAoAno()
        {
            await CreateCreditCardAsync("Nubank");
            await CreateYearsWithOneItemEachAsync();

            var dashboard = (await RunAsync(services => services.GetRequiredService<IDashboardService>().GetAsync(2026, CalendarMonth.Jan, new DateOnly(2026, 1, 20)))).Value!;

            Assert.Equal(20, dashboard.Totals.Expenses);
            var month = Assert.Single(dashboard.History);
            Assert.Equal(CalendarMonth.Jan, month.Month);
            Assert.Equal(2026, month.Year);
        }

        [Fact]
        public async Task TodosOsAnos_AgrupaPorAnoESemOsQuadrosDoMes()
        {
            await CreateCreditCardAsync("Nubank");
            await CreateYearsWithOneItemEachAsync();

            var dashboard = (await RunAsync(services => services.GetRequiredService<IDashboardService>().GetAsync(null, null, new DateOnly(2026, 1, 20)))).Value!;

            Assert.Null(dashboard.DailySpending);
            Assert.Null(dashboard.SpendingPace);
            Assert.Null(dashboard.FutureCommitments);
            Assert.Equal(30, dashboard.Totals.Expenses);
            Assert.Equal(30, dashboard.SpendingByCategory.Sum(x => x.Total));
            Assert.Equal(new[] { 2025, 2026 }, dashboard.History.Select(x => x.Year));
            Assert.All(dashboard.History, x => Assert.Null(x.Month));
            Assert.Equal(new[] { 2025, 2026 }, dashboard.CreditCardHistory.Select(x => x.Year));
            Assert.Equal(20, Assert.Single(dashboard.CreditCardHistory.Single(x => x.Year == 2026).Invoices).Total);
        }

        [Fact]
        public async Task AnoSemMes_Falha()
        {
            var result = await RunAsync(services => services.GetRequiredService<IDashboardService>().GetAsync(2026, null, new DateOnly(2026, 1, 20)));

            Assert.False(result.IsSuccess);
            Assert.False(result.Error!.IsNotFound);
        }

        private async Task CreateYearsWithOneItemEachAsync()
        {
            await CreateEffectiveDateAsync(CalendarMonth.Dec, 2025);
            await AddItemAsync((await GetInvoiceAsync(CalendarMonth.Dec, 2025)).Id, subtotal: 10, quantity: 1, dueDate: new DateOnly(2025, 12, 5));
            await CreateEffectiveDateAsync(CalendarMonth.Jan, 2026);
            await AddItemAsync((await GetInvoiceAsync(CalendarMonth.Jan, 2026)).Id, subtotal: 20, quantity: 1, dueDate: new DateOnly(2026, 1, 5));
        }
    }
}
