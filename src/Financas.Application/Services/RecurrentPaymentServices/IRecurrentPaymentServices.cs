using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Financeiro.Application.Models.RecurrentPaymentModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.RecurrentPaymentServices
{
	public interface IRecurrentPaymentServices
	{
		Task<Result<List<RecurrentPaymentView>>> GetListAsync();
		Task<Result<RecurrentPaymentView>> GetByIdAsync(Guid id);
		Task<Result<List<RecurrentPaymentView>>> GetByCreditCardAsync(Guid creditCardId);
		Task<Result<List<RecurrentPaymentView>>> GetImmediateExpenseRecurrentPaymentsAsync();
		Task<Result<bool>> AddAsync(RecurrentPaymentRequest request);
		Task<Result<bool>> UpdateAsync(Guid id, RecurrentPaymentRequest request);
		Task<Result<bool>> DeleteAsync(Guid id);
	}
}
