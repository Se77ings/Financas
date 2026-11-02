using Financeiro.Business.Aggregates.Receivable;

namespace Financeiro.Business.Interfaces
{
	public interface IReceivableRepository : IRepository<Receivable>
	{
		Task<bool> ExistsAsync(string description, DateOnly date);
		Task<bool> ExistsOtherAsync(Guid id,string description, DateOnly date);
	}
}
