using Microsoft.Extensions.DependencyInjection;
using WageHistory.Business.Aggregates.Interfaces;
using WageHistory.Application.Services.WageServices;
using WageHistory.Infra.Repository;

namespace WageHistory.Application.Configuration
{
	public static class DependencyInjection
	{
		public static IServiceCollection RegisterServices(this IServiceCollection services)
		{
			services.AddScoped<IWageRepository, WageRepository>();
			services.AddScoped<IWageServices, WageServices>();

			return services;
		}
	}
}
