namespace Financeiro.Business.Models.ReceivableModels
{
	public class ReceivableSummary
	{
		public Guid Id { get; set; }
		public string Description { get; set; }
		public decimal Total { get; set; }
		public DateOnly Date { get; set; }

	}
}
