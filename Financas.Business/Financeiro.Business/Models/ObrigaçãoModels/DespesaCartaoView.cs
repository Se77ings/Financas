using Financeiro.Business.Aggregates.Obrigação;

namespace Financeiro.Business.Models.ObrigaçãoModels
{
	public class DespesaCartaoView
	{
		public Guid Id { get; set; }
		public string Descricao { get; set; }
		public decimal Total { get; set; }
		public DateTime DataVencimento { get; set; }
		public decimal SubTotal { get; set; }
		public decimal Descontos { get; set; } = 0;
		public string? Observacao { get; set; }

		public List<DespesaCartaoItemView> Itens { get; set; } = new List<DespesaCartaoItemView>();


		public static implicit operator DespesaCartaoView(DespesaCartao cartao)
		{
			return new DespesaCartaoView
			{
				Id = cartao.Id,
				Descricao = cartao.Descricao,
				Total = cartao.Total,
				DataVencimento = cartao.DataVencimento,
				SubTotal = cartao.SubTotal,
				Descontos = cartao.Descontos,
				Observacao = cartao.Observacao,
				Itens = cartao.Despesas.Select(d => (DespesaCartaoItemView)d).ToList()
			};
		}
	}
	public class DespesaCartaoItemView
	{
		public Guid Id { get; set; }
		public string Descricao { get; set; }
		public decimal Total { get; set; }
		public DateTime DataVencimento { get; set; }
		public decimal SubTotal { get; set; }
		public decimal Descontos { get; set; } = 0;
		public string? Observacao { get; set; }

		public static implicit operator DespesaCartaoItemView(DespesaCartao.Despesa despesa)
		{
			return new DespesaCartaoItemView
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
