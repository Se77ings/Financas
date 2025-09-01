using Financeiro.Business.Aggregates.Direito;

namespace Financeiro.Business.Models.DireitoModels
{
	public class ContasAReceberView
	{
		public string Descricao { get; set; }
		public decimal SubTotal { get; set; }
		public decimal Descontos { get; set; }
		public decimal Total { get; set; }
		public DateTime Data { get; set; }

		public static implicit operator ContasAReceberView(ContasAReceber contasAReceber)
		{
			return new ContasAReceberView
			{
				Descricao = contasAReceber.Descricao,
				SubTotal = contasAReceber.SubTotal,
				Descontos = contasAReceber.Descontos,
				Total = contasAReceber.Total,
				Data = contasAReceber.Data
			};
		}
	}
}
