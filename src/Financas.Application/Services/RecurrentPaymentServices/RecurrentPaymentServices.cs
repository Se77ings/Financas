using Financas.Business.Shared.ServiceBase;
using Financeiro.Application.Models.RecurrentPaymentModels;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Aggregates.Payable.Validations;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.RecurrentPaymentServices
{
	public class RecurrentPaymentServices : ServiceBase, IRecurrentPaymentServices
	{
		private readonly IRecurrentPaymentRepository _repository;

		public RecurrentPaymentServices(IRecurrentPaymentRepository repository)
		{
			_repository = repository;
		}

		public async Task<Result<bool>> AddAsync(RecurrentPaymentRequest request)
		{
			var recurrentPayment = new RecurrentPayment(
				request.Description,
				request.Value,
				request.FrequencyType,
				request.CreditCardId,
				request.StartDate,
				request.EndDate,
				request.CategoryId,
				request.IsActive
			);

			var validation = await ValidateEntityAsync<RecurrentPayment, RecurrentPaymentValidator>(recurrentPayment);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.AddAsync(recurrentPayment);
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, RecurrentPaymentRequest request)
		{
			var recurrentPayment = await _repository.GetByIdAsync(id);
			if (recurrentPayment == null)
				return Result<bool>.Failure(new Error("Assinatura não encontrada."));

			var updatedRecurrentPayment = new RecurrentPayment(
				request.Description,
				request.Value,
				request.FrequencyType,
				request.CreditCardId,
				request.StartDate,
				request.EndDate,
				request.CategoryId,
				request.IsActive
			);

			var validation = await ValidateEntityAsync<RecurrentPayment, RecurrentPaymentValidator>(updatedRecurrentPayment);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			recurrentPayment.Update(
				request.Description,
				request.Value,
				request.FrequencyType,
				request.CreditCardId,
				request.StartDate,
				request.EndDate,
				request.CategoryId,
				request.IsActive
			);

			await _repository.UpdateAsync(recurrentPayment);
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeleteAsync(Guid id)
		{
			var recurrentPayment = await _repository.GetByIdAsync(id);
			if (recurrentPayment == null)
				return Result<bool>.Failure(new Error("Assinatura não encontrada."));

			await _repository.RemoveAsync(recurrentPayment);
			return Result<bool>.Success(true);
		}

		public async Task<(Result<bool> result, RecurrentPaymentView view)> GetByIdAsync(Guid id)
		{
			var recurrentPayment = await _repository.GetByIdAsync(id);
			if (recurrentPayment == null)
				return (Result<bool>.Failure(new Error("Assinatura não encontrada.")), null);

			return (Result<bool>.Success(true), recurrentPayment);
		}

		public async Task<(Result<bool> result, List<RecurrentPaymentView> lista)> GetByCreditCardAsync(Guid creditCardId)
		{
			var list = await _repository.GetByCreditCardAsync(creditCardId);
			return (Result<bool>.Success(true), list.Select(x => (RecurrentPaymentView)x).ToList());
		}

		public async Task<(Result<bool> result, List<RecurrentPaymentView> lista)> GetImmediateExpenseRecurrentPaymentsAsync()
		{
			var list = await _repository.GetWithoutCreditCardAsync();
			return (Result<bool>.Success(true), list.Select(x => (RecurrentPaymentView)x).ToList());
		}

		public async Task<(Result<bool> result, List<RecurrentPaymentView> lista)> GetListAsync()
		{
			var list = await _repository.GetListAsync();
			return (Result<bool>.Success(true), list.Select(x => (RecurrentPaymentView)x).ToList());
		}
	}
}
