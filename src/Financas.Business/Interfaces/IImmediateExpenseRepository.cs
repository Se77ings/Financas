using Financeiro.Business.Aggregates.Payable;

namespace Financeiro.Business.Interfaces
{
	public interface IImmediateExpenseRepository : IRepository<ImmediateExpense>
	{
		Task<bool> ExistsAsync(string description, DateOnly date);
		Task<bool> ExistsOtherAsync(Guid id, string description, DateOnly date);
	}
}
