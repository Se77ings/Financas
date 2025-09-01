namespace Financeiro.Business.Aggregates.Direito
{
	public class ContasAReceber : Entidade
	{
		public string Descricao { get; private set; }
		public decimal SubTotal { get; private set; }
		public decimal Descontos { get; private set; } = 0;
		public decimal Total { get; private set; }
		public DateTime Data { get; private set; }

		/// EF Relation
		public Guid CompetenciaId { get; private set; }
		public Competencia Competencia { get; private set; }

		protected ContasAReceber() { }


		public ContasAReceber(string descricao, decimal subtotal, DateTime data, decimal? descontos, Guid competenciaId)
		{
			Descricao = descricao;
			SubTotal = subtotal;
			Descontos = descontos.HasValue ? descontos.Value : 0;
			Data = data;
			Total = SubTotal - Descontos;
			CompetenciaId = competenciaId;
		}

		public void Atualizar(string descricao, decimal subtotal, DateTime data, decimal? descontos, Guid competenciaId)
		{
			Descricao = descricao;
			SubTotal = subtotal;
			Descontos = descontos.HasValue ? descontos.Value : 0;
			Data = data;
			Total = SubTotal - Descontos;
			CompetenciaId = competenciaId;
		}
	}

}
