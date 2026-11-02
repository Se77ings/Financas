using Financas.Application.Services.EffectiveDateServices;
using Financas.Business.Shared.ServiceBase;
using Financeiro.Application.Models.ReceivableModels;
using Financeiro.Business.Aggregates.Receivable;
using Financeiro.Business.Aggregates.Receivable.Validations;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Shared.Localization;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.ReceivableServices
{
	public class ReceivableServices : ServiceBase, IReceivableServices
	{
		private const string ReceivableNotFoundMessage = "Recebimento não encontrado.";

		private readonly IReceivableRepository _repository;
		private readonly IEffectiveDateServices _effectiveDateServices;

		public ReceivableServices(IReceivableRepository repository, IEffectiveDateServices effectiveDateServices)
		{
			_repository = repository;
			_effectiveDateServices = effectiveDateServices;
		}

		public async Task<Result<bool>> AddAsync(ReceivableRequest request)
		{
			var effectiveDateResult = await _effectiveDateServices.ResolveForOperationAsync(request.Date, request.EffectiveDateId);
			if (!effectiveDateResult.IsSuccess)
				return Result<bool>.Failure(effectiveDateResult.Error!);

			var effectiveDateId = effectiveDateResult.Value!;

			//TODO: Verificar se já existe essa entidade.
			var contaAReceber = new Receivable(request.Description, request.SubTotal, request.Date, request.Discounts, effectiveDateId, request.CategoryId);

			var validation = await ValidateEntityAsync<Receivable, ReceivableValidator>(contaAReceber);

			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.AddAsync(contaAReceber);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, ReceivableRequest request)
		{
			var effectiveDateResult = await _effectiveDateServices.ResolveForOperationAsync(request.Date, request.EffectiveDateId);
			if (!effectiveDateResult.IsSuccess)
				return Result<bool>.Failure(effectiveDateResult.Error!);

			var effectiveDateId = effectiveDateResult.Value!;

			var contaAReceber = await GetReceivableById(id);
			if (contaAReceber == null)
				return Result<bool>.Failure(Error.NotFound(ReceivableNotFoundMessage));

			//TODO: Verificar se já existe a nova entidade ou homônima.
			contaAReceber.Update(request.Description, request.SubTotal, request.Date, request.Discounts, effectiveDateId, request.CategoryId);

			var validation = await ValidateEntityAsync<Receivable, ReceivableValidator>(contaAReceber);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.UpdateAsync(contaAReceber);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeleteAsync(Guid id)
		{
			var contaAReceber = await GetReceivableById(id);
			if (contaAReceber == null)
				return Result<bool>.Failure(Error.NotFound(ReceivableNotFoundMessage));

			await _repository.RemoveAsync(contaAReceber);

			return Result<bool>.Success(true);
		}

		public async Task<Result<ReceivableView>> GetByIdAsync(Guid id)
		{
			var contaAReceber = await GetReceivableById(id);
			if (contaAReceber == null)
				return Result<ReceivableView>.Failure(Error.NotFound(ReceivableNotFoundMessage));

			return Result<ReceivableView>.Success((ReceivableView)contaAReceber);
		}

		public async Task<Result<List<ReceivableSummary>>> GetListAsync()
		{
			var list = await _repository.GetListAsync();

			if (list == null)
				return Result<List<ReceivableSummary>>.Failure(new Error(Errors.EmptyData));

			var result = list.Select(c => new ReceivableSummary { Id = c.Id, Date = c.Date, Description = c.Description, Total = c.Total }).ToList();

			return Result<List<ReceivableSummary>>.Success(result);
		}

		#region Métodos internos

		public async Task<Receivable> GetReceivableById(Guid id)
		{
			return await _repository.GetByIdAsync(id);

		}

		#endregion
	}
}
