using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Financeiro.Application.Models.SubscriptionModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.SubscriptionServices
{
	public interface ISubscriptionServices
	{
		Task<(Result<bool> result, List<SubscriptionView> lista)> GetListAsync();
		Task<(Result<bool> result, SubscriptionView view)> GetByIdAsync(Guid id);
		Task<(Result<bool> result, List<SubscriptionView> lista)> GetByCreditCardAsync(Guid creditCardId);
		Task<Result<bool>> AddAsync(SubscriptionRequest request);
		Task<Result<bool>> UpdateAsync(Guid id, SubscriptionRequest request);
		Task<Result<bool>> DeleteAsync(Guid id);
	}
}
