using Financeiro.Application.Models.PayableModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.PayableServices.Interfaces
{
	public interface IImmediateExpenseServices
	{
		public Task<Result<ImmediateExpenseView>> GetByIdAsync(Guid id);
		public Task<Result<List<ImmediateExpenseView>>> GetListAsync();
		public Task<Result<bool>> AddAsync(ImmediateExpenseRequest request);
		public Task<Result<bool>> UpdateAsync(Guid id, ImmediateExpenseRequest request);
		public Task<Result<bool>> DeleteAsync(Guid id);
	}
}
