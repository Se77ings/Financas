using System.Globalization;
using Financeiro.Business.Shared;

namespace Financeiro.Application.Models.EffectiveDateModels
{
	public class EffectiveDateSummary
	{
		public Guid Id { get; set; }
		public CalendarMonth Month { get; set; }
		public int Year { get; set; }

		public string MonthAbbreviation => CalendarMonthHelper.GetAbbreviation(Month, CultureInfo.CurrentCulture);

		public string Display => $"{MonthAbbreviation}/{Year}";

		public int ReceivableCount { get; set; }
		public int PayableCount { get; set; }

		public decimal Balance { get; set; }
	}
}
