using Financas.Application.Services.PayableServices.Interfaces;
using Financas.Business.Shared.ServiceBase;
using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Aggregates.Validações;
using Financeiro.Application.Models.PayableModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.PayableServices
{
	public class ImmediateExpenseServices : ServiceBase, IImmediateExpenseServices
	{
		private readonly IPayableRepository _repository;

		public ImmediateExpenseServices(IPayableRepository repository)
		{
			_repository = repository;
		}

		public async Task<Result<bool>> AddAsync(ImmediateExpenseRequest request)
		{
			// TODO: Verificar se já existe essa entidade.
			var despesa = new ImmediateExpense(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, request.EffectiveDateId);

			var validation = await ValidateEntityAsync<ImmediateExpense, ImmediateExpenseValidator>(despesa);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.AddAsync(despesa);
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, ImmediateExpenseRequest request)
		{
			var expenseResult = await GetImmediateExpenseById(id);

			var novaEntidade = new ImmediateExpense(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, request.EffectiveDateId);
			// TODO: Verificar se já existe a nova entidade ou homônima.

			var validation = await ValidateEntityAsync<ImmediateExpense, ImmediateExpenseValidator>(novaEntidade);
			
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			expenseResult.Update(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, request.EffectiveDateId);

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
