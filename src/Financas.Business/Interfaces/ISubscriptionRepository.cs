using Financeiro.Business.Aggregates.Payable;

namespace Financeiro.Business.Interfaces
{
	public interface ISubscriptionRepository : IRepository<Subscription>
	{
		Task<List<Subscription>> GetActiveSubscriptionsAsync();
		Task<List<Subscription>> GetByCreditCardAsync(Guid creditCardId);
		Task<List<Subscription>> GetWithoutCreditCardAsync();
		Task DeactivateByCreditCardAsync(Guid creditCardId);
	}
}
