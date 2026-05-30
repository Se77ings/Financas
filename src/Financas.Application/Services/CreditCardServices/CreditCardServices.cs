using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Financas.Business.Shared.ServiceBase;
using Financeiro.Application.Models.CreditCardModels;
using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Aggregates.Validações;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.CreditCardServices
{
	public class CreditCardServices : ServiceBase, ICreditCardServices
	{
		private readonly ICreditCardRepository _repository;

		public CreditCardServices(ICreditCardRepository repository)
		{
			_repository = repository;
		}

		public async Task<Result<bool>> AddAsync(CreditCardRequest request)
		{
			var creditCard = new CreditCard(request.Name, request.ClosingDay, request.DueDay, request.Limit, request.IsActive);

			if (await _repository.ExistsAsync(request.Name))
			{
				return Result<bool>.Failure(new Error("Já existe um cartão cadastrado com este nome."));
			}

			var validation = await ValidateEntityAsync<CreditCard, CreditCardValidator>(creditCard);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.AddAsync(creditCard);
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, CreditCardRequest request)
		{
			var creditCard = await _repository.GetByIdAsync(id);
			if (creditCard == null)
				return Result<bool>.Failure(new Error("Cartão não encontrado."));

			if (await _repository.ExistsOtherAsync(id, request.Name))
			{
				return Result<bool>.Failure(new Error("Já existe outro cartão cadastrado com este nome."));
			}

			var updatedCard = new CreditCard(request.Name, request.ClosingDay, request.DueDay, request.Limit, request.IsActive);
			var validation = await ValidateEntityAsync<CreditCard, CreditCardValidator>(updatedCard);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			creditCard.Update(request.Name, request.ClosingDay, request.DueDay, request.Limit, request.IsActive);
			await _repository.UpdateAsync(creditCard);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeleteAsync(Guid id)
		{
			var creditCard = await _repository.GetByIdAsync(id);
			if (creditCard == null)
				return Result<bool>.Failure(new Error("Cartão não encontrado."));

			await _repository.RemoveAsync(creditCard);
			return Result<bool>.Success(true);
		}

		public async Task<(Result<bool> result, CreditCardView view)> GetByIdAsync(Guid id)
		{
			var creditCard = await _repository.GetByIdAsync(id);
			if (creditCard == null)
				return (Result<bool>.Failure(new Error("Cartão não encontrado.")), null);

			return (Result<bool>.Success(true), creditCard);
		}

		public async Task<(Result<bool> result, List<CreditCardView> lista)> GetListAsync()
		{
			var list = await _repository.GetListAsync();
			return (Result<bool>.Success(true), list.Select(x => (CreditCardView)x).ToList());
		}
	}
}
