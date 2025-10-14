using Financeiro.Business.Aggregates.Obrigação;

namespace Financeiro.Business.Aggregates.Interfaces
{
	public interface IDespesaCartaoRepository : IRepository<DespesaCartao>
	{
		Task AddItemAsync(DespesaCartao.Despesa item);
		Task<bool> ExistsAsync(string descricao, DateTime date);
		Task<bool> ExistsOtherAsync(Guid id, string descricao, DateTime date);

		Task<bool> ExistsItemAsync(string descricao, DateTime date);
		Task<bool> ExistsOtherItemAsync(Guid id, string descricao, DateTime date);

	}
}
