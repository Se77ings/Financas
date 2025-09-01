using Financeiro.Business.Aggregates.Obrigação;

namespace Financeiro.Business.Models.ObrigaçãoModels
{
	public class DespesaAVistaView
	{
		public Guid Id { get; set; }
		public string Descricao { get; set; }
		public decimal Total { get; set; }
		public DateTime DataVencimento { get; set; }
		public decimal SubTotal { get; set; }
		public decimal Descontos { get; set; }
		public string? Observacao { get; set; }

		public static implicit operator DespesaAVistaView(DespesaAVista despesa)
		{
			return new DespesaAVistaView
			{
				Id = despesa.Id,
				Descricao = despesa.Descricao,
				Total = despesa.Total,
				DataVencimento = despesa.DataVencimento,
				SubTotal = despesa.SubTotal,
				Descontos = despesa.Descontos,
				Observacao = despesa.Observacao,
			};
		}
	}
}
