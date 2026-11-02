using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Financeiro.Application.Models.RecurrentPaymentModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.RecurrentPaymentServices
{
	public interface IRecurrentPaymentServices
	{
		Task<(Result<bool> result, List<RecurrentPaymentView> lista)> GetListAsync();
		Task<(Result<bool> result, RecurrentPaymentView view)> GetByIdAsync(Guid id);
		Task<(Result<bool> result, List<RecurrentPaymentView> lista)> GetByCreditCardAsync(Guid creditCardId);
		Task<(Result<bool> result, List<RecurrentPaymentView> lista)> GetImmediateExpenseRecurrentPaymentsAsync();
		Task<Result<bool>> AddAsync(RecurrentPaymentRequest request);
		Task<Result<bool>> UpdateAsync(Guid id, RecurrentPaymentRequest request);
		Task<Result<bool>> DeleteAsync(Guid id);
	}
}
