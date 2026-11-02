using Financeiro.Business.Aggregates.EffectiveDate;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Interfaces;
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

        public async Task<Result<bool>> GenerateAutomaticExpensesAsync(EffectiveDate effectiveDate, CalendarMonth month, int year)
        {
            try
            {
                var allCards = await _creditCardRepository.GetListAsync();
                var activeCards = allCards.Where(c => c.IsActive).ToList();

                var activeSubscriptions = await _subscriptionRepository.GetActiveSubscriptionsAsync();

                await ProcessCardInvoicesAsync(activeCards, activeSubscriptions, effectiveDate, month, year);
                await ProcessRecurrentImmediateExpenseAsync(activeSubscriptions, effectiveDate.Id, month, year);


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
                    var cardExpense = CreateCardExpense(card, effectiveDateId, (CalendarMonth)effectiveDate.Month, effectiveDate.Year);
                    await _cardExpenseRepository.AddAsync(cardExpense);
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(new Error($"Erro ao sincronizar faturas de novos cartões: {ex.Message}"));
            }
        }

        private async Task ProcessCardInvoicesAsync(List<CreditCard> activeCards, IEnumerable<Subscription> activeSubscriptions, EffectiveDate effectiveDate, CalendarMonth month, int year)
        {
            foreach (var card in activeCards)
            {
                var cardExpense = CreateCardExpense(card, effectiveDate.Id, month, year);
                var cardSubscriptions = activeSubscriptions.Where(s => s.CreditCardId == card.Id).ToList();

                foreach (var sub in cardSubscriptions)
                {
                    if (IsSubscriptionActiveForMonth(sub, month, year))
                    {
                        var expenseItem = CreateSubscriptionExpenseItem(sub, cardExpense.Id, effectiveDate.Id, month, year);
                        cardExpense.AddExpense(expenseItem);
                    }
                }

                (var previousMonth, var previousYear) = effectiveDate.GetPreviousPeriod();

                var previousEffectiveDate = await _effectiveDateRepository.GetByMonthYearAsync(previousMonth, previousYear);

                var previousCreditCard = previousEffectiveDate.GetCardExpenseByCreditCardId(card.Id);

                foreach (var previousExpense in previousCreditCard.Expenses)
                {
                    if (previousExpense.QuantityInstallments != 1 && previousExpense.CurrentInstallment < previousExpense.QuantityInstallments)
                    {
                        cardExpense.AddExpense(new CardExpense.Expense(previousExpense.Description, previousExpense.SubTotal, previousExpense.DueDate, previousExpense.Discount, previousExpense.Observation, previousExpense.CurrentInstallment + 1, previousExpense.QuantityInstallments, effectiveDate.Id, card.Id, previousExpense.CategoryId));
                    }
                }

                await _cardExpenseRepository.AddAsync(cardExpense);
            }
        }
        // TODO: Isso ainda não funcionará como esperado, mas serviirá futuramente para criar pagamentos à vista que são recorrentes (Ex: Conta de Água, Luz)... #Fn-1
        // Quando for implementar, renomear tudo que for Subscription, para RecurrentPayment.
        private async Task ProcessRecurrentImmediateExpenseAsync(IEnumerable<Subscription> activeSubscriptions, Guid effectiveDateId, CalendarMonth month, int year)
        {
            var standaloneSubs = activeSubscriptions.Where(s => !s.CreditCardId.HasValue).ToList();

            foreach (var sub in standaloneSubs)
            {
                if (IsSubscriptionActiveForMonth(sub, month, year))
                {
                    var immediateExpense = CreateStandaloneExpense(sub, effectiveDateId, month, year);
                    await _payableRepository.AddAsync(immediateExpense);
                }
            }
        }

        private bool IsSubscriptionActiveForMonth(Subscription sub, CalendarMonth month, int year)
        {
            var competenceStart = new DateOnly(year, (int)month, 1);
            var competenceEnd = new DateOnly(year, (int)month, DateTime.DaysInMonth(year, (int)month));

            if (sub.StartDate > competenceEnd)
                return false;

            if (sub.EndDate.HasValue && sub.EndDate.Value < competenceStart)
                return false;

            var diffMonths = (year - sub.StartDate.Year) * 12 + ((int)month - sub.StartDate.Month);
            if (diffMonths < 0)
                return false;

            return sub.FrequencyType switch
            {
                FrequencyType.Mensal => diffMonths % 1 == 0,
                FrequencyType.Trimestral => diffMonths % 3 == 0,
                FrequencyType.Semestral => diffMonths % 6 == 0,
                FrequencyType.Anual => diffMonths % 12 == 0,
                _ => false
            };
        }

        private CardExpense CreateCardExpense(CreditCard card, Guid effectiveDateId, CalendarMonth month, int year)
        {
            var invoiceDescription = $"Fatura {card.Name} - {month}/{year}";
            var daysInMonth = DateTime.DaysInMonth(year, (int)month);
            var dueDay = Math.Min(card.DueDay, daysInMonth);
            var dueDate = new DateOnly(year, (int)month, dueDay);

            return new CardExpense(invoiceDescription, 0, dueDate, 0, null, effectiveDateId, card.Id, null);
        }

        private CardExpense.Expense CreateSubscriptionExpenseItem(Subscription sub, Guid cardExpenseId, Guid effectiveDateId, CalendarMonth month, int year)
        {
            var itemDueDay = Math.Min(sub.StartDate.Day, DateTime.DaysInMonth(year, (int)month));
            var itemDueDate = new DateOnly(year, (int)month, itemDueDay);

            return new CardExpense.Expense(
                sub.Description,
                sub.Value,
                itemDueDate,
                0,
                "Gerado automaticamente por assinatura recorrente",
                1,
                1,
                effectiveDateId,
                cardExpenseId,
                null // TODO: Implementar categoria para assinaturas recorrentes, quando desenvolver a tela no #FN-3
            );
        }

        private ImmediateExpense CreateStandaloneExpense(Subscription sub, Guid effectiveDateId, CalendarMonth month, int year)
        {
            var subDay = Math.Min(sub.StartDate.Day, DateTime.DaysInMonth(year, (int)month));
            var itemDueDate = new DateOnly(year, (int)month, subDay);

            return new ImmediateExpense(
                sub.Description,
                sub.Value,
                itemDueDate,
                0,
                "Gerado automaticamente por assinatura recorrente",
                effectiveDateId,
                null // TODO: Implementar categoria para assinaturas recorrentes, quando desenvolver a tela no #FN-3
            );
        }
    }
}
