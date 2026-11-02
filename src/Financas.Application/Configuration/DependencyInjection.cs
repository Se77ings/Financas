using Financas.Application.Services.CategoryServices;
using Financas.Application.Services.CreditCardServices;
using Financas.Application.Services.DashboardServices;
using Financas.Application.Services.EffectiveDateServices;
using Financas.Application.Services.PayableServices;
using Financas.Application.Services.PayableServices.Interfaces;
using Financas.Application.Services.ReceivableServices;
using Financas.Application.Services.RecurrentPaymentServices;
using Financeiro.Business.Interfaces;
using Financeiro.Infra.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Financas.Application.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IEffectiveDateRepository, EffectiveDateRepository>();
            services.AddScoped<IEffectiveDateServices, EffectiveDateServices>();
            services.AddScoped<IEffectiveDateGeneratorService, EffectiveDateGeneratorService>();

            services.AddScoped<IReceivableRepository, ReceivableRepository>();
            services.AddScoped<IReceivableServices, ReceivableServices>();

            services.AddScoped<IImmediateExpenseRepository, ImmediateExpenseRepository>();
            services.AddScoped<IImmediateExpenseServices, ImmediateExpenseServices>();

            services.AddScoped<ICardExpenseRepository, CardExpenseRepository>();
            services.AddScoped<ICardExpenseServices, CardExpenseServices>();

            services.AddScoped<ICreditCardRepository, CreditCardRepository>();
            services.AddScoped<ICreditCardServices, CreditCardServices>();

            services.AddScoped<IRecurrentPaymentRepository, RecurrentPaymentRepository>();
            services.AddScoped<IRecurrentPaymentServices, RecurrentPaymentServices>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryEntriesRepository, CategoryEntriesRepository>();
            services.AddScoped<ICategoryManagementService, CategoryManagementService>();

            services.AddScoped<IDashboardService, DashboardService>();

            services.AddScoped<ITransactionRunner, TransactionRunner>();

            return services;
        }
    }
}
