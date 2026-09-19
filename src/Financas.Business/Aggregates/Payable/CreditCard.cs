using Financeiro.Business.Shared;

namespace Financeiro.Business.Aggregates.Payable
{
	public class CreditCard : Entity
	{
		public string Name { get; private set; }
		public int ClosingDay { get; private set; }
		public int DueDay { get; private set; }
		public decimal Limit { get; private set; }
		public bool IsActive { get; private set; }

		private readonly List<CardExpense> _cardExpenses = new();
		public IEnumerable<CardExpense> CardExpenses => _cardExpenses;

		protected CreditCard() { }

		public CreditCard(string name, int closingDay, int dueDay, decimal limit, bool isActive = true)
		{
			Name = name;
			ClosingDay = closingDay;
			DueDay = dueDay;
			Limit = limit;
			IsActive = isActive;
		}

		public void Update(string name, int closingDay, int dueDay, decimal limit, bool isActive)
		{
			Name = name;
			ClosingDay = closingDay;
			DueDay = dueDay;
			Limit = limit;
			IsActive = isActive;
		}
	}
}
