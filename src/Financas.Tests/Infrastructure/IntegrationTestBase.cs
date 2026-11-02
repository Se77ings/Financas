using Financas.Application.Services.CreditCardServices;
using Financas.Application.Services.EffectiveDateServices;
using Financas.Application.Services.PayableServices.Interfaces;
using Financeiro.Application.Models.CreditCardModels;
using Financeiro.Application.Models.EffectiveDateModels;
using Financeiro.Application.Models.PayableModels;
using Financeiro.Business.Aggregates.EffectiveDate;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Shared;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Financas.Tests.Infrastructure
{
    public abstract class IntegrationTestBase : IDisposable
    {
        protected TestDatabase Database { get; } = new();

        public void Dispose() => Database.Dispose();

        protected Task<T> RunAsync<T>(Func<IServiceProvider, Task<T>> action) => Database.RunAsync(action);

        protected Task<Guid> CreateCreditCardAsync(string name = "Cartão") => RunAsync(async services =>
        {
            var result = await services.GetRequiredService<ICreditCardServices>().AddAsync(new CreditCardRequest
            {
                Name = name,
                ClosingDay = 28,
                DueDay = 5,
                Limit = 5000,
            });
            Assert.True(result.IsSuccess, result.Error?.Message);

            return (await services.GetRequiredService<FinanceiroDBContext>().CreditCard.AsNoTracking().SingleAsync(x => x.Name == name)).Id;
        });

        protected Task<Guid> CreateEffectiveDateAsync(CalendarMonth month, int year) => RunAsync(async services =>
        {
            var result = await services.GetRequiredService<IEffectiveDateServices>().AddAsync(new EffectiveDateRequest { Month = month, Year = year });
            Assert.True(result.IsSuccess, result.Error?.Message);
            return (await GetEffectiveDateAsync(services, month, year)).Id;
        });

        protected Task AddItemAsync(Guid invoiceId, decimal subtotal, int quantity, DateOnly dueDate, string description = "Notebook") => RunAsync(async services =>
        {
            var result = await services.GetRequiredService<ICardExpenseServices>().AddItemAsync(invoiceId, new CardExpenseItemRequest
            {
                Description = description,
                SubTotal = subtotal,
                DueDate = dueDate,
                CurrentInstallment = 1,
                QuantityInstallments = quantity,
            });
            Assert.True(result.IsSuccess, result.Error?.Message);
            return result;
        });

        protected Task<CardExpense> GetInvoiceAsync(CalendarMonth month, int year) => RunAsync(services =>
            services.GetRequiredService<FinanceiroDBContext>().Payable
                .AsNoTracking()
                .OfType<CardExpense>()
                .Include(x => x.Expenses)
                .SingleAsync(x => x.EffectiveDate.Month == month && x.EffectiveDate.Year == year));

        private static Task<EffectiveDate> GetEffectiveDateAsync(IServiceProvider services, CalendarMonth month, int year) =>
            services.GetRequiredService<FinanceiroDBContext>().EffectiveDate.AsNoTracking().SingleAsync(x => x.Month == month && x.Year == year);
    }
}
