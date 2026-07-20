using WageHistory.Application.Models.WageModels;
using WageHistory.Business.Shared.ResultPattern;

namespace WageHistory.Application.Services.WageServices
{
	public interface IWageServices
	{
		Task<(Result<bool> result, List<WageSummary> lista)> GetListAsync();
		Task<(Result<bool> result, WageView? view)> GetByIdAsync(Guid id);
		Task<Result<bool>> AddAsync(WageRequest request);
		Task<Result<bool>> UpdateAsync(Guid id, WageRequest request);
		Task<Result<bool>> DeleteAsync(Guid id);
	}
}
