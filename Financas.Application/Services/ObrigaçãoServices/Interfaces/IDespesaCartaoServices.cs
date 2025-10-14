using Financeiro.Business.Models.ObrigaçãoModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.ObrigaçãoServices.Interfaces
{
	public interface IDespesaCartaoServices
	{
		Task<(Result<bool> result, List<DespesaCartaoView> lista)> GetListAsync();
		Task<(Result<bool> result, DespesaCartaoView view)> GetByIdAsync(Guid id);
		Task<Result<bool>> AddAsync(DespesaCartaoRequest request);
		Task<Result<bool>> UpdateAsync(Guid id, DespesaCartaoRequest request);
		Task<Result<bool>> DeleteAsync(Guid id);

		Task<Result<bool>> AddItemAsync(Guid id, DespesaCartaoItemRequest request);
		Task<Result<bool>> UpdateItemAsync(Guid id, Guid itemId, DespesaCartaoItemRequest request);
		Task<Result<bool>> RemoveItemAsync(Guid id, Guid itemId);
	}
}
