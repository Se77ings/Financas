using Financas.Business.Shared.ServiceBase;
using Financeiro.Business.Aggregates;
using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Validações;
using Financeiro.Application.Models.EffectiveDateModels;
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

			// verificar duplicidade
			if (await _repository.ExistsAsync(request.Month, request.Year))
			{
				return Result<bool>.Failure(new Error("Já existe uma competência para esse mês/ano."));
			}

			// TODO: Verificar se já existe essa entidade.

			var validation = await ValidateEntityAsync<EffectiveDate, EffectiveDateValidator>(effectiveDate);

			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.AddAsync(effectiveDate);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> EnsureExistsAsync(EffectiveDateRequest request)
		{
			// se já existe, retornar sucesso com Value = false (não criado)
			if (await _repository.ExistsAsync(request.Month, request.Year))
				return Result<bool>.Success(false);

			// tentar criar; use AddAsync para validar
			var addResult = await AddAsync(request);
			if (!addResult.IsSuccess)
				return addResult;

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

			// Impedir remoção se existirem itens filhos
			if ((effectiveDate.Receivable != null && effectiveDate.Receivable.Any()) || (effectiveDate.Payable != null && effectiveDate.Payable.Any()))
			{
				return Result<bool>.Failure(new Error("Não é possível remover a competência enquanto existirem contas vinculadas (recebíveis ou pagáveis)."));
			}

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
			if (list == null)
				return (Result<bool>.Failure(new Error("Nenhuma competência encontrada.")), null);

			// Mapear para summary com informações úteis para exibição
			var summaries = list.Select(x => new EffectiveDateSummary
			{
				Id = x.Id,
				Month = x.Month,
				Year = x.Year,
				ReceivableCount = x.Receivable?.Count() ?? 0,
				PayableCount = x.Payable?.Count() ?? 0,
				Balance = (x.Receivable?.Sum(r => r.Total) ?? 0) - (x.Payable?.Sum(p => p.Total) ?? 0)
			}).ToList();

			return (Result<bool>.Success(true), summaries);
		}

		#region Métodos internos
		public async Task<EffectiveDate> GetEffectiveDate(Guid id)
		{
			return await _repository.GetByIdAsync(id);

		}

		#endregion
	}
}
