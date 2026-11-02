using Financas.Application.Services.EffectiveDateServices;
using Financas.Application.Services.PayableServices.Interfaces;
using Financas.Business.Shared.ServiceBase;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Application.Models.PayableModels;
using Financeiro.Business.Shared.ResultPattern;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Aggregates.Payable.Validations;
using Financas.Application.Services.RecurrentPaymentServices;
using Financeiro.Application.Models.RecurrentPaymentModels;

namespace Financas.Application.Services.PayableServices
{
	public class ImmediateExpenseServices : ServiceBase, IImmediateExpenseServices
	{
		private const string ExpenseNotFoundMessage = "Gasto não encontrado.";

		private readonly IImmediateExpenseRepository _repository;
		private readonly IEffectiveDateServices _effectiveDateServices;
		private readonly IRecurrentPaymentServices _recurrentPaymentServices;

		public ImmediateExpenseServices(IImmediateExpenseRepository repository, IEffectiveDateServices effectiveDateServices, IRecurrentPaymentServices recurrentPaymentServices)
		{
			_repository = repository;
			_effectiveDateServices = effectiveDateServices;
			_recurrentPaymentServices = recurrentPaymentServices;
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

			if (request.IsRecurrent)
			{
				var recurrentPaymentRequest = new RecurrentPaymentRequest
				{
					Description = request.Description,
					Value = request.SubTotal,
					FrequencyType = request.RecurrenceFrequencyType,
					StartDate = request.RecurrenceStartDate,
					EndDate = request.RecurrenceEndDate,
					IsActive = true,
					CategoryId = request.CategoryId
				};

				var result = await _recurrentPaymentServices.AddAsync(recurrentPaymentRequest);
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

			var expense = await GetImmediateExpenseById(id);
			if (expense == null)
				return Result<bool>.Failure(Error.NotFound(ExpenseNotFoundMessage));

			// TODO: Verificar se já existe a nova entidade ou homônima.
			expense.Update(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, effectiveDateId, request.CategoryId);

			var validation = await ValidateEntityAsync<ImmediateExpense, ImmediateExpenseValidator>(expense);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.UpdateAsync(expense);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeleteAsync(Guid id)
		{
			var expense = await GetImmediateExpenseById(id);
			if (expense == null)
				return Result<bool>.Failure(Error.NotFound(ExpenseNotFoundMessage));

			await _repository.RemoveAsync(expense);

			return Result<bool>.Success(true);
		}

		public async Task<Result<ImmediateExpenseView>> GetByIdAsync(Guid id)
		{
			var expense = await GetImmediateExpenseById(id);
			if (expense == null)
				return Result<ImmediateExpenseView>.Failure(Error.NotFound(ExpenseNotFoundMessage));

			return Result<ImmediateExpenseView>.Success((ImmediateExpenseView)expense);
		}

		public async Task<Result<List<ImmediateExpenseView>>> GetListAsync()
		{
			var list = await _repository.GetListAsync();

			return Result<List<ImmediateExpenseView>>.Success(list.Select(x => (ImmediateExpenseView)x).ToList());
		}

		#region Métodos internos

		private async Task<ImmediateExpense> GetImmediateExpenseById(Guid id)
		{
			return await _repository.GetByIdAsync(id);
		}



		#endregion
	}
}
