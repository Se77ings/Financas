using Financas.Application.Services.CompetenciaServices;
using Financas.Application.Services.DireitoServices;
using Financas.Application.Services.ObrigaçãoServices;
using Financas.Application.Services.PayableServices;
using Financas.Application.Services.PayableServices.Interfaces;
using Financas.Application.Services.CreditCardServices;
using Financas.Application.Services.SubscriptionServices;
using Financeiro.Infra.Repository;
using Microsoft.Extensions.DependencyInjection;
using Financeiro.Business.Interfaces;

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
            services.AddScoped<ReceivableServices, RececivableServices>();

            services.AddScoped<IPayableRepository, PayableRepository>();
            services.AddScoped<IImmediateExpenseServices, ImmediateExpenseServices>();

            services.AddScoped<ICardExpenseRepository, CardExpenseRepository>();
            services.AddScoped<ICardExpenseServices, CardExpenseServices>();

            services.AddScoped<ICreditCardRepository, CreditCardRepository>();
            services.AddScoped<ICreditCardServices, CreditCardServices>();

            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ISubscriptionServices, SubscriptionServices>();

            return services;
        }
    }
}
