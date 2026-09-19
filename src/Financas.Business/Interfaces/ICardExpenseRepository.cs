using Financeiro.Business.Aggregates.Payable;

namespace Financeiro.Business.Interfaces
{
	public interface ICardExpenseRepository : IRepository<CardExpense>
	{
		Task AddItemAsync(CardExpense.Expense item);
		Task RemoveItemAsync(CardExpense.Expense item);
		Task RecalculateCardExpenseAsync(Guid cardExpenseId);
		Task<bool> ExistsAsync(string description, DateOnly date);
		Task<bool> ExistsOtherAsync(Guid id, string description, DateOnly date);

		Task<bool> ExistsItemAsync(string description, DateOnly date);
		Task<bool> ExistsOtherItemAsync(Guid id, string description, DateOnly date);
		Task<CardExpense?> GetByCardAndEffectiveDateAsync(Guid creditCardId, Guid effectiveDateId);
		Task<List<CardExpense>> GetCardExpensesByItemIdsAsync(List<Guid> itemIds);
		Task UpdateItemAsync(CardExpense.Expense item);
	}
}
