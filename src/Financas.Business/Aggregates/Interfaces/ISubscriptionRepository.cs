using Financeiro.Business.Aggregates.Payable;

namespace Financeiro.Business.Aggregates.Interfaces
{
	public interface ISubscriptionRepository : IRepository<Subscription>
	{
		Task<List<Subscription>> GetActiveSubscriptionsAsync();
	}
}
