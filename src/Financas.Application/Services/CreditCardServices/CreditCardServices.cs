using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Financas.Business.Shared.ServiceBase;
using Financeiro.Application.Models.CreditCardModels;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Aggregates.Payable.Validations;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.CreditCardServices
{
	public class CreditCardServices : ServiceBase, ICreditCardServices
	{
		private readonly ICreditCardRepository _repository;
		private readonly ICardExpenseRepository _cardExpenseRepository;
		private readonly IRecurrentPaymentRepository _recurrentPaymentRepository;

		public CreditCardServices(ICreditCardRepository repository, ICardExpenseRepository cardExpenseRepository, IRecurrentPaymentRepository recurrentPaymentRepository)
		{
			_repository = repository;
			_cardExpenseRepository = cardExpenseRepository;
			_recurrentPaymentRepository = recurrentPaymentRepository;
		}

		public async Task<Result<bool>> AddAsync(CreditCardRequest request)
		{
			var creditCard = new CreditCard(request.Name, request.ClosingDay, request.DueDay, request.Limit);

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

			var updatedCard = new CreditCard(request.Name, request.ClosingDay, request.DueDay, request.Limit);
			var validation = await ValidateEntityAsync<CreditCard, CreditCardValidator>(updatedCard);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			creditCard.Update(request.Name, request.ClosingDay, request.DueDay, request.Limit);
			await _repository.UpdateAsync(creditCard);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> ActivateAsync(Guid id)
		{
			var creditCard = await _repository.GetByIdAsync(id);
			if (creditCard == null)
				return Result<bool>.Failure(new Error("Cartão não encontrado."));

			creditCard.Activate();
			await _repository.UpdateAsync(creditCard);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeactivateAsync(Guid id)
		{
			var creditCard = await _repository.GetByIdAsync(id);
			if (creditCard == null)
				return Result<bool>.Failure(new Error("Cartão não encontrado."));

			var latestInvoice = await _cardExpenseRepository.GetLatestByCreditCardAsync(id);
			var pending = (latestInvoice?.Expenses ?? Enumerable.Empty<CardExpense.Expense>())
				.Where(x => x.CurrentInstallment < x.QuantityInstallments)
				.Select(x => $"{x.Description} (faltam {x.QuantityInstallments - x.CurrentInstallment})")
				.ToList();

			if (pending.Count > 0)
				return Result<bool>.Failure(new Error($"Não é possível desativar o cartão: há compras parceladas com parcelas a lançar: {string.Join(", ", pending)}."));

			await _recurrentPaymentRepository.DeactivateByCreditCardAsync(id);

			creditCard.Deactivate();
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
