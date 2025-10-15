using Financas.Application.Services.CompetenciaServices;
using Financas.Application.Services.DireitoServices;
using Financas.Application.Services.ObrigaçãoServices;
using Financas.Application.Services.PayableServices;
using Financas.Application.Services.PayableServices.Interfaces;
using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Infra.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Financas.Application.Configuration
{
	public static class DependencyInjection
	{
		public static void RegisterServices(this IServiceCollection services)
		{
			services.AddScoped<IEffectiveDateRepository, EffectiveDateRepository>();
			services.AddScoped<IEffectiveDateServices, EffectiveDateServices>();

			services.AddScoped<IReceivableRepository, ReceivableRepository>();
			services.AddScoped<ReceivableServices, RececivableServices>();

			services.AddScoped<IPayableRepository, PayableRepository>();
			services.AddScoped<IImmediateExpenseServices, ImmediateExpenseServices>();

			services.AddScoped<ICardExpenseRepository, CardExpenseRepository>();
			services.AddScoped<ICardExpenseServices, CardExpenseServices>();
		}
	}
}
