using Financas.Business.Shared.ServiceBase;
using Financeiro.Business.Aggregates;
using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Validações;
using Financeiro.Business.Models.EffectiveDateModels;
using Financeiro.Business.Shared.ResultPattern;
using static Financeiro.Business.Shared.Localization.ErrorMessages;

namespace Financas.Application.Services.CompetenciaServices
{
	public class EffectiveDateServices : ServiceBase, IEffectiveDateServices
	{
		private readonly IEffectiveDateRepository _repository;
		public EffectiveDateServices(IEffectiveDateRepository repository)
		{
			_repository = repository;
		}

		public async Task<Result<bool>> AddAsync(EffectiveDateRequest request)
		{
			var effectiveDate = new EffectiveDate(request.Month, request.Year);

			// TODO: Verificar se já existe essa entidade.

			var validation = await ValidateEntityAsync<EffectiveDate, EffectiveDateValidator>(effectiveDate);

			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.AddAsync(effectiveDate);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, EffectiveDateRequest request)
		{

			var effectiveDate = await GetEffectiveDate(id);
			//TODO: Verificar se já existe a nova entidade ou homônima.

			if (effectiveDate == null)
				return Result<bool>.Failure(new Error(RecordNotFoundFormat(nameof(EffectiveDate))));

			var validation = await ValidateEntityAsync<EffectiveDate, EffectiveDateValidator>(effectiveDate);

			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			effectiveDate.Update(request.Month, request.Year);

			await _repository.UpdateAsync(effectiveDate);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeleteAsync(Guid id)
		{
			var effectiveDate = await GetEffectiveDate(id);

			if (effectiveDate == null)
				return Result<bool>.Failure(new Error(RecordNotFoundFormat(nameof(EffectiveDate))));

			await _repository.RemoveAsync(effectiveDate);

			return Result<bool>.Success(true);
		}

		public async Task<(Result<bool> result, EffectiveDateView view)> GetByIdAsync(Guid id)
		{
			var effectiveDate = await GetEffectiveDate(id);

			return (Result<bool>.Success(true), (EffectiveDateView)effectiveDate);
		}

		public async Task<(Result<bool> result, List<EffectiveDateSummary> lista)> GetListAsync()
		{

			var list = await _repository.GetListAsync();
			// TODO: Mudar para implicit operator
			return (Result<bool>.Success(true), list.Select(x => new EffectiveDateSummary { Id = x.Id, Year = x.Year, Month = x.Month }).ToList());
		}

		#region Métodos internos
		public async Task<EffectiveDate> GetEffectiveDate(Guid id)
		{
			return await _repository.GetByIdAsync(id);

		}

		#endregion
	}
}
