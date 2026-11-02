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
        private readonly IRecurrentPaymentRepository _recurrentPaymentRepository;
        private readonly ICardExpenseRepository _cardExpenseRepository;
        private readonly IPayableRepository _payableRepository;

        public EffectiveDateGeneratorService(
            IEffectiveDateRepository effectiveDateRepository,
            ICreditCardRepository creditCardRepository,
            IRecurrentPaymentRepository recurrentPaymentRepository,
            ICardExpenseRepository cardExpenseRepository,
            IPayableRepository payableRepository)
        {
            _effectiveDateRepository = effectiveDateRepository;
            _creditCardRepository = creditCardRepository;
            _recurrentPaymentRepository = recurrentPaymentRepository;
            _cardExpenseRepository = cardExpenseRepository;
            _payableRepository = payableRepository;
        }

        public async Task<Result<bool>> GenerateAutomaticExpensesAsync(EffectiveDate effectiveDate, CalendarMonth month, int year)
        {
            try
            {
                var allCards = await _creditCardRepository.GetListAsync();
                var activeCards = allCards.Where(c => c.IsActive).ToList();

                var activeRecurrentPayments = await _recurrentPaymentRepository.GetActiveRecurrentPaymentsAsync();

                await ProcessCardInvoicesAsync(activeCards, activeRecurrentPayments, effectiveDate, month, year);
                await ProcessRecurrentImmediateExpenseAsync(activeRecurrentPayments, effectiveDate.Id, month, year);


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

        private async Task ProcessCardInvoicesAsync(List<CreditCard> activeCards, IEnumerable<RecurrentPayment> activeRecurrentPayments, EffectiveDate effectiveDate, CalendarMonth month, int year)
        {
            // Não existe na primeira competência nem quando há um mês faltando; aí não há parcelas a levar.
            (var previousMonth, var previousYear) = effectiveDate.GetPreviousPeriod();
            var previousEffectiveDate = await _effectiveDateRepository.GetByMonthYearAsync(previousMonth, previousYear);

            foreach (var card in activeCards)
            {
                var cardExpense = CreateCardExpense(card, effectiveDate.Id, month, year);
                var cardRecurrentPayments = activeRecurrentPayments.Where(s => s.CreditCardId == card.Id).ToList();

                foreach (var sub in cardRecurrentPayments)
                {
                    if (sub.IsDueIn(month, year))
                    {
                        var expenseItem = CreateRecurrentPaymentCardItem(sub, cardExpense.Id, effectiveDate.Id, month, year);
                        cardExpense.AddExpense(expenseItem);
                    }
                }

                // Um cartão criado depois da competência anterior não tem fatura nela.
                var previousInvoice = previousEffectiveDate?.GetCardExpenseByCreditCardId(card.Id);
                var pendingInstallments = previousInvoice?.Expenses
                    .Where(x => x.CurrentInstallment < x.QuantityInstallments)
                    ?? Enumerable.Empty<CardExpense.Expense>();

                foreach (var previousExpense in pendingInstallments)
                {
                    cardExpense.AddExpense(new CardExpense.Expense(
                        previousExpense.Description,
                        previousExpense.SubTotal,
                        previousExpense.DueDate.AddMonths(1),
                        previousExpense.Discount,
                        previousExpense.Observation,
                        previousExpense.CurrentInstallment + 1,
                        previousExpense.QuantityInstallments,
                        effectiveDate.Id,
                        cardExpense.Id,
                        previousExpense.CategoryId));
                }

                await _cardExpenseRepository.AddAsync(cardExpense);
            }
        }
        private async Task ProcessRecurrentImmediateExpenseAsync(IEnumerable<RecurrentPayment> activeRecurrentPayments, Guid effectiveDateId, CalendarMonth month, int year)
        {
            var standaloneSubs = activeRecurrentPayments.Where(s => !s.CreditCardId.HasValue).ToList();

            foreach (var sub in standaloneSubs)
            {
                if (sub.IsDueIn(month, year))
                {
                    var immediateExpense = CreateStandaloneExpense(sub, effectiveDateId, month, year);
                    await _payableRepository.AddAsync(immediateExpense);
                }
            }
        }

        private CardExpense CreateCardExpense(CreditCard card, Guid effectiveDateId, CalendarMonth month, int year)
        {
            var invoiceDescription = $"Fatura {card.Name} - {month}/{year}";
            var daysInMonth = DateTime.DaysInMonth(year, (int)month);
            var dueDay = Math.Min(card.DueDay, daysInMonth);
            var dueDate = new DateOnly(year, (int)month, dueDay);

            return new CardExpense(invoiceDescription, 0, dueDate, 0, null, effectiveDateId, card.Id, null);
        }

        private CardExpense.Expense CreateRecurrentPaymentCardItem(RecurrentPayment sub, Guid cardExpenseId, Guid effectiveDateId, CalendarMonth month, int year)
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
                sub.CategoryId
            );
        }

        private ImmediateExpense CreateStandaloneExpense(RecurrentPayment sub, Guid effectiveDateId, CalendarMonth month, int year)
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
                sub.CategoryId
            );
        }
    }
}
