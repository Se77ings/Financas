using Financeiro.Business.Shared;

namespace Financeiro.Application.Models.DashboardModels
{
    public class MonthlyHistoryView
    {
        public Guid EffectiveDateId { get; set; }
        public CalendarMonth Month { get; set; }
        public int Year { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public decimal Balance { get; set; }
    }
}
