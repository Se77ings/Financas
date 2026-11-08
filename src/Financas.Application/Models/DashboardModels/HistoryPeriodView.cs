using Financeiro.Business.Shared;

namespace Financeiro.Application.Models.DashboardModels
{
    /// <summary>Uma competência, ou um ano inteiro quando <see cref="Month"/> e <see cref="EffectiveDateId"/> vêm nulos.</summary>
    public class HistoryPeriodView
    {
        public Guid? EffectiveDateId { get; set; }
        public CalendarMonth? Month { get; set; }
        public int Year { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public decimal Balance { get; set; }
    }
}
