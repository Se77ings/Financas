using Financeiro.Business.Aggregates.Payable;

namespace Financeiro.Business.Aggregates.Interfaces
{
	public interface IPayableRepository : IRepository<ImmediateExpense>
	{
		Task<bool> ExistsAsync(string description, DateOnly date);
		Task<bool> ExistsOtherAsync(Guid id, string description, DateOnly date);
	}
}
