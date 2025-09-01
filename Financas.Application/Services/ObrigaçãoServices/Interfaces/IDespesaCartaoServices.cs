using Financeiro.Business.Models.ObrigaçãoModels;
using Financeiro.Business.ResultPattern;

namespace Financas.Application.Services.ObrigaçãoServices.Interfaces
{
	public interface IDespesaCartaoServices
	{
		public Task<(Result<bool> result, List<DespesaCartaoView> lista)> GetListAsync();
		public Task<(Result<bool> result, DespesaCartaoView view)> GetByIdAsync(Guid id);
		public Task<Result<bool>> AddAsync(DespesaCartaoRequest request);
		public Task<Result<bool>> UpdateAsync(Guid id, DespesaCartaoRequest request);
		public Task<Result<bool>> DeleteAsync(Guid id);

		//Obs: Esses métodos abaixo, não possuem repositório. na realidade fazem um Update de DespesaCartao!!!!!
		public Task<bool> AddItemAsync(Guid id, DespesaCartaoItemRequest request);
		public Task<bool> UpdateItemAsync(Guid id, Guid itemId, DespesaCartaoItemRequest request);
		public Task<bool> RemoveItemAsync(Guid id, Guid itemId);

	}
}
