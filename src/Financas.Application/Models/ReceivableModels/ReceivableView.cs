using Financeiro.Business.Aggregates.Receivable;

namespace Financeiro.Application.Models.ReceivableModels
{
	public class ReceivableView
	{
		public Guid Id { get; set; }
		public string Description { get; set; }
		public decimal SubTotal { get; set; }
		public decimal Discounts { get; set; }
		public decimal Total { get; set; }
		public DateOnly Date { get; set; }
		public Guid EffectiveDateId { get; set;  }
		public Guid? CategoryId { get; set; }

		public static implicit operator ReceivableView(Receivable receivable)
		{
			return new ReceivableView
			{
				Id = receivable.Id,
				Description = receivable.Description,
				SubTotal = receivable.SubTotal,
				Discounts = receivable.Discounts,
				Total = receivable.Total,
				Date = receivable.Date,
				EffectiveDateId = receivable.EffectiveDateId,
				CategoryId = receivable.CategoryId
			};
		}
	}
}
