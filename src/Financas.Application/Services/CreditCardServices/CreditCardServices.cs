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
				return Result<bool>.Failure(Error.NotFound("Cartão não encontrado."));

			if (await _repository.ExistsOtherAsync(id, request.Name))
			{
				return Result<bool>.Failure(new Error("Já existe outro cartão cadastrado com este nome."));
			}

			creditCard.Update(request.Name, request.ClosingDay, request.DueDay, request.Limit);
			var validation = await ValidateEntityAsync<CreditCard, CreditCardValidator>(creditCard);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.UpdateAsync(creditCard);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> ActivateAsync(Guid id)
		{
			var creditCard = await _repository.GetByIdAsync(id);
			if (creditCard == null)
				return Result<bool>.Failure(Error.NotFound("Cartão não encontrado."));

			creditCard.Activate();
			await _repository.UpdateAsync(creditCard);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeactivateAsync(Guid id)
		{
			var creditCard = await _repository.GetByIdAsync(id);
			if (creditCard == null)
				return Result<bool>.Failure(Error.NotFound("Cartão não encontrado."));

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
				return Result<bool>.Failure(Error.NotFound("Cartão não encontrado."));

			var invoiceCount = await _repository.CountInvoicesAsync(id);
			var recurrentPaymentCount = await _repository.CountRecurrentPaymentsAsync(id);
			if (invoiceCount > 0 || recurrentPaymentCount > 0)
			{
				var links = new List<string>();
				if (invoiceCount > 0)
					links.Add($"{invoiceCount} fatura(s)");
				if (recurrentPaymentCount > 0)
					links.Add($"{recurrentPaymentCount} recorrência(s)");

				return Result<bool>.Failure(new Error($"Não é possível excluir o cartão: há {string.Join(" e ", links)} vinculadas. Desative o cartão em vez de excluí-lo."));
			}

			await _repository.RemoveAsync(creditCard);
			return Result<bool>.Success(true);
		}

		public async Task<Result<CreditCardView>> GetByIdAsync(Guid id)
		{
			var creditCard = await _repository.GetByIdAsync(id);
			if (creditCard == null)
				return Result<CreditCardView>.Failure(Error.NotFound("Cartão não encontrado."));

			return Result<CreditCardView>.Success(creditCard);
		}

		public async Task<Result<List<CreditCardView>>> GetListAsync()
		{
			var list = await _repository.GetListAsync();
			return Result<List<CreditCardView>>.Success(list.Select(x => (CreditCardView)x).ToList());
		}
	}
}
