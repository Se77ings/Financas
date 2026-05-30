using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Shared;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.CompetenciaServices
{
	public class EffectiveDateGeneratorService : IEffectiveDateGeneratorService
	{
		private readonly IEffectiveDateRepository _effectiveDateRepository;
		private readonly ICreditCardRepository _creditCardRepository;
		private readonly ISubscriptionRepository _subscriptionRepository;
		private readonly ICardExpenseRepository _cardExpenseRepository;
		private readonly IPayableRepository _payableRepository;

		public EffectiveDateGeneratorService(
			IEffectiveDateRepository effectiveDateRepository,
			ICreditCardRepository creditCardRepository,
			ISubscriptionRepository subscriptionRepository,
			ICardExpenseRepository cardExpenseRepository,
			IPayableRepository payableRepository)
		{
			_effectiveDateRepository = effectiveDateRepository;
			_creditCardRepository = creditCardRepository;
			_subscriptionRepository = subscriptionRepository;
			_cardExpenseRepository = cardExpenseRepository;
			_payableRepository = payableRepository;
		}

		public async Task<Result<bool>> GenerateAutomaticExpensesAsync(Guid effectiveDateId, CalendarMonth month, int year)
		{
			try
			{
				var allCards = await _creditCardRepository.GetListAsync();
				var activeCards = allCards.Where(c => c.IsActive).ToList();

				var cardInvoices = new Dictionary<Guid, CardExpense>();

				foreach (var card in activeCards)
				{
					var invoiceDescription = $"Fatura {card.Name} - {month}/{year}";
					
					var daysInMonth = DateTime.DaysInMonth(year, (int)month);
					var dueDay = Math.Min(card.DueDay, daysInMonth);
					var dueDate = new DateOnly(year, (int)month, dueDay);

					var cardExpense = new CardExpense(invoiceDescription, 0, dueDate, 0, null, effectiveDateId, card.Id);
					
					await _cardExpenseRepository.AddAsync(cardExpense);
					cardInvoices[card.Id] = cardExpense;
				}

				var activeSubscriptions = await _subscriptionRepository.GetActiveSubscriptionsAsync();

				var competenceStart = new DateOnly(year, (int)month, 1);
				var competenceEnd = new DateOnly(year, (int)month, DateTime.DaysInMonth(year, (int)month));

				foreach (var sub in activeSubscriptions)
				{
					// Check active period
					if (sub.StartDate > competenceEnd)
						continue;

					if (sub.EndDate.HasValue && sub.EndDate.Value < competenceStart)
						continue;

					var diffMonths = (year - sub.StartDate.Year) * 12 + ((int)month - sub.StartDate.Month);
					if (diffMonths < 0)
						continue;

					bool isMatch = sub.FrequencyType switch
					{
						FrequencyType.Mensal => diffMonths % 1 == 0,
						FrequencyType.Trimestral => diffMonths % 3 == 0,
						FrequencyType.Semestral => diffMonths % 6 == 0,
						FrequencyType.Anual => diffMonths % 12 == 0,
						_ => false
					};

					if (!isMatch)
						continue;

					// C) Materialize expense item
					if (sub.CreditCardId.HasValue)
					{
						// Card Subscription: add to generated card invoice
						if (cardInvoices.TryGetValue(sub.CreditCardId.Value, out var invoice))
						{
							var itemDueDate = invoice.DueDate; // Matches card invoice due date
							var expenseItem = new CardExpense.Expense(
								sub.Description,
								sub.Value,
								itemDueDate,
								0,
								"Gerado automaticamente por assinatura recorrente",
								1,
								1,
								effectiveDateId,
								invoice.Id
							);

							invoice.AddExpense(expenseItem);
							await _cardExpenseRepository.UpdateAsync(invoice);
						}
					}
					else
					{
						// Standalone Subscription: generate immediate expense
						var subDay = Math.Min(sub.StartDate.Day, DateTime.DaysInMonth(year, (int)month));
						var itemDueDate = new DateOnly(year, (int)month, subDay);

						var immediateExpense = new ImmediateExpense(
							sub.Description,
							sub.Value,
							itemDueDate,
							0,
							"Gerado automaticamente por assinatura recorrente",
							effectiveDateId
						);

						await _payableRepository.AddAsync(immediateExpense);
					}
				}

				return Result<bool>.Success(true);
			}
			catch (Exception ex)
			{
				return Result<bool>.Failure(new Error($"Erro ao gerar faturas e recorrências automáticas: {ex.Message}"));
			}
		}

		public async Task<Result<bool>> SyncMissingCardInvoicesAsync(Guid effectiveDateId)
		{
			try
			{
				var effectiveDate = await _effectiveDateRepository.GetByIdAsync(effectiveDateId);
				if (effectiveDate == null)
					return Result<bool>.Failure(new Error("Competência não encontrada."));

				var allCards = await _creditCardRepository.GetListAsync();
				var activeCards = allCards.Where(c => c.IsActive).ToList();

				var cardExpenses = await _cardExpenseRepository.GetListAsync();
				var existingCardIds = cardExpenses
					.Where(x => x.EffectiveDateId == effectiveDateId)
					.Select(x => x.CreditCardId)
					.ToList();

				var missingCards = activeCards.Where(c => !existingCardIds.Contains(c.Id)).ToList();

				foreach (var card in missingCards)
				{
					var invoiceDescription = $"Fatura {card.Name} - {effectiveDate.Month}/{effectiveDate.Year}";
					var daysInMonth = DateTime.DaysInMonth(effectiveDate.Year, (int)effectiveDate.Month);
					var dueDay = Math.Min(card.DueDay, daysInMonth);
					var dueDate = new DateOnly(effectiveDate.Year, (int)effectiveDate.Month, dueDay);

					var cardExpense = new CardExpense(invoiceDescription, 0, dueDate, 0, null, effectiveDateId, card.Id);
					await _cardExpenseRepository.AddAsync(cardExpense);
				}

				return Result<bool>.Success(true);
			}
			catch (Exception ex)
			{
				return Result<bool>.Failure(new Error($"Erro ao sincronizar faturas de novos cartões: {ex.Message}"));
			}
		}
	}
}
