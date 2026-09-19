using Financas.Business.Shared.ServiceBase;
using Financeiro.Application.Models.SubscriptionModels;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Aggregates.Payable.Validations;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.SubscriptionServices
{
	public class SubscriptionServices : ServiceBase, ISubscriptionServices
	{
		private readonly ISubscriptionRepository _repository;

		public SubscriptionServices(ISubscriptionRepository repository)
		{
			_repository = repository;
		}

		public async Task<Result<bool>> AddAsync(SubscriptionRequest request)
		{
			var subscription = new Subscription(
				request.Description,
				request.Value,
				request.FrequencyType,
				request.CreditCardId,
				request.StartDate,
				request.EndDate,
				request.IsActive
			);

			var validation = await ValidateEntityAsync<Subscription, SubscriptionValidator>(subscription);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.AddAsync(subscription);
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, SubscriptionRequest request)
		{
			var subscription = await _repository.GetByIdAsync(id);
			if (subscription == null)
				return Result<bool>.Failure(new Error("Assinatura não encontrada."));

			var updatedSubscription = new Subscription(
				request.Description,
				request.Value,
				request.FrequencyType,
				request.CreditCardId,
				request.StartDate,
				request.EndDate,
				request.IsActive
			);

			var validation = await ValidateEntityAsync<Subscription, SubscriptionValidator>(updatedSubscription);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			subscription.Update(
				request.Description,
				request.Value,
				request.FrequencyType,
				request.CreditCardId,
				request.StartDate,
				request.EndDate,
				request.IsActive
			);

			await _repository.UpdateAsync(subscription);
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeleteAsync(Guid id)
		{
			var subscription = await _repository.GetByIdAsync(id);
			if (subscription == null)
				return Result<bool>.Failure(new Error("Assinatura não encontrada."));

			await _repository.RemoveAsync(subscription);
			return Result<bool>.Success(true);
		}

		public async Task<(Result<bool> result, SubscriptionView view)> GetByIdAsync(Guid id)
		{
			var subscription = await _repository.GetByIdAsync(id);
			if (subscription == null)
				return (Result<bool>.Failure(new Error("Assinatura não encontrada.")), null);

			return (Result<bool>.Success(true), subscription);
		}

		public async Task<(Result<bool> result, List<SubscriptionView> lista)> GetListAsync()
		{
			var list = await _repository.GetListAsync();
			return (Result<bool>.Success(true), list.Select(x => (SubscriptionView)x).ToList());
		}
	}
}
