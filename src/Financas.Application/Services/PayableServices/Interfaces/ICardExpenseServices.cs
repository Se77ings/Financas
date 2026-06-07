using Financeiro.Application.Models.PayableModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.PayableServices.Interfaces
{
	public interface ICardExpenseServices
	{
		Task<(Result<bool> result, List<CardExpenseView> lista)> GetListAsync();
		Task<(Result<bool> result, CardExpenseView view)> GetByIdAsync(Guid id);
		Task<Result<bool>> AddAsync(CardExpenseRequest request);
		Task<Result<bool>> UpdateAsync(Guid id, CardExpenseRequest request);
		Task<Result<bool>> DeleteAsync(Guid id);

		Task<Result<bool>> AddItemAsync(Guid id, CardExpenseItemRequest request);
		Task<Result<bool>> UpdateItemAsync(Guid id, Guid itemId, CardExpenseItemRequest request);
		Task<Result<bool>> RemoveItemAsync(Guid id, Guid itemId);
		Task<Result<bool>> TransferItemsAsync(CardExpenseItemTransferRequest request);
	}
}
