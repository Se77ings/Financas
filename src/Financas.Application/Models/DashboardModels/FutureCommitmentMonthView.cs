using Financeiro.Business.Shared;

namespace Financeiro.Application.Models.DashboardModels
{
    public class FutureCommitmentMonthView
    {
        public CalendarMonth Month { get; set; }
        public int Year { get; set; }
        public List<InstallmentCommitmentView> InstallmentsByCreditCard { get; set; } = new();
        public decimal CardSubscriptions { get; set; }
        public decimal ImmediateRecurrences { get; set; }
        public decimal Total { get; set; }
    }
}
