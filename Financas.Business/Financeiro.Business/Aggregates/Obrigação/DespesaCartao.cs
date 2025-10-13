namespace Financeiro.Business.Aggregates.Obrigação
{
	public class DespesaCartao : ContasAPagar
	{
		private List<Despesa> _despesas = new();
		public ICollection<Despesa> Despesas => _despesas;

		#region Construtores
		protected DespesaCartao() { }
		public DespesaCartao(string descricao, decimal subtotal, DateTime dataVencimento, decimal? descontos, string observacao, Guid competencia) : base(descricao, subtotal, dataVencimento, descontos, observacao, competencia)
		{ }
		#endregion

		public void AdicionarDespesa(Despesa despesa)
		{
			_despesas.Add(despesa);
			RecalculateValue();
		}
		public void RemoverDespesa(Guid despesaId)
		{
			_despesas.Remove(_despesas.Find(x => x.Id == despesaId));
			RecalculateValue();
		}

		public void AtualizarDespesa(Guid despesaId, string descricao, decimal subtotal, DateTime dataVencimento, decimal? descontos, string observacao, int parcelaAtual, int qtdParcelas)
		{
			var despesa = _despesas.Find(x => x.Id == despesaId);

			despesa.AtualizarDespesa(descricao, subtotal, dataVencimento, descontos, observacao, parcelaAtual, qtdParcelas);
			RecalculateValue();
		}

		private void RecalculateValue()
		{
			var listSubTotal = _despesas.Sum(x => x.SubTotal);
			var listDiscounts = _despesas.Sum(x => x.Descontos);
			this.SetValues(listSubTotal, listDiscounts);
		}		

		public class Despesa : ContasAPagar
		{
			public int ParcelaAtual { get; private set; }
			public int QtdParcelas { get; private set; }

			public Guid DespesaCartaoId { get; private set; }
			public DespesaCartao DespesaCartao { get; private set; }

			public Despesa(string descricao, decimal subtotal, DateTime dataVencimento, decimal? descontos, string observacao, int parcelaAtual, int qtdParcelas, Guid competenciaId, Guid despesaCartaoId) : base(descricao, subtotal, dataVencimento, descontos, observacao, competenciaId)
			{
				ParcelaAtual = parcelaAtual;
				QtdParcelas = qtdParcelas;
				DespesaCartaoId = despesaCartaoId;
			}

			protected Despesa() { }

			public void AtualizarDespesa(string descricao, decimal subtotal, DateTime dataVencimento, decimal? descontos, string observacao, int parcelaAtual, int qtdParcelas)
			{
				ParcelaAtual = parcelaAtual;
				QtdParcelas = qtdParcelas;

				this.Atualizar(descricao, subtotal, dataVencimento, descontos, observacao, CompetenciaId);
			}
		}
	}

}
