using System.Globalization;

namespace Financeiro.Business.Shared
{
	public static class CalendarMonthHelper
	{
		public static CalendarMonth FromDateTime(DateTime date) =>
			(CalendarMonth)date.Month;

		public static bool IsDefined(int value) =>
			value >= 1 && value <= 12;

		public static bool TryParse(int value, out CalendarMonth month)
		{
			if (!IsDefined(value))
			{
				month = default;
				return false;
			}

			month = (CalendarMonth)value;
			return true;
		}

		public static string GetAbbreviation(CalendarMonth month, CultureInfo? culture = null)
		{
			culture ??= CultureInfo.CurrentCulture;
			var date = new DateTime(2000, (int)month, 1);
			return date.ToString("MMM", culture);
		}
	}
}
