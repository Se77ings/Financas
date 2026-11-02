using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Financeiro.Application.Models.CreditCardModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.CreditCardServices
{
	public interface ICreditCardServices
	{
		Task<Result<List<CreditCardView>>> GetListAsync();
		Task<Result<CreditCardView>> GetByIdAsync(Guid id);
		Task<Result<bool>> AddAsync(CreditCardRequest request);
		Task<Result<bool>> UpdateAsync(Guid id, CreditCardRequest request);
		Task<Result<bool>> ActivateAsync(Guid id);
		Task<Result<bool>> DeactivateAsync(Guid id);
		Task<Result<bool>> DeleteAsync(Guid id);
	}
}
