using Financeiro.Business.Aggregates.Receivable;

namespace Financeiro.Business.Models.ReceivableModels
{
	public class ReceivableView
	{
		public string Description { get; set; }
		public decimal SubTotal { get; set; }
		public decimal Discounts { get; set; }
		public decimal Total { get; set; }
		public DateOnly Date { get; set; }

		public static implicit operator ReceivableView(Receivable receivable)
		{
			return new ReceivableView
			{
				Description = receivable.Description,
				SubTotal = receivable.SubTotal,
				Discounts = receivable.Discounts,
				Total = receivable.Total,
				Date = receivable.Date
			};
		}
	}
}
