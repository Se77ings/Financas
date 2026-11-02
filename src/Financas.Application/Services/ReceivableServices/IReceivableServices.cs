using Financeiro.Application.Models.ReceivableModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.ReceivableServices
{
	public interface IReceivableServices
	{
		public Task<Result<List<ReceivableSummary>>> GetListAsync();

		public Task<Result<ReceivableView>> GetByIdAsync(Guid id);

		public Task<Result<bool>> AddAsync(ReceivableRequest request);
		public Task<Result<bool>> UpdateAsync(Guid id, ReceivableRequest request);
		public Task<Result<bool>> DeleteAsync(Guid id);
	}
}
