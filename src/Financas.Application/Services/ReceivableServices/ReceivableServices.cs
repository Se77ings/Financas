using Financeiro.Application.Models.ReceivableModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.DireitoServices
{
	public interface ReceivableServices
	{
		public Task<(Result<bool> result, List<ReceivableSummary> lista)> GetListAsync();

		public Task<(Result<bool> result, ReceivableView view)> GetByIdAsync(Guid id);

		public Task<Result<bool>> AddAsync(ReceivableRequest request);
		public Task<Result<bool>> UpdateAsync(Guid id, ReceivableRequest request);
		public Task<Result<bool>> DeleteAsync(Guid id);
	}
}
