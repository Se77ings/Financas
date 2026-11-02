using Financeiro.Business.Aggregates.Payable;

namespace Financeiro.Business.Interfaces
{
	public interface IRecurrentPaymentRepository : IRepository<RecurrentPayment>
	{
		Task<List<RecurrentPayment>> GetActiveRecurrentPaymentsAsync();
		Task<List<RecurrentPayment>> GetByCreditCardAsync(Guid creditCardId);
		Task<List<RecurrentPayment>> GetWithoutCreditCardAsync();
		Task DeactivateByCreditCardAsync(Guid creditCardId);
	}
}
