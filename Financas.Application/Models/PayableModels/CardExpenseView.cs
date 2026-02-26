using Financeiro.Business.Aggregates.Payable;

namespace Financeiro.Application.Models.PayableModels
{
	public class CardExpenseView
	{
		public Guid Id { get; set; }
		public string Description { get; set; }
		public decimal Total { get; set; }
		public DateOnly DueDate { get; set; }
		public decimal SubTotal { get; set; }
		public decimal Discounts { get; set; } = 0;
		public string? Observation { get; set; }

		public List<CardExpenseItemView> Items { get; set; } = new List<CardExpenseItemView>();


		public static implicit operator CardExpenseView(CardExpense card)
		{
			return new CardExpenseView
			{
				Id = card.Id,
				Description = card.Description,
				Total = card.Total,
				DueDate = card.DueDate,
				SubTotal = card.SubTotal,
				Discounts = card.Discount,
				Observation = card.Observation,
				Items = card.Expenses.Select(d => (CardExpenseItemView)d).ToList()
			};
		}
	}
	public class CardExpenseItemView
	{
		public Guid Id { get; set; }
		public string Description { get; set; }
		public decimal Total { get; set; }
		public DateOnly DueDate { get; set; }
		public decimal SubTotal { get; set; }
		public decimal Discounts { get; set; } = 0;
		public string? Observation { get; set; }

		public static implicit operator CardExpenseItemView(CardExpense.Expense expense)
		{
			return new CardExpenseItemView
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
