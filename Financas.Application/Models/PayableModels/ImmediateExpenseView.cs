using Financeiro.Business.Aggregates.Payable;

namespace Financeiro.Business.Models.PayableModels
{
	public class ImmediateExpenseView
	{
		public Guid Id { get; set; }
		public string Description { get; set; }
		public decimal Total { get; set; }
		public DateOnly DueDate { get; set; }
		public decimal SubTotal { get; set; }
		public decimal Discounts { get; set; }
		public string? Observation { get; set; }

		public static implicit operator ImmediateExpenseView(ImmediateExpense expense)
		{
			return new ImmediateExpenseView
			{
				Id = expense.Id,
				Description = expense.Description,
				Total = expense.Total,
				DueDate = expense.DueDate,
				SubTotal = expense.SubTotal,
				Discounts = expense.Discount,
				Observation = expense.Observation,
			};
		}
	}
}
