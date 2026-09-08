using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Shared;

namespace Financeiro.Business.Aggregates
{
    public class EffectiveDate : Entity
    {
        public CalendarMonth Month { get; private set; }
        public int Year { get; private set; }
        private List<Receivable.Receivable> _receivable = new();
        private List<Payable.Payable> _payable = new();

        public IEnumerable<Receivable.Receivable> Receivable => _receivable;
        public IEnumerable<Payable.Payable> Payable => _payable;
        public (CalendarMonth month, int year) GetNextPeriod(int skip = 1)
        => CalendarMonthHelper.AddMonths(this.Month, this.Year, skip);

        public (CalendarMonth month, int year) GetPreviousPeriod()
            => CalendarMonthHelper.AddMonths(this.Month, this.Year, -1);
        protected EffectiveDate() { }

        public EffectiveDate(CalendarMonth month, int year)
        {
            Month = month;
            Year = year;
        }

        public void Update(CalendarMonth month, int year)
        {
            Month = month;
            Year = year;
        }

        public void AddPayable(Payable.Payable payable)
        {
            _payable.Add(payable);
        }

        public void AddReceivable(Receivable.Receivable receivable)
        {
            _receivable.Add(receivable);
        }

        public decimal CalculateTotal()
        {
            var pay = _payable.Sum(a => a.Total);
            var receive = _receivable.Sum(a => a.Total);

            return receive - pay;
        }

        public void GenerateFinancialStatement()
        {
            throw new NotImplementedException();
        }
        public CardExpense? GetCardExpenseByCreditCardId(Guid creditCardId)
        {
            return Payable
                .OfType<CardExpense>()
                .FirstOrDefault(x => x.CreditCardId == creditCardId);
        }
    }
}
