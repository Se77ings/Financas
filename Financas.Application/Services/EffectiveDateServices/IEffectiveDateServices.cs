using Financeiro.Business.Models.EffectiveDateModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.CompetenciaServices
{
	public interface IEffectiveDateServices

	{
		Task<(Result<bool> result, List<EffectiveDateSummary> lista)> GetListAsync();
		Task<(Result<bool> result, EffectiveDateView view)> GetByIdAsync(Guid id);
		Task<Result<bool>> AddAsync(EffectiveDateRequest request);
		Task<Result<bool>> UpdateAsync(Guid id, EffectiveDateRequest request);
		Task<Result<bool>> DeleteAsync(Guid id);
		

	}
}
