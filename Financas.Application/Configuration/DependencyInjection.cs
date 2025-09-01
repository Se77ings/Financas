using Financas.Application.Services.CompetenciaServices;
using Financas.Application.Services.DireitoServices;
using Financas.Application.Services.ObrigaçãoServices;
using Financas.Application.Services.ObrigaçãoServices.Interfaces;
using Financeiro.Business.Interfaces;
using Financeiro.Infra.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Financas.Application.Configuration
{
	public static class DependencyInjection
	{
		public static void RegisterServices(this IServiceCollection services)
		{
			services.AddScoped<ICompetenciaRepository, CompetenciaRepository>();
			services.AddScoped<ICompetenciaServices, CompetenciaServices>();

			services.AddScoped<IContasAReceberRepository, ContasAReceberRepository>();
			services.AddScoped<IContasAReceberServices, ContasAReceberServices>();

			services.AddScoped<IDespesaAVistaRepository, DespesaAVistaRepository>();
			services.AddScoped<IDespesaAVistaServices, DespesaAVistaServices>();

			services.AddScoped<IDespesaCartaoRepository, DespesaCartaoRepository>();
			services.AddScoped<IDespesaCartaoServices, DespesaCartaoServices>();
		}
	}
}
