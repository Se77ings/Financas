using Financeiro.Application.Models.EffectiveDateModels;
using Financeiro.Business.Shared;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.EffectiveDateServices
{
	public interface IEffectiveDateServices

	{
		Task<Result<List<EffectiveDateSummary>>> GetListAsync();
		Task<Result<EffectiveDateView>> GetByIdAsync(Guid id);
		Task<Result<bool>> AddAsync(EffectiveDateRequest request);

		/// <summary>
		/// Garante que exista uma competência para o mês/ano informados. Se não existir, tenta criar.
		/// Retorna Result.Success(true) se criou uma nova competência, Result.Success(false) se já existia.
		/// </summary>
		Task<Result<bool>> EnsureExistsAsync(EffectiveDateRequest request);

		/// <summary>
		/// Retorna o Id da competência para o mês/ano, criando o registro se necessário.
		/// </summary>
		Task<Result<Guid>> GetOrCreateIdAsync(CalendarMonth month, int year);

		/// <summary>
		/// Usa <paramref name="explicitEffectiveDateId"/> quando informado; caso contrário resolve pela data da operação (get-or-create).
		/// </summary>
		Task<Result<Guid>> ResolveForOperationAsync(DateOnly operationDate, Guid? explicitEffectiveDateId = null);

		Task<Result<bool>> UpdateAsync(Guid id, EffectiveDateRequest request);
		Task<Result<bool>> DeleteAsync(Guid id);
		Task<Result<bool>> SyncMissingCardInvoicesAsync(Guid id);
		Task<Result<List<EffectiveDateSummary>>> GetEligibleForTransferAsync();
	}
}
