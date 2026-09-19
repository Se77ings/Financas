using System.Globalization;
using Financeiro.Application.Models.PayableModels;
using Financeiro.Application.Models.ReceivableModels;
using Financeiro.Business.Aggregates.EffectiveDate;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Shared;

namespace Financeiro.Application.Models.EffectiveDateModels
{
	public class EffectiveDateView
	{
		public CalendarMonth Month { get; set; }
		public int Year { get; set; }

		public string MonthAbbreviation => CalendarMonthHelper.GetAbbreviation(Month, CultureInfo.CurrentCulture);

		public List<ReceivableView> Receivable { get; set; } = new();
		public List<ImmediateExpenseView> Payable { get; set; } = new();
		public List<CardExpenseView> CardExpense { get; set; } = new();

		public static implicit operator EffectiveDateView(EffectiveDate effectiveDate)
		{
			return new EffectiveDateView
			{
				Month = effectiveDate.Month,
				Year = effectiveDate.Year,
				Receivable = effectiveDate.Receivable.Select(c => (ReceivableView)c).ToList(),
				Payable = effectiveDate.Payable.OfType<ImmediateExpense>().Select(d => (ImmediateExpenseView)d).ToList(),
				CardExpense = effectiveDate.Payable.OfType<CardExpense>().Select(d => (CardExpenseView)d).ToList()
			};
		}
	}
}
