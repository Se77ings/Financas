using Financas.Application.Services.CompetenciaServices;
using Financas.Application.Services.PayableServices.Interfaces;
using Financas.Business.Shared.ServiceBase;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Application.Models.PayableModels;
using Financeiro.Business.Shared.ResultPattern;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Aggregates.Payable.Validations;
using Financas.Application.Services.SubscriptionServices;
using Financeiro.Application.Models.SubscriptionModels;

namespace Financas.Application.Services.PayableServices
{
	public class ImmediateExpenseServices : ServiceBase, IImmediateExpenseServices
	{
		private readonly IPayableRepository _repository;
		private readonly IEffectiveDateServices _effectiveDateServices;
		private readonly ISubscriptionServices _subscriptionServices;

		public ImmediateExpenseServices(IPayableRepository repository, IEffectiveDateServices effectiveDateServices, ISubscriptionServices subscriptionServices)
		{
			_repository = repository;
			_effectiveDateServices = effectiveDateServices;
			_subscriptionServices = subscriptionServices;
		}

		public async Task<Result<bool>> AddAsync(ImmediateExpenseRequest request)
		{
			var effectiveDateResult = await _effectiveDateServices.ResolveForOperationAsync(request.DueDate, request.EffectiveDateId);
			if (!effectiveDateResult.IsSuccess)
				return Result<bool>.Failure(effectiveDateResult.Error!);

			var effectiveDateId = effectiveDateResult.Value!;

			// TODO: Verificar se já existe essa entidade.
			var despesa = new ImmediateExpense(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, effectiveDateId, request.CategoryId);

			var validation = await ValidateEntityAsync<ImmediateExpense, ImmediateExpenseValidator>(despesa);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.AddAsync(despesa);

			if (request.isSubscription)
			{
				var subscriptionRequest = new SubscriptionRequest
				{
					Description = request.Description,
					Value = request.SubTotal,
					FrequencyType = request.SubscriptionFrequencyType,
					StartDate = request.SubscriptionStartDate,
					EndDate = request.SubscriptionEndDate,
					IsActive = true,
					CategoryId = request.CategoryId
				};

				var result = await _subscriptionServices.AddAsync(subscriptionRequest);
				if (!result.IsSuccess)
					return Result<bool>.Failure(new Error("O gasto foi salvo, mas houve erro ao criar a recorrência."));
			}

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, ImmediateExpenseRequest request)
		{
			var effectiveDateResult = await _effectiveDateServices.ResolveForOperationAsync(request.DueDate, request.EffectiveDateId);
			if (!effectiveDateResult.IsSuccess)
				return Result<bool>.Failure(effectiveDateResult.Error!);

			var effectiveDateId = effectiveDateResult.Value!;

			var expenseResult = await GetImmediateExpenseById(id);

			var novaEntidade = new ImmediateExpense(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, effectiveDateId, request.CategoryId);
			// TODO: Verificar se já existe a nova entidade ou homônima.

			var validation = await ValidateEntityAsync<ImmediateExpense, ImmediateExpenseValidator>(novaEntidade);
			
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			expenseResult.Update(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, effectiveDateId, request.CategoryId);

			await _repository.UpdateAsync(expenseResult);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeleteAsync(Guid id)
		{
			var expenseResult = await GetImmediateExpenseById(id);

			await _repository.RemoveAsync(expenseResult);

			return Result<bool>.Success(true);
		}

		public async Task<(Result<bool> result, ImmediateExpenseView view)> GetByIdAsync(Guid id)
		{
			var expenseResult = await GetImmediateExpenseById(id);

			return (Result<bool>.Success(true), (ImmediateExpenseView)expenseResult);
		}

		public async Task<(Result<bool> result, List<ImmediateExpenseView> data)> GetListAsync()
		{
			var list = await _repository.GetListAsync();

			return (Result<bool>.Success(true), list.Select(x => (ImmediateExpenseView)x).ToList());
		}

		#region Métodos internos

		private async Task<ImmediateExpense> GetImmediateExpenseById(Guid id)
		{
			return await _repository.GetByIdAsync(id);
		}



		#endregion
	}
}
