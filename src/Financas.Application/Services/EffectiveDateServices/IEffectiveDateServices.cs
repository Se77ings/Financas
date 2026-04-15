using Financeiro.Application.Models.EffectiveDateModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.CompetenciaServices
{
	public interface IEffectiveDateServices

	{
		Task<(Result<bool> result, List<EffectiveDateSummary> lista)> GetListAsync();
		Task<(Result<bool> result, EffectiveDateView view)> GetByIdAsync(Guid id);
		Task<Result<bool>> AddAsync(EffectiveDateRequest request);

		/// <summary>
		/// Garante que exista uma competência para o mês/ano informados. Se não existir, tenta criar.
		/// Retorna Result.Success(true) se criou uma nova competência, Result.Success(false) se já existia.
		/// </summary>
		Task<Result<bool>> EnsureExistsAsync(EffectiveDateRequest request);
		Task<Result<bool>> UpdateAsync(Guid id, EffectiveDateRequest request);
		Task<Result<bool>> DeleteAsync(Guid id);
		

	}
}
