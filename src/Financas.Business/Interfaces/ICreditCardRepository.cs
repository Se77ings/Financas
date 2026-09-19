using Financeiro.Business.Aggregates.Payable;

namespace Financeiro.Business.Interfaces
{
	public interface ICreditCardRepository : IRepository<CreditCard>
	{
		Task<bool> ExistsAsync(string name);
		Task<bool> ExistsOtherAsync(Guid id, string name);
	}
}
