using Financeiro.Business.Aggregates.EffectiveDate;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Shared;
using Financeiro.Business.Shared.ResultPattern;
using Microsoft.Extensions.Logging;

namespace Financas.Application.Services.EffectiveDateServices
{
    public class EffectiveDateGeneratorService : IEffectiveDateGeneratorService
    {
        private const string GenerationErrorMessage = "Erro ao gerar as faturas e recorrências da competência.";

        private readonly IEffectiveDateRepository _effectiveDateRepository;
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IRecurrentPaymentRepository _recurrentPaymentRepository;
        private readonly ICardExpenseRepository _cardExpenseRepository;
        private readonly IImmediateExpenseRepository _payableRepository;
        private readonly ILogger<EffectiveDateGeneratorService> _logger;

        public EffectiveDateGeneratorService(
            IEffectiveDateRepository effectiveDateRepository,
            ICreditCardRepository creditCardRepository,
            IRecurrentPaymentRepository recurrentPaymentRepository,
            ICardExpenseRepository cardExpenseRepository,
            IImmediateExpenseRepository payableRepository,
            ILogger<EffectiveDateGeneratorService> logger)
        {
            _effectiveDateRepository = effectiveDateRepository;
            _creditCardRepository = creditCardRepository;
            _recurrentPaymentRepository = recurrentPaymentRepository;
            _cardExpenseRepository = cardExpenseRepository;
            _payableRepository = payableRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> GenerateAutomaticExpensesAsync(EffectiveDate effectiveDate, CalendarMonth month, int year)
        {
            try
            {
                var allCards = await _creditCardRepository.GetListAsync();
                var activeCards = allCards.Where(c => c.IsActive).ToList();

                var activeRecurrentPayments = await _recurrentPaymentRepository.GetActiveRecurrentPaymentsAsync();

                await ProcessCardInvoicesAsync(activeCards, activeRecurrentPayments, effectiveDate);
                await ProcessRecurrentImmediateExpenseAsync(activeRecurrentPayments, effectiveDate.Id, month, year);


                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar as faturas e recorrências da competência {Month}/{Year}.", month, year);
                return Result<bool>.Failure(new Error(GenerationErrorMessage));
            }
        }

        public async Task<Result<bool>> SyncMissingCardInvoicesAsync(Guid effectiveDateId)
        {
            try
            {
                var effectiveDate = await _effectiveDateRepository.GetByIdAsync(effectiveDateId);
                if (effectiveDate == null)
                    return Result<bool>.Failure(Error.NotFound("Competência não encontrada."));

                var allCards = await _creditCardRepository.GetListAsync();
                var activeCards = allCards.Where(c => c.IsActive).ToList();

                var cardExpenses = await _cardExpenseRepository.GetListAsync();
                var existingCardIds = cardExpenses
                    .Where(x => x.EffectiveDateId == effectiveDateId)
                    .Select(x => x.CreditCardId)
                    .ToList();

                var missingCards = activeCards.Where(c => !existingCardIds.Contains(c.Id)).ToList();
                if (missingCards.Count == 0)
                    return Result<bool>.Success(true);

                var activeRecurrentPayments = await _recurrentPaymentRepository.GetActiveRecurrentPaymentsAsync();
                var dueRecurrentPayments = DueIn(activeRecurrentPayments, effectiveDate);
                var previousEffectiveDate = await GetPreviousEffectiveDateAsync(effectiveDate);

                foreach (var card in missingCards)
                {
                    var cardExpense = BuildCardInvoice(card, effectiveDate, dueRecurrentPayments, previousEffectiveDate);
                    await _cardExpenseRepository.AddAsync(cardExpense);
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao sincronizar as faturas dos cartões da competência {EffectiveDateId}.", effectiveDateId);
                return Result<bool>.Failure(new Error(GenerationErrorMessage));
            }
        }

        private async Task ProcessCardInvoicesAsync(List<CreditCard> activeCards, IEnumerable<RecurrentPayment> activeRecurrentPayments, EffectiveDate effectiveDate)
        {
            var dueRecurrentPayments = DueIn(activeRecurrentPayments, effectiveDate);
            var previousEffectiveDate = await GetPreviousEffectiveDateAsync(effectiveDate);

            foreach (var card in activeCards)
            {
                var cardExpense = BuildCardInvoice(card, effectiveDate, dueRecurrentPayments, previousEffectiveDate);
                await _cardExpenseRepository.AddAsync(cardExpense);
            }
        }

        // Não existe na primeira competência nem quando há um mês faltando; aí não há parcelas a levar.
        private async Task<EffectiveDate?> GetPreviousEffectiveDateAsync(EffectiveDate effectiveDate)
        {
            (var previousMonth, var previousYear) = effectiveDate.GetPreviousPeriod();
            return await _effectiveDateRepository.GetByMonthYearAsync(previousMonth, previousYear);
        }

        private static List<RecurrentPayment> DueIn(IEnumerable<RecurrentPayment> recurrentPayments, EffectiveDate effectiveDate)
        {
            return recurrentPayments.Where(s => s.IsDueIn(effectiveDate.Month, effectiveDate.Year)).ToList();
        }

        // A fatura nasce com as assinaturas do cartão que vencem na competência e com a parcela seguinte
        // de cada compra parcelada que ainda não terminou na fatura anterior.
        private CardExpense BuildCardInvoice(CreditCard card, EffectiveDate effectiveDate, IEnumerable<RecurrentPayment> dueRecurrentPayments, EffectiveDate? previousEffectiveDate)
        {
            var month = effectiveDate.Month;
            var year = effectiveDate.Year;
            var cardExpense = CreateCardExpense(card, effectiveDate.Id, month, year);

            foreach (var sub in dueRecurrentPayments.Where(s => s.CreditCardId == card.Id))
            {
                var expenseItem = CreateRecurrentPaymentCardItem(sub, cardExpense.Id, effectiveDate.Id, month, year);
                cardExpense.AddExpense(expenseItem);
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

            return cardExpense;
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
