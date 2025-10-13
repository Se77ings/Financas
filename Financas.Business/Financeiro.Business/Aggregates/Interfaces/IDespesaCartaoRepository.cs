using Financeiro.Business.Aggregates.Obrigação;

namespace Financeiro.Business.Aggregates.Interfaces
{
	public interface IDespesaCartaoRepository : IRepository<DespesaCartao>
	{
		Task AddItemAsync(DespesaCartao.Despesa item);
	}
}
