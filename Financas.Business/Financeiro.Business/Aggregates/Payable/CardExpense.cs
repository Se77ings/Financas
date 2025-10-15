namespace Financeiro.Business.Aggregates.Payable
{
	public class CardExpense : Payable
	{
		private List<Expense> _expenses = new();
		public ICollection<Expense> Expenses => _expenses;

		#region Construtores
		protected CardExpense() { }
		public CardExpense(string descricao, decimal subtotal, DateOnly dataVencimento, decimal? descontos, string observacao, Guid competencia) : base(descricao, subtotal, dataVencimento, descontos, observacao, competencia)
		{ }
		#endregion

		public void AddExpense(Expense expense)
		{
			_expenses.Add(expense);
			RecalculateValue();
		}
		public void RemoveExpense(Guid expenseId)
		{
			_expenses.Remove(_expenses.Find(x => x.Id == expenseId));
			RecalculateValue();
		}

		public void UpdateExpense(Guid expenseId, string description, decimal subtotal, DateOnly dueDate, decimal? discounts, string observation, int currentInstallment, int quantityInstallments)
		{
			var expense = _expenses.Find(x => x.Id == expenseId);

			expense.UpdateExpense(description, subtotal, dueDate, discounts, observation, currentInstallment, quantityInstallments);
			RecalculateValue();
		}

		private void RecalculateValue()
		{
			var listSubTotal = _expenses.Sum(x => x.SubTotal);
			var listDiscounts = _expenses.Sum(x => x.Discount);
			this.SetValues(listSubTotal, listDiscounts);
		}		

		public class Expense : Payable
		{
			public int CurrentInstallment { get; private set; }
			public int QuantityInstallments { get; private set; }

			public Guid CardExpenseId { get; private set; }
			public CardExpense CardExpense { get; private set; }

			public Expense(string description, decimal subtotal, DateOnly dueDate, decimal? discounts, string observation, int currentInstallment, int quantityInstallments, Guid effectiveDate, Guid cardExpenseId) : base(description, subtotal, dueDate, discounts, observation, effectiveDate)
			{
				CurrentInstallment = currentInstallment;
				QuantityInstallments = quantityInstallments;
				CardExpenseId = cardExpenseId;
			}

			protected Expense() { }

			public void UpdateExpense(string descriptions, decimal subtotal, DateOnly dueDate, decimal? discounts, string observation, int currentInstallment, int quantityInstallments)
			{
				CurrentInstallment = currentInstallment;
				QuantityInstallments = quantityInstallments;

				this.Update(descriptions, subtotal, dueDate, discounts, observation, EffectiveDateId);
			}
		}
	}

}
