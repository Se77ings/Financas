using Financeiro.Business.Aggregates.Obrigação;

namespace Financeiro.Business.Aggregates.Interfaces
{
	public interface IDespesaAVistaRepository : IRepository<DespesaAVista>
	{
		Task<bool> ExistsAsync(string descricao, DateTime date);
		Task<bool> ExistsOtherAsync(Guid id,string descricao, DateTime date);
	}
}
