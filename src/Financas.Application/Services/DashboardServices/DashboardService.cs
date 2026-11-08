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
        private const int AverageIncomeMonths = 6;
        private const int CreditCardHistoryPreviousMonths = 4;

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

        public async Task<Result<DashboardView>> GetAsync(int? year, CalendarMonth? month, DateOnly today)
        {
            if (year.HasValue && !month.HasValue)
                return Result<DashboardView>.Failure(new Error("Informe o mês junto com o ano."));

            var effectiveDates = (await _effectiveDateRepository.GetListAsync())
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            EffectiveDate? selected = null;
            if (year.HasValue)
            {
                selected = effectiveDates.FirstOrDefault(x => x.Year == year && x.Month == month);
                if (selected is null)
                    return Result<DashboardView>.Failure(Error.NotFound("Competência não encontrada."));
            }

            var creditCards = await _creditCardRepository.GetListAsync();
            var recurrentPayments = await _recurrentPaymentRepository.GetActiveRecurrentPaymentsAsync();
            var categories = await _categoryRepository.GetListAsync();

            var context = new DashboardContext(effectiveDates, recurrentPayments, creditCards);

            var view = selected is null
                ? new DashboardView
                {
                    Totals = BuildTotals(effectiveDates),
                    SpendingByCategory = BuildSpendingByCategory(context, effectiveDates, categories),
                    CreditCardHistory = BuildYearlyCreditCardHistory(context),
                    History = BuildYearlyHistory(context)
                }
                : new DashboardView
                {
                    Totals = BuildTotals(effectiveDates.Where(x => x.Year == selected.Year)),
                    DailySpending = BuildDailySpending(context, selected, today),
                    SpendingPace = BuildSpendingPace(context, selected, today),
                    SpendingByCategory = BuildSpendingByCategory(context, [selected], categories),
                    CreditCardHistory = BuildCreditCardHistory(context, selected),
                    FutureCommitments = BuildFutureCommitments(context, selected, creditCards),
                    History = BuildMonthlyHistory(context, selected.Year)
                };

            return Result<DashboardView>.Success(view);
        }

        private static DashboardTotalsView BuildTotals(IEnumerable<EffectiveDate> effectiveDates)
        {
            var scope = effectiveDates.ToList();
            var income = scope.Sum(Income);
            var expenses = scope.Sum(Expenses);

            return new DashboardTotalsView { Income = income, Expenses = expenses, Balance = income - expenses };
        }

        private static DailySpendingView BuildDailySpending(DashboardContext context, EffectiveDate selected, DateOnly today)
        {
            var monthStart = new DateOnly(selected.Year, (int)selected.Month, 1);

            return new DailySpendingView
            {
                LastDays = BuildDays(context, today.AddDays(-(DailySpendingDays - 1)), today),
                SelectedMonth = BuildDays(context, monthStart, monthStart.AddMonths(1).AddDays(-1))
            };
        }

        private static List<DailySpendingDayView> BuildDays(DashboardContext context, DateOnly firstDay, DateOnly lastDay)
        {
            var spendingByDate = context.Spendings
                .Where(x => x.DueDate >= firstDay && x.DueDate <= lastDay)
                .GroupBy(x => x.DueDate)
                .ToDictionary(g => g.Key, g => g.ToList());

            return Enumerable.Range(0, lastDay.DayNumber - firstDay.DayNumber + 1)
                .Select(offset => firstDay.AddDays(offset))
                .Select(date => new DailySpendingDayView
                {
                    Date = date,
                    Entries = spendingByDate.TryGetValue(date, out var spendings)
                        ? spendings.OrderByDescending(x => x.Total).Select(context.ToEntry).ToList()
                        : new List<SpendingEntryView>(),
                    IdealDaily = context.IdealDailyFor((CalendarMonth)date.Month, date.Year)
                })
                .ToList();
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

        private static List<CategorySpendingView> BuildSpendingByCategory(DashboardContext context, IEnumerable<EffectiveDate> effectiveDates, List<Financeiro.Business.Aggregates.Category.Category> categories)
        {
            var spendings = effectiveDates.SelectMany(Spendings).ToList();
            var total = spendings.Sum(x => x.Total);
            var names = categories.ToDictionary(x => x.Id, x => x.Name);

            return spendings
                .GroupBy(x => x.CategoryId)
                .Select(g => new CategorySpendingView
                {
                    CategoryId = g.Key,
                    Name = g.Key.HasValue && names.TryGetValue(g.Key.Value, out var name) ? name : "Sem categoria",
                    Total = g.Sum(x => x.Total),
                    Percentage = total == 0 ? 0 : Math.Round(g.Sum(x => x.Total) / total * 100, 1),
                    Entries = g.OrderBy(x => x.DueDate).ThenByDescending(x => x.Total).Select(context.ToEntry).ToList()
                })
                .OrderByDescending(x => x.Total)
                .ToList();
        }

        private static List<CreditCardInvoicePeriodView> BuildCreditCardHistory(DashboardContext context, EffectiveDate selected)
        {
            var selectedIndex = context.EffectiveDates.IndexOf(selected);
            var firstIndex = Math.Max(0, selectedIndex - CreditCardHistoryPreviousMonths);
            var lastIndex = Math.Min(context.EffectiveDates.Count - 1, selectedIndex + 1);

            return context.EffectiveDates
                .Skip(firstIndex)
                .Take(lastIndex - firstIndex + 1)
                .Select(effectiveDate => new CreditCardInvoicePeriodView
                {
                    EffectiveDateId = effectiveDate.Id,
                    Month = effectiveDate.Month,
                    Year = effectiveDate.Year,
                    IsSelected = effectiveDate.Id == selected.Id,
                    Invoices = InvoiceAmounts(context, [effectiveDate])
                })
                .ToList();
        }

        private static List<CreditCardInvoicePeriodView> BuildYearlyCreditCardHistory(DashboardContext context)
        {
            return context.EffectiveDates
                .GroupBy(x => x.Year)
                .Select(year => new CreditCardInvoicePeriodView
                {
                    Year = year.Key,
                    Invoices = InvoiceAmounts(context, year)
                })
                .ToList();
        }

        // O total de cada fatura é a soma dos itens, como no restante do dashboard.
        private static List<CreditCardInvoiceAmountView> InvoiceAmounts(DashboardContext context, IEnumerable<EffectiveDate> effectiveDates)
        {
            return effectiveDates
                .SelectMany(x => x.Payable.OfType<CardExpense.Expense>())
                .GroupBy(context.CreditCardIdOf)
                .Where(g => g.Key.HasValue)
                .Select(g => new CreditCardInvoiceAmountView
                {
                    CreditCardId = g.Key!.Value,
                    Name = context.CreditCardName(g.Key.Value),
                    Total = g.Sum(x => x.Total)
                })
                .Where(x => x.Total > 0)
                .OrderBy(x => x.Name)
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

        private static List<HistoryPeriodView> BuildMonthlyHistory(DashboardContext context, int year)
        {
            return context.EffectiveDates
                .Where(x => x.Year == year)
                .Select(x => new HistoryPeriodView
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

        private static List<HistoryPeriodView> BuildYearlyHistory(DashboardContext context)
        {
            return context.EffectiveDates
                .GroupBy(x => x.Year)
                .Select(year =>
                {
                    var income = year.Sum(Income);
                    var expenses = year.Sum(Expenses);
                    return new HistoryPeriodView { Year = year.Key, Income = income, Expenses = expenses, Balance = income - expenses };
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
            private readonly Dictionary<Guid, string> _creditCardNames;
            private readonly Dictionary<Guid, Guid> _invoiceCreditCards;

            public List<EffectiveDate> EffectiveDates { get; }
            public List<RecurrentPayment> RecurrentPayments { get; }
            public List<Payable> Spendings { get; }

            public DashboardContext(List<EffectiveDate> effectiveDates, List<RecurrentPayment> recurrentPayments, List<CreditCard> creditCards)
            {
                EffectiveDates = effectiveDates;
                RecurrentPayments = recurrentPayments;
                Spendings = effectiveDates.SelectMany(DashboardService.Spendings).ToList();
                _idealDailyByMonth = effectiveDates.ToDictionary(x => (x.Month, x.Year), CalculateIdealDaily);
                _creditCardNames = creditCards.ToDictionary(x => x.Id, x => x.Name);
                _invoiceCreditCards = effectiveDates.SelectMany(x => x.Payable.OfType<CardExpense>()).ToDictionary(x => x.Id, x => x.CreditCardId);
            }

            public Guid? CreditCardIdOf(CardExpense.Expense item) =>
                _invoiceCreditCards.TryGetValue(item.CardExpenseId, out var cardId) ? cardId : null;

            public string CreditCardName(Guid creditCardId) =>
                _creditCardNames.TryGetValue(creditCardId, out var name) ? name : "Cartão";

            public SpendingEntryView ToEntry(Payable spending)
            {
                var item = spending as CardExpense.Expense;

                return new SpendingEntryView
                {
                    Description = spending.Description,
                    Date = spending.DueDate,
                    Total = spending.Total,
                    CategoryId = spending.CategoryId,
                    CreditCardName = item is not null && _invoiceCreditCards.TryGetValue(item.CardExpenseId, out var cardId) ? CreditCardName(cardId) : null,
                    Installment = item is { QuantityInstallments: > 1 } ? $"{item.CurrentInstallment}/{item.QuantityInstallments}" : null
                };
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
