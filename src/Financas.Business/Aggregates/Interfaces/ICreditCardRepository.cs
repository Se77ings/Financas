using Financeiro.Business.Aggregates.Payable;

namespace Financeiro.Business.Aggregates.Interfaces
{
	public interface ICreditCardRepository : IRepository<CreditCard>
	{
		Task<bool> ExistsAsync(string name);
		Task<bool> ExistsOtherAsync(Guid id, string name);
	}
}
