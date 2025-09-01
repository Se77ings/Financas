using Financeiro.Business.Aggregates;
using Financeiro.Business.Aggregates.Obrigação;
using Financeiro.Business.Models.DireitoModels;
using Financeiro.Business.Models.ObrigaçãoModels;

namespace Financeiro.Business.Models.CompetenciaModels
{
	public class CompetenciaView
	{

		public string Mes { get; set; }
		public int Ano { get; set; }

		public List<ContasAReceberView> ContasAReceberViews { get; set; } = new();
		public List<DespesaAVistaView> DespesasAVista { get; set; } = new();
		public List<DespesaCartaoView> DespesasCartao { get; set; } = new();



		public static implicit operator CompetenciaView(Competencia competencia)
		{
			return new CompetenciaView
			{
				Mes = competencia.Mes,
				Ano = competencia.Ano,
				ContasAReceberViews = competencia.ContasAReceber.Select(c => (ContasAReceberView)c).ToList(),
				DespesasAVista = competencia.ContasAPagar.OfType<DespesaAVista>().Select(d => (DespesaAVistaView)d).ToList(),
				DespesasCartao = competencia.ContasAPagar.OfType<DespesaCartao>().Select(d => (DespesaCartaoView)d).ToList()
			};
		}

	}
}
