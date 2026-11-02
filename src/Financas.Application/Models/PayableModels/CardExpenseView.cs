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
		public Guid CreditCardId { get; set; }

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
				CreditCardId = card.CreditCardId,
				Items = card.Expenses.OrderByDescending(d => d.DueDate).Select(d => (CardExpenseItemView)d).ToList()
			};
		}
	}
    public class CardExpenseItemView : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        private bool _isChecked;

        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
        public DateOnly DueDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discounts { get; set; } = 0;
        public string? Observation { get; set; }
        public int CurrentInstallment { get; set; }
        public int QuantityInstallments { get; set; }
        public Guid? CategoryId { get; set; }
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value) return;
                _isChecked = value;
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(IsChecked)));
            }
        }

        public string DisplayDescription => (CurrentInstallment != 1 || QuantityInstallments != 1)
            ? $"{Description} - {CurrentInstallment}/{QuantityInstallments}"
            : Description;

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
                CurrentInstallment = expense.CurrentInstallment,
                QuantityInstallments = expense.QuantityInstallments,
                CategoryId = expense.CategoryId,
                IsChecked = false
            };
        }
    }
}
