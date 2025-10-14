using Financeiro.Business.Aggregates.Direito;

namespace Financeiro.Business.Aggregates.Interfaces
{
	public interface IContasAReceberRepository : IRepository<ContasAReceber>
	{
		Task<bool> ExistsAsync(string descricao, DateTime date);
		Task<bool> ExistsOtherAsync(Guid id,string descricao, DateTime date);
	}
}
