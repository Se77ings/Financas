using Financeiro.Business.Aggregates.Payable;

namespace Financeiro.Business.Aggregates.Interfaces
{
	public interface ICardExpenseRepository : IRepository<CardExpense>
	{
		Task AddItemAsync(CardExpense.Expense item);
		Task<bool> ExistsAsync(string description, DateOnly date);
		Task<bool> ExistsOtherAsync(Guid id, string description, DateOnly date);

		Task<bool> ExistsItemAsync(string description, DateOnly date);
		Task<bool> ExistsOtherItemAsync(Guid id, string description, DateOnly date);

	}
}
