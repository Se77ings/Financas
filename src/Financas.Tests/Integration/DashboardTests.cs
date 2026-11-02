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
            var ids = new List<Guid>();
            foreach (var month in new[] { CalendarMonth.May, CalendarMonth.Jun, CalendarMonth.Jul, CalendarMonth.Aug, CalendarMonth.Sep, CalendarMonth.Oct, CalendarMonth.Nov })
            {
                ids.Add(await CreateEffectiveDateAsync(month, 2026));
                var invoice = await GetInvoiceAsync(month, 2026);
                await AddItemAsync(invoice.Id, subtotal: (int)month * 10, quantity: 1, dueDate: new DateOnly(2026, (int)month, 5));
            }
            var septemberId = ids[4];

            var dashboard = (await RunAsync(services => services.GetRequiredService<IDashboardService>().GetAsync(septemberId, new DateOnly(2026, 9, 20)))).Value!;

            Assert.Equal(
                new[] { CalendarMonth.May, CalendarMonth.Jun, CalendarMonth.Jul, CalendarMonth.Aug, CalendarMonth.Sep, CalendarMonth.Oct },
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
            var octoberId = await CreateEffectiveDateAsync(CalendarMonth.Oct, 2026);
            var october = await GetInvoiceAsync(CalendarMonth.Oct, 2026);
            await AddItemAsync(october.Id, subtotal: 50, quantity: 3, dueDate: new DateOnly(2026, 10, 7), description: "Tênis");

            var dashboard = (await RunAsync(services => services.GetRequiredService<IDashboardService>().GetAsync(octoberId, new DateOnly(2026, 10, 20)))).Value!;

            Assert.Equal(31, dashboard.DailySpending.SelectedMonth.Count);
            Assert.Equal(30, dashboard.DailySpending.LastDays.Count);
            var entry = Assert.Single(dashboard.DailySpending.SelectedMonth.Single(x => x.Date == new DateOnly(2026, 10, 7)).Entries);
            Assert.Equal("Tênis", entry.Description);
            Assert.Equal("Nubank", entry.CreditCardName);
            Assert.Equal("1/3", entry.Installment);
            Assert.Equal(50, Assert.Single(Assert.Single(dashboard.SpendingByCategory).Entries).Total);
        }
    }
}
