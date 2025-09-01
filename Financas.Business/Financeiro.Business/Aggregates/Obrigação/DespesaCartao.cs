using System.ComponentModel.Design;

namespace Financeiro.Business.Aggregates.Obrigação
{
	public class DespesaCartao : ContasAPagar
	{
		private List<Despesa> _despesas = new();
		public IEnumerable<Despesa> Despesas => _despesas;

		#region Construtores
		protected DespesaCartao() { }
		public DespesaCartao(string descricao, decimal subtotal, DateTime dataVencimento, decimal? descontos, string observacao, Guid competencia) : base(descricao, subtotal, dataVencimento, descontos, observacao, competencia)
		{ }
		#endregion

		public void AdicionarDespesa(string descricao, decimal subtotal, DateTime dataVencimento, decimal? descontos, string observacao, int parcelaAtual, int qtdParcelas, Guid competenciaId)
		{
			var despesa = new Despesa(descricao, subtotal, dataVencimento, descontos, observacao, parcelaAtual, qtdParcelas, competenciaId);

			_despesas.Add(despesa);
		}
		public void RemoverDespesa(Guid despesaId)
		{
			_despesas.Remove(_despesas.Find(x => x.Id == despesaId));
		}

		public void AtualizarDespesa(Guid despesaId, string descricao, decimal subtotal, DateTime dataVencimento, decimal? descontos, string observacao, int parcelaAtual, int qtdParcelas)
		{
			var despesa = _despesas.Find(x => x.Id == despesaId);

			despesa.AtualizarDespesa(descricao, subtotal, dataVencimento, descontos, observacao, parcelaAtual, qtdParcelas);
		}

		public class Despesa : ContasAPagar
		{
			public int ParcelaAtual { get; private set; }
			public int QtdParcelas { get; private set; }

			public Guid DespesaCartaoId { get; private set; }
			public DespesaCartao DespesaCartao { get; private set; }

			public Despesa(string descricao, decimal subtotal, DateTime dataVencimento, decimal? descontos, string observacao, int parcelaAtual, int qtdParcelas, Guid competenciaId) : base(descricao, subtotal, dataVencimento, descontos, observacao, competenciaId)
			{
				ParcelaAtual = parcelaAtual;
				QtdParcelas = qtdParcelas;
			}

			protected Despesa() { }

			public void AtualizarDespesa(string descricao, decimal subtotal, DateTime dataVencimento, decimal? descontos, string observacao, int parcelaAtual, int qtdParcelas )
			{
				ParcelaAtual = parcelaAtual;
				QtdParcelas = qtdParcelas;

				this.Atualizar(descricao, subtotal, dataVencimento, descontos, observacao, CompetenciaId);
			}
		}
	}

}
