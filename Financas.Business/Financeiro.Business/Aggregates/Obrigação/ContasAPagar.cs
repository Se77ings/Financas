namespace Financeiro.Business.Aggregates.Obrigação
{
	public abstract class ContasAPagar : Entidade
	{
		public string Descricao { get; private set; }
		public decimal Total { get; private set; }
		public DateTime DataVencimento { get; private set; }
		public decimal SubTotal { get; private set; }
		public decimal Descontos { get; private set; } = 0;
		public string? Observacao { get; private set; }
		public Guid CompetenciaId { get; private set; }
		public Competencia Competencia { get; private set; }
		//TODO: Urgente: implementar bool recorrente.
		// caso for recorrente, ao gerar a proxima competencia, verificar na anterior se há dados recorrentes
		// caso haja, gerá-los apenas atualizando o mês


		protected ContasAPagar() { }

		public ContasAPagar(string descricao, decimal subtotal, DateTime dataVencimento, decimal? descontos, string observacao, Guid competenciaId)
		{
			Descricao = descricao;
			SubTotal = subtotal;
			DataVencimento = dataVencimento;
			Descontos = descontos.HasValue ? descontos.Value : 0;
			Observacao = observacao;
			CompetenciaId = competenciaId;

			CalculateTotal();
		}
		public void Atualizar(string descricao, decimal subtotal, DateTime dataVencimento, decimal? descontos, string observacao, Guid competenciaId)
		{
			Descricao = descricao;
			SubTotal = subtotal;
			DataVencimento = dataVencimento;
			Descontos = descontos.HasValue ? descontos.Value : 0;
			Observacao = observacao;
			Total = SubTotal - Descontos;
			CompetenciaId = competenciaId;

			CalculateTotal();
		}

		internal void SetValues(decimal subTotal, decimal discounts)
		{
			this.SubTotal = subTotal;
			this.Descontos = discounts;
			CalculateTotal();
		}

		private void CalculateTotal()
		{
			this.Total = this.SubTotal - this.Descontos;
		}
	}
}
