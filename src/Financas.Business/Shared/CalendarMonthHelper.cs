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

        private static readonly string[] PortugueseAbbreviations = ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"];

        /// <summary>
        /// "Dez/2026". Fixo em português porque vira texto gravado, como a descrição da fatura: não pode depender
        /// do idioma do computador.
        /// </summary>
        public static string ToPortugueseLabel(CalendarMonth month, int year) =>
            $"{PortugueseAbbreviations[(int)month - 1]}/{year}";

        public static (CalendarMonth month, int year) AddMonths(CalendarMonth currentMonth, int currentYear, int monthsToAdd)
        {
            var date = new DateTime(currentYear, (int)currentMonth, 1).AddMonths(monthsToAdd);

            return ((CalendarMonth)date.Month, date.Year);
        }


    }
}
