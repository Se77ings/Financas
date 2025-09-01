namespace Financeiro.Business.Aggregates.Obrigação
{
	public class DespesaAVista : ContasAPagar
	{

		protected DespesaAVista() { }
		public DespesaAVista(string descricao, decimal subtotal, DateTime dataVencimento, decimal? descontos, string observacao, Guid competenciaId) : base(descricao, subtotal, dataVencimento, descontos, observacao, competenciaId)
		{

		}
	}
}
