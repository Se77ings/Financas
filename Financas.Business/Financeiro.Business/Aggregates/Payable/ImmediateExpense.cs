namespace Financeiro.Business.Aggregates.Payable
{
	public class ImmediateExpense : Payable
	{

		protected ImmediateExpense() { }
		public ImmediateExpense(string description, decimal subtotal, DateOnly dueDate, decimal? discounts, string observation, Guid effectiveDateId) : base(description, subtotal, dueDate, discounts, observation, effectiveDateId)
		{

		}
	}
}
