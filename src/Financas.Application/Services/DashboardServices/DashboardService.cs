using Financeiro.Application.Models.DashboardModels;
using Financeiro.Business.Aggregates.EffectiveDate;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Shared;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.DashboardServices
{
    public class DashboardService : IDashboardService
    {
        private const int DailySpendingDays = 30;
        private const int FutureCommitmentMonths = 6;
        private const int MonthlyHistoryMonths = 12;
        private const int AverageIncomeMonths = 6;

        private readonly IEffectiveDateRepository _effectiveDateRepository;
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IRecurrentPaymentRepository _recurrentPaymentRepository;
        private readonly ICategoryRepository _categoryRepository;

        public DashboardService(
            IEffectiveDateRepository effectiveDateRepository,
            ICreditCardRepository creditCardRepository,
            IRecurrentPaymentRepository recurrentPaymentRepository,
            ICategoryRepository categoryRepository)
        {
            _effectiveDateRepository = effectiveDateRepository;
            _creditCardRepository = creditCardRepository;
            _recurrentPaymentRepository = recurrentPaymentRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<(Result<bool> result, DashboardView view)> GetAsync(Guid effectiveDateId, DateOnly today)
        {
            var effectiveDates = (await _effectiveDateRepository.GetListAsync())
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            var selected = effectiveDates.FirstOrDefault(x => x.Id == effectiveDateId);
            if (selected is null)
                return (Result<bool>.Failure(new Error("Competência não encontrada.")), null);

            var creditCards = await _creditCardRepository.GetListAsync();
            var recurrentPayments = await _recurrentPaymentRepository.GetActiveRecurrentPaymentsAsync();
            var categories = await _categoryRepository.GetListAsync();

            var context = new DashboardContext(effectiveDates, recurrentPayments);

            var view = new DashboardView
            {
                Totals = BuildTotals(context),
                DailySpending = BuildDailySpending(context, today),
                SpendingPace = BuildSpendingPace(context, selected, today),
                SpendingByCategory = BuildSpendingByCategory(selected, categories),
                CreditCards = BuildCreditCards(selected, creditCards),
                FutureCommitments = BuildFutureCommitments(context, selected, creditCards),
                MonthlyHistory = BuildMonthlyHistory(context, selected)
            };

            return (Result<bool>.Success(true), view);
        }

        private static DashboardTotalsView BuildTotals(DashboardContext context)
        {
            var income = context.EffectiveDates.Sum(Income);
            var expenses = context.EffectiveDates.Sum(Expenses);

            return new DashboardTotalsView { Income = income, Expenses = expenses, Balance = income - expenses };
        }

        private static DailySpendingView BuildDailySpending(DashboardContext context, DateOnly today)
        {
            var firstDay = today.AddDays(-(DailySpendingDays - 1));
            var spendingByDate = context.Spendings
                .Where(x => x.DueDate >= firstDay && x.DueDate <= today)
                .GroupBy(x => x.DueDate)
                .ToDictionary(g => g.Key, g => g.ToList());

            var days = Enumerable.Range(0, DailySpendingDays)
                .Select(offset => firstDay.AddDays(offset))
                .Select(date => new DailySpendingDayView
                {
                    Date = date,
                    Total = spendingByDate.TryGetValue(date, out var spendings) ? spendings.Sum(x => x.Total) : 0,
                    ByCategory = spendings is null
                        ? new List<CategoryAmountView>()
                        : spendings.GroupBy(x => x.CategoryId).Select(g => new CategoryAmountView { CategoryId = g.Key, Total = g.Sum(x => x.Total) }).ToList(),
                    IdealDaily = context.IdealDailyFor((CalendarMonth)date.Month, date.Year)
                })
                .ToList();

            return new DailySpendingView { Days = days };
        }

        // O ritmo usa a data de cada lançamento (cada parcela no dia em que cai), não a competência a que
        // ele pertence; só o gasto ideal vem da competência.
        private static SpendingPaceView BuildSpendingPace(DashboardContext context, EffectiveDate selected, DateOnly today)
        {
            var year = selected.Year;
            var month = (int)selected.Month;
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var idealDaily = context.IdealDailyFor(selected.Month, year) ?? 0;

            var previous = new DateOnly(year, month, 1).AddMonths(-1);
            var current = AccumulateByDay(context.Spendings, year, month);
            var previousMonth = AccumulateByDay(context.Spendings, previous.Year, previous.Month);

            var days = Enumerable.Range(1, daysInMonth)
                .Select(day => new SpendingPaceDayView
                {
                    Day = day,
                    Accumulated = new DateOnly(year, month, day) <= today ? current[day - 1] : null,
                    PreviousMonthAccumulated = day <= previousMonth.Length ? previousMonth[day - 1] : null,
                    IdealAccumulated = Math.Round(idealDaily * day, 2)
                })
                .ToList();

            return new SpendingPaceView { IdealDaily = idealDaily, Days = days };
        }

        private static decimal[] AccumulateByDay(IEnumerable<Payable> spendings, int year, int month)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var totals = new decimal[daysInMonth];

            foreach (var spending in spendings.Where(x => x.DueDate.Year == year && x.DueDate.Month == month))
                totals[spending.DueDate.Day - 1] += spending.Total;

            for (var i = 1; i < totals.Length; i++)
                totals[i] += totals[i - 1];

            return totals;
        }

        private static List<CategorySpendingView> BuildSpendingByCategory(EffectiveDate selected, List<Financeiro.Business.Aggregates.Category.Category> categories)
        {
            var spendings = Spendings(selected).ToList();
            var total = spendings.Sum(x => x.Total);
            var names = categories.ToDictionary(x => x.Id, x => x.Name);

            return spendings
                .GroupBy(x => x.CategoryId)
                .Select(g => new CategorySpendingView
                {
                    CategoryId = g.Key,
                    Name = g.Key.HasValue && names.TryGetValue(g.Key.Value, out var name) ? name : "Sem categoria",
                    Total = g.Sum(x => x.Total),
                    Percentage = total == 0 ? 0 : Math.Round(g.Sum(x => x.Total) / total * 100, 1)
                })
                .OrderByDescending(x => x.Total)
                .ToList();
        }

        private static List<CreditCardUsageView> BuildCreditCards(EffectiveDate selected, List<CreditCard> creditCards)
        {
            var invoices = selected.Payable.OfType<CardExpense>().ToList();
            var items = selected.Payable.OfType<CardExpense.Expense>().ToList();

            return creditCards
                .Where(card => card.IsActive)
                .OrderBy(card => card.Name)
                .Select(card =>
                {
                    var invoice = invoices.FirstOrDefault(x => x.CreditCardId == card.Id);
                    var invoiceItems = invoice is null ? new List<CardExpense.Expense>() : items.Where(x => x.CardExpenseId == invoice.Id).ToList();
                    var invoiceTotal = invoiceItems.Sum(x => x.Total);
                    var futureInstallments = invoiceItems
                        .Where(x => x.CurrentInstallment < x.QuantityInstallments)
                        .Sum(x => (x.QuantityInstallments - x.CurrentInstallment) * x.Total);
                    var committed = invoiceTotal + futureInstallments;

                    return new CreditCardUsageView
                    {
                        CreditCardId = card.Id,
                        Name = card.Name,
                        Limit = card.Limit,
                        ClosingDay = card.ClosingDay,
                        DueDay = card.DueDay,
                        InvoiceTotal = invoiceTotal,
                        InvoiceDueDate = invoice?.DueDate,
                        FutureInstallments = futureInstallments,
                        Committed = committed,
                        UsagePercentage = card.Limit == 0 ? 0 : Math.Round(committed / card.Limit * 100, 1)
                    };
                })
                .ToList();
        }

        private static FutureCommitmentsView BuildFutureCommitments(DashboardContext context, EffectiveDate selected, List<CreditCard> creditCards)
        {
            var cardNames = creditCards.ToDictionary(x => x.Id, x => x.Name);
            var invoiceCards = selected.Payable.OfType<CardExpense>().ToDictionary(x => x.Id, x => x.CreditCardId);
            var installmentItems = selected.Payable.OfType<CardExpense.Expense>()
                .Where(x => x.CurrentInstallment < x.QuantityInstallments)
                .ToList();

            var start = new DateOnly(selected.Year, (int)selected.Month, 1);
            var months = Enumerable.Range(1, FutureCommitmentMonths)
                .Select(offset =>
                {
                    var date = start.AddMonths(offset);
                    var month = (CalendarMonth)date.Month;

                    var installments = installmentItems
                        .Where(x => x.QuantityInstallments - x.CurrentInstallment >= offset)
                        .GroupBy(x => invoiceCards.TryGetValue(x.CardExpenseId, out var cardId) && cardNames.TryGetValue(cardId, out var name) ? name : "Cartão")
                        .Select(g => new InstallmentCommitmentView { CreditCardName = g.Key, Total = g.Sum(x => x.Total) })
                        .OrderBy(x => x.CreditCardName)
                        .ToList();

                    var dueRecurrentPayments = context.RecurrentPayments.Where(x => x.IsDueIn(month, date.Year)).ToList();
                    var cardRecurrentPayments = dueRecurrentPayments.Where(x => x.CreditCardId.HasValue).Sum(x => x.Value);
                    var immediateRecurrences = dueRecurrentPayments.Where(x => !x.CreditCardId.HasValue).Sum(x => x.Value);

                    return new FutureCommitmentMonthView
                    {
                        Month = month,
                        Year = date.Year,
                        InstallmentsByCreditCard = installments,
                        CardRecurrentPayments = cardRecurrentPayments,
                        ImmediateRecurrentPayments = immediateRecurrences,
                        Total = installments.Sum(x => x.Total) + cardRecurrentPayments + immediateRecurrences
                    };
                })
                .ToList();

            var recentIncomes = context.EffectiveDates
                .Where(x => IsSameOrBefore(x, selected))
                .Select(Income)
                .Where(income => income > 0)
                .TakeLast(AverageIncomeMonths)
                .ToList();

            return new FutureCommitmentsView
            {
                AverageIncome = recentIncomes.Count == 0 ? 0 : Math.Round(recentIncomes.Average(), 2),
                Months = months
            };
        }

        private static List<MonthlyHistoryView> BuildMonthlyHistory(DashboardContext context, EffectiveDate selected)
        {
            return context.EffectiveDates
                .Where(x => IsSameOrBefore(x, selected))
                .TakeLast(MonthlyHistoryMonths)
                .Select(x => new MonthlyHistoryView
                {
                    EffectiveDateId = x.Id,
                    Month = x.Month,
                    Year = x.Year,
                    Income = Income(x),
                    Expenses = Expenses(x),
                    Balance = Income(x) - Expenses(x)
                })
                .ToList();
        }

        private static bool IsSameOrBefore(EffectiveDate effectiveDate, EffectiveDate reference)
        {
            return effectiveDate.Year < reference.Year
                || (effectiveDate.Year == reference.Year && effectiveDate.Month <= reference.Month);
        }

        private static decimal Income(EffectiveDate effectiveDate) => effectiveDate.Receivable.Sum(x => x.Total);

        private static decimal Expenses(EffectiveDate effectiveDate) => Spendings(effectiveDate).Sum(x => x.Total);

        // A fatura (CardExpense) é a soma dos próprios itens: entram os itens, nunca a fatura, para não contar o cartão em dobro.
        private static IEnumerable<Payable> Spendings(EffectiveDate effectiveDate)
        {
            return effectiveDate.Payable.Where(x => x is ImmediateExpense || x is CardExpense.Expense);
        }

        private sealed class DashboardContext
        {
            private readonly Dictionary<(CalendarMonth, int), decimal> _idealDailyByMonth;

            public List<EffectiveDate> EffectiveDates { get; }
            public List<RecurrentPayment> RecurrentPayments { get; }
            public List<Payable> Spendings { get; }

            public DashboardContext(List<EffectiveDate> effectiveDates, List<RecurrentPayment> recurrentPayments)
            {
                EffectiveDates = effectiveDates;
                RecurrentPayments = recurrentPayments;
                Spendings = effectiveDates.SelectMany(DashboardService.Spendings).ToList();
                _idealDailyByMonth = effectiveDates.ToDictionary(x => (x.Month, x.Year), CalculateIdealDaily);
            }

            public decimal? IdealDailyFor(CalendarMonth month, int year)
            {
                return _idealDailyByMonth.TryGetValue((month, year), out var ideal) ? ideal : null;
            }

            // Gasto ideal por dia = (receita da competência − recorrências à vista − assinaturas de cartão
            // − parcelas das faturas da competência) ÷ dias do mês. Pode ser negativo quando os compromissos
            // superam a receita.
            private decimal CalculateIdealDaily(EffectiveDate effectiveDate)
            {
                var dueRecurrentPayments = RecurrentPayments.Where(x => x.IsDueIn(effectiveDate.Month, effectiveDate.Year)).Sum(x => x.Value);
                var installments = effectiveDate.Payable.OfType<CardExpense.Expense>()
                    .Where(x => x.QuantityInstallments > 1)
                    .Sum(x => x.Total);

                var available = Income(effectiveDate) - dueRecurrentPayments - installments;
                var daysInMonth = DateTime.DaysInMonth(effectiveDate.Year, (int)effectiveDate.Month);

                return Math.Round(available / daysInMonth, 2);
            }
        }
    }
}
